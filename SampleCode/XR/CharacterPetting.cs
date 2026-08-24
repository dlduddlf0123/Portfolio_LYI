using System;
using UnityEngine;

namespace Portfolio.SampleCode.XR
{
    /// <summary>
    /// Moves a character bone toward a hand contact point to create a petting effect.
    ///
    /// Adapted from:
    /// 2024/VisionPetty/Character/CharacterPetting.cs
    ///
    /// Animation and character-state calls remain in the original project code.
    /// This sample isolates the spatial deformation and recovery behavior.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CharacterPetting : MonoBehaviour
    {
        [Serializable]
        private sealed class PettingRegion
        {
            [SerializeField] private Transform bone;

            [NonSerialized] public Transform Contact;
            [NonSerialized] public Vector3 RestLocalPosition;
            [NonSerialized] public bool IsActive;

            public Transform Bone => bone;
        }

        [SerializeField] private PettingRegion[] regions = Array.Empty<PettingRegion>();
        [SerializeField, Min(0f)] private float maximumOffset = 0.01f;
        [SerializeField, Min(0f)] private float followSpeed = 12f;
        [SerializeField] private bool constrainVerticalOffset = true;

        private void Awake()
        {
            CaptureRestPose();
        }

        private void LateUpdate()
        {
            float blend = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);

            for (int i = 0; i < regions.Length; i++)
            {
                PettingRegion region = regions[i];
                if (region == null || region.Bone == null)
                {
                    continue;
                }

                if (region.IsActive && region.Contact != null)
                {
                    ApplyContact(region, blend);
                }
                else
                {
                    region.IsActive = false;
                    region.Contact = null;
                    region.Bone.localPosition = Vector3.Lerp(
                        region.Bone.localPosition,
                        region.RestLocalPosition,
                        blend);
                }
            }
        }

        private void OnDisable()
        {
            RestoreImmediately();
        }

        public void CaptureRestPose()
        {
            for (int i = 0; i < regions.Length; i++)
            {
                PettingRegion region = regions[i];
                if (region != null && region.Bone != null)
                {
                    region.RestLocalPosition = region.Bone.localPosition;
                }
            }
        }

        public bool BeginPetting(int regionIndex, Transform contact)
        {
            PettingRegion selectedRegion;
            if (contact == null || !TryGetRegion(regionIndex, out selectedRegion))
            {
                return false;
            }

            for (int i = 0; i < regions.Length; i++)
            {
                PettingRegion region = regions[i];
                if (region == null)
                {
                    continue;
                }

                region.IsActive = false;
                region.Contact = null;
            }

            selectedRegion.Contact = contact;
            selectedRegion.IsActive = true;
            return true;
        }

        public void EndPetting(int regionIndex)
        {
            PettingRegion region;
            if (!TryGetRegion(regionIndex, out region))
            {
                return;
            }

            region.IsActive = false;
            region.Contact = null;
        }

        private void ApplyContact(PettingRegion region, float blend)
        {
            Transform bone = region.Bone;
            Vector3 restWorldPosition = bone.parent == null
                ? region.RestLocalPosition
                : bone.parent.TransformPoint(region.RestLocalPosition);

            Vector3 contactPosition = region.Contact.position;
            if (constrainVerticalOffset)
            {
                contactPosition.y = restWorldPosition.y;
            }

            Vector3 offset = Vector3.ClampMagnitude(
                contactPosition - restWorldPosition,
                maximumOffset);

            bone.position = Vector3.Lerp(
                bone.position,
                restWorldPosition + offset,
                blend);
        }

        private bool TryGetRegion(int index, out PettingRegion region)
        {
            region = null;

            if (index < 0 || index >= regions.Length)
            {
                return false;
            }

            region = regions[index];
            return region != null && region.Bone != null;
        }

        private void RestoreImmediately()
        {
            for (int i = 0; i < regions.Length; i++)
            {
                PettingRegion region = regions[i];
                if (region == null || region.Bone == null)
                {
                    continue;
                }

                region.IsActive = false;
                region.Contact = null;
                region.Bone.localPosition = region.RestLocalPosition;
            }
        }
    }
}
