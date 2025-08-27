using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rabyrinth.ReadOnlys;

public class HP_Bar : MonoBehaviour {
    public Slider Hp_Bar { get; private set; }
    public Transform targetpos;
    public GameObject target;
    public GameManager GameMgr;
    
      
	// Use this for initialization
	void Awake () {
        GameMgr = MonoSingleton<GameManager>.Inst;
        Hp_Bar = gameObject.GetComponent<Slider>();
        
	}
	public void DamageSet(float HP, float MaxHP)
    {
       
        float nowHP = HP / MaxHP;
        Hp_Bar.value = nowHP;
    }

    //public void init()
    //{
    //    gameObject.SetActive(true);
    //    StartCoroutine("HPtrace");
    //}

    //public IEnumerator HPtrace()
    //{
        
    //    while (GameMgr.Player.MainTarget != null)
    //    {
    //        Vector3 newPos = Camera.main.WorldToScreenPoint(GameMgr.Player.MainTarget.transform.localPosition);
    //        newPos = new Vector3((newPos.x - Screen.width * 0.5f), (newPos.y - Screen.height * 0.5f) + 150);
    //        transform.localPosition = newPos;

    //        float EnemyHP = (GameMgr.Player.MainTarget.Status.HP) / (GameMgr.Player.MainTarget.Status.MaxHP);
    //        Hp_Bar.value = EnemyHP;
    //        yield return null;
    //    }
    //    GameMgr.Main_UI.qHp_Bar.Enqueue(this);
    //    gameObject.SetActive(false);
    //}        
   
}
