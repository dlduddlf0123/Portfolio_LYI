using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBoat : InteractionManager
{
    Character header;
    public GameObject boat;
    Transform firstParent;

    [SerializeField]
    WindColl[] arr_windColl;

    public ParticleSystem particle_windTrail;
    AudioSource windSound;

    public Transform endPos;


    public float speed = 1f;
    public float wind = 0.0f;
    public float MAX_SPEED = 1.5f;

    protected override void DoAwake()
    {
        header = gameMgr.currentEpisode.currentStage.arr_header[0];

        windSound = GetComponent<AudioSource>();
        arr_windColl[0].gameObject.SetActive(false);
    }

    private void Update()
    {
        if (wind > 0)
        {
            if (wind > 0.5f && !particle_windTrail.isPlaying)
            {
                particle_windTrail.Play();
            }
            else if (wind < 0.5f && particle_windTrail.isPlaying)
            {
                particle_windTrail.Stop();
            }

            wind -= Time.deltaTime *speed;
            boat.transform.position = Vector3.Lerp(boat.transform.position, endPos.position, Time.deltaTime * speed * wind);


            if (Vector3.Distance(boat.transform.position,endPos.position) < 0.1f &&
                gameMgr.statGame == GameStatus.INTERACTION)
            {
                EndInteraction();
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10 &&
            gameMgr.statGame == GameStatus.INTERACTION &&
            gameMgr.currentEpisode.currentStage.currentInteraction == 5)
        {
            wind++;
            if (wind > MAX_SPEED)
            {
                wind = MAX_SPEED;
            }
        }
    }


    public override void StartInteraction()
    {
        base.StartInteraction();
        stageMgr.StopAllCoroutines();

        firstParent = header.transform.parent;
        header.transform.parent = boat.transform;
        header.transform.localPosition = Vector3.up * 0.1f;

        windSound.Play();
        arr_windColl[0].gameObject.SetActive(true);

        list_guidePosition.Add(boat.transform.position);
        PlayGuideParticle();
        gameMgr.uiMgr.ui_game.ChangeHandIcon(HandIcon.FRONT);
    }

    public override void EndInteraction()
    {
        StopGuideParticle();

        base.EndInteraction();

        gameMgr.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_SUCCESS);

        boat.transform.position = endPos.transform.position;
        header.transform.localPosition = Vector3.up * 0.1f;
        header.transform.parent = firstParent;
        header.transform.localRotation = Quaternion.Euler(Vector3.up * -90f);

        particle_windTrail.Stop();
        windSound.Stop();

        gameObject.SetActive(false);
    }

    public void TurnSignal(Transform _target)
    {
        gameMgr.currentEpisode.currentStage.arr_header[0].TurnLook(_target);
    }
}
