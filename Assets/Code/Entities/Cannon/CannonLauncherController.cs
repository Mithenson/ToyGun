using System.Runtime.Serialization;
using ByteSize;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ToyGun
{
	public class CannonLauncherController : ScriptWithDependencies
	{
		[SerializeField]
		private float _maxDragOffset;

		[SerializeField]
		private AnimationCurve _offsetToIntensityMapping;
		
		private CameraService _cameraService;
		private InputService _inputService;
		private InputAction _aimInput;
		private InputAction _holdInput;
		private Entity _crosshair;
		
		private OneTypeOneInstanceDependency<CannonLauncherBehaviour> _launcherBehaviourDependency = new OneTypeOneInstanceDependency<CannonLauncherBehaviour>();
		private OneTypeOneInstanceDependency<CannonLauncherPresentation> _launcherPresentationDependency = new OneTypeOneInstanceDependency<CannonLauncherPresentation>();

		private bool _isHolding;

		[Inject]
		public void Inject(
			CameraService cameraService,
			InputService inputService,
			[Inject(Id = LookupId.Crosshair)] Entity crosshair)
		{
			_cameraService = cameraService;
			_inputService = inputService;

			if (!_inputService.TryGetInput(InputKind.CrossPlatform_Aim, out _aimInput)
			    || !_inputService.TryGetInput(InputKind.CrossPlatform_Hold, out _holdInput))
				throw new InvalidDataContractException("EXCEPTION");

			_crosshair = crosshair;
		}
		
		protected override Dependency[] CollectDependencies() =>
			new Dependency[] { _launcherBehaviourDependency, _launcherPresentationDependency };

		void OnEnable() =>
			_holdInput.performed += OnHoldInputPerformed;
		void OnDisable() =>
			_holdInput.performed -= OnHoldInputPerformed;

		private void OnHoldInputPerformed(InputAction.CallbackContext ctxt)
		{
			if (!_launcherBehaviourDependency.TryGetReference(out var launcher)
			    || !_launcherPresentationDependency.TryGetReference(out var launcherPresentation)
			    || !_crosshair.TryGetReference(out CrosshairBehaviour crosshairBehaviour))
				return;

			var state = ctxt.ReadValueAsButton();

			if (state)
			{
				if (crosshairBehaviour.HoveredInteractableCollider != launcherPresentation.Collider
				    || !launcher.TryStartHold()
				    || !crosshairBehaviour.TryStartInteractionWithExplicitTarget(launcherPresentation.transform))
					return;
				
				_isHolding = true;
			}
			else if (_isHolding)
			{
				_isHolding = false;

				crosshairBehaviour.EndInteractionWithCurrentExplicitTarget();
				launcher.Release();
			}
		}
		
		void Update()
		{
			if (!_isHolding || !_launcherPresentationDependency.TryGetReference(out var launcherPresentation))
				return;
			
			var aim = _aimInput.ReadValue<Vector2>();
			var worldAimPosition = (Vector2)_cameraService.Source.ScreenToWorldPoint(aim);

			var delta = launcherPresentation.Center - worldAimPosition;

			launcherPresentation.ComputeOffsets(out var minOffset, out _);
			var offsetRatio = Mathf.InverseLerp(minOffset, _maxDragOffset, delta.magnitude);
			var intensity = _offsetToIntensityMapping.Evaluate(offsetRatio);
	
			_launcherBehaviourDependency.Relay(launcher => launcher.Hold(delta.normalized,intensity));
		}
	}
}