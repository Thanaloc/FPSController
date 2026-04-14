using UnityEngine;

namespace FPSController
{
    [CreateAssetMenu(menuName = "FPSController/Settings")]
    public class FPSControllerSettingsSO : ScriptableObject
    {
        [Header("Sprint")]
        public float SprintFOVMultiplier = 0.17f;

        [Header("Jump")]
        public float JumpForce = 7f;
        [Tooltip("Grace period after leaving ground where jump is still allowed.")]
        public float CoyoteTime = 0.12f;
        [Tooltip("Buffer window: pressing jump just before landing queues the jump.")]
        public float JumpBufferTime = 0.1f;

        [Header("Air")]
        [Range(0f, 1f)]
        public float AirControlMultiplier = 0.4f;

        [Header("Gravity")]
        [Tooltip("Gravity multiplier while falling. Higher = snappier descent.")]
        public float FallGravityMultiplier = 2.2f;

        [Tooltip("Vertical speed below which apex gravity kicks in.")]
        public float ApexThreshold = 2.5f;

        [Tooltip("Gravity multiplier in the apex zone. Higher = snappier peak.")]
        public float ApexGravityMultiplier = 3f;
    }
}