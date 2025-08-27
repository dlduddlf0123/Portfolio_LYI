using UnityEngine;
using System.Collections;
using Rabyrinth.ReadOnlys;

public class PlayerCamera : MonoBehaviour
{
    public PlayerCharacter Target { get; set; }
    public bool isShake { get; private set; }

    public float dist;
    public float height;

    private float damp;

    private Camera cam;
    private GameManager GameMgr;

    //private ColorCurvesManager color;


    public void Init()
    {
        cam = GetComponent<Camera>();
        GameMgr = MonoSingleton<GameManager>.Inst;
        StartCoroutine(FallowPlayer());
        isShake = false;
        //color = GetComponent<ColorCurvesManager>();
        //color.SaturationA = 0;
    }
    public IEnumerator ShakeCamera(float _time, float _power, ShakeType _type = ShakeType.def)
    {
        isShake = true;

        float time = 0.0f;
        Vector3 pos;
        while (time < _time)
        {
            if (Target.PlayerState == CharacterState.die)
                yield break;

            time += Time.deltaTime;
            pos = Random.insideUnitCircle * _power;
            switch(_type)
            {
                case ShakeType.def:
                    transform.position =
                        new Vector3(
                            transform.position.x + pos.x,
                            transform.position.y + pos.y,
                            transform.position.z);
                    break;
                case ShakeType.hor:
                    transform.position =
                        new Vector3(
                            transform.position.x + pos.x,
                            transform.position.y,
                            transform.position.z);
                    break;
                case ShakeType.ver:
                    transform.position =
                        new Vector3(
                            transform.position.x,
                            transform.position.y + pos.y,
                            transform.position.z);
                    break;
            }
            transform.position =
                new Vector3(transform.position.x + pos.x,
                    transform.position.y + pos.y,
                    transform.position.z);

            yield return null;
        }

        isShake = false;
    }

    private IEnumerator FallowPlayer()
    {
        while (true)
        {
            Vector3 targetPos = Target.transform.position
                - (Vector3.forward * dist)
                + (Vector3.up * height);

            damp = Time.deltaTime * 5.0f;

            transform.position = Vector3.Lerp(transform.position, targetPos, damp);

            yield return null;
        }
    }
}
