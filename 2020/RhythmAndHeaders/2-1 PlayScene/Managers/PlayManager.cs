using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;


public class PlayManager : MonoBehaviour
{
    public GameManager gameMgr;
    public SoundManager soundMgr;
    public UIManager uiMgr;
    public InputManager[] inputMgr; //0 = 왼쪽, 1 = 오른쪽
    public JudgeCanvas judgeUI;

    public AudioSource mAudio;
    public Camera currentCam;
    public Camera[] cameraList;

    public Transform platformTr;
    public GameObject[] platform;

    public GameObject character;
    public Character player;

    public Checker check;

    CSVparser parse = new CSVparser();
    //노트 싱크관련 변수
    public int maxLine = 3;
    public float bpm = 60;

    public float noteSync = 0f;
    public float startTime = 0f;
    public float platformSpeed = 1;
    public float platformDistance;//7.5 = 60BPM

    private int platformCount;

    //판정 관련 변수
    public int score = 0;
    public int combo = 0;
    public float judgeThreshold = 4;

    public int count_fantastic = 0;
    public int count_perfect = 0;
    public int count_great = 0;
    public int count_good = 0;
    public int count_bad = 0;
    public int count_miss = 0;
    public int count_item = 0;
    public int count_note = 0;
    //연출관련 변수
    public ParticleSystem[] particles;

    public Material feverSkybox;
    public Material defaultSkybox;

    public int feverPoint = 0;
    public bool isFever = false;
    public bool isLongNote = false;
    public bool isTouched;

    public AudioClip[] songlist = new AudioClip[2];
    public bool isPaused;
    [Header("- Transform")]
    public Transform startPosition;
    public List<Transform> guidePoint = new List<Transform>();  //판정선 위치를 public으로 받는다.
    public Transform hitChecker;

    public Stack<GameObject> notePool = new Stack<GameObject>();

    public GameObject currentLongnote;
    public GameObject currentNote;
    public GameObject doubleNoteHit;
    public int isCheckingNote =0; // 0:일반노트 1:롱노트

    public Transform remembertransform;

    public AutoPlaying auto;

    //7.5f/(bpm/60) = 간격

