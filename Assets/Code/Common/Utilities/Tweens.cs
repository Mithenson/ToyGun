using System;
using System.Collections;
using System.Data;
using UnityEngine;

namespace ByteSize
{
	public static class Tweens
	{
		public static Tween DoDynamicMove(
			MonoBehaviour context,
			Func<Vector3> getter,
			Action<Vector3> setter,
			Func<Vector3> targetGetter, 
			float duration,
			AnimationCurve curve)
		{
			var tween = new Tween();
			DoDynamicMove(context, tween, getter, setter, targetGetter, duration, curve);

			return tween;
		}
		public static void DoDynamicMove(
			MonoBehaviour context, 
			Tween tween,
			Func<Vector3> getter,
			Action<Vector3> setter,
			Func<Vector3> targetGetter,
			float duration, 
			AnimationCurve curve)
		{
			var routine = context.StartCoroutine(DoDynamicMove(tween, getter(), setter, targetGetter, duration, curve));
			tween.Reset(context, routine);
		}
		private static IEnumerator DoDynamicMove(
			Tween tween, 
			Vector3 start,
			Action<Vector3> setter,
			Func<Vector3> targetGetter,
			float duration, 
			AnimationCurve curve)
		{
			if (duration <= 0.0f)
				throw new InvalidConstraintException("EXCEPTION");
			
			var time = 0.0f;
			while (time < duration)
			{
				Execute(time / duration);
				time += Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}

			Execute(1.0f);
			tween.Complete();

			void Execute(float ratio)
			{
				var target = targetGetter();
				var current = Vector3.Lerp(start, target, curve.Evaluate(ratio));

				setter(current);
			}
		}	
	}
}