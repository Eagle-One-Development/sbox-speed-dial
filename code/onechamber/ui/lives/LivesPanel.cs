using SpeedDial.Classic.Player;

using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class LivesPanel : Panel
{
	private Label LivesLabel { get; set; }

	public override void Tick()
	{
		SetClass( "visible", Game.LocalClient.Pawn is not ClassicSpectator );

		if ( Game.LocalClient.Pawn is OneChamberPlayer player )
		{
			LivesLabel.Text = $"{player.Lives}";
		}
	}
}
