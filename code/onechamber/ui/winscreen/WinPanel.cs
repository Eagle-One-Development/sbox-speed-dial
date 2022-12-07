using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.UI;

[UseTemplate]
public partial class OneChamberWinPanel : Panel
{
	public Client Client { get; set; }
	public Image Portrait { get; set; }
	public Image Avatar { get; set; }
	public Image Icon { get; set; }
	public Label Position { get; set; }
	public Label Name { get; set; }


	public void UpdateFrom( Client client, int position )
	{
		if ( !client.IsValid() )
		{
			Client = null;
			return;
		}
		Client = client;
		var pawn = client.Pawn as OneChamberPlayer;
		Portrait.Texture = pawn?.Character?.PortraitTexture;
		Avatar.SetTexture( $"avatar:{client.SteamId}" );
		Icon.SetTexture( position == 1 ? "materials/ui/misc/crown.png" : position == 2 ? "materials/ui/misc/dice.png" : "materials/ui/misc/crosshair.png" );
		Position.Text = $"{(position == 1 ? "WINNER" : position == 2 ? "2ND" : "3RD")}";
		Name.Text = $"{client.Name.ToUpper()}";
	}
}
