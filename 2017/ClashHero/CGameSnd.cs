// You need to "CSndInfo.cs"
// 'SoundObject' Tags must be declared in the tag manager before using them

using UnityEngine;
using System.Collections;

/*
	var myAudio : AudioClip;
	AudioSource kAudioSource;		
  	if(!audio) AddComponent(AudioSource); 
  	audio.clip = myAudio; 
  	audio.volume = 0.5; 
  	audio.Play(); 
 */

public class Cooltime
{
	public int iId = 0;
	public float fTime = 0;
}

public enum eSound : int
{
    bgm_main   = 101,
    bgm_battle    = 102,
    //snd_ui_click = 201,
    //snd_levelup = 202,

    ui_button      	= 1000,
	ui_alarm      	= 1001,
	ui_beep      	= 1002,
	ui_popup		= 1003,

	battle_notice     		= 1004,
	battle_win      	= 1005,
	battle_lose      	= 1006,

	skill_arrow      	= 2000,
	skill_magic      	= 2001,
	skill_punch      	= 2002,
	skill_slap1      	= 2003,
	skill_slap2      	= 2004,
	skill_sword      	= 2005,
	skill_sword2      	= 2006,


    snd_end
};

public class CGameSnd : MonoBehaviour 
{				
	//GameObject kRoot = null;
	
	//public float fVolume = 1.0f; 		
	public float fVolume_bgm = 1.0f;
	public float fVolume_fx = 1.0f;
	
	public GameObject kBgm = null;
	
	ArrayList SourceArray = new ArrayList();
	ArrayList CloneArray = new ArrayList();
	
	ArrayList CooltimeArray = new ArrayList();
		
	private static CGameSnd s_instance = null;
	
	public static CGameSnd Instance
    {
		get 
	    {
	    	if (s_instance == null)
	        {
	        	s_instance //= new CGameSnd();
				 = FindObjectOfType(typeof(CGameSnd)) as CGameSnd;
	        }
	        return s_instance;
	    }
	}
	
	void Awake () 
	{
		if(s_instance != null) 
		{
			Debug.LogError("Cannot have two instances of CGameSnd."); 
			return; 
		}
		s_instance = this;

		DontDestroyOnLoad(this);		
		//Debug.Log("CGameSnd Awake");		
	}

	// Use this for initialization
	void Start ()
	{
		//CooltimeArray.Clear();

		//kRoot = GameObject.Find("root_sound");
				
		//kRoot.transform.position = Camera.mainCamera.transform.position;
		
		//LoadSoundSource();

		//AddClone( 1 , Vector3(0, 0, 0) );
	}

    /*
	// Update is called once per frame 
	void Update () 
	{

		if( Input.GetKeyDown(KeyCode.Alpha1) )		
		{	
			//PlayBGM( (int)eSound.eBgm_Title );
			
			//CooltimeArray.Clear();				
			//Cooltime kCool = new Cooltime();
			//kCool.iId = 12;						
			//CooltimeArray.Add( kCool );			
			//Cooltime kCool2 = GetCooltime( 12 );				
			//print("CooltimeArray   " + CooltimeArray.Count + " " + kCool2.iId);

		}
		
		if( Input.GetKeyDown(KeyCode.Alpha2) )
		{			
			//PlaySound((int)eSound.eSnd_Attack);

            //play
            AudioClip   soundClip = Resources.Load("Sound/soundname") as AudioClip;
            AudioSource audioSource = new AudioSource();
            audioSource.clip = soundClip;
            audioSource.Play();
		}

		if( Input.GetKeyDown(KeyCode.R) )
		{	
			RemoveAllClone();			
		}

	}
*/


    //----------------------------------------------
    public void LoadSource()
	{
        AddSource((int)eSound.bgm_main);
        AddSource((int)eSound.bgm_battle);

        AddSource((int)eSound.ui_button   	, "snd_ui_click");
		AddSource((int)eSound.ui_alarm   	, "snd_ui_alarm");
		AddSource((int)eSound.ui_beep   	, "snd_ui_beep");
		AddSource((int)eSound.ui_popup   	, "snd_ui_popup");
		AddSource((int)eSound.battle_notice	, "snd_levelup");
		AddSource((int)eSound.battle_win   	, "battle_win");
		AddSource((int)eSound.battle_lose   , "battle_lose");
		AddSource((int)eSound.skill_arrow   , "skill_arrow");
		AddSource((int)eSound.skill_magic   , "skill_magic");
		AddSource((int)eSound.skill_punch   , "skill_punch");
		AddSource((int)eSound.skill_slap1   , "skill_slap1");
		AddSource((int)eSound.skill_slap2   , "skill_slap2");
		AddSource((int)eSound.skill_sword   , "skill_sword");
		AddSource((int)eSound.skill_sword2  , "skill_sword2");
    }   
	
