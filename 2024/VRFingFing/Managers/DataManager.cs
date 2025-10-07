using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AroundEffect {
    /// <summary>
    /// 9/25/2023-LYI
    /// 게임 데이터 관련 관리 클래스
    /// Addressable을 통해 로드한 데이터를 가공하여 저장
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        GameManager gameMgr;
        CSVLoader csvLoader;

        //각 스테이지 별 리스트    
        public List<List<GameObject>> list__stageType = new();

        public TextAsset CSV_mixedStage;
        public Dictionary<int, int> dic_mixedToStage = new();
        public Dictionary<int, int> dic_StageToMixed = new();


        private void Awake()
        {
            gameMgr = GameManager.Instance;
            csvLoader = GetComponent<CSVLoader>();
        }



        /// <summary>
        /// 9/25/2023-LYI
        /// Addressable로 호출한 프리팹들 각 분류별 정리
        /// </summary>
        public void SetStageDatas()
        {
            list__stageType.Clear();
            for (int i = 0; i < 10; i++)
            {
                List<GameObject> list = new();
                list__stageType.Add(list);
            }

            foreach (var item in gameMgr.addressableMgr.dic_stagePrefab)
            {
                char s;
                s = item.Key[0];
                int i = (int)(s - '0');
                list__stageType[i - 1].Add(item.Value);
            }

        }


        /// <summary>
        /// 5/17/2024-LYI
        /// 번호 섞기 기능 추가로 인한 번호 리타겟팅 기능 추가
        /// </summary>
        public void SetMixedStageData()
        {
            Dictionary<int, List<object>> dic_origin = csvLoader.ReadCSVDataDic(CSV_mixedStage);

            foreach (var item in dic_origin)
            {
                if (item.Value.Count > 1)
                {
                    int num = System.Convert.ToInt32(item.Value[1]);
                    dic_mixedToStage.Add(item.Key, num);
                    dic_StageToMixed.Add(num, item.Key);
                }
            }

            Debug.Log("Mixed setting complete:" + dic_mixedToStage.Count);
        }

    }
}