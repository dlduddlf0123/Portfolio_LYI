using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingCanon : MonoBehaviour
{
    public Transform tipTr;
    public float shootingPower = 300f;

    bool isThrowing = false;
    
    private void OnCollisionEnter(Collision coll)
    {
        //if Item layer collision
        if (coll.gameObject.layer == 8)
        {
            if (!isThrowing)
            {
                StartCoroutine(ThrowBall(coll.gameObject));
                isThrowing = true;
            }
        }
    }


    public IEnumerator ThrowBall(GameObject _go)
    {
        float t = 0.5f;
        while (t > 0)
        {
            _go.transform.parent = tipTr;
            _go.transform.position = tipTr.position;
            _go.transform.rotation = tipTr.rotation;
            t -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }

        if ( _go.GetComponent<Item>())
        {
            _go.GetComponent<Item>().isThrowing = true;
        }

        _go.transform.parent = null;
        _go.GetComponent<Rigidbody>().AddForce(tipTr.forward * shootingPower);
        GameManager.Instance.PlayEffect(tipTr, GameManager.Instance.particles[0],ReadOnly.Defines.SOUND_SFX_CLICK);

        isThrowing = false;
    }
}
