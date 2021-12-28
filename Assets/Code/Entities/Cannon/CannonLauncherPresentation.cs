using ByteSize;
using MoreMountains.Feedbacks;
using UnityEngine;
using Zenject;

namespace ToyGun
{
	public class CannonLauncherPresentation : ScriptWithDependencies
	{
		public Collider2D Collider => _collider;
		public Transform Pivot => _pivot;
		public Vector2 Center => _pivot.position;

		[SerializeField]
		private Transform _pivot;
		
		[SerializeField]
		private Vector2 _intensityToOffsetMapping;

		[SerializeField]
		private MMFeedbacks _releaseFeedbacks;
		
		[SerializeField]
		private MMFeedbacks _launchFeedbacks;
	
		private Collider2D _collider;
		
		private OneTypeOneInstanceDependency<CannonLauncherBehaviour> _launcherDependency = new OneTypeOneInstanceDependency<CannonLauncherBehaviour>();

		protected override Dependency[] CollectDependencies() => 
			new Dependency[] { _launcherDependency };
		
		public override void OnCreation()
		{
			base.OnCreation();
			_launcherDependency.OnReferenceAcquired += OnReferenceAcquired;
			_launcherDependency.OnReferenceLost += OnReferenceLost;
		}

		void Awake()
		{
			_collider = GetComponent<Collider2D>();
			if (_collider == null)
				throw new MissingComponentException("EXCEPTION");
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			_launcherDependency.OnReferenceAcquired -= OnReferenceAcquired;
			_launcherDependency.OnReferenceLost -= OnReferenceLost;
		}

		public void ComputeOffsets(out float min, out float max)
		{
			min = (Center - (Vector2)Pivot.TransformPoint(new Vector2(0.0f, _intensityToOffsetMapping.x))).magnitude;
			max = (Center - (Vector2)Pivot.TransformPoint(new Vector2(0.0f, _intensityToOffsetMapping.y))).magnitude;
		}
		public float ConvertOffsetToIntensity(float offset)
		{
			ComputeOffsets(out var min, out var max);
			return Mathf.InverseLerp(min, max, offset);
		}

		private void OnReferenceAcquired(CannonLauncherBehaviour cannonLauncherBehaviour)
		{
			cannonLauncherBehaviour.OnHoldStart += OnHoldStart;
			cannonLauncherBehaviour.OnHold += OnHold;
			cannonLauncherBehaviour.OnRelease += OnRelease;
			cannonLauncherBehaviour.OnLaunch += OnLaunch;
		}
		private void OnReferenceLost(CannonLauncherBehaviour cannonLauncherBehaviour)
		{
			cannonLauncherBehaviour.OnHoldStart -= OnHoldStart;
			cannonLauncherBehaviour.OnHold -= OnHold;
			cannonLauncherBehaviour.OnRelease -= OnRelease;
			cannonLauncherBehaviour.OnLaunch -= OnLaunch;
		}

		private void OnHoldStart()
		{
			
		}
		private void OnHold(float angle, float intensity)
		{
			var localPosition = transform.localPosition;
			localPosition.y = Mathf.Lerp(_intensityToOffsetMapping.x, _intensityToOffsetMapping.y, intensity);
			transform.localPosition = localPosition;

			Pivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		private void OnRelease()
		{
			if (!_launcherDependency.TryGetReference(out var launcherBehaviour))
				return;
			
			_releaseFeedbacks?.PlayFeedbacks(transform.position, launcherBehaviour.Intensity);
		}
		private void OnLaunch()
		{
			if (!_launcherDependency.TryGetReference(out var launcherBehaviour))
				return;
			
			_launchFeedbacks?.PlayFeedbacks(transform.position, launcherBehaviour.Intensity);
		}
	}
}