using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

public class CGameTable : MonoBehaviour {

    // common
    public TableInfo_system[] kInfo_system = null;
    public TableInfo_text[] kInfo_text = null;
	public TableInfo_charic[] kInfo_charic = null;

    public TableInfo_sound[] kInfo_sound = null;
    public TableInfo_fx[] kInfo_fx = null;
    //public TableInfo_levelup[] kInfo_levelup = null;
    //
    //public TableInfo_mission[] kInfo_mission = null;
    //public TableInfo_item[] kInfo_item = null;
    //public TableInfo_skill[] kInfo_skill = null;
    //public TableInfo_shop[] kInfo_shop = null;


    private static CGameTable s_Instance = null;

    public static CGameTable Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance //= new CGameFx();				
                    = FindObjectOfType(typeof(CGameTable)) as CGameTable;
            }
            return s_Instance;
        }
    }
    // Use this for initialization
    void Start () {
		
	}

    public void InitTableData()
    {
        Load_TableInfo_text();
		Load_TableInfo_charic();
        Load_TableInfo_sound();
        //Load_TableInfo_fx();
    }

    // Update is called once per frame
    void Update () {
		
	}

    TextAsset LoadTextAsset(string _txtFile)
    {
        TextAsset ta;
        ta = Resources.Load("table/" + _txtFile) as TextAsset;
        return ta;
    }

    // csv -------------------------------------------------------------
    public void ReadCSV()
    {
        // Simlpe CSV - Split() 메서드를 이용하여 ',' 구분하여 잘라냄.
        //StreamReader sr = new StreamReader("csvtest.csv", Encoding.GetEncoding("euc-kr"));
        //while (!sr.EndOfStream)
        //{   
        //    string line = sr.ReadLine();
        //    string[] Cells = line.Split(','); //',' 구분하여 잘라냄
        //    Console.WriteLine("{0},{1}", Cells[0], Cells[1]);
        //}
        //sr.Close();

        // 문장사용할때 - CR,LF 사용시 - // 엑셀 유니코드 TXT
        StreamReader sr = new StreamReader("csvtest.txt", Encoding.GetEncoding("euc-kr"));
        List<string> line = LineSplit(sr.ReadToEnd());
        sr.Close();
        for (int i = 0; i < line.Count; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            for (int j = 0; j < Cells.Length; j++)
            {
                if (Cells[j] == "") continue;

                //Console.WriteLine("Cell : " + Cells[j]);
            }
        }
    }

    //엑셀 유니코드 TXT 저장, "" 가 있는 경우, 콤마 CR LF등 사용시. //느림.
    public List<string> LineSplit(string text)
    {
        //Console.WriteLine("LineSplit " + text.Length);

        char[] text_buff = text.ToCharArray();

        List<string> lines = new List<string>();

        int linenum = 0;
        bool makecell = false;

        StringBuilder sb = new StringBuilder("");

        for (int i = 0; i < text.Length; i++)
        {
            char c = text_buff[i];
            //int value = Convert.ToInt32(c); Console.WriteLine(String.Format("{0:x4}", value) + " " + c.ToString());

            if (c == '"')
            {
                char nc = text_buff[i + 1];
                if (nc == '"') { i++; } //next char
                else
                {
                    if (makecell == false) { makecell = true; c = nc; i++; } //next char
                    else { makecell = false; c = nc; i++; } //next char
                }
            }

            //0x0a : LF ( Line Feed : 다음줄로 캐럿을 이동 '\n')
            //0x0d : CR ( Carrage Return : 캐럿을 제일 처음으로 복귀 )			    
            if (c == '\n' && makecell == false)
            {
                char pc = text_buff[i - 1];
                if (pc != '\n')	//file end
                {
                    lines.Add(sb.ToString()); sb.Remove(0, sb.Length);
                    linenum++;
                }
            }
            else if (c == '\r' && makecell == false)
            {
            }
            else
            {
                sb.Append(c.ToString());
            }
        }

        return lines;
    }

    //-----------------------------------------------------------------------
    public TableInfo_system Get_TableInfo_system(int _index)
    {
        foreach (TableInfo_system kInfo in kInfo_system)
        {
            if (kInfo != null)
                if (kInfo.iID == _index) return kInfo;
        }
        return null;
    }

    void Load_TableInfo_system()
    {
        if (kInfo_system != null) return;

        // 문장사용할때 - CR,LF 사용시 - // 엑셀 유니코드 TXT
        //StreamReader sr = new StreamReader("table/system.txt", Encoding.GetEncoding("euc-kr"));
        //List<string> line = LineSplit(sr.ReadToEnd());
        //sr.Close();

        string txtFilePath = "system";
        TextAsset ta = LoadTextAsset(txtFilePath);
        String[] line = ta.text.Split('\n'); // line Split


        TableInfo_system[] kInfo = new TableInfo_system[line.Length];

        for (int i = 0; i < line.Length; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;
            //for (int j = 0; j < Cells.Length; j++)    System.Diagnostics.Trace.WriteLine(" LoadDataTable : " + Cells[j], ">>>>>");

            kInfo[i - 1] = new TableInfo_system();
            kInfo[i - 1].iID = int.Parse(Cells[0]);
            //kInfo[i-1].iID	    = Cells[1] ;
            kInfo[i - 1].fValue = float.Parse(Cells[2]);
        }

        kInfo_system = kInfo;
    }
    //-----------------------------------------------------------------------
    public TableInfo_text Get_TableInfo_text(int _index)
    {
        foreach (TableInfo_text kInfo in kInfo_text)
        {
            if (kInfo != null)
                if (kInfo.iID == _index) return kInfo;
        }
        return null;
    }

    void Load_TableInfo_text()
    {
        if (kInfo_text != null) return;

        // 문장사용할때 - CR,LF 사용시 - // 엑셀 유니코드 TXT
        //StreamReader sr = new StreamReader("text_l.txt", Encoding.GetEncoding("euc-kr"));
        //List<string> line = LineSplit(sr.ReadToEnd());
        //sr.Close();

        string txtFilePath = "text";
        TextAsset ta = LoadTextAsset(txtFilePath);
        //String[] line = ta.text.Split('\n'); // line Split
        List<string> line = LineSplit(ta.text);

        TableInfo_text[] kInfo = new TableInfo_text[line.Count];

        for (int i = 0; i < line.Count; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;
            //for (int j = 0; j < Cells.Length; j++)    System.Diagnostics.Trace.WriteLine(" LoadDataTable : " + Cells[j], ">>>>>");

            kInfo[i - 1] = new TableInfo_text();

            kInfo[i - 1].iID = int.Parse(Cells[0]);
            kInfo[i - 1].Korean = Cells[1];
            kInfo[i - 1].English = Cells[2];
            kInfo[i - 1].Japanese = Cells[3];
            kInfo[i - 1].Chinese = Cells[4];
        }
        kInfo_text = kInfo;
    }

	//-----------------------------------------------------------------------
	public TableInfo_charic Get_TableInfo_charic(int _index)
	{
		foreach (TableInfo_charic kInfo in kInfo_charic)
		{
			if (kInfo != null)
			if (kInfo.index == _index) return kInfo;
		}
		return null;
	}

	void Load_TableInfo_charic()
	{
		if (kInfo_charic != null) return;

		string txtFilePath = "charic";
		TextAsset ta = LoadTextAsset(txtFilePath);
		List<string> line = LineSplit(ta.text);

		TableInfo_charic[] kInfo = new TableInfo_charic[line.Count];

		for (int i = 0; i < line.Count; i++)
		{
			//Console.WriteLine("line : " + line[i]);
			if (line[i] == null) continue;
			if (i == 0) continue; 	// Title skip

			string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
			if (Cells[0] == "") continue;
			//for (int j = 0; j < Cells.Length; j++)    System.Diagnostics.Trace.WriteLine(" LoadDataTable : " + Cells[j], ">>>>>");

			kInfo[i - 1] = new TableInfo_charic();

			kInfo[i - 1].index = int.Parse(Cells[0]);
			kInfo[i - 1].name = Cells[1];
			kInfo[i - 1].resource = Cells[2];
			kInfo[i - 1].type = int.Parse(Cells[3]);
			kInfo[i - 1].speed = float.Parse(Cells[4]);
			kInfo[i - 1].hp = int.Parse(Cells[5]);
			kInfo[i - 1].ap = int.Parse(Cells[6]);
			kInfo[i - 1].aspeed = float.Parse(Cells[7]);
			kInfo[i - 1].dp = int.Parse(Cells[8]);
			kInfo[i - 1].mana = int.Parse(Cells[9]);
			kInfo[i - 1].group = int.Parse(Cells[10]);
			kInfo[i - 1].distance = float.Parse(Cells[11]);
			kInfo[i - 1].target_type = int.Parse(Cells[12]);
			kInfo[i - 1].target_count = int.Parse(Cells[13]);
			kInfo[i - 1].target_range = float.Parse(Cells[14]);
			kInfo[i - 1].projectile = int.Parse(Cells[15]);
			kInfo[i - 1].hit_time = float.Parse(Cells[16]);
			kInfo[i - 1].fx = int.Parse(Cells[17]);
			kInfo[i - 1].sound = int.Parse(Cells[18]);
		}
		kInfo_charic = kInfo;
	}


    //sound --------------------------------------------------------------------------------------
    public TableInfo_sound Get_TableInfo_sound(int _index)
    {
        foreach (TableInfo_sound kInfo in kInfo_sound)
        {
            if (kInfo != null) if (kInfo.iID == _index) return kInfo;
        }
        return null;
    }
    void Load_TableInfo_sound()
    {
        if (kInfo_sound != null) return;

        string txtFilePath = "sound";
        TextAsset ta = LoadTextAsset(txtFilePath);
        String[] line = ta.text.Split('\n'); // line Split

        TableInfo_sound[] kInfo = new TableInfo_sound[line.Length];

        for (int i = 0; i < line.Length; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;

            //for(int i=0; i < Cells.Length ; i++) {	Debug.Log("Cell"+i+"="+Cells[i]); }
            //-----------------------------------------------------			
            kInfo[i - 1] = new TableInfo_sound();

            kInfo[i - 1].iID = int.Parse(Cells[0]);
            //kInfo[i-2].sDesc		= Cells[1];
            kInfo[i - 1].sResource = Cells[2];
            kInfo[i - 1].iType = int.Parse(Cells[3]);
            kInfo[i - 1].iVolume = int.Parse(Cells[4]);
            kInfo[i - 1].iLoop = int.Parse(Cells[5]);
        }

        kInfo_sound = kInfo;
    }

    //fx --------------------------------------------------------------------------------------
    public TableInfo_fx Get_TableInfo_fx(int _index)
    {
        foreach (TableInfo_fx kInfo in kInfo_fx)
        {
            if (kInfo != null) if (kInfo.iID == _index) return kInfo;
        }
        return null;
    }
    void Load_TableInfo_fx()
    {
        if (kInfo_fx != null) return;

        string txtFilePath = "fx";
        TextAsset ta = LoadTextAsset(txtFilePath);
        String[] line = ta.text.Split('\n'); // line Split

        TableInfo_fx[] kInfo = new TableInfo_fx[line.Length];

        for (int i = 0; i < line.Length; i++)
        {
            //Console.WriteLine("line : " + line[i]);
            if (line[i] == null) continue;
            if (i == 0) continue; 	// Title skip

            string[] Cells = line[i].Split("\t"[0]);	// cell split, tab
            if (Cells[0] == "") continue;

            //for(int i=0; i < Cells.Length ; i++) {	Debug.Log("Cell"+i+"="+Cells[i]); }
            //-----------------------------------------------------			
            kInfo[i - 1] = new TableInfo_fx();

            kInfo[i - 1].iID = int.Parse(Cells[0]);
            kInfo[i - 1].sDesc = Cells[1];
            kInfo[i - 1].sResource = Cells[2];
            kInfo[i - 1].iLoop = int.Parse(Cells[3]);
        }

        kInfo_fx = kInfo;
    }

}

