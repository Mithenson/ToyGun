using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;

namespace ByteSize.Editor
{
	public static class MenuItems
	{
		[MenuItem("CONTEXT/PolygonCollider2D/Add light")]
		private static void AddLight(MenuCommand command)
		{
			var collider = (PolygonCollider2D)command.context;

			if (!collider.TryGetComponent(out Light2D light))
				light = collider.gameObject.AddComponent<Light2D>();

			light.lightType = Light2D.LightType.Freeform;

			var outerOffset = 0.1f;
			var innerOffset = 0.075f;
			var lightPath = new List<Vector3>();
			var colliderPath = collider.GetPath(0);

			AddPoint(colliderPath[0], colliderPath[colliderPath.Length - 1], colliderPath[1], outerOffset);
			for (var i = 1; i < colliderPath.Length - 1; i++)
				AddPoint(colliderPath[i],colliderPath[i - 1], colliderPath[i + 1], outerOffset);
			AddPoint(colliderPath[colliderPath.Length - 1],colliderPath[colliderPath.Length - 2], colliderPath[0], outerOffset);

			AddPoint(colliderPath[colliderPath.Length - 1],colliderPath[colliderPath.Length - 2], colliderPath[0], -innerOffset);
			for (var i = colliderPath.Length - 2; i > 0; i--)
				AddPoint(colliderPath[i],colliderPath[i - 1], colliderPath[i + 1], -innerOffset);
			AddPoint(colliderPath[0], colliderPath[colliderPath.Length - 1], colliderPath[1], -innerOffset);

			void AddPoint(Vector2 current, Vector2 previous, Vector2 next, float orientedOffset)
			{
				var a = (current - previous).normalized;
				a = Vector2.Perpendicular(a);
				
				var b = (next - current).normalized;
				b = Vector2.Perpendicular(b);

				lightPath.Add(current + (a + b).normalized * orientedOffset);
			}
			
			var fieldInfo = typeof(Light2D).GetField("m_ShapePath", BindingFlags.Instance | BindingFlags.NonPublic);
			fieldInfo.SetValue(light, lightPath.ToArray());
		}
	}
}