using System;
using System.Collections;
using ByteSize;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins;
using DG.Tweening.Plugins.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Tween = ByteSize.Tween;

namespace ToyGun
{
	public class CrosshairPresentation : ScriptWithDependencies
	{
		[SerializeField]
		private Sprite _defaultSprite;
		
		[SerializeField]
		private Sprite _canInteractSprite;
		
		[SerializeField]
		private Sprite _isInteractingSprite;

		[SerializeField]
		private float _timeToSwitch;
		
		[SerializeField]
		private AnimationCurve _switchCurve;

		private SpriteRenderer _renderer;

		private OneTypeOneInstanceDependency<CrosshairBehaviour> _crosshairBehaviourDependency = new OneTypeOneInstanceDependency<CrosshairBehaviour>();

		private Tween _switchTween = new Tween();
		
		protected override Dependency[] CollectDependencies() => 
			new Dependency[] { _crosshairBehaviourDependency };
		
		public override void OnCreation()
		{
			base.OnCreation();
			
			_crosshairBehaviourDependency.OnReferenceAcquired += OnReferenceAcquired;
			_crosshairBehaviourDependency.OnReferenceLost += OnReferenceLost;
		}
		
		void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
			if (_renderer == null)
				throw new MissingComponentException("EXCEPTION");
			
			HideAndLockCursor();
		}
		
		protected override void OnDestroy()
		{
			base.OnDestroy();
			
			_crosshairBehaviourDependency.OnReferenceAcquired -= OnReferenceAcquired;
			_crosshairBehaviourDependency.OnReferenceLost -= OnReferenceLost;
		}

		void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
				HideAndLockCursor();
		}

		void Update()
		{
			if (!_crosshairBehaviourDependency.TryGetReference(out var crosshairBehaviour) || _switchTween.IsActive)
				return;
			
			transform.position = crosshairBehaviour.Position;
		}
		
		private void OnReferenceAcquired(CrosshairBehaviour crosshairBehaviour)
		{
			crosshairBehaviour.OnStateChange += OnStateChange;
			crosshairBehaviour.OnExplicitTargetChange += OnExplicitTargetChange;
		}
		private void OnReferenceLost(CrosshairBehaviour crosshairBehaviour)
		{
			crosshairBehaviour.OnStateChange -= OnStateChange;
			crosshairBehaviour.OnExplicitTargetChange -= OnExplicitTargetChange;
		}

		private void OnStateChange(CrosshairState state)
		{
			if (state.HasFlag(CrosshairState.CanInteract))
				_renderer.sprite = _canInteractSprite;
			else if (state.HasFlag(CrosshairState.IsInteracting))
				_renderer.sprite = _isInteractingSprite;
			else
				_renderer.sprite = _defaultSprite;
		}
		private void OnExplicitTargetChange(Transform _)
		{
			_switchTween.Stop();
			
			Tweens.DoDynamicMove(
				this,
				_switchTween,
				() => transform.position,
				val => transform.position = val,
				() =>
				{
					if (!_crosshairBehaviourDependency.TryGetReference(out var crosshairBehaviour))
						return default;

					return crosshairBehaviour.Position;
				},
				_timeToSwitch,
				_switchCurve);
		}
		
		private void HideAndLockCursor()
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}
	}
}