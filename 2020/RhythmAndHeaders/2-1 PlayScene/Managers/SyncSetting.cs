using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SyncSetting : MonoBehaviour
{
    GameManager gameMgr;
    public bool isTutorial = false;
    public AudioSource syncSong;

    public Image marker;
    public Button bt_back;
    public GameObject checkMarker;
    public GameObject syncPoint;
    public float userSyncValue;
    public float defaultSyncValue = 0;
    public List<float> check = new List<float>();

    public float songBpm;
    public float secPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    public float dspSongTime;

    public float firstBeatOffset;

    public float beatsPerLoop;

    public int completedLoops = 0;

    public float loopPositionInBeats;

    public bool isSongPlayed = false;

    public float defaultSync;

    private void Awake()
    {
        gameMgr = GameManager.Instance;
        syncPoint.transform.localPosition = new Vector3(gameMgr.userSync, -599, 0);
        
        secPerBeat = 60f / songBpm;
        //dspSongTime = (float)AudioSettings.dspTime;
        //syncSong.Play(); 
        //syncSong.loop = true;
        //SongPlay();
        Debug.Log(syncPoint.transform.localPosition.x);
    }
    private void Update()
    {
        //gameMgr.userSync = syncPoint.transform.localPosition.x;
        if (isSongPlayed==true)
        {
            SliderMove();
        }
        if(Input.GetMouseButtonDown(0))
        {
            check.Add(loopPositionInBeats);
            GameObject go;
            go = Instantiate(checkMarker,marker.transform.position,Quaternion.identity,marker.transform.parent.transform);
            if(isTutorial==true)
            {
                gameObject.GetComponent<TutorialManager>().syncpointList.Add(go.transform.localPosition.x);
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            foreach(float position in check)
            {
                Debug.Log(position);
            }
        }
    }
    public void SongPlay()
    {
        dspSongTime = (float)AudioSettings.dspTime;
        syncSong.Play();
        isSongPlayed = true;
    }
    public void SliderMove()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);
        songPositionInBeats = songPosition / secPerBeat;

        marker.rectTransform.localPosition = new Vector3(loopPositionInBeats * (Screen.width / 2) - Screen.width / 2, -599, 0);

        //marker.rectTransform.position = new Vector3(loopPositionInBeats / 2960, 0, 0);
        //marker.transform.Translate(loopPositionInBeats*1480, 0, 0);
        if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
            completedLoops++;
        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;
    }
}
