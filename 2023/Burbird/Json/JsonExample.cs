using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Burbird;
using System.IO;
using System.Text;

public class JsonExample : MonoBehaviour
{
    public List<string> list_enemy = new List<string>();
    public List<string> list_perk = new List<string>();
    public List<string> list_dropItem = new List<string>();

    public string[][] list_arr_enemyName = new string[5][];

    // Start is called before the first frame update
    void Start()
    {

        //JsonStage stage = new JsonStage();
        //stage.stageNum = 1;
        //stage.stageName = "Forest";
        //stage.maxRoom = 50;
        //stage.steminaForPlay = 5;
        //stage.list_enemy = list_enemy;
        //stage.list_perk = list_perk;
        //stage.list_dropItem = list_dropItem;

        for (int i = 0; i < 5; i++)
        {
            string[] arr_enemyName = new string[3];

            arr_enemyName[0] = list_enemy[0];
            arr_enemyName[1] = list_enemy[1];
            arr_enemyName[2] = list_enemy[2];

            list_arr_enemyName[i] =arr_enemyName;
        }

        string json = JsonUtility.ToJson(list_arr_enemyName);
        CreateJsonFile(Application.dataPath, "JsonExample1", json);

        //string json2 = LoadJsonFile(Application.dataPath, "JsonExample1");

        //JsonStage stage2 = LoadStageData(json2);
    }

    JsonStage LoadStageData(string jsonData)
    {
        return JsonUtility.FromJson<JsonStage>(jsonData);
    }
    JsonStage LoadStageData(TextAsset textAsset)
    {
        return JsonUtility.FromJson<JsonStage>(textAsset.text);
    }

    void CreateJsonFile(string path, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", path, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    string LoadJsonFile(string path, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", path, fileName), FileMode.Open);

        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return jsonData;
    }
}
