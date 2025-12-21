using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRoadInteraction : InteractionManager
{
    public LineRenderer line;
    public LineRenderer zinoLine;
    public LineCollider[] arr_linePosGO;

    public ParticleSystem lineEffect;
    public List<ParticleSystem> list_lineEffect;

    public Transform[] arr_fixedPos;

    public float collSize = 0.5f;
    int endCount = 0;
    protected override void DoAwake()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        zinoLine.enabled = false;
        endCount = 0;
    }

    void LineInit()
    {
        arr_linePosGO = new LineCollider[(line.positionCount - 1) * 4];

        for (int index = 0; index + 1 < line.positionCount; index++)
        {
            index *= 4;
            for (int i = 0; i < 4; i++)
            {
                if ((int)(index * 0.25f) + 1 < line.positionCount)
                {
                    Vector3 p1 = line.GetPosition((int)(index * 0.25f));
                    Vector3 p2 = line.GetPosition((int)(index * 0.25f) + 1);
                    list_guidePosition.Add(p1 + (p2 - p1) * 0.25f * i);
                }

                arr_linePosGO[index + i] = new GameObject("LineColl" + (index + i)).AddComponent<LineCollider>();
                arr_linePosGO[index + i].tag = "End";

                BoxCollider coll = arr_linePosGO[index + i].gameObject.AddComponent<BoxCollider>();
                coll.isTrigger = true;
                coll.size = new Vector3(collSize, 3, collSize);

                arr_linePosGO[index + i].transform.SetParent(transform);
                arr_linePosGO[index + i].transform.localPosition = list_guidePosition[index + i];

                arr_linePosGO[index + i].line = this;
                arr_linePosGO[index + i].vertexNum = index + i;

            }
            index = (int)(index * 0.25f);

            //if (index + 1 < line.positionCount)
            //{
            //    Vector3 p1 = line.GetPosition(index);
            //    Vector3 p2 = line.GetPosition(index + 1);
            //    Vector3 p3 = p1 + (p2 - p1) * 0.25f;
            //    Vector3 p4= p1 + (p2 - p1) * 0.5f;
            //    Vector3 p5 = p1 + (p2 - p1) * 0.75f;

            //    list_guidePosition.Add(p1);
            //    list_guidePosition.Add(p3);
            //    list_guidePosition.Add(p4);
            //    list_guidePosition.Add(p5);
            //}
        }

        line.enabled = false;

        //lineEffect = Instantiate(lineEffect.gameObject,transform).GetComponent<ParticleSystem>();
        //lineEffect.Play();
        //lineEffect.transform.position = arr_linePosGO[arr_linePosGO.Length-1].transform.position;
        PlayGuideParticle();
    }

    public override void PlayGuideParticle()
    {
        for (int i = 0; i < list_guidePosition.Count; i++)
        {
            if (list_guideParticle.Count < list_guidePosition.Count)
            {
                list_guideParticle.Add(Instantiate(gameMgr.b_stagePrefab.LoadAsset<GameObject>("InteractionEffect")).GetComponent<ParticleSystem>());
                list_guideParticle[i].transform.parent = transform;

                list_lineEffect.Add(Instantiate(lineEffect,transform));
            }
            list_guideParticle[i].transform.localPosition = list_guidePosition[i];
            list_lineEffect[i].transform.localPosition = list_guidePosition[i];
        }
        for (int i = 0; i < list_guideParticle.Count; i++)
        {
            list_guideParticle[i].Play();
            list_guideParticle[i].transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        }
        list_guideParticle[list_guideParticle.Count - 1].Stop();
        list_lineEffect[list_lineEffect.Count - 1].Play();
    }

    void BakeMeshCollider()
    {
        MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        line.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;
        meshCollider.isTrigger = true;
    }

    public void CharacterMove()
    {
        StopAllCoroutines();

        ZinoLineFollowSignal(zinoLine);

        KantoLineFollowSignal(line);
    }

    public void KantoLineFollowSignal(LineRenderer _line)
    {
        StartCoroutine(LineFollowMove(gameMgr.currentEpisode.currentStage.arr_header[0], _line));
    }
    public void ZinoLineFollowSignal(LineRenderer _line)
    {
        StartCoroutine(LineFollowMove(gameMgr.currentEpisode.currentStage.arr_header[1], _line));
    }
    public void MoveToTarget()
    {
        gameMgr.currentEpisode.currentStage.arr_header[0].MoveCharacter(Vector3.zero, 3f,() =>
        gameMgr.currentEpisode.currentStage.arr_header[0].transform.LookAt(gameMgr.currentEpisode.currentStage.arr_header[1].transform));
        gameMgr.currentEpisode.currentStage.arr_header[1].MoveCharacter(Vector3.zero, 3f,() =>
        gameMgr.currentEpisode.currentStage.arr_header[1].transform.LookAt(gameMgr.currentEpisode.currentStage.arr_header[0].transform));
    }
    IEnumerator LineFollowMove(Character _chara, LineRenderer _line)
    {
        //gameMgr.currentEpisode.currentStage.GetComponent<GrassStage>().grassEffect.recoverySpeed = 0f;
        StopGuideParticle();
        for (int i = 0; i < list_lineEffect.Count; i++)
        {
            list_lineEffect[i].Stop();
        }

        Character chara = _chara;
        //gameMgr.currentEpisode.currentStage.list_endPos[3].transform.position = arr_linePosGO[0].transform.position;
        chara.SetAnim(2);
        for (int i = _line.positionCount - 1; i >= 0; i--)
        {
            yield return StartCoroutine(chara.WalkMove(_line.transform.position + _line.GetPosition(i), 3));
        }

        if (endCount > 0)
        {
            EndInteraction();
        }
        else
        {
            endCount++;
        }
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        LineInit();
        gameMgr.currentEpisode.currentStage.arr_header[0].transform.position = new Vector3(arr_fixedPos[0].position.x, gameMgr.currentEpisode.currentStage.arr_header[0].transform.position.y, arr_fixedPos[0].position.z);
        gameMgr.currentEpisode.currentStage.arr_header[1].transform.position = new Vector3(arr_fixedPos[1].position.x, gameMgr.currentEpisode.currentStage.arr_header[1].transform.position.y, arr_fixedPos[1].position.z);
        gameMgr.currentEpisode.currentStage.arr_header[0].transform.localRotation = Quaternion.Euler(0, 90, 0);
        gameMgr.currentEpisode.currentStage.arr_header[1].transform.localRotation = Quaternion.Euler(0, -90, 0);
    }

    public override void EndInteraction()
    {
        endCount = 0;
        base.EndInteraction();

        StopAllCoroutines();
        gameMgr.currentEpisode.currentStage.arr_header[0].StopMove();
        gameMgr.currentEpisode.currentStage.arr_header[1].StopMove();

        Debug.Log("arr_fixedPos[2] = " + arr_fixedPos[2].position.z);
        Debug.Log("arr_fixedPos[3] = " + arr_fixedPos[3].position.z);
        gameMgr.currentEpisode.currentStage.arr_header[0].transform.position = new Vector3(arr_fixedPos[2].position.x, gameMgr.currentEpisode.currentStage.arr_header[0].transform.position.y, arr_fixedPos[2].position.z);
        gameMgr.currentEpisode.currentStage.arr_header[1].transform.position = new Vector3(arr_fixedPos[3].position.x, gameMgr.currentEpisode.currentStage.arr_header[1].transform.position.y, arr_fixedPos[3].position.z);

        gameMgr.currentEpisode.currentStage.arr_header[0].transform.localRotation = Quaternion.Euler(0, -90, 0);
        gameMgr.currentEpisode.currentStage.arr_header[1].transform.localRotation = Quaternion.Euler(0, 90, 0);

        gameObject.SetActive(false);
    }

}
