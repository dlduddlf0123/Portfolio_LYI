using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCollider : MonoBehaviour
{
    public LightRoadInteraction line;

    public Vector3 firstPos = Vector3.zero;

    public bool isColled = false;
    public int vertexNum = 0;
    float t = 0;

    void Start()
    {
        firstPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider _coll)
    {
        if (_coll.gameObject.CompareTag("Player"))
        {
            if (!isColled)
            {
                if (vertexNum != line.arr_linePosGO.Length - 1)
                {
                    if (!line.arr_linePosGO[vertexNum + 1].isColled)
                    {
                        return;
                    }
                }
                

                if (vertexNum - 1 < 0)
                {
                    //Line End
                    for (int i = 0; i < line.arr_linePosGO.Length; i++)
                    {
                        line.arr_linePosGO[i].transform.localPosition = line.arr_linePosGO[i].firstPos;
                    }

                    line.CharacterMove();
                    line.line.enabled = false;
                    return;
                }

                isColled = true;

                line.list_guideParticle[vertexNum-1].Stop();
                line.list_lineEffect[vertexNum - 1].Play();
                line.list_lineEffect[vertexNum].Stop();
                //StartCoroutine(FollowHand(_coll.gameObject.transform));
            }

        }
    }

    IEnumerator FollowHand(Transform _tr)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = line.arr_linePosGO[vertexNum - 1].transform.position;

        while (isColled && !line.arr_linePosGO[vertexNum - 1].isColled)
        {
            if ((1 - Vector3.Distance(_tr.position, endPos) / Vector3.Distance(startPos, endPos)) > t )
            {
                t = 1 - Vector3.Distance(_tr.position, endPos) / Vector3.Distance(startPos, endPos);
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

                transform.position = currentPos;
                line.line.SetPosition(vertexNum, new Vector3(transform.localPosition.x, 0, transform.localPosition.z));
            }
            yield return new WaitForSeconds(0.01f);
        }

        if (line.line.positionCount > 1)
        {
            line.line.positionCount--;
        }

        transform.position = firstPos;
        isColled = false;
        StopAllCoroutines();

    }
}
