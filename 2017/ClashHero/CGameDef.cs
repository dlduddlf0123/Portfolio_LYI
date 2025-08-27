using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameDef : MonoBehaviour 
{
	//----------------------------------------------------------------------------------------------

	public void LocalDB_init() //클라 정보, 최초 1회.
	{
		Player player = CGame.Instance.kPlayer;

		PlayerPrefs.SetString("project_init", "on");

		//player.sVersion = "";
		player.user_id = "";
		player.nickname = "";

		player.cash = 100;
		player.gold = 100;

		LocalDB_save();
	}

	public void LocalDB_save() 
	{
		Player player = CGame.Instance.kPlayer;
		//iVolume_bgm = (int)CSndManager.instance.fVolume_bgm * 100;
		//iVolume_fx = (int)CSndManager.instance.fVolume_fx * 100;

		//Serialize -----------------------------
		string db =
			player.sVersion + "\t" + //버젼정보..
			player.user_id + "\t" +
			player.nickname + "\t" +
			"";

		PlayerPrefs.SetString("project_data", db);  //save
		//print("LocalDB_save " + db);
	}

	public void LocalDB_load() //클라 정보, 초기화 로딩.
	{
		Player player = CGame.Instance.kPlayer;

		string db = PlayerPrefs.GetString("project_data");  //load                                                            
		print("LoadDatabase : " + db.Length + " " + db);

		//DeSerialize -----------------------------

		string[] source = db.Split("\t"[0]);
		int offset = 0;

		string sVersion_reset = "0.0.0";
		string sVersion_saved = source[offset++];
		if (sVersion_saved.CompareTo(sVersion_reset) <= 0)   // 리셋버젼 이하이다. // live에선 안됨.
		{
			//기존의 저장된 데이타를 쓰지 않는다. // 리셋. //캐릭 재생성.
			print("RESET:    sVersion_reset :" + sVersion_reset + "  sVersion_saved :" + sVersion_saved);
			LocalDB_init(); // init
			return;
		}

		//if (sVersion_saved.CompareTo("0.0.0") <= 0) //이전버전
		//{
		//	player.user_id = source[offset++];
		//	player.nickname = source[offset++];
		//}
		//else //현재버전
		{
			player.user_id = source[offset++];
			player.nickname = source[offset++];
		}

	}

}

//CTimeUnit kTime = new CTimeUnit();
//string lasttime = kTime.CreateTime(5);
//MyApplication.print("" + kTime.GetString_remain_time());

public class CTimeUnit
{
	public static string datePatt = "yyyy-MM-dd HH:mm:ss";
	//public static string datePatt = "yyyy/MM/dd hh:mm:ss tt";

	DateTime m_time_start;
	DateTime m_time_goal;


	public string CreateTime(int _sec)
	{
		m_time_start = DateTime.Now;
		m_time_goal = DateTime.Now.AddSeconds(_sec);

		string nowtime = DateTime.Now.ToString(datePatt);
		return nowtime;
	}

	public void SetGoalTime(string _time_last)
	{
		m_time_goal = DateTime.ParseExact(_time_last, datePatt, null);
	}

	public int GetRemainTime()
	{
		TimeSpan ts_time = m_time_goal - DateTime.Now;
		int sec = (int)ts_time.TotalSeconds;
		return sec;
	}

	public string GetString_remain_time()
	{
		string rt = "";
		TimeSpan ts_remainingtime = m_time_goal - DateTime.Now;
		//print ("" + ts_remainingtime.ToString());	
		if (ts_remainingtime.Seconds > 0) {
			rt = String.Format("{0:00}:{1:00}", ts_remainingtime.Minutes, ts_remainingtime.Seconds);
		} else {
			rt = String.Format ("00:00");
		}

		return rt;
	}

}
