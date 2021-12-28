using System;

namespace ToyGun
{
	[Flags]
	public enum CrosshairState : int
	{
		None = 0,
		
		CanInteract = 1,
		IsInteracting = 2,
		
		HasAnExplicitTarget = 4
	}
}