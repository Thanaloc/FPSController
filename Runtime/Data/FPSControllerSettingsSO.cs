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
    }
}