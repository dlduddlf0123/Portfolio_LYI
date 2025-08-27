using UnityEngine;

public class ParticleMaterialManager : MonoBehaviour
{
    public string propertyName;
    public Vector2 Tilng;
    public Vector2 Offset;

    private void Awake()
    {
        Material mat = GetComponent<ParticleSystemRenderer>().material;
        mat.SetTextureScale(propertyName, Tilng);
        mat.SetTextureOffset(propertyName, Offset);
    }
}
