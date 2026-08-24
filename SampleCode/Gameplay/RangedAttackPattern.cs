using System;
using System.Collections;
using UnityEngine;

namespace Portfolio.SampleCode.Gameplay
{
    public abstract class ProjectilePool : MonoBehaviour
    {
        public abstract PooledProjectile Rent();
    }

    public abstract class PooledProjectile : MonoBehaviour
    {
        public abstract void Launch(Vector2 direction, float speed, int damage);
    }

    /// <summary>
    /// A pooled ranged attack with an aiming telegraph and a locked firing direction.
    ///
    /// Adapted from:
    /// 2023/Burbird/Character/Enemy/Attack/EnemyRangedAttack.cs
    /// 2023/Burbird/Character/Enemy/Attack/EnemyAttack_SnipeShot.cs
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RangedAttackPattern : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform muzzle;
        [SerializeField] private Transform target;
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private LineRenderer telegraph;

        [Header("Attack")]
        [SerializeField, Min(0f)] private float aimDuration = 1.5f;
        [SerializeField, Min(0f)] private float lockedWarningDuration = 0.2f;
        [SerializeField, Min(0f)] private float cooldown = 1f;
        [SerializeField, Min(1)] private int projectileCount = 1;
        [SerializeField, Min(0f)] private float intervalBetweenProjectiles = 0.25f;
        [SerializeField, Min(0f)] private float projectileSpeed = 5f;
        [SerializeField, Min(0)] private int projectileDamage = 10;

        [Header("Telegraph")]
        [SerializeField, Min(0f)] private float maximumTelegraphDistance = 30f;
        [SerializeField] private LayerMask obstructionMask;
        [SerializeField] private Color aimingColor = Color.red;
        [SerializeField] private Color lockedColor = Color.yellow;

        private Coroutine attackRoutine;

        public event Action AttackFinished;

        public bool IsAttacking => attackRoutine != null;

        private void Awake()
        {
            if (telegraph != null)
            {
                telegraph.useWorldSpace = true;
                telegraph.positionCount = 2;
                telegraph.enabled = false;
            }
        }

        public bool BeginAttack()
        {
            if (IsAttacking || muzzle == null || target == null ||
                projectilePool == null || telegraph == null)
            {
                return false;
            }

            attackRoutine = StartCoroutine(AttackSequence());
            return true;
        }

        public void CancelAttack()
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }

            SetTelegraphVisible(false);
        }

        private void OnDisable()
        {
            CancelAttack();
        }

        private IEnumerator AttackSequence()
        {
            SetTelegraphVisible(true);
            SetTelegraphColor(aimingColor);

            Vector2 lockedDirection = GetAimDirection();
            float elapsed = 0f;
            while (elapsed < aimDuration)
            {
                if (muzzle == null || target == null || telegraph == null)
                {
                    AbortSequence();
                    yield break;
                }

                lockedDirection = GetAimDirection();
                UpdateTelegraph(lockedDirection);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetTelegraphColor(lockedColor);
            UpdateTelegraph(lockedDirection);

            if (lockedWarningDuration > 0f)
            {
                yield return new WaitForSeconds(lockedWarningDuration);
            }

            SetTelegraphVisible(false);

            int shotCount = Mathf.Max(1, projectileCount);
            for (int i = 0; i < shotCount; i++)
            {
                Fire(lockedDirection);

                if (i < shotCount - 1 && intervalBetweenProjectiles > 0f)
                {
                    yield return new WaitForSeconds(intervalBetweenProjectiles);
                }
            }

            if (cooldown > 0f)
            {
                yield return new WaitForSeconds(cooldown);
            }

            attackRoutine = null;
            AttackFinished?.Invoke();
        }

        private void Fire(Vector2 direction)
        {
            if (projectilePool == null || muzzle == null)
            {
                return;
            }

            PooledProjectile projectile = projectilePool.Rent();
            if (projectile == null)
            {
                return;
            }

            projectile.transform.SetPositionAndRotation(
                muzzle.position,
                Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, direction.y, 0f)));
            projectile.Launch(direction, projectileSpeed, projectileDamage);
        }

        private Vector2 GetAimDirection()
        {
            Vector2 direction = target.position - muzzle.position;
            return direction.sqrMagnitude > Mathf.Epsilon
                ? direction.normalized
                : (Vector2)muzzle.right;
        }

        private void UpdateTelegraph(Vector2 direction)
        {
            Vector2 origin = muzzle.position;
            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                direction,
                maximumTelegraphDistance,
                obstructionMask);

            Vector2 end = hit.collider == null
                ? origin + direction * maximumTelegraphDistance
                : hit.point;

            telegraph.SetPosition(0, origin);
            telegraph.SetPosition(1, end);
        }

        private void SetTelegraphColor(Color color)
        {
            telegraph.startColor = color;
            telegraph.endColor = color;
        }

        private void SetTelegraphVisible(bool visible)
        {
            if (telegraph != null)
            {
                telegraph.enabled = visible;
            }
        }

        private void AbortSequence()
        {
            SetTelegraphVisible(false);
            attackRoutine = null;
        }
    }
}
