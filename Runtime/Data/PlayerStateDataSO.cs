using UnityEngine;

namespace FPSController
{
    [CreateAssetMenu(menuName = "FPSController/State Data")]
    public class PlayerStateDataSO : ScriptableObject
    {
        [Header("Movement")]
        public float MoveSpeed;
        public float ColliderHeight = 2f;

        [Header("Camera")]
        public float CameraHeight = 1.6f;
        public float BobFrequency;
        public float BobAmplitude;
    }
}
