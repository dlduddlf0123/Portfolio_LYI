using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SerializeField]
public class JsonStage
{
    public int stageNum;
    public string stageName;
    public int maxRoom;
    public int steminaForPlay;

    //public int stageStatBonus; 

    public List<string> list_enemy = new List<string>();
    public List<string> list_perk = new List<string>();
    public List<string> list_dropItem = new List<string>();
}
