using System;
using UnityEngine;

namespace UltimaGuardia
{
    /// <summary>Authored rigid-piece skeleton. Contains presentation references, never combat state.</summary>
    public sealed class CharacterRig : MonoBehaviour
    {
        public CombatantData definition;
        public Transform hips, chest, head;
        public Transform rightUpperArm, rightForearm, rightHand;
        public Transform leftUpperArm, leftForearm, leftHand;
        public Transform rightThigh, rightShin, rightFoot;
        public Transform leftThigh, leftShin, leftFoot;
        public Transform weaponGrip, weapon, castOrigin, itemOrigin, hitAnchor;
        public Transform[] joints = Array.Empty<Transform>();
        [HideInInspector] public Vector3[] restPositions = Array.Empty<Vector3>();
        [HideInInspector] public Quaternion[] restRotations = Array.Empty<Quaternion>();

        public void CaptureRestPose()
        {
            restPositions = new Vector3[joints.Length];
            restRotations = new Quaternion[joints.Length];
            for (int i = 0; i < joints.Length; i++)
            {
                restPositions[i] = joints[i].localPosition;
                restRotations[i] = joints[i].localRotation;
            }
        }

        public void RestoreRestPose()
        {
            for (int i = 0; i < joints.Length; i++)
                joints[i].SetLocalPositionAndRotation(restPositions[i], restRotations[i]);
        }
    }
}
