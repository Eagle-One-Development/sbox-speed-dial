namespace SpeedDial.Classic.UI;

public partial class KillFeedEntry
{
	public Label Left { get; set; }
	public Label Right { get; set; }
	public Image Method { get; set; }
	public bool Important { get; set; }

	public RealTimeSince TimeSinceCreated = 0;

	public KillFeedEntry()
	{
		Left = AddChild<Label>( "left" );
		Method = AddChild<Image>( "method" );
		Method.SetTexture( "materials/ui/killicons/generic.png" );
		Right = AddChild<Label>( "right" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( TimeSinceCreated > (Important ? 12 : 6) )
		{
			Delete();
		}
	}
}
