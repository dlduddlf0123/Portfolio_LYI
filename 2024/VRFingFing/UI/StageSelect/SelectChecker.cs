using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;

namespace VRTokTok.UI
{

    public class SelectChecker : MonoBehaviour
    {
        TableManager tableMgr;

        public SelectSphere colledObj;
        public bool isSelect = false;
        public int stageNum = 1001;

        private void Awake()
        {
            tableMgr = GameManager.Instance.tableMgr;
        }

        // Start is called before the first frame update
        void Start()
        {

        }



        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Item"))
            {
                isSelect = true;
                colledObj = other.gameObject.GetComponentInParent<SelectSphere>();
                colledObj.Select();
                stageNum = colledObj.stageNum;

                tableMgr.ChangeSelectStage(stageNum);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Item"))
            {
                if (colledObj == null)
                {
                    return;
                }
                colledObj.Deselect();
                isSelect = false;
                colledObj = null;
                //stageNum = 1001;
                //tableMgr.ui_table.ChangeSelectStage(stageNum);
            }
        }



    }
}