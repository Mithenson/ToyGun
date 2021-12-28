using System.Collections;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ByteSize
{
	public abstract class FeedbackSingleModification<T> : MMFeedback
	{
		#region Nested types

		public enum Modes
		{
			OverTime,
			Instant
		}

		public enum Transitions
		{
			CustomStartToDestination,
			ToDestination
		}
		
		public enum StartFetches
		{
			AtInit,
			AtPlay,
		}

		#endregion

		#region Editor specific

		private bool NeedsStartFetch => Mode == Modes.OverTime && Transition == Transitions.ToDestination;

		#endregion

		public override float FeedbackDuration
		{
			get => Mode == Modes.Instant ? 0.0f : ApplyTimeMultiplier(Duration);
			set => Duration = value;
		}

		#region Settings: execution

		[Header("Execution")]
		public Modes Mode;
			
		[MMFEnumCondition("Mode", (int)Modes.OverTime)]
		public float Duration;

		[MMFEnumCondition("Mode", (int)Modes.OverTime)]
		public Transitions Transition;
		
		[ShowIf("NeedsStartFetch")]
		public StartFetches StartFetch;
		
		#endregion
		
		#region Settings: over time

		[Header("Over time"), MMFEnumCondition("Mode", (int)Modes.OverTime)]
		public AnimationCurve Curve;
		
		[MMFEnumCondition("Transition", (int)Transitions.CustomStartToDestination)]
		public T CustomStart;

		#endregion

		#region Settings: specifics

		[Header("Specifics")]
		public T BaseLine;
		
		public T Destination;

		#endregion

		private T Start => Transition == Transitions.CustomStartToDestination ? CustomStart : _start;

		private T _start;
		private Coroutine _routine;

		#region Callbacks

		protected override void CustomInitialization(GameObject owner)
		{
			if (Mode == Modes.OverTime 
			    && Transition == Transitions.ToDestination
			    && StartFetch == StartFetches.AtInit)
				_start = GetStartFromInitialization();
		}

		protected override void CustomPlayFeedback(Vector3 position, float intensity = 1.0f)
		{
			if (Mode == Modes.OverTime
			    && Transition == Transitions.ToDestination 
			    && StartFetch == StartFetches.AtPlay)
				_start = GetStartFromPlay();

			switch (Mode)
			{
				case Modes.OverTime:
					_routine = StartCoroutine(Sequence(intensity));
					break;

				case Modes.Instant:
					Execute(1.0f, intensity);
					break;
			}
		}

		protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
		{
			if (!Active || _routine == null)
				return;

			StopCoroutine(_routine);
			_routine = null;
		}

		#endregion

		protected virtual IEnumerator Sequence(float intensity)
		{
			var journey = NormalPlayDirection ? 0f : FeedbackDuration;
			ComputeStartAndDestination(out var start, out var destination, intensity);

			while (journey >= 0
			       && journey <= FeedbackDuration
			       && FeedbackDuration > 0)
			{
				var remappedTime = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
				var evaluation = Mathf.Clamp01(Curve.Evaluate(remappedTime));
				
				Execute(start, destination, evaluation);
				
				journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
				yield return null;
			}
			
			_routine = null;    
			yield return null;
		}

		private void ComputeStartAndDestination(out T start, out T destination, float intensity)
		{
			start = Interpolate(BaseLine, Start, intensity);
			destination = Interpolate(BaseLine, Destination, intensity);
		}

		private void Execute(float evaluation, float intensity)
		{
			ComputeStartAndDestination(out var start, out var destination, intensity);
			Execute(start, destination, evaluation);
		}
		private void Execute(T start, T destination, float evaluation)
		{
			var interpolation = Interpolate(start, destination, evaluation);
			ProcessInterpolation(interpolation);
		}

		protected virtual T GetStartFromInitialization() => 
			GetStart();
		protected virtual T GetStartFromPlay() =>
			GetStart();
		protected abstract T GetStart();
		
		protected abstract T Interpolate(T start, T destination, float interpolation);
		protected abstract void ProcessInterpolation(T interpolation);
	}
}