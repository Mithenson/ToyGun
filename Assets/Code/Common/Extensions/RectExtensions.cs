using UnityEngine;

namespace ByteSize
{
	public static class RectExtensions
	{
		public static Rect RestrainInside(this Rect value, Rect container)
		{
			if (value.width > container.width)
				value.width = container.width;

			if (value.height > container.height)
				value.height = container.height;

			if (value.x < container.x)
				value.x = container.x;
			else
			{
				var diff = value.xMax - container.xMax;
				if (diff > 0.0f)
					value.x += diff;
			}

			if (value.y < container.y)
				value.y = container.y;
			else
			{
				var diff = value.yMax - container.yMax;
				if (diff > 0.0f)
					value.y -= diff;
			}

			return value;
		}

		public static Rect Pad(this Rect value, float padding) =>
			Pad(value, padding, padding, padding, padding);
		public static Rect Pad(this Rect value, float horizontal, float vertical) =>
			Pad(value, horizontal, horizontal, vertical, vertical);
		public static Rect Pad(this Rect value, float left, float right, float up, float down)
		{
			value.x += left;
			value.width -= right * 2.0f;
			value.y += up;
			value.height -= down * 2.0f;

			return value;
		}
	}
}