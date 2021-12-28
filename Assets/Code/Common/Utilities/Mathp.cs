using UnityEngine;

namespace ByteSize
{
	public struct Mathp
	{
		public static float ConvertDirectionToAngle(Vector2 direction)
		{
			var value = Mathf.Atan2(direction.y, direction.x) / Mathf.PI * 180.0f;
			
			if (value < 0.0f) 
				value += 360.0f;

			return value;
		}

		public static float WrapAngle(float angle) => 
			Mathf.Repeat(angle, 360.0f);

		public static float ClampAngle(float angle, float min, float max)
		{
			if (min > max)
			{
				if (angle > min || angle < max)
					return angle;
			}
			else
			{
				if (angle > min && angle < max)
					return angle;
			}

			return Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)) ? min : max;
		}
	}
}