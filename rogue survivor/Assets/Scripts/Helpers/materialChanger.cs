using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialChanger : MonoBehaviour
{
    [SerializeField] private Color color;
    private MaterialPropertyBlock propertyBlock;
    private Renderer myRenderer;
    [SerializeField] private Texture texture;

    [Header("More Mesh")]
    public bool randomColor;
    public Color[] randomColors;


    private void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        myRenderer = GetComponent<Renderer>();

        myRenderer.GetPropertyBlock(propertyBlock);

        if (!randomColor)
        {
            propertyBlock.SetColor("_Color", color);
        }
        else
        {
            propertyBlock.SetColor("_Color", randomColors[Random.Range(0, randomColors.Length)]);            
        }


        if (texture != null)
        {
            propertyBlock.SetTexture("_MainTex", texture);
        }

        myRenderer.SetPropertyBlock(propertyBlock);
    }
}
