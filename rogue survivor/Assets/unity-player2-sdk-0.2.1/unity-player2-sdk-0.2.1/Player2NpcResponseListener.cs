namespace player2_sdk
{


    using System;
    using System.Collections.Generic;
    using System.Text;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Networking;

    [Serializable]
    public class NpcApiChatResponse
    {
        public string npc_id;
        [CanBeNull] public string message;
        [CanBeNull] public SingleTextToSpeechData audio;
        [CanBeNull] public List<FunctionCallResponse> command;
    }

    [Serializable]
    public class SingleTextToSpeechData
    {
        public string data;
    }

    [Serializable]
    public class FunctionCallResponse
    {
        public string name;
        public string arguments;

    public FunctionCall ToFunctionCall(GameObject ai)
        {
            var args = JsonConvert.DeserializeObject<JObject>(arguments);
            return new FunctionCall
            {
                name = name,
                arguments = args,
                aiObject = ai
            };
        }
    }

    [Serializable]
    public class FunctionCall
    {
        public string name;
        public JObject arguments;
        public GameObject aiObject;
    }



    [Serializable]
    public class NpcResponseEvent : UnityEvent<NpcApiChatResponse>
    {
    }

    public class Player2NpcResponseListener : MonoBehaviour
    {
        public string _baseUrl = null;
        [Header("Reconnection Settings")]
        [SerializeField]
        [Tooltip("Delay in seconds between reconnection attempts")]
        public float _reconnectDelay = 2.0f;

        [SerializeField]
        [Tooltip("Maximum number of reconnection attempts before giving up")]
        public int _maxReconnectAttempts = 5;

        private string apiKey;
        private bool _isListening = false;
        private int _reconnectAttempts = 0;
        private string _lastEventId = null;
        private string _traceId = null;

        // SSE event parsing state
        private string _currentEventId = null;
        private string _currentEventType = null;
        private StringBuilder _currentEventData = new StringBuilder();

        // Buffer protection
        private const int MAX_EVENT_SIZE = 2 * 1024 * 1024; // 2MB max per event
        private const float CONNECTION_TIMEOUT = 300.0f; // 5 minutes - server should send pings every 15 seconds

        private Dictionary<string, UnityEvent<NpcApiChatResponse>> _responseEvents =
            new Dictionary<string, UnityEvent<NpcApiChatResponse>>();

        public JsonSerializerSettings JsonSerializerSettings;
        public UnityEvent<string> newApiKey = new UnityEvent<string>();

        public bool IsListening => _isListening;

        /// <summary>
        /// Configures the reconnection settings for the response listener
        /// </summary>
        /// <param name="maxAttempts">Maximum number of reconnection attempts (default: 5)</param>
        /// <param name="delaySeconds">Delay in seconds between reconnection attempts (default: 2.0f)</param>
        public void SetReconnectionSettings(int maxAttempts, float delaySeconds)
        {
            _maxReconnectAttempts = maxAttempts;
            _reconnectDelay = delaySeconds;
            Debug.Log($"Reconnection settings configured: {maxAttempts} attempts, {delaySeconds}s delay");
        }

        /// <summary>
        /// Gets the current reconnection settings
        /// </summary>
        public (int maxAttempts, float delay) GetReconnectionSettings()
        {
            return (_maxReconnectAttempts, _reconnectDelay);
        }

        private void Awake()
        {
            // Ensure JsonSerializerSettings is initialized
            if (newApiKey == null)
            {
                newApiKey = new UnityEvent<string>();
            }
            if (_responseEvents == null)
            {
                _responseEvents = new Dictionary<string, UnityEvent<NpcApiChatResponse>>();
            }
            if (_currentEventData == null)
            {
                _currentEventData = new StringBuilder();
            }


            newApiKey.AddListener((apiKey) =>
            {
                bool start = this.apiKey == null;

                this.apiKey = apiKey;
                if (start)
                {
                    StartListening();
                }
            });
        }


        public void RegisterNpc(string npcId, UnityEvent<NpcApiChatResponse> onNpcResponse)
        {
            if (_responseEvents == null)
            {
                Debug.LogError("Response events dictionary is null!");
                return;
            }

            if (string.IsNullOrEmpty(npcId))
            {
                Debug.LogError("Cannot register NPC with null or empty ID");
                return;
            }

            if (_responseEvents.ContainsKey(npcId))
            {
                _responseEvents[npcId] = onNpcResponse;
                Debug.Log($"Updated NPC response listener for: {npcId}");
            }
            else
            {
                _responseEvents.Add(npcId, onNpcResponse);
                Debug.Log($"Registered NPC response listener for: {npcId} (Total NPCs: {_responseEvents.Count})");
            }
        }

        public void UnregisterNpc(string npcId)
        {
            if (_responseEvents.ContainsKey(npcId))
            {
                _responseEvents.Remove(npcId);
                Debug.Log($"Unregistered NPC response listener for: {npcId} (Remaining NPCs: {_responseEvents.Count})");
            }
            else
            {
                Debug.LogWarning($"Attempted to unregister non-existent NPC: {npcId}");
            }
        }

        public void StartListening()
        {
            // Check if component is still valid before proceeding
            if (this == null || !isActiveAndEnabled)
            {
                Debug.LogWarning("Cannot start listening: component is not valid");
                return;
            }

            if (_isListening)
            {
                Debug.LogWarning("Already listening for responses");
                return;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("Cannot start listening: user is not authenticated");
                return;
            }

            _isListening = true;
            _reconnectAttempts = 0;
            // Preserve Last-Event-Id across stop/start cycles for proper reconnection
            Debug.Log($"Starting NPC response listener... (Registered NPCs: {string.Join(", ", _responseEvents.Keys)}) Current Last-Event-Id: {_lastEventId ?? "none"}");

            // Fire and forget async operation
            _ = ListenForResponsesAsync();
        }

        public void StopListening()
        {
            if (!_isListening)
                return;

                        _isListening = false;

            // Clean up any partial event state when stopping
            ResetEventState();

            Debug.Log($"Stopped listening for NPC responses (Last-Event-Id: {_lastEventId ?? "none"})");
        }

        private async Awaitable ListenForResponsesAsync()
        {
            // Initial component validity check
            if (this == null || !isActiveAndEnabled)
            {
                Debug.LogWarning("Cannot start response listener: component is not valid");
                return;
            }

            while (_isListening && this != null && isActiveAndEnabled)
            {
                // Additional check in case component becomes invalid during loop
                if (this == null || !isActiveAndEnabled)
                {
                    Debug.Log("Component became invalid during response listening, stopping...");
                    break;
                }

                try
                {
                    Debug.Log("Starting streaming connection...");
                    await ProcessStreamingResponsesAsync();

                    // If we get here and we're still supposed to be listening,
                    // it means the connection ended unexpectedly - reconnect
                    if (_isListening)
                    {
                        Debug.LogWarning("Streaming connection ended unexpectedly, attempting to reconnect...");
                        await HandleReconnectionAsync();
                    }
                }
                catch (Exception ex)
                {
                    // Null-safe exception logging
                    string errorMessage = ex != null && !string.IsNullOrEmpty(ex.Message)
                        ? ex.Message
                        : ex != null
                            ? $"{ex.GetType().Name}: {ex.ToString()}"
                            : "Unknown error (exception was null)";
                    Debug.LogError($"Error in response listener: {errorMessage}");

                    if (_isListening && this != null)
                    {
                        await HandleReconnectionAsync();
                    }
                    else
                    {
                        Debug.Log("Stopping listener due to error while not listening");
                        break;
                    }
                }
            }

            Debug.Log("Response listener task ended");
        }

        private async Awaitable ProcessStreamingResponsesAsync()
        {
            // Validate base URL and API key before proceeding
            if (string.IsNullOrEmpty(_baseUrl))
            {
                Debug.LogError("Cannot connect to response stream: base URL is not set");
                throw new Exception("Base URL is not configured");
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("Cannot connect to response stream: API key is not set");
                throw new Exception("API key is not configured");
            }

            string url = $"{_baseUrl}/npcs/responses";

            // Validate URL format
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                Debug.LogError($"Invalid URL format: {url}");
                throw new Exception($"Invalid URL format: {url}");
            }

            // Test basic connectivity to the server
            try
            {
                var testUri = new Uri(url);
                Debug.Log($"Testing connectivity to: {testUri.Host}:{testUri.Port}");
            }
            catch (Exception uriEx)
            {
                Debug.LogError($"URI parsing error: {uriEx.Message}");
                throw new Exception($"URI parsing error: {uriEx.Message}");
            }

            // Log connection details including Last-Event-Id and X-Player2-Trace-Id
            if (!string.IsNullOrEmpty(_lastEventId) || !string.IsNullOrEmpty(_traceId))
            {
                Debug.Log($"Connecting to response stream: {url} (reconnecting with Last-Event-Id: {_lastEventId ?? "none"}, X-Player2-Trace-Id: {_traceId ?? "none"})");
            }
            else
            {
                Debug.Log($"Connecting to response stream: {url} (fresh connection, no Last-Event-Id or X-Player2-Trace-Id)");
            }

            // Reset SSE parsing state for new connection
            ResetEventState();

            using var request = UnityWebRequest.Get(url);

            // Disable timeout for SSE streaming connection (0 = no timeout)
            request.timeout = 0;

            // Set headers for streaming
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Accept", "text/event-stream");
            request.SetRequestHeader("Cache-Control", "no-cache");
            request.SetRequestHeader("Connection", "keep-alive");

            // Send Last-Event-Id if we have one (for reconnection)
            if (!string.IsNullOrEmpty(_lastEventId))
            {
                request.SetRequestHeader("Last-Event-Id", _lastEventId);
            }

            // Send X-Player2-Trace-Id if we have one (for request tracing)
            if (!string.IsNullOrEmpty(_traceId))
            {
                request.SetRequestHeader("X-Player2-Trace-Id", _traceId);
            }

            // Start the request
            var operation = request.SendWebRequest();

            StringBuilder lineBuffer = new StringBuilder();
            int lastProcessedLength = 0;
            bool connectionEstablished = false;
            float lastDataTime = Time.time;
            var downloadHandler = request?.downloadHandler;

            // Keep streaming until we stop listening or encounter an error
            while (_isListening && this != null && isActiveAndEnabled)
            {
                // If operation finished, decide what to do
                if (operation.isDone)
                {
                    // Check if request is still valid
                    if (request == null)
                    {
                        Debug.LogError("Request became null during processing");
                        break;
                    }

                    // Process any remaining data before handling disconnection
                    if (downloadHandler != null && downloadHandler.text != null && downloadHandler.text.Length > lastProcessedLength)
                    {
                        string newData = downloadHandler.text.Substring(lastProcessedLength);
                        ProcessNewData(newData, lineBuffer);

                        // Process any final incomplete line if connection was lost
                        if (lineBuffer.Length > 0)
                        {
                            ProcessLine(lineBuffer.ToString());
                        }

                        // Process any accumulated but incomplete SSE event on disconnection
                        if (_currentEventData.Length > 0 || !string.IsNullOrEmpty(_currentEventId))
                        {
                            Debug.Log("Processing incomplete SSE event due to connection loss");
                            ProcessCompleteEvent();
                        }
                    }

                    // Distinguish success vs error vs early finish
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Streaming request completed normally (server closed connection). Exiting stream loop.");
                        break; // Exit loop; caller will handle reconnection if still listening
                    }
                    else
                    {
                        // Handle all non-success results gracefully
                        string errorMsg = request != null && !string.IsNullOrWhiteSpace(request.error)
                            ? request.error
                            : $"HTTP {(request != null && request.responseCode != 0 ? request.responseCode.ToString() : "<no status>")}";

                        string lastEventInfo = !string.IsNullOrEmpty(_lastEventId)
                            ? _lastEventId
                            : "none";

                        // Check for X-Player2-Trace-Id in error response
                        string responseTraceId = request.GetResponseHeader("X-Player2-Trace-Id");
                        if (!string.IsNullOrEmpty(responseTraceId))
                        {
                            _traceId = responseTraceId;
                        }

                        // Special handling for common streaming errors
                        string traceInfo = !string.IsNullOrEmpty(_traceId) ? _traceId : "none";

                        if (errorMsg.Contains("Curl error 18"))
                        {
                            Debug.LogError($"Server closed connection unexpectedly (Curl error 18), reconnecting with Last-Event-Id: {lastEventInfo}, X-Player2-Trace-Id: {traceInfo}");
                        }
                        else if (errorMsg.Contains("Curl error 56"))
                        {
                            Debug.LogError($"Connection reset by server (Curl error 56), reconnecting with Last-Event-Id: {lastEventInfo}, X-Player2-Trace-Id: {traceInfo}");
                        }
                        else
                        {
                            Debug.LogError($"UnityWebRequest.Result returned {request?.result}, errorMsg: {errorMsg}, responseCode: {request?.responseCode}, reconnecting with Last-Event-Id: {lastEventInfo}, X-Player2-Trace-Id: {traceInfo}");
                        }

                        break; // Exit loop to allow reconnection
                    }
                }

                // Not done yet; wait for some data

                // Connection establishment detection: first bytes arrived
                if (!connectionEstablished)
                {
                    if (downloadHandler != null && downloadHandler.text != null && downloadHandler.text.Length > 0)
                    {
                        connectionEstablished = true;
                        lastDataTime = Time.time; // Reset timeout timer on connection

                        // Capture X-Player2-Trace-Id from response headers
                        string newTraceId = request.GetResponseHeader("X-Player2-Trace-Id");
                        if (!string.IsNullOrEmpty(newTraceId) && newTraceId != _traceId)
                        {
                            _traceId = newTraceId;
                            Debug.Log($"Captured X-Player2-Trace-Id: {_traceId}");
                        }

                        Debug.Log("Streaming connection established (first bytes received)");
                    }
                }

                if (downloadHandler != null && downloadHandler.text != null && downloadHandler.text.Length > lastProcessedLength)
                {
                    string newData = downloadHandler.text.Substring(lastProcessedLength);
                    lastProcessedLength = downloadHandler.text.Length;
                    lastDataTime = Time.time; // Reset timeout - any data including pings keeps connection alive

                    // Avoid logging entire buffer each time (can get very large). Log a preview instead.
                    if (Debug.isDebugBuild)
                    {
                        var preview = newData.Length > 200 ? newData.Substring(0, 200) + "..." : newData;
                        Debug.Log($"Received {newData.Length} new chars (total {lastProcessedLength}). Preview: {preview}");
                    }

                    ProcessNewData(newData, lineBuffer);
                }

                // Check for connection timeout (no data received for too long)
                // Only check timeout if CONNECTION_TIMEOUT is greater than 0
                if (CONNECTION_TIMEOUT > 0 && connectionEstablished && (Time.time - lastDataTime) > CONNECTION_TIMEOUT)
                {
                    string lastEventInfo = !string.IsNullOrEmpty(_lastEventId)
                        ? _lastEventId
                        : "none";
                    string traceInfo = !string.IsNullOrEmpty(_traceId)
                        ? _traceId
                        : "none";
                    Debug.LogError($"No data received for {CONNECTION_TIMEOUT} seconds (expected pings every 15s), reconnecting with Last-Event-Id: {lastEventInfo}, X-Player2-Trace-Id: {traceInfo}");
                    break; // Exit loop to trigger reconnection
                }

                // Check if component is still valid during loop execution
                if (this == null || !isActiveAndEnabled)
                {
                    Debug.Log("Component became invalid during response listening, stopping...");
                    break;
                }

                // Small delay to prevent excessive polling (unity main thread friendly)
                await Awaitable.WaitForSecondsAsync(0.05f);
            }

            Debug.Log("Streaming loop ended");
        }

        private void ProcessNewData(string newData, StringBuilder lineBuffer)
        {
            if (newData == null)
            {
                Debug.LogWarning("Received null newData in ProcessNewData");
                return;
            }

            if (lineBuffer == null)
            {
                Debug.LogError("lineBuffer is null in ProcessNewData");
                return;
            }

            for (int i = 0; i < newData.Length; i++)
            {
                char c = newData[i];

                if (c == '\n')
                {
                    // Process complete line (including empty lines which terminate SSE events)
                    ProcessLine(lineBuffer.ToString());
                    lineBuffer.Clear();
                }
                else if (c != '\r') // Skip carriage returns
                {
                    lineBuffer.Append(c);
                }
            }
        }

        private void ProcessLine(string line)
        {
            if (line == null)
            {
                Debug.LogWarning("Received null line in ProcessLine");
                return;
            }

            // Empty line indicates end of SSE event - process it
            if (string.IsNullOrEmpty(line))
            {
                ProcessCompleteEvent();
                return;
            }

            // Skip comments (lines starting with :)
            if (line.StartsWith(":"))
                return;

            // Parse SSE fields according to spec
            // Format: "field:value" or "field: value" (space after colon is optional)
            int colonIndex = line.IndexOf(':');
            if (colonIndex < 0)
            {
                // Line without colon - ignore per SSE spec
                return;
            }

            string fieldName = line.Substring(0, colonIndex);
            string fieldValue = colonIndex + 1 < line.Length ? line.Substring(colonIndex + 1) : "";

            // Remove single leading space if present (per SSE spec)
            if (fieldValue.Length > 0 && fieldValue[0] == ' ')
            {
                fieldValue = fieldValue.Substring(1);
            }

            // Process known fields
            if (string.IsNullOrEmpty(fieldName))
            {
                return; // Empty field name, ignore
            }

            switch (fieldName)
            {
                case "id":
                    // Event IDs should not contain spaces per spec, but trim for safety
                    _currentEventId = fieldValue.Trim();
                    break;

                case "event":
                    // Event types should be preserved as-is (though typically single words)
                    _currentEventType = fieldValue;
                    break;

                case "data":
                    AppendDataLine(fieldValue);
                    break;

                // Unknown fields are ignored per SSE spec (including "retry" for now)
            }
        }

        private void AppendDataLine(string data)
        {
            if (_currentEventData == null)
            {
                Debug.LogError("_currentEventData is null in AppendDataLine");
                return;
            }

            // Check buffer overflow before appending
            int newLength = _currentEventData.Length + (data != null ? data.Length : 0) + 1; // +1 for potential newline
            if (newLength > MAX_EVENT_SIZE)
            {
                Debug.LogError($"SSE event would exceed max size ({MAX_EVENT_SIZE} bytes), discarding");
                ResetEventState();
                return;
            }

            // For multi-line data, add newlines between data lines
            if (_currentEventData.Length > 0)
            {
                _currentEventData.Append('\n');
            }
            _currentEventData.Append(data ?? "");
        }

        private void ProcessCompleteEvent()
        {
            try
            {
                // Store the event ID for reconnection (even for ping events)
                if (!string.IsNullOrEmpty(_currentEventId))
                {
                    string previousEventId = _lastEventId;
                    _lastEventId = _currentEventId;

                    // Log event ID updates for debugging
                    if (_currentEventType == "ping")
                    {
                        Debug.Log($"Updated Last-Event-Id from ping event: {_lastEventId}");
                    }
                    else
                    {
                        Debug.Log($"Updated Last-Event-Id from data event: {_lastEventId}");
                    }
                }

                // Ignore ping events but still track their event ID
                if (_currentEventType == "ping")
                {
                    return;
                }

                // Only process events with data
                if (_currentEventData.Length > 0)
                {
                    string dataString = _currentEventData.ToString();

                    if (string.IsNullOrEmpty(dataString))
                    {
                        return;
                    }

                    NpcApiChatResponse response =
                        JsonConvert.DeserializeObject<NpcApiChatResponse>(dataString, JsonSerializerSettings ?? new JsonSerializerSettings());

                    if (response?.npc_id != null && _responseEvents != null)
                    {
                        if (_responseEvents.ContainsKey(response.npc_id))
                        {
                            Debug.Log($"Received SSE response from NPC {response.npc_id}: {response.message} (Event-Id: {_currentEventId})");

                            // Null-safety check for event handler
                            try
                            {
                                _responseEvents[response.npc_id]?.Invoke(response);
                                // Reset reconnect attempts on successful processing
                                _reconnectAttempts = 0;
                            }
                            catch (Exception handlerEx)
                            {
                                Debug.LogError($"Error in NPC response handler for {response.npc_id}: {handlerEx.Message}");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Received SSE response for unregistered NPC: {response.npc_id}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Received SSE event with invalid or missing npc_id: {dataString.Substring(0, Math.Min(100, dataString.Length))}...");
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                Debug.LogError($"JSON parsing error in SSE event: {jsonEx.Message}. Data: {_currentEventData.ToString().Substring(0, Math.Min(200, _currentEventData.Length))}...");
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error processing SSE event: {e.Message}. Data length: {_currentEventData.Length}");
            }
            finally
            {
                ResetEventState();
            }
        }

        private void ResetEventState()
        {
            _currentEventId = null;
            _currentEventType = null;
            if (_currentEventData != null)
            {
                _currentEventData.Clear();
            }
        }

        private async Awaitable HandleReconnectionAsync()
        {
            _reconnectAttempts++;

            if (_reconnectAttempts > _maxReconnectAttempts)
            {
                Debug.LogError($"Max reconnection attempts ({_maxReconnectAttempts}) reached. Stopping listener.");
                _isListening = false;
                return;
            }

            Debug.Log(
                $"Reconnection attempt {_reconnectAttempts}/{_maxReconnectAttempts} in {_reconnectDelay} seconds (Last-Event-Id: {_lastEventId ?? "none"}, X-Player2-Trace-Id: {_traceId ?? "none"})...");
            await Awaitable.WaitForSecondsAsync(_reconnectDelay);
        }

        private void OnDestroy()
        {
            StopListening();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // Check if component is still valid before proceeding
            if (this == null || !isActiveAndEnabled)
            {
                return;
            }

            if (pauseStatus)
            {
                StopListening();
            }
            else if (!string.IsNullOrEmpty(apiKey))
            {
                StartListening();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // Check if component is still valid before proceeding
            if (this == null || !isActiveAndEnabled)
            {
                return;
            }

            if (!hasFocus)
            {
                StopListening();
            }
            else if (!string.IsNullOrEmpty(apiKey))
            {
                StartListening();
            }
        }
    }
}
