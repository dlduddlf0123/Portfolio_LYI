using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorpuzzle : MonoBehaviour
{
    public List<Colorblock> list_PuzzleBlock = new List<Colorblock>();
    public Movable targetBlock;   //움직여지는 플랫폼
    
    public int[] puzzleKey;
    public int successCount;
    public float waitTime = 1.0f;
    public bool isSolved = false;

    private void Awake()
    {
        successCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            list_PuzzleBlock.Add(this.transform.GetChild(i).GetComponent<Colorblock>());
            list_PuzzleBlock[i].waitTime = waitTime;
        }
    }

    public void PuzzleCheck()
    {
        if (isSolved)
        {
            return;
        }

        for (int i = 0; i < list_PuzzleBlock.Count; i++)
        {
            if (puzzleKey[i] == list_PuzzleBlock[i].colorCount)
            {
                ++successCount;
            }
        }

        if (successCount == list_PuzzleBlock.Count)
        {
            GameManager.Instance.soundMgr.PlaySfx(this.transform, GameManager.Instance.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_SOLVE));
            targetBlock.Active(true);
            isSolved = true;
        }
        Debug.Log(successCount);
        successCount = 0;
    }
}
