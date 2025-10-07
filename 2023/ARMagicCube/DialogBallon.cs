using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class DialogBallon : MonoBehaviour
{
    GameObject mainCamera;
    Canvas canvas_dialog;

    private void Awake()
    {
        mainCamera = GameManager.Instance.xrOrigin.Camera.gameObject;
        canvas_dialog = this.GetComponent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mainCamera != null){
            canvas_dialog.transform.rotation =
                Quaternion.LookRotation(canvas_dialog.transform.position - mainCamera.transform.position);
            }
    }
}
