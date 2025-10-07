using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldCanvas : MonoBehaviour
{
    public Image img_timer;
    ParticleSystem particle_timer;

    Coroutine currentCoroutine = null;

    Quaternion originalRot;

    void Awake()
    {
        particle_timer = img_timer.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        img_timer.fillAmount = 0.0f;
        originalRot = transform.rotation;
    }

    void Update()
    {
        transform.rotation = originalRot * GameManager.Instance.arMainCamera.transform.rotation;
    }

    public void StopTimer()
    {
        if (currentCoroutine != null)
        {
            img_timer.fillAmount = 0.0f;
            StopCoroutine(currentCoroutine);
        }
    }

    public void StartTimer(Vector3 _pos, float _time, UnityAction _action)
    {
        StopTimer();
        transform.position = _pos;
        currentCoroutine = StartCoroutine(TimerIcon(_time, _action));
    }

    IEnumerator TimerIcon(float _time, UnityAction _action)
    {
        float t = 0.0f;

        img_timer.fillAmount = 0.0f;
        while (t < _time)
        {
            img_timer.fillAmount += Time.deltaTime / _time;
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        img_timer.fillAmount = 0.0f;
        particle_timer.Play();
        GameManager.Instance.soundMgr.PlaySfx(transform.position, ReadOnly.Defines.SOUND_SFX_INTERACT);
        _action.Invoke();
    }
}