public class TableInfo_system
{
    public int iID = 0;
    public float fValue;
}

public class TableInfo_text
{
    public int iID = 0;
    public string Korean = null;
    public string English = null;
    public string Japanese = null;
    public string Chinese = null;
}

public class TableInfo_charic
{
	public int 		index = 0;
	public string 	name = "";
	public string 	resource = "";
	public int 		type = 0;     			// 0: 유닛 1: 건물 
	public float 	speed = 0;				// 이동 속도
	public int 		hp = 1000;				// health power
	public int 		ap = 100;				// attack power
	public float 	aspeed = 1;				// attack speed //공속.
	public int 		dp = 0;					// defence power

	public int 		mana = 4;				// 엘릭서 소모.
	public int 		group = 1;				// 멀티 여부 //스폰할때 나오는 갯수.

	public float 	distance = 0;  			// 공격 가능 거리
	public int 		target_type = 0;		//0: 유닛,건물  1:건물만
	public int 		target_count = 0;		//1: 단일  2이상: 광역
	public float 	target_range = 0;		//광역 범위
	public int 		projectile = 0;			//발사체 여부
	public float 	hit_time = 0;			// fire time
	public int 		fx = 0;
	public int 		sound = 0;
}
public class TableInfo_sound //: MonoBehaviour
{
    public int iID = 0;
    public string sDesc = null;
    public string sResource = null;
    public int iType = 0;
    public int iVolume = 100;
    public int iLoop = 0;
}
public class TableInfo_fx //: MonoBehaviour
{
    public int iID = 0;			// 레벨.
    public string sDesc = null;
    public string sResource = null;
    public int iLoop = 0;
}