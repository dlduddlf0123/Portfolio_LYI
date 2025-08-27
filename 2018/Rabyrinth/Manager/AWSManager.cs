using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Rabyrinth.ReadOnlys;
public class AWSManager : MonoBehaviour
{
    //public Text testText;

    private CognitoAWSCredentials credentials;
    private DynamoDBContext[] context;
    private AmazonDynamoDBClient[] ddbClient;

    private GameManager GameMgr;

    //private List<CharacterEntity> characterEntities = new List<CharacterEntity>();

    private string Id;

    //private DynamoDBContext PlayerDataContext
    //{
    //    get
    //    {
    //        if (_context == null)
    //            _context[0] = new DynamoDBContext(_client[0]);

    //        return _context[0];
    //    }
    //}

    //private DynamoDBContext PlayerAuthContext
    //{
    //    get
    //    {
    //        if (_context == null)
    //            _context[2] = new DynamoDBContext(_client[2]);

    //        return _context[2];
    //    }
    //}

    // Use this for initialization
    public void Awake()
    {
        context = new DynamoDBContext[4];
        ddbClient = new AmazonDynamoDBClient[4];

        GameMgr = MonoSingleton<GameManager>.Inst;

        Id = "1";

        UnityInitializer.AttachToGameObject(this.gameObject);

        credentials = new CognitoAWSCredentials(GameMgr.cognitoIdentityPoolString, RegionEndpoint.APNortheast2);

        GameMgr.Main_UI.SetLoading(true);

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
                                GoogleLogin();
#else
        CheckIdentity(() =>
        {
            InitDynamoDB(DDB_Table.GameDataTable, () =>
                LoadGameData());

            InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                LoadPlayerData());

            InitDynamoDB(DDB_Table.RankingDataTable, () =>
                SaveRankingData("NAME", 0.0f, 1, (_bool) => LoadRankingData()));
        });