	// Add Source ----------------------------------------------------------------
	GameObject AddSource(int _sound_index, string _sound_name  ="")
	{
		GameObject kGO = new GameObject();
        //kGO.tag = "SoundObject";	
        kGO.transform.parent = gameObject.transform;
        kGO.transform.position = new Vector3(0, 0, 0);

        string szPrefab = "";
        if (_sound_name == "") //TableInfo_sound ���� �ε���ȣ��.
        {
            kGO.name = "Snd_" + _sound_index;
		    TableInfo_sound kTableInfo_sound = CGameTable.Instance.Get_TableInfo_sound((int)_sound_index);
            if (kTableInfo_sound == null)
            {
                Debug.Log("ERROR : CGameSnd Get_TableInfo_sound: " + _sound_index); 
                return null;
            }
		    szPrefab = kTableInfo_sound.sResource;
            //Debug.Log("sound AddSource: " + szPrefab ); 
        }
        else //
        {
            kGO.name = "Snd_" + _sound_index;
            szPrefab = _sound_name;
        }

        // AudioSource
        //if( kGO.audio == null) 
        kGO.AddComponent(typeof(AudioSource));                

        AudioClip	obj = (AudioClip)Resources.Load("sound/" + szPrefab, typeof(AudioClip));
        if (obj == null)
        {
            Debug.Log("ERROR: CGameSound AddSource Load Failed: " + szPrefab);
            Destroy(kGO);
            return null;
        }
		kGO.GetComponent<AudioSource>().clip = obj;
		kGO.GetComponent<AudioSource>().playOnAwake = false;		
		//kGO.audio.volume = 1.0f;
					
		// CSndInfo
		CSndInfo kInfo = (CSndInfo)kGO.GetComponent("CSndInfo");
		if( kInfo == null )
			kInfo = (CSndInfo)kGO.AddComponent<CSndInfo>();
		
		kInfo.iID 			= kGO.GetInstanceID();			
		kInfo.audiolength 	= kGO.GetComponent<AudioSource>().clip.length;
        kInfo._index     = (int)_sound_index;

        // Add List
        SourceArray.Add( kGO );
		
		return kGO;
			
	}

	GameObject GetSource(int _index)
	{
		foreach ( GameObject kGO in SourceArray )			
		{
            if (kGO == null) continue;
			CSndInfo kObject = (CSndInfo)kGO.GetComponent("CSndInfo");			
			if( kObject._index == _index )
			{
				return kGO;
			}
		}	
		return null;
	}
	
	int GetID( GameObject kGO)
	{
		CSndInfo kInfo = (CSndInfo)kGO.GetComponent("CSndInfo");
		return kInfo.iID;
	}	
	
	// Add Fx Clone	
	GameObject AddClone(int _index, Vector3 pos )
	{
		GameObject kGO = GetSource( _index );
		if( kGO == null)
		{			
			kGO = AddSource( _index );
		}
		if( kGO )
		{
			//kGO.audio.PlayOneShot( kGO.audio.clip );
						
			GameObject kGOClone = (GameObject)Instantiate( kGO );
			kGOClone.transform.position = pos;
			
			CSndInfo kInfo = (CSndInfo)kGOClone.GetComponent("CSndInfo");
			kInfo.iID 		= kGOClone.GetInstanceID();
			kInfo._index = _index;
			kInfo.bRemove 	= false;

			CloneArray.Add( kGOClone );
			
			return kGOClone;
		}
		return null;
	}
	
	public void RemoveClone( GameObject kGO )
	{	
		CloneArray.Remove( kGO );
		Destroy (kGO); 		
	}
	
	public void RemoveAllClone()
	{
		foreach ( Object obj in CloneArray )
			Destroy (obj); 
		CloneArray.Clear();
	}

	public void RemoveAll()
	{
		//GameObject[] kGOs = GameObject.FindGameObjectsWithTag ("SoundObject");
		//foreach ( Object obj in kGOs )
		//	Destroy (obj); 	
		
		foreach ( Object obj in CloneArray )
		{
			if(obj) Destroy (obj); 
		}
		CloneArray.Clear();
		
		foreach ( Object obj in SourceArray )
		{
			if(obj) Destroy (obj); 
		}
		SourceArray.Clear();
		
	}	
	
