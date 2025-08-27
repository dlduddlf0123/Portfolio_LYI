using UnityEngine;

public class CharacterMaterialManager : MonoBehaviour
{
    [SerializeField]
    public Vector2 Tilng;
    public Vector2 Offset;

    private void Awake()
    {
        Material mat = GetComponent<SkinnedMeshRenderer>().material;
        mat.SetTextureScale("_Diffuse", Tilng);
        mat.SetTextureOffset("_Diffuse", Offset);
        mat.SetTextureScale("_Illumination", Tilng);
        mat.SetTextureOffset("_Illumination", Offset);
    }
}
