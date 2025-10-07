using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour
{
    public GameObject apple;
    List<GameObject> listApple;
    public GameObject currentApple { get; set; }

    Transform target;
    
    private Transform Camera = null;
    private Transform Anchor = null;
    private int state = 0;
    private Renderer candidate = null;
    //private Renderer selected = null;

    bool isFired;

    void Awake()
    {
        Anchor = new GameObject("Anchor").transform;
        Anchor.parent = transform;
        listApple = new List<GameObject>();
    }

    void Start()
    {
        //Camera = GestureProvider.Current.transform;
    }

    private void Update()
    {
        if (state == 1)
        {
            //currentApple.transform.position = (GestureProvider.LeftHand.position + GestureProvider.RightHand.position) / 2;
            var forward = currentApple.transform.position - Camera.position;
            currentApple.transform.position += forward;
            transform.position = Anchor.position = Camera.position;
            transform.rotation = Anchor.rotation = Quaternion.LookRotation(forward, Camera.up);

        }

    }

    void ReadyFire()
    {
        isFired = false;
        if (listApple.Count == 0)
        {
            GameObject _go = Instantiate(apple);
            listApple.Add(_go);
        }
        listApple[0].SetActive(true);
        currentApple = listApple[0];
        listApple.RemoveAt(0); 
        //// find hit objects by raycast
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity)&& hit.point!=null)
        //{
        //    target.position = hit.point;
        //}
    }

    void FireApple(GameObject _go)
    {
        Rigidbody rb = _go.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = transform.forward  * 10f;
        isFired = true;
    }

    public void OnStateChanged(int state)
    {
        this.state = state;
        if (state == 1)
        {
            ReadyFire();
        }
        else if(state == 2)
        {
            FireApple(currentApple);
        }
        else
        {
            if (isFired == false)
            {
                currentApple.SetActive(false);
                listApple.Add(currentApple);
            }
            currentApple.GetComponent<Rigidbody>().isKinematic = false;
        }

        //if (state == 2)
        //{
        //    selected = candidate;
        //    if (selected != null)
        //    {
        //        selected.GetComponent<Rigidbody>().useGravity = false;
        //        selected.GetComponent<Rigidbody>().drag = 5f;
        //        Anchor.SetParent(selected.transform.parent, true);
        //        selected.transform.SetParent(Anchor, true);
        //    }
        //}
        //else if (selected != null)
        //{
        //    selected.GetComponent<Rigidbody>().useGravity = true;
        //    selected.GetComponent<Rigidbody>().drag = 0.5f;
        //    selected.transform.SetParent(Anchor.parent, true);
        //    Anchor.SetParent(transform, true);
        //    selected = null;
        //}
        //else if (state != 1)
        //    ClearCandidate();
    }

    void SetCandidate(Collider other)
    {
        if (candidate != null)
            ClearCandidate();
        candidate = other.GetComponent<Renderer>();
        if (candidate != null)
        {
            candidate.material.EnableKeyword("_EMISSION");
        }
    }

    void ClearCandidate()
    {
        if (candidate != null)
            candidate.material.DisableKeyword("_EMISSION");
        candidate = null;
    }
}


