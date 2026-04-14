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
    }
}