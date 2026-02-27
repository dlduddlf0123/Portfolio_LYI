using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    public GameObject baseObject;
    public bool activeChild = true;


    public void ButtonMeshCombine()
    {

        // 기본 객체의 MeshFilter와 MeshRenderer 가져오기
        //MeshFilter baseMeshFilter = baseObject.GetComponent<MeshFilter>();
        MeshRenderer baseMeshRenderer = null;
        if (baseObject != null)
        {
            baseMeshRenderer = baseObject.GetComponent<MeshRenderer>();
        }

        // 자식 객체들의 MeshFilter 배열 가져오기
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        // CombineInstance 배열 생성
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // 모든 메시의 노멀값과 탄젠트값을 합칠 변수 생성
        Vector3[] baseNormals = new Vector3[0];
        Vector4[] baseTangents = new Vector4[0];

        // CombineInstance 배열에 각각의 자식 객체 메시 정보 설정 및 노멀값, 탄젠트값 합침
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(activeChild); // 자식 객체 비활성화

            if (baseObject != null)
            {
                // 노멀값과 탄젠트값 합침
                baseNormals = ConcatenateArrays(baseNormals, meshFilters[i].sharedMesh.normals);
                baseTangents = ConcatenateArrays(baseTangents, meshFilters[i].sharedMesh.tangents);
            }
        }

        // 기본 객체에 결합된 메시 생성
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, true);

        // 합쳐진 메시에 노멀값과 탄젠트값 설정
        combinedMesh.normals = baseNormals;
        combinedMesh.tangents = baseTangents;

        if (baseObject != null)
        {
            // 기본 객체의 머티리얼 설정
            baseMeshRenderer.materials = baseMeshRenderer.sharedMaterials;
        }

        // 새 게임 오브젝트 생성 및 메시 설정
        GameObject newObject = new GameObject("CombinedMesh");
        MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
        MeshRenderer newMeshRenderer = newObject.AddComponent<MeshRenderer>();
        newMeshFilter.sharedMesh = combinedMesh;

        newObject.transform.SetParent(transform.parent);
        //newObject.name = FindObjectOfType<VRTokTok.Manager.StageManager>().gameObject.name;

        if (baseObject != null)
        {
            newMeshRenderer.sharedMaterials = baseMeshRenderer.sharedMaterials;
            // 센터 피봇 적용
            newObject.transform.position = baseObject.transform.position;
            newObject.transform.rotation = baseObject.transform.rotation;
        }
    }

    // 두 배열을 합치는 함수
    private T[] ConcatenateArrays<T>(T[] array1, T[] array2)
    {
        T[] result = new T[array1.Length + array2.Length];
        array1.CopyTo(result, 0);
        array2.CopyTo(result, array1.Length);
        return result;
    }
}