    //싱글톤 선언
    private static PlayManager s_instance = null;
    public static PlayManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(PlayManager)) as PlayManager;
            }
            return s_instance;
        }
    }


    void Awake()
    {
        if (FindObjectsOfType(typeof(PlayManager)).Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        gameMgr = GameManager.Instance;

        Init();

        particles = new ParticleSystem[this.transform.GetChild(3).childCount];
        for (int idx = 0; idx < particles.Length; idx++)
        {
            particles[idx] = this.transform.GetChild(3).GetChild(idx).GetComponent<ParticleSystem>();
        }
    }
    public void Init() //곡정보, 캐릭터 정보 받아와서 인게임 실행시 시작할때 실행 -> gameManager에서 준 값으로 다 초기화
    {
        //곡 관련 데이터 읽기
        list__note = ReadNoteDatas();
        bpm = float.Parse(list__note[5][1].ToString());
        startTime = float.Parse(list__note[6][1].ToString());
        GetComponent<AudioSource>().clip = songlist[int.Parse(list__note[7][1].ToString())];
        //캐릭터 초기화
        GameObject go = Instantiate(gameMgr.selectedCharacter,
            startPosition.position,
            Quaternion.Euler(new Vector3(0, 71, 0)),
            character.transform);
        player = go.GetComponent<Character>();
        Debug.Log(go.transform.position);
    }

    private void Start()
    {
        StartCoroutine(StartCount());
    }
    public IEnumerator StartCount()
    {
        platformDistance = (bpm / 60);   //BPM을 노트의 거리로 환산,bpm csv 파일 읽어올 때 코드 ->platformDistance  = (gameMgr.uiMgr.currentsongNum,1) / 60;
        MakeNote();
        for (int i = 0; i < 10; i++)  //시작하자마자 보여줄 노트 개수 설정
        {
            PopNote();
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(PlatformMove());
        mAudio.Play();
    }

    /// <summary>
    /// 노트 생성, 생성된 노트를 notePool 배열(스택)에 넣은 뒤 게임 시작 시 활성화
    /// </summary>
    public void MakeNote()
    {
        platformCount = list__note[1].Count - 1;

        platformTr.position = (Vector3.left * (25 + platformSpeed)) +   //위치 0으로 만들기
            Vector3.right * platformSpeed * platformDistance * startTime +  //곡 시작 시간만큼 오른쪽으로 이동
            Vector3.right * gameMgr.userSync*(0.02f);   //노트 싱크 조절

        GameObject obj;
        obj = Instantiate(platform[3], platformTr.GetChild(0));
        obj.transform.localPosition = new Vector3(platformCount * platformSpeed / 2, obj.transform.localScale.y * 0.5f, 0f);


        while (platformCount > 0)
        {
            GameObject go;
            if (list__note[1][platformCount].ToString() == "ㅇ")
            {
                go = Instantiate(platform[1], platformTr.GetChild(0));
                SettingNote(1, go);
            }
            if (list__note[2][platformCount].ToString() == "ㅇ")
            {
                go = Instantiate(platform[1], platformTr.GetChild(0));
                SettingNote(2, go);
            }
            if (list__note[3][platformCount].ToString() == "ㅇ")
            {
                go = Instantiate(platform[1], platformTr.GetChild(0));
                SettingNote(3, go);
            }
            if (list__note[4][platformCount].ToString() == "ㅇ")
            {
                go = Instantiate(platform[1], platformTr.GetChild(0));
                SettingNote(4, go);
            }
            if (list__note[1][platformCount].ToString() != "ㅇ" && list__note[1][platformCount].ToString() != "")
            {
                if (platformCount != list__note[1].Count - 1)
                {
                    go = Instantiate(platform[4], platformTr.GetChild(0));
                    SettingLongNote(1, go);
                }
            }
            if (list__note[2][platformCount].ToString() != "ㅇ" && list__note[2][platformCount].ToString() != "")
            {
                if (platformCount != list__note[1].Count - 1)
                {
                    go = Instantiate(platform[4], platformTr.GetChild(0));
                    SettingLongNote(2, go);
                }
            }
            if (list__note[3][platformCount].ToString() != "ㅇ" && list__note[3][platformCount].ToString() != "")
            {
                if (platformCount != list__note[1].Count - 1)
                {
                    go = Instantiate(platform[4], platformTr.GetChild(0));
                    SettingLongNote(3, go);
                }
            }
            if (list__note[4][platformCount].ToString() != "ㅇ" && list__note[4][platformCount].ToString() != "")
            {
                if (platformCount != list__note[1].Count - 1)
                {
                    go = Instantiate(platform[4], platformTr.GetChild(0));
                    SettingLongNote(4, go);
                }
            }
            if (list__note[1][platformCount].ToString() == "end")
            {
                go = Instantiate(platform[3], platformTr.GetChild(0));
                go.transform.localPosition = new Vector3(platformCount * platformSpeed / 2, go.transform.localScale.y * 0.5f, 11f);
            }

            platformCount--;
        }
        Debug.Log("생성된 노트 수 : " + count_note);

    }
    /// <summary>
    /// 스택에서 노트를 꺼내서 활성화시키는 함수
    /// </summary>
    public void PopNote()
    {
        if (notePool.Count > 0)
        {
            notePool.Pop().SetActive(true);
        }
    }

    public IEnumerator PlatformMove()
    {
        while (true)
        {
            platformTr.position -= Vector3.right * platformSpeed * platformDistance * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }


    public void SyncLeft()
    {
        platformTr.position -= Vector3.right;
        Debug.Log("SyncLeft");
    }

    public void SyncRight()
    {
        platformTr.position += Vector3.right;
        Debug.Log("SyncRight");
    }

    /// <summary>
    /// 점수 획득
    /// </summary>
    public void GetPoint(int judgeScore, bool fever)
    {
        //콤보 상승
        combo++;
        uiMgr.SetComboText(combo);
        if (feverPoint >= 15 && isFever == false)
        {
            //StartCoroutine(FeverTime());
            FeverOn();
        }

        if (isFever == true)
        {
            score = score + (int)(judgeScore * 2);
        }
        else
        {
            score += judgeScore;
        }
        uiMgr.SetScoreText(score);



        //체력 회복
        if (player.status.hp < player.status.maxHP)
        {
            player.status.hp += 5;
            if (player.status.hp > player.status.maxHP)
            {
                player.status.hp = player.status.maxHP;
            }
            uiMgr.SetHPGauge(player.status.hp);
        }
    }

    /// <summary>
    /// 물체 지나갔을 때
    /// </summary>
    public void Pass()
    {
        count_miss++;
        judgeUI.ChangeJudgeText(5);
        combo = 0;
        uiMgr.SetComboText(combo);
        if(isFever==true)
        {
            FeverOff();
        }
    }

    List<List<object>> list__note = new List<List<object>>();
    /// <summary>
    /// CSV파일 노트 읽어오기
    /// </summary>   
    public List<List<object>> ReadNoteDatas()
    {
        Table table;
        table = parse.ParsingCSV(gameMgr.ReadSongData(gameMgr.uiMgr.currentSongNum, 0));
        List<List<object>> _datas = new List<List<object>>();
        for (int i = 0; i < table.Row.Count; i++)
        {
            List<object> _data = table.Row[i].Col;
            _datas.Add(_data);
        }
        return _datas;
    }
    /// <summary>
    /// 파티클 재생 함수
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_p"></param>
    public void PlayEffect(Vector3 _position, ParticleSystem _p)
    {
        _p.transform.position = _position;
        _p.Play();
    }
    /// <summary>
    /// 피버 타임 발생 함수
    /// </summary>
    /// <returns></returns>
/*    public IEnumerator FeverTime()
    {
        isFever = true;
        //uiMgr.bt_item.gameObject.SetActive(true);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        currentCam.fieldOfView = 30;
        CameraChangeToFever();
        particles[4].gameObject.SetActive(true); //트레일 이펙트
        RenderSettings.skybox = feverSkybox;
        currentCam = cameraList[1];
        feverPoint = 0;
        StartCoroutine(uiMgr.FeverTimeUI());
        yield return new WaitForSeconds(4.8f);
        isFever = false;
        yield return new WaitForSeconds(0.2f);
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 71, 0));
        currentCam = cameraList[0];
        currentCam.fieldOfView = 60;
        Camera.main.transform.position = remembertransform.position;
        Camera.main.transform.rotation = remembertransform.rotation;
        particles[4].gameObject.SetActive(false);
        RenderSettings.skybox = defaultSkybox;
        //uiMgr.feverTime.gameObject.SetActive(false);
    }*/
    /// <summary>
    ///게임 중 일시정지 기능
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;

    }
    /// <summary>
    /// Pause 후 카운트다운
    /// </summary>
    public void Play()
    {
        uiMgr.CountDown.gameObject.SetActive(true);
    }
    public void SettingNote(int lineNum, GameObject go)
    {
        if (lineNum == 1 || lineNum == 3)
        {
            //go.GetComponent<Renderer>().material.color = Color.red;
            go.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else if(lineNum==2||lineNum ==4)
            go.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = Color.blue;
            //go.GetComponent<Renderer>().material.color = Color.blue;

        go.transform.localPosition = new Vector3(platformCount * platformSpeed / 2, guidePoint[lineNum-1].position.y, guidePoint[lineNum-1].position.z);
        go.GetComponent<Note>().checkPosition = guidePoint[lineNum-1];
        go.GetComponent<Note>().lineNum = lineNum-1;
        count_note++;
        notePool.Push(go);
        go.SetActive(false);
    }
    public void SettingLongNote(int lineNum,GameObject go)
    {
        go.GetComponent<LongNote>().beatCount = int.Parse(list__note[lineNum][platformCount].ToString());
        go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
        go.transform.localPosition = new Vector3(platformCount * platformSpeed / 2, guidePoint[lineNum-1].position.y, guidePoint[lineNum-1].position.z);
        go.GetComponent<LongNote>().checkPosition = guidePoint[lineNum-1];
        go.GetComponent<LongNote>().lineNum = lineNum-1;
        count_note += go.GetComponent<LongNote>().beatCount;
        notePool.Push(go);
        go.SetActive(false);
    }

    IEnumerator FeverCamera(Transform _transform)
    {
        Transform cam = Camera.main.transform;
        while(isFever)
        {
            cam.position = Vector3.Lerp(cam.position, _transform.position, Time.deltaTime * 7);
            cam.rotation = Quaternion.Lerp(cam.rotation, _transform.rotation, Time.deltaTime * 7);
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void CameraChangeToFever()
    {
        StartCoroutine(FeverCamera(cameraList[1].transform));
    }
    public void CameraChangeToNormal()
    {
        StartCoroutine(FeverCamera(remembertransform));
    }
    public void FeverOn()
    {
        isFever = true;
        //uiMgr.bt_item.gameObject.SetActive(true);
        uiMgr.FeverTimeUI();
        player.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        currentCam.fieldOfView = 30;
        CameraChangeToFever();
        particles[4].gameObject.SetActive(true); //트레일 이펙트
        RenderSettings.skybox = feverSkybox;
        currentCam = cameraList[1];
        feverPoint = 0;
        StartCoroutine(uiMgr.FeverTimeUI());
    }
    public void FeverOff()
    {            
            isFever = false;
            player.transform.rotation = Quaternion.Euler(new Vector3(0, 71, 0));
            currentCam = cameraList[0];
            currentCam.fieldOfView = 60;
            Camera.main.transform.position = remembertransform.position;
            Camera.main.transform.rotation = remembertransform.rotation;
            particles[4].gameObject.SetActive(false);
            RenderSettings.skybox = defaultSkybox;       
    }
    public void AutoPlaying()
    {
        if(this.GetComponent<AutoPlaying>().enabled == false)
            this.GetComponent<AutoPlaying>().enabled = true;
    }
}