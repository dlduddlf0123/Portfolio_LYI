using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VRTokTok.Manager;
using UnityEngine.Events;
using Oculus.Interaction;
using VRTokTok.Character;

namespace VRTokTok { 

    /// <summary>
    /// 10/27/2023-LYI
    /// TokGround 활용
    /// PokeInteractable
    /// 
    /// </summary>
public class TokSelect : MonoBehaviour
{
        HeaderSelect headerSelect;
        public Tok_CheeringCharacter header;

        public InteractableUnityEventWrapper Event { get; set; } //바닥 이벤트 체크, 스테이지마다 변경됨


        public UnityAction onSelectTok;

        bool isFirst = true;


        private void Start()
        {
            Init();
        }


        public void Init()
        {
            onSelectTok = OnTok;

            if (isFirst)
            {
                headerSelect = GetComponentInParent<HeaderSelect>();
                Event = GetComponent<InteractableUnityEventWrapper>();
                header = GetComponentInParent<Tok_CheeringCharacter>();

                Event.WhenSelect.AddListener(onSelectTok);
                isFirst = false;
            }
        }


        /// <summary>
        /// 10/27/2023-LYI
        /// 캐릭터 터치 시 동작
        /// 선택된 캐릭터 보내기
        /// </summary>
        public void OnTok()
        {
            headerSelect.SelectTok(header);
        }

    }
}
