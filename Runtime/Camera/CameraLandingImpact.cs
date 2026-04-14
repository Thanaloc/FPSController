using UnityEngine;

namespace FPSController
{
	/// <summary>
	/// Optional component. Produces a short camera dip on landing, proportional to fall speed.
	/// Assign to the Player GameObject alongside the other FPS components.
	/// PlayerMotor reads ImpactOffset if this component is wired in its Inspector slot.
	/// </summary>
	public class CameraLandingImpact : MonoBehaviour
	{
		[SerializeField] private PlayerMotor _Motor;

		[Header("Tuning")]
		[SerializeField, Tooltip("Maximum downward dip in units.")]
		private float _MaxDip = 0.15f;

		[SerializeField, Tooltip("Fall speed (absolute) at which max dip is reached.")]
		private float _MaxFallSpeed = 15f;

		[SerializeField, Tooltip("How fast the offset recovers to zero.")]
		private float _RecoverySpeed = 8f;

		private float _impactOffset;
		private float _impactVelocity;

		/// <summary>
		/// Current vertical camera offset from landing impact. Read by PlayerMotor.
		/// </summary>
		public float ImpactOffset => _impactOffset;

		private void OnEnable()
		{
			_Motor.OnLanded += OnLanded;
		}

		private void OnDisable()
		{
			_Motor.OnLanded -= OnLanded;
		}

		private void Update()
		{
			// Spring-like recovery toward zero
			_impactOffset = Mathf.Lerp(_impactOffset, 0f, _RecoverySpeed * Time.deltaTime);

			// Kill tiny residual
			if (Mathf.Abs(_impactOffset) < 0.0001f)
				_impactOffset = 0f;
		}

		private void OnLanded(float p_verticalVelocity)
		{
			float fallSpeed = Mathf.Abs(p_verticalVelocity);
			float intensity = Mathf.Clamp01(fallSpeed / _MaxFallSpeed);
			_impactOffset = -intensity * _MaxDip;
		}
	}
}