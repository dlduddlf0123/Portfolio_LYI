using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 음식물 담는 그릇, 주인이 정해져 있음
/// </summary>
public class FoodBowl : MonoBehaviour
{
    StageManager stageMgr;
    public Character bowlMaster;
    public Transform[] dishPoints;

    public Food bowlFood;

    private void Start()
    {
        stageMgr = GameManager.Instance.currentPlay.GetComponent<StageManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
             other.transform.parent = this.transform;
            other.transform.localPosition = new Vector3(0, other.transform.localPosition.y, 0);
            // other.transform.localRotation = Quaternion.identity;

            GameManager.Instance.PlayEffect(this.transform, GameManager.Instance.particles[1], ReadOnly.Defines.SOUND_SFX_CLICK);

            stageMgr.ChangeSelectCharacter(bowlMaster);
            bowlMaster.Stop();
            bowlMaster.isEvent = true;
            bowlMaster.StartCoroutine(bowlMaster.MoveToBowl(other.gameObject, dishPoints));
        }
    }

}
