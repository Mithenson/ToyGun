using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ByteSize
{
	public class HierarchyService
	{
		private const int SEARCH_DEPTH = 3;

		private Dictionary<HierarchyMark.Option, Transform> _marks;

		public HierarchyService()
		{
			_marks = new Dictionary<HierarchyMark.Option, Transform>();
			
			var activeScene = SceneManager.GetActiveScene();
			foreach (var root in activeScene.GetRootGameObjects())
				TryCollectHierarchyMarksOn(root.transform);
		}

		private void TryCollectHierarchyMarksOn(Transform transform, int depth = 0)
		{
			if (transform.TryGetComponent(out HierarchyMark mark))
				_marks.Add(mark.Value, transform);

			depth++;
			if (depth > SEARCH_DEPTH)
				return;
			
			foreach (Transform child in transform)
				TryCollectHierarchyMarksOn(child, depth);
		}

		public Transform this[HierarchyMark.Option mark] =>
			_marks[mark];
	}
}