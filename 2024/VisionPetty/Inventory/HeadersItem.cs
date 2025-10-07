using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using MoreMountains.InventoryEngine;

namespace AroundEffect
{

    [CreateAssetMenu(fileName = "HeadersItem", menuName = "Custom/HeadersItem", order = 0)]
    [Serializable]
    public class HeadersItem : InventoryItem
    {
        GameManager gameMgr;

        [Header("Headers Item")]
        public int itemID;



        private void Awake()
        {
            gameMgr = GameManager.Instance;

        }


        public override bool Use(string playerID)
        {

            gameMgr.lifeMgr.itemSpawner.ItemSpawn(this);
            return true;
        }


        public override bool Drop(string playerID)
        {
            Debug.Log(itemID + ": Drop()");

            gameMgr.lifeMgr.itemSpawner.ItemSpawn(this);

            return true;
        }


        public override bool Pick(string playerID)
        {
            return base.Pick(playerID);
        }


    }
}