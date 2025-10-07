using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pathfinding;

namespace AroundEffect
{

    /// <summary>
    /// 8/22/2024-LYI
    /// 생활 관련 컨텐츠 관리
    /// 캐릭터 관련 정보 홀딩? -> 캐릭터 매니저??
    /// 캐릭터 AI 동작 지시
    /// 생활 관련 인벤토리, UI 관리
    /// 생활 컨텐츠에서 화면 전환 관리 
    /// </summary>
    public class LifeManager : MonoBehaviour
    {
        GameManager gameMgr;

        public LifeUIManager lifeUIMgr;
        public ItemSpawner itemSpawner;


        public CharacterManager[] arr_character;
        public CharacterManager currentCharacter; //현재 선택된 캐릭터

        public AstarPath astarPath;
        public Transform[] arr_headersMovePoints;

        public MiniGameManager miniGameMgr;
        public GameObject lifePlayMap;

        public Bed life_bed;
        public Portal life_portal;

        private void Awake()
        {
            LifeInit();
        }

        public void LifeInit()
        {
            gameMgr = GameManager.Instance;

            itemSpawner.SpawnerInit();
            SetNavigationGraph();
        }


        /// <summary>
        /// 11/13/2024-LYI
        /// 입력 부분에서 캐릭터 선택 시 호출
        /// 선택될 시 해당 처리
        /// </summary>
        /// <param name="charMgr"></param>
        public void OnCharacterSelect(CharacterManager charMgr)
        {
            //선택된 캐릭터가 손 위에있으면
            if (charMgr.Gesture.isOnHand)
            {
                return;
            }
            currentCharacter = charMgr;

            currentCharacter.AI.OnSelect();
        }


        /// <summary>
        /// 9/3/2024-LYI
        /// 뭔가 입력 쪽에서 트리거 됐을 때 작동
        /// 캐릭터 정보 중 Type으로 긁어오기
        /// Enum Type과 Array 번호를 같게 해서 변형 가능하도록 할 것 1~6
        /// </summary>
        /// <param name="type"></param>
        public void SelectCharacter(HeaderType type)
        {
           // currentCharacter = 

            switch (type)
            {
                case HeaderType.KANTO:
                    break;
                case HeaderType.ZINO:
                    break;
                case HeaderType.OODADA:
                    break;
                case HeaderType.COCO:
                    break;
                case HeaderType.DOINK:
                    break;
                case HeaderType.TENA:
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// 12/3/2024-LYI
        /// Call from promise
        /// 약속 이벤트 이후 새 캐릭터 호출
        /// </summary>
        public void SpawnCharacter()
        {
            int a = 0;
            for (int i = 0; i < arr_character.Length; i++)
            {
                if (arr_character[i].gameObject.activeInHierarchy == false)
                {
                    a = i;
                    break;
                }
            }


            life_portal.SpawnCharacter(arr_character[a]);
        }


        /// <summary>
        /// 12/4/2024-LYI
        /// 미니게임 시작
        /// </summary>
        public void StartMiniGame()
        {
            gameMgr.statGame = GameStatus.MINIGAME;
            lifePlayMap.SetActive(false);
            miniGameMgr.gameObject.SetActive(true);

            miniGameMgr.MiniGameInit();
        }


        /// <summary>
        /// 12/4/2024-LYI
        /// 미니게임 끝
        /// </summary>
        public void EndMiniGame()
        {
            gameMgr.statGame = GameStatus.LIFE;
            lifePlayMap.SetActive(true);
            miniGameMgr.gameObject.SetActive(false);

            miniGameMgr.MiniGameInit();


            SetNavigationGraph();

            for (int i = 0; i < arr_character.Length; i++)
            {
                if (arr_character[i].gameObject.activeInHierarchy)
                {
                    arr_character[i].AI.AIMove(AIState.IDLE);
                }
            }
        }


        public void SetNavigationGraph()
        {
            if (astarPath.data.recastGraph != null)
            {
                astarPath.enabled = true;
                RecastGraph recastGraph = astarPath.data.recastGraph;

                recastGraph.SnapBoundsToScene();
                //recastGraph.forcedBoundsCenter = transform.position;
                //recastGraph.forcedBoundsSize = new Vector3(recastGraph.forcedBoundsSize.x, 1, recastGraph.forcedBoundsSize.z);

                Debug.Log("RecastGraph Scan: " + recastGraph.forcedBoundsCenter + " / " + transform.position);
                recastGraph.Scan();

            }

        }

    }
}