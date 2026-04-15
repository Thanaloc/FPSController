using UnityEngine;

namespace FPSController
{
    [CreateAssetMenu(menuName = "FPSController/Settings")]
    public class FPSControllerSettingsSO : ScriptableObject
    {
        [Header("Sprint")]
        public float SprintFOVMultiplier = 0.17f;

        [Header("Jump")]
        public float JumpForce = 5f;

        [Tooltip("Gravity multiplier while falling. Higher = snappier descent.")]
        public float FallGravityMultiplier = 2.5f;

        [Tooltip("Gravity multiplier near apex. Higher = shorter hang time.")]
        public float ApexGravityMultiplier = 3.5f;

        [Tooltip("Vertical speed threshold defining the apex zone.")]
        public float ApexThreshold = 2f;
    }
}