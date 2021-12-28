using MoreMountains.Feedbacks;
using UnityEngine;

namespace ByteSize
{
	[AddComponentMenu(""), FeedbackPath("Byte size/Scale")]
	public sealed class FeedbackScale : FeedbackSingleModification<Vector3>
	{
		#if UNITY_EDITOR

		public override Color FeedbackColor => MMFeedbacksInspectorColors.TransformColor;

		#endif
		
		public Transform Target;

		protected override Vector3 GetStart() => 
			Target.localScale;

		protected override Vector3 Interpolate(Vector3 start, Vector3 destination, float interpolation) => 
			Vector3.Lerp(start, destination, interpolation);
		protected override void ProcessInterpolation(Vector3 interpolation) =>
			Target.localScale = interpolation;
	}
}