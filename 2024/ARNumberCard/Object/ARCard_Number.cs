using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AroundEffect
{
    public class ARCard_Number : ARCard
    {

        //[Header("Number")]



        public override void ARCardInit()
        {
            base.ARCardInit();
            isSymbol = false;
        }


        public override void OnCardEnable()
        {
            base.OnCardEnable();
        }


        public override void OnCardDisable()
        {
            base.OnCardDisable();
        }

        public override void OnCardAdd(bool isLeft, ARCard card)
        {
            base.OnCardAdd(isLeft, card);

        }

        public override void OnCardRemove(bool isLeft)
        {
            base.OnCardRemove(isLeft);
           
           // gameMgr.arCardMgr.RemoveNumberCard(this);
        }


        //if (card.isSymbol)
        //{
        //    gameMgr.arCardMgr.AddNumberCard(isLeft, card);
        //}

        //양쪽 카드가 차있는 경우?
        //if (leftCard != null && rightCard != null)
        //{
        //    //등록된 것이 있는경우 돌아가기
        //    if (gameMgr.arCardMgr.list_linkedCard.Count > 2)
        //    {
        //        return;
        //    }

        //    //오른쪽 검사 진행
        //    //카드가 오른쪽으로 진행
        //    //등호 체크
        //    Queue<ARCard> rightQueue = new();
        //    ARCard linkedCard = this.GetComponent<ARCard>();
        //    bool isEqual = false;
        //    while (true)
        //    {
        //        if (linkedCard.cardName == "=")
        //        {
        //            isEqual = true;
        //            break;
        //        }
        //        rightQueue.Enqueue(linkedCard.rightCard);
        //        linkedCard = linkedCard.rightCard;
        //        if (linkedCard.rightCard ==null)
        //        {
        //            break;
        //        }
        //    }

        //    //등호가 있었으면
        //    if (isEqual)
        //    {
        //        //왼쪽 카드가 있는 경우
        //        if (leftCard == null)
        //        {

        //        }
        //        else
        //        {
        //            //갯수 체크
        //            Stack<ARCard> leftStack = gameMgr.arCardMgr.StackLeft(this);
        //            //등호 상태 변경
        //            //rightQueue.
        //        }
        //    }

        //    }
    }
}