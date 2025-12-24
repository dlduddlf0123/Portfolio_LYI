using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollower : MonoBehaviour
{
    [SerializeField]
    GameObject followTarget;

    public ParticleSystem effect_hand;
    public AudioSource sfx_hand;

    TrailRenderer trail_hand;
    Collider coll_hand;

    public float moveSpeed = 8f;
    public bool isRayActive = false;

    private void Awake()
    {
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
        GameManager gameMgr = GameManager.Instance;
        Camera cam = gameMgr.mainCamera;

        while (true)
        {
            if (followTarget.activeSelf)
            {
                transform.position = Vector3.Lerp(transform.position, followTarget.transform.position, moveSpeed * Time.deltaTime);

                if (gameMgr.statGame == GameStatus.GAMEPLAY &&
                    isRayActive)
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
