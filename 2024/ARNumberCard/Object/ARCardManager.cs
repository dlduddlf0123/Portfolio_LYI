using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace AroundEffect {

    /// <summary>
    /// 8/26/2024-LYI
    /// Manager of arcards
    /// 카드 연결만 관리, 계산식 계산
    /// </summary>
    public class ARCardManager : MonoBehaviour
    {
        public List<GameObject> list_activeCard = new();
        public List<GameObject> list_disableCard = new();



        //public List<ARCard> list_addedCard = new();
        ////연결된 AR카드 정리
        //public LinkedList<ARCard> list_linkedCard = new();

        //public int cardResult = 0;

        //public void RefreshList()
        //{
        //    list_addedCard.Clear();

        //    foreach (var item in list_linkedCard)
        //    {
        //        list_addedCard.Add(item);
        //    }

        //}


        public void ResetAllCard()
        {
            if (list_activeCard.Count > 0)
            {
                for (int i = 0; i < list_activeCard.Count; i++)
                {
                    if (!list_activeCard[i].GetComponent<ARCard>().isCombined)
                    {
                        list_activeCard[i].GetComponent<ARCard>().OnCardDisable();
                    }
                    //list_activeCard[i].gameObject.SetActive(false);
                }

                //list_activeCard.Clear();
            }
          
        }



        /// <summary>
        /// 8/26/2024-LYI
        /// Stack으로 수식 왼쪽 집어넣기
        /// </summary>
        /// <param name="card"></param>
        public Queue<ARCard> QueueLeft(ARCard card)
        {
            Queue<ARCard> leftQueue = new();
            ARCard linkedCard = card;
            while (true)
            {
                leftQueue.Enqueue(linkedCard.leftCard);
                linkedCard = linkedCard.leftCard;

                if (linkedCard.leftCard == null)
                {
                    break;
                }
            }

            return leftQueue;
        }

        /// <summary>
        /// 8/26/2024-LYI
        /// Queue로 수식 오른쪽 집어넣기
        /// </summary>
        /// <param name="card"></param>
        public Queue<ARCard> QueueRight(ARCard card)
        {
            //카드가 오른쪽으로 진행
            Queue<ARCard> rightQueue = new Queue<ARCard>();
            ARCard linkedCard = card;
            while (true)
            {
                rightQueue.Enqueue(linkedCard.rightCard);
                linkedCard = linkedCard.rightCard;

                if (linkedCard.rightCard == null)
                {
                    break;
                }
            }

            return rightQueue;
        }

        public Stack<ARCard> StackLeft(ARCard card)
        {
            //갯수 체크
            Stack<ARCard> leftStack = new();
            ARCard cardNode = card;
            while (cardNode.leftCard != null)
            {
                leftStack.Push(cardNode.leftCard);
                cardNode = cardNode.leftCard;
            }

            return leftStack;
        }



        //  /// <summary>
        //  /// 8/26/2024-LYI
        //  /// 기호와 충돌 시 호출
        //  /// 계산할 카드 등록
        //  /// 수식 양쪽으로 붙었을 때만 등록??
        //  /// +, -로부터 Linked list 끝까지 탐색 진행하면서 등록, 순서 정렬
        //  /// </summary>
        //  /// <param name="card"></param>
        //  public void AddSymbolCard(bool isLeft, ARCard card)
        //  {
        //      if (!card.isSymbol)
        //      {
        //          return;
        //      }

        //      //왼쪽은 Insert, 오른쪽은 Add
        //      if (list_linkedCard.Count == 0)
        //      {
        //          //왼쪽부터 넣기
        //          Queue<ARCard> leftQueue = QueueLeft(card);
        //          int loopCount = leftQueue.Count;
        //          for (int i = 0; i < loopCount; i++)
        //          {
        //              list_linkedCard.AddFirst(leftQueue.Dequeue());
        //          }

        //          //현재 기호 넣기
        //          list_linkedCard.AddLast(card);

        //          //오른쪽 넣기
        //          Queue<ARCard> rightQueue = QueueRight(card);
        //          loopCount = rightQueue.Count;
        //          for (int i = 0; i < loopCount; i++)
        //          {
        //              list_linkedCard.AddLast(rightQueue.Dequeue());
        //          }
        //      }
        //      else if(list_linkedCard.Count > 2)
        //      {
        //          //이미 수식이 적용되어 있는 경우
        //          Queue<ARCard> leftQueue = QueueLeft(card);
        //          Queue<ARCard> rightQueue = QueueRight(card);

        //          //해당 것이 어디로 붙는가?
        //          if (isLeft)
        //          {
        //              //현재 기호 넣기
        //              list_linkedCard.AddFirst(card);
        //              int loopCount = leftQueue.Count;
        //              //왼쪽부터 넣기
        //              for (int i = 0; i < loopCount; i++)
        //              {
        //                  list_linkedCard.AddFirst(leftQueue.Dequeue());
        //              }

        //          }
        //          else
        //          {    //현재 기호 넣기
        //              list_linkedCard.AddLast(card);
        //              int loopCount = rightQueue.Count;
        //              //오른쪽 넣기
        //              for (int i = 0; i < loopCount; i++)
        //              {
        //                  list_linkedCard.AddLast(rightQueue.Dequeue());
        //              }
        //          }

        //      }
        //      RefreshList();
        //  }


        //  /// <summary>
        //  /// 8/26/2024-LYI
        //  /// 숫자 카드가 추가로 붙을 시?
        //  /// </summary>
        //  public void AddNumberCard(bool isLeft, ARCard card)
        //  {
        //      //수식이 없으므로 등록하지 않는다
        //      if (list_linkedCard.Count < 3)
        //      {
        //          return;
        //      }

        //      if (list_linkedCard.Count > 2)
        //      {
        //          //이미 수식이 적용되어 있는 경우
        //          Queue<ARCard> leftQueue = QueueLeft(card);
        //          Queue<ARCard> rightQueue = QueueRight(card);

        //          //왼쪽에서 붙는가
        //          if (isLeft)
        //          { 
        //              //현재 기호 넣기
        //              list_linkedCard.AddFirst(card);
        //              //왼쪽부터 넣기
        //              for (int i = 0; i < leftQueue.Count; i++)
        //              {
        //                  list_linkedCard.AddFirst(leftQueue.Dequeue());
        //              }
        //          }
        //          else
        //          {
        //              //현재 기호 넣기
        //              list_linkedCard.AddLast(card);
        //              //오른쪽 넣기
        //              for (int i = 0; i < rightQueue.Count; i++)
        //              {
        //                  list_linkedCard.AddLast(rightQueue.Dequeue());
        //              }
        //          }

        //      }

        //      RefreshList();

        //  }


        //  /// <summary>
        //  /// 8/26/2024-LYI
        //  /// 기호가 제거되는 경우
        //  /// 양쪽 심볼이 없으면 클리어
        //  /// 아니면 더 많은쪽 남기고 자신+반대 방향 제거
        //  /// 같은 크기면 2개 덩어리?
        //  /// 덩어리 같이 작동하게 못하나
        //  /// 일단 되는것 부터 해라
        //  /// </summary>
        //  public void RemoveSymbolCard(ARCard card)
        //  {
        //      if (list_linkedCard.Count < 3)
        //      {
        //          list_linkedCard.Clear();
        //          RefreshList();
        //          return;
        //      }


        //      //카드가 아니라 노드 기준
        //      LinkedListNode<ARCard> node = list_linkedCard.Find(card);

        //      //해당 카드 기준 오른쪽 왼쪽 길이 체크
        //      int rightCount = 0;
        //      int leftCount = 0;
        //      int symbolCount = 0;
        //      LinkedListNode<ARCard> nodeRight = node;
        //      LinkedListNode<ARCard> nodeLeft = node;


        //      while (true)
        //      {
        //          if (nodeRight.Next == null)
        //          {
        //              break;
        //          }
        //          rightCount++;
        //          nodeRight = nodeRight.Next;
        //          if (nodeRight.Value.isSymbol) symbolCount++;

        //      }
        //      while (true)
        //      {
        //          if (nodeRight.Previous == null)
        //          {
        //              break;
        //          }
        //          leftCount++;
        //          nodeLeft= nodeLeft.Previous;
        //          if (nodeLeft.Value.isSymbol) symbolCount++;
        //      }

        //      //심볼 적으면 없애기
        //      if (symbolCount < 2)
        //      {
        //          list_linkedCard.Clear();
        //      }
        //      else
        //      {
        //          if (leftCount < rightCount)
        //          {
        //              for (int i = 0; i < leftCount + 1; i++)
        //              {
        //                  list_linkedCard.RemoveFirst();
        //              }
        //          }
        //          else
        //          {
        //              for (int i = 0; i < rightCount + 1; i++)
        //              {
        //                  list_linkedCard.RemoveLast();
        //              }
        //          }
        //      }

        //      RefreshList();


        //      ////왼쪽이 짤렸으면 오른쪽 아니면 반대
        //      //if (card.leftCard == null)
        //      //{
        //      //    Queue<ARCard> rightQueue = QueueRight(card);

        //      //    int loopCount = rightQueue.Count+1;
        //      //    for (int i = 0; i < loopCount; i++)
        //      //    {
        //      //        list_linkedCard.RemoveLast();
        //      //    }
        //      //}
        //      //else if (card.rightCard == null)
        //      //{
        //      //    Queue<ARCard> leftQueue = QueueLeft(card);

        //      //    int loopCount = leftQueue.Count+1;
        //      //    for (int i = 0; i < loopCount; i++)
        //      //    {
        //      //        list_linkedCard.RemoveFirst();
        //      //    }
        //      //}


        //  }


        //  /// <summary>
        //  /// 8/27/2024-LYI
        //  /// 숫자 카드 제거
        //  /// </summary>
        //  /// <param name="card"></param>
        //  public void RemoveNumberCard(ARCard card)
        //  {
        //      if (list_linkedCard.Count < 3)
        //      {
        //          return;
        //      }

        //      //왼쪽이 짤렸으면 오른쪽 아니면 반대
        //      if (card.leftCard == null)
        //      {
        //          Queue<ARCard> rightQueue = QueueRight(card);

        //          for (int i = 0; i < rightQueue.Count + 1; i++)
        //          {
        //              list_linkedCard.RemoveLast();
        //          }
        //      }
        //      else if (card.rightCard == null)
        //      {
        //          Queue<ARCard> leftQueue = QueueLeft(card);

        //          for (int i = 0; i < leftQueue.Count + 1; i++)
        //          {
        //              list_linkedCard.RemoveFirst();
        //          }
        //      }

        //      RefreshList();

        //  }

        //  /// <summary>
        //  /// 8/26/2024-LYI
        //  /// =에서 호출 식 결과값 전달
        //  /// </summary>
        //  public int CalculateARCardResult()
        //  {
        //      if (list_linkedCard.Count <1)
        //      {
        //          return 0;
        //      }

        //      string expression = "";

        //      LinkedListNode<ARCard> node = list_linkedCard.First;

        //      while (node != null)
        //      {
        //          expression += node.Value.cardName;
        //          node = node.Next;
        //      }
        //      //for (int i = 0; i < list_linkedCard.Count; i++)
        //      //{
        //      //    expression += list_linkedCard.First.Value.cardName;
        //      //}

        //      //계산을 위한 선언
        //      DataTable table = new DataTable();
        //      //수식형 문자열을 계산
        //      int result = Convert.ToInt32(table.Compute(expression, string.Empty));

        //      cardResult = result;

        //      return cardResult;
        //  }






    }
}