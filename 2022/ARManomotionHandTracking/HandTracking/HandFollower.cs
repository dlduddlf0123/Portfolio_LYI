using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollower : MonoBehaviour
{
    [SerializeField]
    GameObject followTarget;

    GameObject handChecker;

    public ParticleSystem effect_hand;
    public AudioSource sfx_hand;

    TrailRenderer trail_hand;
    Collider coll_hand;

    public float moveSpeed = 8f;

    private void Awake()
    {
        handChecker = GameManager.Instance.arMainCamera.transform.GetChild(0).gameObject;

        effect_hand = transform.GetChild(0).GetComponent<ParticleSystem>();
        sfx_hand = transform.GetChild(0).GetComponent<AudioSource>();

        if (GetComponent<TrailRenderer>())
        {
            trail_hand = GetComponent<TrailRenderer>();
        }
        coll_hand = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        transform.position = followTarget.transform.position;
        StartCoroutine(FollowHand());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator FollowHand()
    {
        Camera cam = GameManager.Instance.arMainCamera;
        while (true)
        {

            if (followTarget.activeSelf)
            {
                transform.position = Vector3.Lerp(transform.position, followTarget.transform.position, moveSpeed * Time.deltaTime);

                if (GameManager.Instance.statGame == GameStatus.INTERACTION)
                {
                    RaycastHit hit;
                    float rayDist = 1000f;

                    Ray ray = cam.ScreenPointToRay(cam.WorldToScreenPoint(this.transform.position));

                    Debug.DrawRay(ray.origin, ray.direction * rayDist, Color.red, 0.02f);

                    if (Physics.Raycast(ray, out hit, rayDist))
                    {
                        if (hit.collider.gameObject.layer == 11)
                        {
                            RayInteractObject _ray = hit.collider.GetComponent<RayInteractObject>();
                            _ray.rayOriginTag = this.gameObject.tag;
                            _ray.m_RayEvent.Invoke();
                        }
                    }
                }
            

                if (Vector3.Distance(followTarget.transform.position, handChecker.transform.position) < 1f)
                {
                    ToggleHandActive(false);
                }
                else
                {
                    ToggleHandActive(true);
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void ToggleHandActive(bool _isOn)
    {
        if (trail_hand != null)
        {
            trail_hand.enabled = _isOn;
        }
    }

    public void ToggleHandEffect(bool _isOn)
    {
        if (_isOn)
        {
            effect_hand.Play();
            sfx_hand.Play();
        }
        else
        {
            effect_hand.Stop();
            sfx_hand.Stop();
        }
    }
}