	// Play BGM ----------------------------------------------------------------
	
	public void PlayBGM(eSound _index )
	{
		if( kBgm != null)
		{
			CSndInfo kInfo = (CSndInfo)kBgm.GetComponent("CSndInfo");
            //print (kInfo._index + " " + _index);
			if( kInfo._index == (int)_index )
				return;
			
			StopBGM();
		}
		
		kBgm = PlaySound( (int)_index, Vector3.zero, true, fVolume_bgm );
		kBgm.transform.parent = gameObject.transform;
		//print("PlayBGM " + _index);
	}

	public void StopBGM()
	{
		CGameSnd.Instance.RemoveClone( kBgm );
		//Destroy ( kBgm );
	}

	// PlaySound  ---------------------------------------------------------------

	public GameObject PlaySound(eSound _index )
	{
		if(_index == 0)return null;
		return PlaySound( (int)_index, Vector3.zero, false, fVolume_fx );
		/*GameObject kGO = AddClone( _index, Vector3.zero );
		if(kGO == null)	
			return null;*/
	}
    public GameObject PlaySound(int _index)
    {
        if (_index == 0) return null;
        return PlaySound(_index, Vector3.zero, false, fVolume_fx);
    }

    public GameObject PlaySound(int _index, Vector3 pos, bool bLoop, float _volume )
	{
		if( ! AddCooltime( _index ) ) 
			return null;	
		
		GameObject kGO = AddClone( _index, pos );
		if(kGO == null)	
			return null;
		
		kGO.transform.parent = gameObject.transform; // AudioListener
		kGO.transform.position = new Vector3(0,0,-10);		
		
		CSndInfo kInfo = (CSndInfo)kGO.GetComponent("CSndInfo");
		
		if(bLoop)
		{			
			kGO.GetComponent<AudioSource>().loop = true;
		}
		else
		{
			kGO.GetComponent<AudioSource>().loop = false;
			kInfo.DeathTime = kInfo.audiolength;
			kInfo.bRemove = true;
		}
				
		kGO.GetComponent<AudioSource>().playOnAwake = true;
		kGO.GetComponent<AudioSource>().volume = _volume; 		//print("sound: " + _index + "  _volume: " + _volume);
		kGO.GetComponent<AudioSource>().Play();
		//kGO.audio.PlayOneShot( kGO.audio.clip );
		
		return kGO;
	}

    public void StopSound(GameObject go)
    {
        CGameSnd.Instance.RemoveClone(go);
    }
	

	public void SetVolume_bgm()
	{
		if( kBgm != null )	{
			kBgm.GetComponent<AudioSource>().volume = fVolume_bgm;
			//kBgm.audio.Play();
		}
	}

    //--------------------------------------------------------------------------
    bool AddCooltime(int _index)
    {
        bool bActive = false;

        Cooltime kCool = GetCooltime(_index);
        if (kCool == null)
        {
            kCool = new Cooltime();
            kCool.iId = _index;
            kCool.fTime = Time.time;

            CooltimeArray.Add(kCool);

            bActive = true;
        }
        else
        {
            if (Time.time > kCool.fTime + 0.2f) //time.
                bActive = true;

            kCool.fTime = Time.time;
        }

        return bActive;
    }

    Cooltime GetCooltime(int _index)
    {
        foreach (Cooltime kObject in CooltimeArray)
        {
            if (kObject == null) continue;

            if (kObject.iId == _index)
            {
                return kObject;
            }
        }
        return null;
    }
}

public class CSndInfo : MonoBehaviour
{
    public int iID = 0;
    public int _index = 0;
    public bool bLoop = false;
    public bool bRemove = false;
    public float DeathTime = 0.0f;
    //public AudioSource kAudioSource = null;
    public float audiolength = 0.0f;

    void Awake()
    {
        //DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (bRemove)
        {
            DeathTime -= Time.deltaTime;
            if (DeathTime <= 0.0f)
            {
                CGameSnd.Instance.RemoveClone(gameObject);
                //Destroy( gameObject );
            }
        }

        //if(audio == null)	return;		
        //playOneShot
        //if( !bLoop )
        //	if(!audio.isPlaying)
        //	{
        //		Destroy( gameObject );
        //	}
    }

}