using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace AroundEffect
{
    [RequireComponent(typeof(Canvas))]
    public class CameraFollowCanvas : MonoBehaviour
    {
        GameObject mainCamera;
        Canvas canvas_dialog;

        private void Awake()
        {
            mainCamera = GameManager.Instance.mainCam.gameObject;
            canvas_dialog = this.GetComponent<Canvas>();
        }


        // Update is called once per frame
        void Update()
        {
            if (mainCamera != null)
            {
                canvas_dialog.transform.rotation =
                    Quaternion.LookRotation(canvas_dialog.transform.position - mainCamera.transform.position);
            }
        }
    }
}