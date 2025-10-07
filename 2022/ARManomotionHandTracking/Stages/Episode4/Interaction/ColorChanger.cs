using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    protected Renderer[] arr_skin;
    protected Material m_sharedMat;

    private void Awake()
    {
        arr_skin = GetComponentsInChildren<Renderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_sharedMat = arr_skin[0].sharedMaterial;
        m_sharedMat.SetFloat("_ChangeHeight", 1);
    }

    public float changeSpeed = 1f;
    public IEnumerator ChangeColor()
    {
        float fill = 1;
        //m_material.SetFloat("_BandHeight", 0.1f);
        while (fill > -1)
        {
            fill -= Time.deltaTime * changeSpeed;

            m_sharedMat.SetFloat("_ChangeHeight", fill);
            yield return new WaitForSeconds(0.01f);
        }
        // m_sharedMat.mainTexture = m_sharedMat.GetTexture("_BlankTex");
        //        m_sharedMat.SetTexture("_BlankTex", gameMgr.b_stagePrefab.LoadAsset<Texture>(""));
    }

}
