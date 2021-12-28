using System;
using System.Runtime.Serialization;
using ByteSize;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace ToyGun
{
	public class CrosshairBehaviour : Script
	{
		public event Action<CrosshairState> OnStateChange;
		public event Action<Transform> OnExplicitTargetChange;
		
		public Collider2D HoveredInteractableCollider { get; private set; }
		public Vector2 Position { get; private set; }
		public CrosshairState State { get; private set; }
		public Transform ExplicitTarget { get; private set; }

		private InteractionConfiguration _interactionConfiguration;
		private CameraService _cameraService;
		private InputService _inputService;
		private InputAction _aimInput;
		
		private Collider2D[] _hitBuffer;

		[Inject]
		public void Inject(InteractionConfiguration interactionConfiguration, CameraService cameraService, InputService inputService)
		{
			_interactionConfiguration = interactionConfiguration;
			_cameraService = cameraService;
			_inputService = inputService;
			
			if (!_inputService.TryGetInput(InputKind.CrossPlatform_Aim, out _aimInput))
				throw new InvalidDataContractException("EXCEPTION");
		}

		void Awake() =>
			_hitBuffer = new Collider2D[Constants.Physics.HitBufferCapacity];

		void Update()
		{
			RefreshPosition();
			
			if (!State.HasFlag(CrosshairState.IsInteracting))
				RefreshState();
		}
		
		public bool TryStartInteractionWithExplicitTarget(Transform target)
		{
			if (!TryStartInteraction())
				return false;

			SetExplicitTarget(target);
			return true;
		}
		public bool TryStartInteraction()
		{
			if (!State.HasFlag(CrosshairState.CanInteract))
				return false;

			ChangeState(CrosshairState.IsInteracting);
			return true;
		}
		
		public bool EndInteractionWithCurrentExplicitTarget()
		{
			if (!EndInteraction())
				return false;

			SetExplicitTarget(null);
			return true;
		}
		public bool EndInteraction()
		{
			if (!State.HasFlag(CrosshairState.IsInteracting))
				return false;

			HoveredInteractableCollider = null;
			Refresh();

			return true;
		}
		
		public void SetExplicitTarget(Transform target)
		{
			ExplicitTarget = target;
			State = ExplicitTarget != null ? State.AddFlags(CrosshairState.HasAnExplicitTarget) : State.RemoveFlags(CrosshairState.HasAnExplicitTarget);

			RefreshPosition();
			
			OnExplicitTargetChange?.Invoke(target);
		}
		
		private void Refresh()
		{
			RefreshPosition();
			RefreshState();
		}
		private void RefreshPosition()
		{
			if (State.HasFlag(CrosshairState.HasAnExplicitTarget))
				Position = ExplicitTarget.position;
			else
			{
				var aim = _aimInput.ReadValue<Vector2>();
				var worldAimPosition = (Vector2)_cameraService.Source.ScreenToWorldPoint(aim);
	
				Position = worldAimPosition;
			}
		}
		private void RefreshState()
		{
			var hitCount = Physics2D.OverlapCircleNonAlloc(
				Position,
				_interactionConfiguration.InteractionRange, 
				_hitBuffer,
				Constants.Physics.InteractableMask);
			
			if (hitCount == 0)
			{
				if (State != CrosshairState.None)
					ChangeState(CrosshairState.None);

				return;
			}
			
			var closestSqrDistance = float.PositiveInfinity;
			var bestHitIndex = -1;
			
			for (var i = 0; i < hitCount; i++)
			{
				var closestPoint = _hitBuffer[i].ClosestPoint(Position);
				var sqrDistance = (closestPoint - Position).sqrMagnitude;
				
				if (sqrDistance > closestSqrDistance)
					continue;

				closestSqrDistance = sqrDistance;
				bestHitIndex = i;
			}

			if (State != CrosshairState.CanInteract) 
				ChangeState(CrosshairState.CanInteract);

			HoveredInteractableCollider = _hitBuffer[bestHitIndex];
		}

		private void ChangeState(CrosshairState state)
		{
			State = State.RemoveFlags(CrosshairState.CanInteract | CrosshairState.IsInteracting);
			State = State.AddFlags(state);
			
			OnStateChange?.Invoke(state);
		}
	}
}