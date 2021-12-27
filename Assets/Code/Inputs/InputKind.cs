namespace ByteSize
{
	public enum InputKind
	{
		// (FORWARD) Accelerate : 00-09
		Forward_Standard_Accelerate = 0,
		Forward_Standard_ToggleNitro = 1,
		
		// (HORIZONTAL) Turn : 10-19
		Horizontal_Standard_Turn = 2,
		
		// (BACKWARD) Turn around : 
		Backward_Standard_TurnAround = 3
	}
}