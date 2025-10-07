using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace AroundEffect
{

    public enum MiniGameStatus
    {
        NONE = 0,
        READY,
        GAME,
        RESULT,
    }


    /// <summary>
    /// 12/4/2024-LYI
    /// 물건 피하기 미니게임
    /// </summary>
    public class MiniGameManager : MonoBehaviour
    {
        GameManager gameMgr;

        [Header("Character")]
        public MiniGameCharacter mini_character;
        public Transform[] arr_movePos;


        [Header("UI")]
        public TextMeshPro txt_score;
        public TextMeshPro txt_highScore;

        public Button btn_start;
        public Button btn_exit;



        [Header("ObjectPooling")]
        public List<GameObject> list_obstacleOrigin;

        public List<GameObject> list_disable;
        public Transform tr_disable;
        public Transform tr_active;

        public Transform[] arr_tr_spawn;



        [Header("Property")]
        public MiniGameStatus statMiniGame;



        public float spawnTime = 2f;
        public int gameScore = 0;

        int highScore;
        float spawnTimeMax = 3f;
        float spawnTimeMin = 0.5f;

        bool isInit = false;

        public void MiniGameInit()
        {
            if (!isInit)
            {
                gameMgr = GameManager.Instance;

                btn_start.onClick.AddListener(MiniGameStart);
                btn_exit.onClick.AddListener(MiniGameEnd);


                highScore = ES3.Load("MiniGame_HighScore", 0);
                ChangeHighScoreText(highScore);


                isInit = true;
            }

            spawnTime = spawnTimeMax;
            gameScore = 0;
            ChangeScoreText(gameScore);
            SetActiveMenuButton(true);

            mini_character.transform.position = arr_movePos[1].position;
            mini_character.transform.rotation = arr_movePos[1].rotation;
            mini_character.currentPos = 1;
        }



        Coroutine spawnCoroutine = null;

        /// <summary>
        /// 12/4/2024-LYI
        /// 미니게임 시작 시 호출
        /// 오브젝트 스폰 시작
        /// </summary>
        public void MiniGameStart()
        {
            MiniGameInit();
            SetActiveMenuButton(false);

            statMiniGame = MiniGameStatus.GAME;

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }

            spawnCoroutine = StartCoroutine(ObstacleSpawn());
        }


        /// <summary>
        /// 12/4/2024-LYI
        /// 캐릭터 사망 시 호출
        /// 결과 창 보여주기, 다시하기와 나가기 버튼
        /// </summary>
        public void MiniGameResult()
        {
            SetActiveMenuButton(true);
            statMiniGame = MiniGameStatus.RESULT;

            mini_character.OnGameOver();


            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }

            if (gameScore > highScore)
            {
                highScore = gameScore;
                ChangeHighScoreText(highScore);
                ES3.Save("MiniGame_HighScore", highScore);
            }

        }

        /// <summary>
        /// 12/4/2024-LYI
        /// 원래 육성 플레이로 돌아가기
        /// </summary>
        public void MiniGameEnd()
        {
            gameMgr.lifeMgr.EndMiniGame();

        }

        IEnumerator ObstacleSpawn()
        {
            while (statMiniGame == MiniGameStatus.GAME)
            {
                yield return new WaitForSeconds(spawnTime);

                int randomObj = Random.Range(0, list_obstacleOrigin.Count);
                int randomPoint = Random.Range(0, arr_tr_spawn.Length);
                
                gameMgr.objPoolingMgr.CreateObject(list_disable, list_obstacleOrigin[randomObj],  arr_tr_spawn[randomPoint].position, tr_active);
                GetScore(100);

                if (spawnTime > spawnTimeMin)
                {
                    spawnTime -= 0.05f;
                }
                else
                {
                    spawnTime = spawnTimeMin;
                }

            }
        }

        public void ObjectReset(GameObject go)
        {
            gameMgr.objPoolingMgr.ObjectInit(list_disable, go, tr_disable);
        }

        public void GetScore(int score)
        {
            gameScore += score;
            ChangeScoreText(gameScore);
        }


        public void ChangeScoreText(int score)
        {
            txt_score.text = "Score: " + score.ToString();
        }

        public void ChangeHighScoreText(int score)
        {
            txt_highScore.text = "High Score: " + score.ToString();
        }

        public void SetActiveMenuButton(bool isActive)
        {
            btn_start.gameObject.SetActive(isActive);
            btn_exit.gameObject.SetActive(isActive);
        }
    }
}