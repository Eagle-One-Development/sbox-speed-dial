namespace SpeedDial;

public static class SpeedDialEvent
{
	public static class Gamemode
	{
		public class ResetAttribute : EventAttribute
		{
			public ResetAttribute() : base( "sd.gamemode.reset" ) { }
		}

		public class StartAttribute : EventAttribute
		{
			public StartAttribute() : base( "sd.gamemode.start" ) { }
		}

		public class EndAttribute : EventAttribute
		{
			public EndAttribute() : base( "sd.gamemode.end" ) { }
		}
	}
}
