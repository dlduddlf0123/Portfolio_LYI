using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    public Text text;

    GameManager gameMgr;
    public NRHandMove NRHandMove;
    public HandFollower handFollower;
    
    public bool toggleHandIcon = false;
    public bool toggleHandOcclusion = true;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
    }

    private void Start()
    {
        LoadHandPose();
    }

    public void SaveHandPose()
    {
        PlayerPrefs.SetInt("ToggleHandIcon", System.Convert.ToInt32(toggleHandIcon));
        PlayerPrefs.SetInt("ToggleHandOcclusion", System.Convert.ToInt32(toggleHandOcclusion));
    }

    public void LoadHandPose()
    {
        toggleHandIcon = System.Convert.ToBoolean(PlayerPrefs.GetInt("ToggleHandIcon", 0));
        toggleHandOcclusion = System.Convert.ToBoolean(PlayerPrefs.GetInt("ToggleHandOcclusion", 1));

        ToggleHandIcon(toggleHandIcon);
        ToggleHandOcclusion(toggleHandOcclusion);
    }


    public void ToggleHandIcon(bool _isActive)
    {
        toggleHandIcon = _isActive;
       // handFollower.GetComponent<MeshRenderer>().enabled = toggleHandIcon;
       // NRHandMove.arr_handFollwer[0].GetComponent<TrailRenderer>().enabled = toggleHandIcon;
    }

    public void ToggleHandOcclusion(bool _isActive)
    {
        //toggleHandOcclusion = _isActive;

        //if (_isActive)
        //{
        //    gameMgr.mainCamera.GetComponent<AROcclusionManager>().requestedHumanDepthMode = HumanSegmentationDepthMode.Best;
        //    gameMgr.mainCamera.GetComponent<AROcclusionManager>().requestedHumanStencilMode = HumanSegmentationStencilMode.Best;
        //}
        //else
        //{
        //    gameMgr.mainCamera.GetComponent<AROcclusionManager>().requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
        //    gameMgr.mainCamera.GetComponent<AROcclusionManager>().requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
        //}

    }

}
