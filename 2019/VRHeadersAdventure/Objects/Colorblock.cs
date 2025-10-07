using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Colorblock : MonoBehaviour
{
    public enum BlockColor
    {
        NONE =0,
        RED,
        BLUE,
        YELLOW
    }
    public enum Interaction
    {
        COLORPUZZLE = 0,
        MOVING,
    }

    public BlockColor blockcolor;
    public Interaction interaction;

    public Colorpuzzle colorPuzzle;
    Material mMaterial;

    public float waitTime = 3.0f;
    public int colorCount;
    public bool isPressed;

    private void Awake()
    {
        colorPuzzle = transform.parent.GetComponent<Colorpuzzle>();
        mMaterial = GetComponent<MeshRenderer>().material;
    }
    void Start()
    {
        blockcolor = 0;
        ChangeColor();
        if(colorPuzzle == null)
        {
            return;
        }
        isPressed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Player"&&
            isPressed == false &&
            colorPuzzle.isSolved == false)
        {
            blockcolor = blockcolor + 1;
                   
            ChangeColor();
            StartCoroutine("Timer");

            if (interaction == Interaction.COLORPUZZLE)
            {
                colorPuzzle.PuzzleCheck();
            }
            if(interaction == Interaction.MOVING)
            {
                return;       
            }

            GameManager.Instance.soundMgr.PlaySfx(this.transform, GameManager.Instance.soundMgr.LoadClip(ReadOnly.Defines.SOUND_SFX_BUTTON));
        }      
    }

    public void ChangeColor()
    {
        switch (blockcolor)
        {
            case BlockColor.NONE:
                colorCount = 0;
                break;
            case BlockColor.RED:
                mMaterial.color = Color.red;
                colorCount = 1;              
                break;
            case BlockColor.BLUE:
                mMaterial.color = Color.blue;
                colorCount = 2;               
                break;
            case BlockColor.YELLOW:
                mMaterial.color = Color.yellow;
                blockcolor = BlockColor.NONE;
                colorCount = 3;            
                break;
            default:
                break;
        }
        
    }

   

    IEnumerator Timer()
    {
        isPressed = true;
        yield return new WaitForSeconds(waitTime);
        isPressed = false;
    }
}
