using UnityEngine;

namespace ByteSize
{
	public class Tween
	{
		public bool IsActive => !IsDone;
		public bool IsDone { get; private set; }

		private MonoBehaviour _context;
		private Coroutine _routine;

		public Tween() => 
			IsDone = true;

		internal void Reset(MonoBehaviour context, Coroutine routine)
		{
			IsDone = false;

			_context = context;
			_routine = routine;
		}
		internal void Complete() => 
			IsDone = true;

		public void Stop()
		{
			if (_context != null && _routine != null)
			{
				_context.StopCoroutine(_routine);
				_routine = null;
			}
			
			IsDone = true;
		}
	}
}