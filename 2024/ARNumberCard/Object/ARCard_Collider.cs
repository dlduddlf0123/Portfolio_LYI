using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AroundEffect
{

    /// <summary>
    /// 8/26/2024-LYI
    /// 왼쪽 오른쪽 등록만 진행
    /// 계산은 매니저가 한다
    /// </summary>
    public class ARCard_Collider : MonoBehaviour
    {
        GameManager gameMgr;

        public ARCard arCard;


        public bool isLeft = false;
        public bool isCardAdded;


        public void ColldierInit()
        {
            gameMgr = GameManager.Instance;
            isCardAdded = false;
        }


        private void OnTriggerEnter(Collider coll)
        {
            if (isLeft)
            {
                if (coll.gameObject.CompareTag("RightCard"))
                {
                    ARCard card = coll.gameObject.GetComponentInParent<ARCard>();

                    if (card.rightCard != card)
                    {
                        arCard.OnCardAdd(isLeft, card);
                    }
                    //OnCardAdd(coll.gameObject.GetComponentInParent<ARCard>());
                }
            }
            else
            {
                if (coll.gameObject.CompareTag("LeftCard"))
                {
                    ARCard card = coll.gameObject.GetComponentInParent<ARCard>();

                    if (card.leftCard != card)
                    {
                        arCard.OnCardAdd(isLeft, card);
                    }
                    //OnCardAdd(coll.gameObject.GetComponentInParent<ARCard>());
                }
            }
        }

        private void OnTriggerExit(Collider coll)
        {
            if (isLeft)
            {
                if (coll.gameObject.CompareTag("RightCard"))
                {
                    ARCard card = coll.gameObject.GetComponentInParent<ARCard>();

                    if (card.rightCard == arCard)
                    {
                        arCard.OnCardRemove(isLeft);
                        card.OnCardRemove(!isLeft);
                    }
                }
            }
            else
            {
                if (coll.gameObject.CompareTag("LeftCard"))
                {
                    ARCard card = coll.gameObject.GetComponentInParent<ARCard>();

                    if (card.leftCard == arCard)
                    {
                        arCard.OnCardRemove(isLeft);
                        card.OnCardRemove(!isLeft);
                    }
                }
            }

        }


        //public void OnCardAdd(ARCard card)
        //{
        //    if (isCardAdded) return;
        //    isCardAdded = true;

        //    //gameMgr.arCardMgr.list_linkedCard.Add(arCard); //링크 등록
        //    Debug.Log(arCard.cardName + "-OnCardAdd:" + card.cardName);


        //    if (isLeft)
        //    {
        //        arCard.leftCard = card;
        //    }
        //    else
        //    {
        //        arCard.rightCard = card;
        //    }

        //    if (arCard.isSymbol)
        //    {
        //        arCard.AddSymbol();
        //    }

        //}
        //public void OnCardRemove()
        //{
        //    if (!isCardAdded) return;
        //    isCardAdded = false;

        //    //gameMgr.arCardMgr.list_linkedCard.Remove(arCard); //링크 제거
        //    Debug.Log(arCard.cardName + "-OnCardRemove");

        //    if (isLeft)
        //    {
        //        arCard.leftCard = null;
        //    }
        //    else
        //    {
        //        arCard.rightCard = null;
        //    }

        //    if (arCard.isSymbol)
        //    {
        //        gameMgr.arCardMgr.RemoveCard();
        //    }
        //}


    }
}