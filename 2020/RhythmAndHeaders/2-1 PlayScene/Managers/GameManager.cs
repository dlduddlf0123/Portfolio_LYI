using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LobbyUIManager uiMgr;
    public CSVparser parse = new CSVparser();

    private static GameManager s_instance = null;
    public static GameManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return s_instance;
        }
    }

    public Table table_songdata { get; set; }

    public AssetBundle b_csvdata;

    public int openSongNum;
    public int highScore;

    public float userSync = 0;

    List<List<object>> list__songdata = new List<List<object>>();

    public List<GameObject> characterList = new List<GameObject>();

    public GameObject selectedCharacter;
    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType(typeof(GameManager)).Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        b_csvdata = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "csv"));

        table_songdata = parse.ParsingCSV("songdata_list");
        
    }

    public string ReadSongData(int _row, int _col)
    {
        Table table = table_songdata;
        string _data = table.Row[_row].Col[_col].ToString();
        return _data;
        
   }

    public void LoadScene(int _scene)
    {   
        SceneManager.LoadScene(_scene);
    }
}
