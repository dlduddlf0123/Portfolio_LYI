using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererMeterial : MonoBehaviour
{

    public string propertyName;
    public Vector2 Tilng;
    public Vector2 Offset;

    private void Awake()
    {
        Material mat = GetComponent<MeshRenderer>().materials[0];
        mat.mainTextureScale = Tilng;
        mat.mainTextureOffset = Offset;
        mat.SetTextureScale(propertyName, Tilng);
        mat.SetTextureOffset(propertyName, Offset);
    }
}