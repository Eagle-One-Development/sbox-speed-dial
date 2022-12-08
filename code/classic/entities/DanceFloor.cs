namespace Sandbox.Classic.Entities;

/// <summary>
/// Dance floor tile entity
/// </summary>
[Library( "func_dance_floor" ), HammerEntity]
[Solid]
public partial class DanceFloor : BrushEntity
{

	public static readonly Color[] Colors = {
			Color.Black,
			Color.Red,
			new Color(1, 0.3f, 0),
			Color.Yellow,
			Color.Green,
			new Color(86f / 255f, 30f / 255f, 138f / 255f), // purple
			Color.Magenta,
			Color.Blue,
			Color.Cyan,
			Color.White
	};

	[Property( Title = "Custom Color Code" )]
	public string CustomColorCode { get; set; } = "";

	[Property( Title = "Color Code" )]
	public int ColorCode { get; set; } = 0;

	[Property( Title = "Color Code Change Interval in seconds" )]
	public int Interval { get; set; } = 1;

	private float NextInterval { get; set; }
	private int CurrentColor = 0;
	private int CurrentCustomIndex = 0;
	private bool UseCustomCode = false;

	public override void Spawn()
	{
		if ( CustomColorCode.Length > 0 )
			UseCustomCode = true;

		Transmit = TransmitType.Always;
		NextInterval = RealTime.Now;

		base.Spawn();
	}

	[Event.Tick.Server]
	public void Simulate()
	{
		if ( RealTime.Now >= NextInterval )
		{

			if ( UseCustomCode )
			{
				if ( CurrentCustomIndex > CustomColorCode.Length - 1 )
				{
					CurrentCustomIndex = 0;
				}
				var color = CustomColorCode[CurrentCustomIndex];
				RenderColor = Colors[int.Parse( $"{color}" )];
				CurrentCustomIndex++;
			}
			else
			{
				if ( CurrentColor > Colors.Length - 1 )
				{
					CurrentColor = 0;
				}
				RenderColor = Colors[CurrentColor];
				CurrentColor++;
			}
			NextInterval = RealTime.Now + Interval;
		}
	}
}
