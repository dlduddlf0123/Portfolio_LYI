using System;
using UnityEngine;

namespace Portfolio.SampleCode.XR
{
    /// <summary>
    /// Detects whether the tracked index finger is extended by comparing two bones.
    ///
    /// Adapted from:
    /// 2024/VRFingFing/Managers/TokTokManager.cs
    ///
    /// The project manager also coordinates selection, movement, UI, and tutorials.
    /// This sample keeps only the tracking validation and gesture decision.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MetaHandGestureDetector : MonoBehaviour
    {
        [Header("Meta hand tracking")]
        [SerializeField] private OVRHand hand;
        [SerializeField] private OVRSkeleton skeleton;

        [Header("Gesture")]
        [SerializeField] private OVRSkeleton.BoneId referenceBone = OVRSkeleton.BoneId.Hand_Index1;
        [SerializeField] private OVRSkeleton.BoneId indexTipBone = OVRSkeleton.BoneId.Hand_IndexTip;
        [SerializeField, Min(0f)] private float minimumExtendedDistance = 0.07f;

        public event Action<bool> GestureChanged;

        public bool IsGestureDetected { get; private set; }
        public float LastMeasuredDistance { get; private set; }

        private void Reset()
        {
            hand = GetComponent<OVRHand>();
            skeleton = GetComponent<OVRSkeleton>();
        }

        private void Awake()
        {
            if (hand == null)
            {
                hand = GetComponent<OVRHand>();
            }

            if (skeleton == null)
            {
                skeleton = GetComponent<OVRSkeleton>();
            }
        }

        private void Update()
        {
            bool detected;
            if (!TryEvaluate(out detected))
            {
                detected = false;
            }

            SetGestureState(detected);
        }

        private void OnDisable()
        {
            SetGestureState(false);
        }

        /// <summary>
        /// Returns false when tracking data is not currently usable.
        /// A valid result can still report detected == false.
        /// </summary>
        public bool TryEvaluate(out bool detected)
        {
            detected = false;
            LastMeasuredDistance = 0f;

            if (hand == null || skeleton == null || !hand.IsTracked)
            {
                return false;
            }

            Transform referenceTransform;
            Transform tipTransform;
            if (!TryGetBoneTransform(referenceBone, out referenceTransform) ||
                !TryGetBoneTransform(indexTipBone, out tipTransform))
            {
                return false;
            }

            LastMeasuredDistance = Vector3.Distance(referenceTransform.position, tipTransform.position);
            detected = LastMeasuredDistance >= minimumExtendedDistance;
            return true;
        }

        private bool TryGetBoneTransform(OVRSkeleton.BoneId boneId, out Transform boneTransform)
        {
            boneTransform = null;

            if (skeleton.Bones == null)
            {
                return false;
            }

            for (int i = 0; i < skeleton.Bones.Count; i++)
            {
                OVRBone bone = skeleton.Bones[i];
                if (bone.Id == boneId && bone.Transform != null)
                {
                    boneTransform = bone.Transform;
                    return true;
                }
            }

            return false;
        }

        private void SetGestureState(bool detected)
        {
            if (IsGestureDetected == detected)
            {
                return;
            }

            IsGestureDetected = detected;
            GestureChanged?.Invoke(detected);
        }
    }
}