        if(GameMgr.isRanking == 1)
            StartCoroutine(RankingDataLoad());
#endif
    }

    private IEnumerator StartRankingDataTable()
    {
        yield return new WaitForSeconds(5.0f);

        InitDynamoDB(DDB_Table.RankingDataTable, () =>
            SaveRankingData("NAME", 0.0f, 1, (_bool) => LoadRankingData()));

        if (GameMgr.isRanking == 1)
            StartCoroutine(RankingDataLoad());
    }


    public void GoogleLogin()
    {
        //testText.text = "로그인 시도";
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
        .Builder()
        .RequestIdToken()
        .RequestServerAuthCode(false)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(GoogleLoginCallback);
    }

    IEnumerator GetIdToken()
    {
        while (String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).GetIdToken()))
            yield return null;

        credentials.AddLogin("accounts.google.com", ((PlayGamesLocalUser)Social.localUser).GetIdToken());

        GameMgr.Main_UI.Log.text += "\n토큰 획득 및 AWS Auth";

        CheckIdentity(() =>
        {
            //StartCoroutine(SetRankingData());

            InitDynamoDB(DDB_Table.GameDataTable, () =>
                LoadGameData());

            InitDynamoDB(DDB_Table.AuthDataTable, () =>
            StartCoroutine(SetLogin()));

            StartCoroutine(StartRankingDataTable());

            if (GameMgr.isRanking == 1)
                StartCoroutine(RankingDataLoad());
        });
    }

    IEnumerator SetLogin()
    {
        while (String.IsNullOrEmpty(((PlayGamesLocalUser)Social.localUser).id))
            yield return null;

        GameMgr.Main_UI.Log.text += "\n아이디 획득 성공 : " + ((PlayGamesLocalUser)Social.localUser).id;

        Id = ((PlayGamesLocalUser)Social.localUser).id;

        //while (String.IsNullOrEmpty(PlayGamesPlatform.Instance.GetUserEmail()))
        //    yield return null;

        //GameMgr.Main_UI.Log.text += "\n아이디 획득 성공 : " + PlayGamesPlatform.Instance.GetUserEmail();

        //Id = PlayGamesPlatform.Instance.GetUserEmail();

        AuthOverLapCheck(Id);
    }

    private void GoogleLoginCallback(bool success)
    {
        if (success)
        {
            StartCoroutine(GetIdToken());
            GameMgr.Main_UI.Log.text += "\n구글 로그인 성공";
        }
        else
        {
            CheckIdentity(() =>
            {
                InitDynamoDB(DDB_Table.GameDataTable, () =>
                    LoadGameData());

                InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                    LoadPlayerData());

                StartCoroutine(StartRankingDataTable());
            });

            GameMgr.Main_UI.Log.text += "\n구글 로그인 실패";
            Debug.LogError("Google login failed. If you are not running in an actual Android/iOS device, this is expected.");
        }
    }

    private void CheckIdentity(Action _callBack)
    {
        credentials.GetIdentityIdAsync(delegate(AmazonCognitoIdentityResult<string> result)
        {
            if (result.Exception != null)
                return;

            GameMgr.Main_UI.Log.text += "\nAws Init 성공 : " + credentials.AccountId;
            Debug.Log("Aws Init 성공");

            _callBack();
        });
    }

    public void SaveAuthData(AuthData _data)
    {
        GameMgr.Main_UI.Log.text += "\nAuth 데이터 새로 저장 ";

        context[(int)DDB_Table.AuthDataTable].SaveAsync(_data, (result) =>
        {
            if (result.Exception == null)
            {
                GameMgr.Main_UI.Log.text += "성공";

                PlayerData data = GameMgr.PlayData.PlayerData != null ?
                    GameMgr.PlayData.PlayerData :
                    new PlayerData
                    {
                        ID = _data.ID,
                        SecurityID = Guid.NewGuid().ToString(),
                        Gem = 0,
                        Gold = 0,
                        KPM = 0,
                        MaxFloor = 1,
                        CurrentFloor = 1,
                        StatusPoint = 0,
                    };
                GameMgr.PlayData.SetPlayerData(data);

                SavePlayerData(GameMgr.PlayData.PlayerData);
            }
            else
            {
                GameMgr.Main_UI.Log.text += "실패";
                //SaveAuthData(_data);
                //testText.text = result.Exception.Message;
            }
        });
    }

    public void SavePlayerData(PlayerData _data)
    {
        if (_data == null)
        {
            string UnauthID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("Unauth", UnauthID);

            _data = new PlayerData
            {
                ID = UnauthID,
                SecurityID = Guid.NewGuid().ToString(),
                Gem = 0,
                Gold = 0,
                KPM = 0,
                MaxFloor = 1,
                CurrentFloor = 1,
                StatusPoint = 0,
            };
            GameMgr.PlayData.SetPlayerData(_data);
            SavePlayerDataSecu(_data);
        }
        else
        {
            context[(int)DDB_Table.PlayerDataTable].LoadAsync<PlayerData>(PlayerPrefs.GetString("Unauth"), (result) =>
            {
                if (result.Exception == null)
                {
                    GameMgr.Main_UI.Log.text += "\nSecurityID Load 성공";

                    if (result.Result.SecurityID == GameMgr.PlayData.SecurityID)
                        SavePlayerDataSecu(_data);
                    else {
                        SavePlayerDataSecu(_data, true);
                        System.Action action = ()=>
                        {
                            GameMgr.GameRestart();
                        };
                        GameMgr.Main_UI.popUpController.PopUpMiddle(",중,복, ,로,그,인, ,입,니,다,.,\n,게,임,을, ,재,시,작, ,합,니,다,.", action);
                    }
                }
                else {
                    GameMgr.Main_UI.Log.text += "\nSecurityID Load 실패";
                    //SavePlayerData(_data);
                    //GameMgr.GameRestart();
                    //testText.text = result.Exception.Message;
                }
            });
        }
    }

    public void SavePlayerDataSecu(PlayerData _data, bool isNotCallBack = false)
    {
        if (isNotCallBack)
            context[(int)DDB_Table.PlayerDataTable].SaveAsync(_data, null);
        else {
            context[(int)DDB_Table.PlayerDataTable].SaveAsync(_data, (result) =>
            {
                if (result.Exception == null)
                {
                    GameMgr.Main_UI.Log.text += "\n플레이어 데이터 저장 성공";

                    //GameMgr.PlayerData.SetPlayerData(_data);
                    //testText.text = "CreateOperation Successed!";
                }
                else
                {
                    CheckIdentity(() =>
                    {
                        InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                            SavePlayerDataSecu(_data));
                    });

                    GameMgr.Main_UI.Log.text = "\n플레이어 데이터 저장 실패 : " + result.Exception.Message;
                    //SavePlayerData(GameMgr.PlayData.PlayerData);
                    //testText.text = result.Exception.Message;
                }
            });
        }
    }

    public void SaveRankingData(string _name, float _time, int score, Action<bool> _callBack = null)
    {
        RankingData rankData = new RankingData { Name = _name, Time = _time, Score = score };
        context[(int)DDB_Table.RankingDataTable].SaveAsync(rankData, (result) =>
        {
            if (result.Exception == null)
            {
                GameMgr.Main_UI.Log.text += "\n랭킹 데이터 저장 성공";
                if (_callBack != null)
                    _callBack(true);
            }
            else
            {
                GameMgr.Main_UI.Log.text += "\n랭킹 데이터 저장 실패";
                //SaveRankingData(_name, score);
                //testText.text = result.Exception.Message;
                if (_callBack != null)
                    _callBack(false);
            }
        });
    }

    public void SaveGameData()
    {
        List<Skill> _lSkill = new List<Skill>();
        #region
        _lSkill.Add(new Skill
        {
            type = SkillType.Meteor,
            level = 1,
            cooltime = 6.0f,
            damage = 1.0f,
            range = 4.0f,
            time = 3.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.IceField,
            level = 1,
            cooltime = 6.0f,
            damage = 0.5f,
            range = 5.0f,
            time = 4.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.DimensionField,
            level = 1,
            cooltime = 8.0f,
            damage = 1.0f,
            range = 5.0f,
            time = 0.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.PlassmaSword,
            level = 1,
            cooltime = 20.0f,
            damage = 1.5f,
            range = 1.5f,
            time = 10.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.FlameSword,
            level = 1,
            cooltime = 20.0f,
            damage = 2.5f,
            range = 1.0f,
            time = 10.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.RecoveryField,
            level = 1,
            cooltime = 10.0f,
            damage = 0.02f,
            range = 6.0f,
            time = 5.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.SP_RecoveryField,
            level = 1,
            cooltime = 10.0f,
            damage = 1.25f,
            range = 6.0f,
            time = 5.0f,
        });

        _lSkill.Add(new Skill
        {
            type = SkillType.Barrier,
            level = 1,
            cooltime = 15.0f,
            damage = 0.5f,
            range = 0.0f,
            time = 5.0f,
        });
        #endregion
        GameData gameData = new GameData
        {
            Index = 0,
            lSkillData = _lSkill,
        };

        context[(int)DDB_Table.GameDataTable].SaveAsync(gameData, (result) =>
        {
            if (result.Exception == null)
            {
                GameMgr.Main_UI.Log.text += "\n게임 데이터 저장 성공";
                //GameMgr.PlayerData.SetPlayerData(_data);
                //testText.text = "CreateOperation Successed!";
            }
            else
            {
                GameMgr.Main_UI.Log.text += "\n게임 데이터 저장 실패";
                //testText.text = result.Exception.Message;
            }
        });

    }

    public void LoadRankingData(Action callBack = null)
    {
        Table.LoadTableAsync(ddbClient[(int)DDB_Table.RankingDataTable],
            GameMgr.ddbTableName[(int)DDB_Table.RankingDataTable], (loadTableResult) =>
            {
                if (loadTableResult.Exception != null)
                {
                    GameMgr.Main_UI.Log.text += "\n랭킹 데이터 테이블 로드 실패! ";
                    CheckIdentity(() =>
                    {
                        InitDynamoDB(DDB_Table.RankingDataTable, () =>
                            LoadRankingData());
                    });
                }
                else
                {
                    try
                    {
                        GameMgr.Main_UI.Log.text += "\n랭킹 검색 시작";

                        var search = context[(int)DDB_Table.RankingDataTable].ScanAsync<RankingData>(
                            new ScanCondition("Score", ScanOperator.GreaterThan, 0));

                        search.GetRemainingAsync(result =>
                        {
                            if (result.Exception != null)
                            {
                                GameMgr.Main_UI.Log.text = "랭킨 데이터 검색 실패 :" + result.Exception.Message;
                                System.Action action = () => { GameMgr.GameRestart(); };
                                GameMgr.Main_UI.popUpController.PopUpMiddle("인터넷 연결 오류 입니다.,\n게임을 재시작합니다.,\n스태프에게 문의해주세요", action);
                                return;
                            }
                            else
                            {
                                List<RankingData> list = new List<RankingData>(result.Result);
                                //list.Sort(delegate(RankingData l, RankingData r) { return r.Score.CompareTo(l.Score); });
                                //list.Sort(delegate (RankingData l, RankingData r) { return l.Time.CompareTo(r.Time); });
                                GameMgr.PlayData.lRankingData = list;

                                if (callBack != null)
                                {
                                    callBack();
                                    return;
                                }

                                list.Sort(delegate (RankingData lhs, RankingData rhs)
                                {
                                    int now = rhs.Score.CompareTo(lhs.Score);
                                    if (now == 0)
                                        return lhs.Time.CompareTo(rhs.Time);
                                    else
                                        return now;
                                });

                                if (result.Result.Count > 0)
                                {
                                    GameMgr.Main_UI.Log.text = "<size=40>\t닉네임</size>\t\t\t<color=aqua>- 지스타 이벤트 순위 -</color>\t\t\t<size=40>점수(플레이 시간)</size>";
                                    string start = "<color=magenta>";
                                    string end = "</color>";
                                    GameMgr.Main_UI.Log2.text = "";
                                    int count = list.Count > 9 ? 10 : list.Count;
                                    for (int index = 0; index < count; index++)
                                    {
                                        DateTime time = DateTime.MinValue + TimeSpan.FromSeconds(list[index].Time);
                                        GameMgr.Main_UI.Log.text += "\n" + start + "<size=45>" + (index + 1) + "위</size> " + list[index].Name + end;
                                        GameMgr.Main_UI.Log2.text += start + list[index].Score.ToString() + "(" + time.ToString("m:ss") + ")" + end + "\n";
                                        start = end = "";
                                    }
                                }
                            }
                        }, null);
                    }
                    catch (AmazonDynamoDBException exception)
                    {
                        Debug.Log(string.Concat("Exception fetching characters from table: {0}", exception.Message));
                        Debug.Log(string.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
                    }
                }
            });
    }
    public void LoadGameData()
    {
        context[(int)DDB_Table.GameDataTable].LoadAsync<GameData>(0, (result) =>
        {
            if (result.Exception == null)
            {
                GameMgr.Main_UI.Log.text += "\nGame Data Load 성공";
                if(GameMgr.PlayData.GameData == null)
                    GameMgr.PlayData.GameData = new GameData {
                        Index = result.Result.Index,
                        lSkillData = result.Result.lSkillData,
                        lCharData = result.Result.lCharData,
                    };
                else
                    GameMgr.PlayData.GameData = result.Result;
            }
            else
            {
                GameMgr.Main_UI.Log.text += "\nGame Data Load 실패 : " + result.Exception.Message;
                CheckIdentity(() =>
                {
                    InitDynamoDB(DDB_Table.GameDataTable, () =>
                        LoadGameData());
                });
                //GameMgr.GameRestart();
                //testText.text = result.Exception.Message;
            }
        });
    }

    public void LoadPlayerData()
    {
        context[(int)DDB_Table.PlayerDataTable].LoadAsync<PlayerData>(PlayerPrefs.GetString("Unauth"), (result) =>
        {
            if (result.Exception == null)
            {
                GameMgr.Main_UI.Log.text += "\nPlayer Data Load 성공";

                if (result.Result == null)
                    SavePlayerData(null);
                else
                {
                    GameMgr.Main_UI.Log.text += "\nSecurityID 발급";
                    result.Result.SecurityID = Guid.NewGuid().ToString();
                    GameMgr.PlayData.SetPlayerData(result.Result);
                    SavePlayerDataSecu(GameMgr.PlayData.PlayerData);
                }
            }
            else
            {
                GameMgr.Main_UI.Log.text += "\nPlayer Data Load 실패";
                CheckIdentity(() =>
                {
                    InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                        LoadPlayerData());
                });
                //LoadPlayerData();
                //GameMgr.GameRestart();
                //testText.text = result.Exception.Message;
            }
        });
    }

    public void AuthOverLapCheck(string _token)
    {
        Table.LoadTableAsync(ddbClient[(int)DDB_Table.AuthDataTable],
            GameMgr.ddbTableName[(int)DDB_Table.AuthDataTable], (loadTableResult) =>
        {
            if (loadTableResult.Exception != null)
            {
                GameMgr.Main_UI.Log.text += "\n인증 데이터 테이블 로드 실패!";
                CheckIdentity(() =>
                {
                    InitDynamoDB(DDB_Table.AuthDataTable, () =>
                        AuthOverLapCheck(_token));
                });
            }
            else
            {
                try
                {
                    GameMgr.Main_UI.Log.text += "\nAuth 중복체크";

                    var search = context[(int)DDB_Table.AuthDataTable].ScanAsync<AuthData>(new ScanCondition("Token", ScanOperator.Equal, _token));

                    search.GetRemainingAsync(result =>
                    {
                        if (result.Exception != null)
                            return;

                        if (result.Result.Count > 0)
                        {
                            //이미있음
                            GameMgr.Main_UI.Log.text += "\nAuth 중복";

                            context[(int)DDB_Table.AuthDataTable].LoadAsync<AuthData>(_token, (_result) =>
                            {
                                if (_result.Exception == null)
                                {
                                    //이미 Unauth가 로컬에 저장중이면 덮어쓸지 선택 기능추가 필요
                                    PlayerPrefs.SetString("Unauth", _result.Result.ID);

                                    InitDynamoDB(DDB_Table.PlayerDataTable, () => { LoadPlayerData(); });
                                }
                            });
                        }
                        else
                        {
                            GameMgr.Main_UI.Log.text += "\nAuth 중복아님";

                            InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                            {
                                IdOverLapCheck(PlayerPrefs.HasKey("Unauth") ?
                                    PlayerPrefs.GetString("Unauth") :
                                    Guid.NewGuid().ToString());
                            });
                        }
                    }, null);
                }
                catch (AmazonDynamoDBException exception)
                {
                    Debug.Log(string.Concat("Exception fetching characters from table: {0}", exception.Message));
                    Debug.Log(string.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
                }
            }
        });
    }

    private void InitDynamoDB(DDB_Table _type, Action _callBack = null)
    {
        if (context[(int)_type] == null)
        {
            ddbClient[(int)_type] = new AmazonDynamoDBClient(credentials, RegionEndpoint.APNortheast2);

            var request = new DescribeTableRequest
            {
                TableName = GameMgr.ddbTableName[(int)_type],
            };

            ddbClient[(int)_type].DescribeTableAsync(request, (ddbresult) =>
            {
                if (ddbresult.Exception != null)
                {
                    CheckIdentity(() =>
                    {
                        InitDynamoDB(_type, _callBack);
                    });
                    return;
                }

                GameMgr.Main_UI.Log.text += "\n" + GameMgr.ddbTableName[(int)_type] + " 연결 성공";

                context[(int)_type] = new DynamoDBContext(ddbClient[(int)_type]);

                if (_callBack != null)
                    _callBack();

            }, null);
        }
        else if (_callBack != null)
        {
            _callBack();
        }
    }

    public void IdOverLapCheck(string _id)
    {
        Table.LoadTableAsync(ddbClient[(int)DDB_Table.PlayerDataTable],
            GameMgr.ddbTableName[(int)DDB_Table.PlayerDataTable], (loadTableResult) =>
            {
                if (loadTableResult.Exception != null)
                {
                    GameMgr.Main_UI.Log.text += "\n플레이어 데이터 테이블 로드 실패!";
                    CheckIdentity(() =>
                    {
                        InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                            IdOverLapCheck(_id));
                    });
                }
                else
                {
                    try
                    {
                        GameMgr.Main_UI.Log.text += "\nID 중복체크";

                        var search = context[(int)DDB_Table.PlayerDataTable].ScanAsync<PlayerData>(
                            new ScanCondition("ID", ScanOperator.Equal, _id));

                        search.GetRemainingAsync(result =>
                        {
                            if (result.Exception != null)
                            {
                                CheckIdentity(() =>
                                {
                                    InitDynamoDB(DDB_Table.PlayerDataTable, () =>
                                        IdOverLapCheck(_id));
                                });
                            }

                            if (result.Result.Count > 0)
                            {
                                GameMgr.Main_UI.Log.text += "\nID 중복";

                                IdOverLapCheck(Guid.NewGuid().ToString());
                            }
                            else
                            {
                                GameMgr.Main_UI.Log.text += "\nID 중복아님";

                                PlayerPrefs.SetString("Unauth", _id);

                                AuthData _data = new AuthData
                                {
                                    Token = Id,
                                    ID = _id,
                                };
                                SaveAuthData(_data);
                            }
                        }, null);
                    }
                    catch (AmazonDynamoDBException exception)
                    {
                        Debug.Log(string.Concat("Exception fetching characters from table: {0}", exception.Message));
                        Debug.Log(string.Concat("Error code: {0}, error type: {1}", exception.ErrorCode, exception.ErrorType));
                    }
                }
            });
    }


    private IEnumerator RankingDataLoad()
    {
        GameMgr.Main_UI.Log.gameObject.SetActive(true);
        GameMgr.Main_UI.Log2.gameObject.SetActive(true);
        while (true)
        {
            yield return new WaitForSeconds(10.0f);

            LoadRankingData();
        }
    }
}