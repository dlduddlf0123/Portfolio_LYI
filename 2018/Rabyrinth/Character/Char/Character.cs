using UnityEngine;
using System.Collections;
using Rabyrinth.ReadOnlys;

public class Character : MonoBehaviour
{
    private void Awake()
    {
        DoAwake();
        ChildAwake();
    }

    private void OnEnable()
    {
        Init();
    }

    public CharacterAttribute Status
    {
        get;
        protected set;
    }

    protected virtual void DoAwake() { /* do nothing */ }
    protected virtual void Init() { /* do nothing */ }
    protected virtual void ChildAwake() { /* do nothing */ }

    public virtual void TakeDamage(int attackdamage, HitEffect _type, bool isCritical = false) { }
}
