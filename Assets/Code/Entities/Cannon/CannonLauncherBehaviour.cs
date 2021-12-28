using System;
using System.Collections;
using ByteSize;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ToyGun
{
	public class CannonLauncherBehaviour : Script
	{
		public event Action OnHoldStart;
		public event Action<float, float> OnHold;
		public event Action OnRelease; 
		public event Action OnLaunch;

		public float Angle => _angle;
		public float Intensity => _intensity;

		[FoldoutGroup("Angle"), SerializeField, Min(0.0f)]
		private float _angleSmoothing;
		
		[FoldoutGroup("Angle"), SerializeField, Range(0.0f, 360.0f)]
		private float _startingAngle;
		
		[FoldoutGroup("Angle"), SerializeField, Range(45.0f, 180.0f)]
		private float _angleConstraint;
		
		[FoldoutGroup("Intensity"), SerializeField, Min(0.0f)]
		private float _intensitySmoothing;

		[FoldoutGroup("Intensity"), SerializeField]
		private Vector2 _intensityToForceMapping;

		[FoldoutGroup("Release"), SerializeField, Min(0.0f)]
		private float _launchDelay;
		
		[FoldoutGroup("Release"), SerializeField, Min(0.0f)]
		private float _cooldown;

		private Coroutine _launchRoutine;
		private Vector2 _angleConstraintRange;
		private float _angleSmoothingVelocity;
		private float _angle;
		private float _intensitySmoothingVelocity;
		private float _intensity;

		void Awake()
		{
			_angle = _startingAngle;
			
			var halfAngleConstraint = _angleConstraint * 0.5f;
			_angleConstraintRange = new Vector2(Mathp.WrapAngle(_startingAngle - halfAngleConstraint), Mathp.WrapAngle(_startingAngle + halfAngleConstraint));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			if (_launchRoutine != null)
				StopCoroutine(_launchRoutine);
		}

		public bool TryStartHold()
		{
			if (_launchRoutine != null)
				return false;
			
			OnHoldStart?.Invoke();
			return true;
		}
		public void Hold(Vector2 direction, float intensity)
		{
			var angle = Mathp.ConvertDirectionToAngle(direction);
			angle = Mathp.ClampAngle(angle, _angleConstraintRange.x, _angleConstraintRange.y);
			_angle = Mathf.SmoothDampAngle(_angle, angle, ref _angleSmoothingVelocity, _angleSmoothing);
			
			intensity = Mathf.Clamp01(intensity);
			_intensity = Mathf.SmoothDamp(_intensity, intensity, ref _intensitySmoothingVelocity, _intensitySmoothing);
			
			OnHold?.Invoke(_angle, _intensity);
		}

		public void Release()
		{
			var force = Mathf.Lerp(_intensityToForceMapping.x, _intensityToForceMapping.y, _intensity);
			_launchRoutine = StartCoroutine(LaunchRoutine(force));
			
			OnRelease?.Invoke();
		}

		private IEnumerator LaunchRoutine(float force)
		{
			yield return new WaitForSeconds(_launchDelay);
			
			Debug.Log($"Throwing: {force}");
			OnLaunch?.Invoke();
			
			yield return new WaitForSeconds(_cooldown);

			_intensity = 0.0f;
			_launchRoutine = null;
		}
	}
}