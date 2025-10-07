using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : Movable
{
    public TriggerButton leftButton;
    public TriggerButton rightButton;
    public TriggerButton verticalUpButton;
    public TriggerButton verticalDownButton;

    public TriggerButton upTriggerButton;
    public TriggerButton downTriggerButton;

    public Quaternion startPos = Quaternion.Euler(0, 0, 0);
    public Quaternion endPos = Quaternion.Euler(90, 0, 0);
    
    float lerpTime = 0;

    private void Awake()
    {
        verticalUpButton.gameObject.SetActive(false);
        verticalDownButton.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        leftButton.SetButtonCallBack(() => Left());
        rightButton.SetButtonCallBack(() => Right());
        verticalUpButton.SetButtonCallBack(
            () => StartCoroutine(CraneMove(startPos, endPos)));
        verticalDownButton.SetButtonCallBack(
            () => StartCoroutine(CraneMove(endPos, startPos)));
    }

    public IEnumerator CraneMove(Quaternion _start, Quaternion _end)
    {
        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(_start, _end, lerpTime);
            yield return new WaitForSeconds(0.01f);
        }
        transform.rotation = _end;
        lerpTime = 0;
    }

    public override void Plus()
    {
        StopAllCoroutines();
        StartCoroutine(CraneMove(endPos, startPos));
    }

    public override void Minus()
    {
        StopAllCoroutines();
        StartCoroutine(CraneMove(startPos, endPos));
    }

    public override void Left()
    {
        StartCoroutine(CraneMove(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, Mathf.Round(transform.eulerAngles.y - 90), transform.eulerAngles.z)));
    }

    public override void Right()
    {
        StartCoroutine(CraneMove(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, Mathf.Round(transform.eulerAngles.y + 90), transform.eulerAngles.z)));
    }
}
