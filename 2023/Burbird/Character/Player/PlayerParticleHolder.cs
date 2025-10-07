using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Burbird
{

    /// <summary>
    /// 플레이어가 사용하는 각종 효과들 저장소
    /// 각 효과들 오브젝트 풀링으로 관리?
    /// </summary>
    public class PlayerParticleHolder : MonoBehaviour
    {
        StageManager stageMgr;

        Transform tr_active;
        Transform tr_disable;

        public GameObject vfx_feather;
        List<GameObject> list_vfx_feather = new List<GameObject>();

        public LineRenderer line_lightning;
        List<GameObject> list_lightning = new List<GameObject>();

        public AudioClip sfx_featherHit;
        void Awake()
        {
            stageMgr = StageManager.Instance;

            tr_active = transform.GetChild(0);
            tr_disable = transform.GetChild(1);
        }

        GameObject CreateObject(List<GameObject> list, GameObject originGo, Vector3 pos)
        {
            GameObject go;

            if (list.Count == 0)
            {
                go = Instantiate(originGo);
            }
            else
            {
                go = list[0];
                list.RemoveAt(0);
            }

            go.transform.SetParent(tr_active);
            go.transform.position = pos;
            go.gameObject.SetActive(true);

            return go;
        }

        public void ObjectInit(List<GameObject> list, GameObject go)
        {
            go.transform.SetParent(tr_disable);
            list.Add(go);
            go.SetActive(false);
        }
        public void SetLine_Lightning(Vector3 start, Vector3 end, float time)
        {
            LineRenderer lightning = CreateObject(list_lightning, line_lightning.gameObject, Vector3.zero).GetComponent<LineRenderer>();
            lightning.SetPosition(0, start);
            lightning.SetPosition(1, end);

            StartCoroutine(LateInit(list_lightning, lightning.gameObject, time));
        }

        public void PlayParticle_FeatherHit(Vector3 pos)
        {
            GameObject go = CreateObject(list_vfx_feather, vfx_feather, pos);

            stageMgr.soundMgr.PlaySfx(pos, sfx_featherHit, Random.Range(0.7f, 1.4f));
            StartCoroutine(LateInit(list_vfx_feather, go, 2f));
        }

        public IEnumerator LateInit(List<GameObject> list, GameObject go, float time)
        {
            yield return new WaitForSeconds(time);
            ObjectInit(list, go);
        }
    }
}