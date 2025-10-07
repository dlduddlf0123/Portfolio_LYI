using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTokTok.Interaction.Shooting
{

    /// <summary>
    /// 9/26/2023-LYI
    /// 슈팅게임에서의 핵심
    /// 버튼을 한번 누르면 작동 상태가 되어 에임과 몸체를 움직인다
    /// 한번 더 버튼을 누르면 조준하는 방향으로 발사한다
    /// </summary>
    public class Shooter : Tok_Interact
    {
        public Shooter_Catapult catapult;
        public Aim aim; //조준점
        public Arrow arrow; //발사체

        public Transform tr_arrowReady; //화살 장전위치(초기화 위치)

        public Circle_Aim circleAim;

        public float shotPower = 1f;
        public bool isAiming = false;

        public bool isCircleAim = false;

        // Start is called before the first frame update
        void Start()
        {

        }


        public override void InteractInit()
        {
            base.InteractInit();

            if (isCircleAim)
            {
                circleAim.AimInit();
                circleAim.gameObject.SetActive(false);
            }
            else
            {
                aim.AimInit();
                aim.gameObject.SetActive(false);
            }


            arrow.shooter = this;
            arrow.ArrowInit();
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 조준 시작
        /// </summary>
        public void StartAim()
        {
            isAiming = true;

            if (isCircleAim)
            {
                circleAim.isAim = true;
                circleAim.gameObject.SetActive(true);
            }
            else
            {
                aim.gameObject.SetActive(true);
                aim.isAim = true;
            }


        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 화살 발사
        /// </summary>
        public void ShootArrow()
        {
            Vector3 shotDirection;



            if (isCircleAim)
            {
                shotDirection = circleAim.GetAimPoint();
                circleAim.gameObject.SetActive(false);
            }
            else
            {
                shotDirection = tr_arrowReady.position + aim.transform.forward - aim.transform.position;
                aim.gameObject.SetActive(false);
            }

            arrow.ShootArrow(shotDirection, shotPower);
        }


        /// <summary>
        /// 10/10/2023-LYI
        /// 발사 입력 받기
        /// 조준 중이 아니면 조준
        /// 조준 중이면 발사
        /// </summary>
        public override void ActiveInteraction()
        {
            base.ActiveInteraction();

            if (isAiming)
            {
                catapult.Attack();
            }
            else
            {
                StartAim();
            }
        }

        public override void DisableInteraction()
        {
            base.DisableInteraction();
        }

    }
}