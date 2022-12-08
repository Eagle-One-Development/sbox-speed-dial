using SpeedDial.Zombie.Entities;
using SpeedDial.Zombie.Player;

namespace SpeedDial.Zombie;

public partial class BuyHint : Panel
{
	public Panel HintContainer { get; set; }

	public string BuyText { get; set; }
	public string Cost { get; set; }
	public string Type { get; set; }
	public override void Tick()
	{
		base.Tick();
		if ( Local.Pawn is not ZombiePlayer player || !player.CurrentBuyZone.IsValid() )
		{
			HintContainer.SetClass( "active", false );
			return;
		}
		BuyZone buyZone = player.CurrentBuyZone;
		BuyText = buyZone.GetBuyText();
		Cost = buyZone.Cost.ToString();
		Type = buyZone.HoldUse ? $"Hold {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} To" : $"Press {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} To";
		var v3 = buyZone.Position.ToScreen();
		if ( !HintContainer.IsValid() )
		{
			Log.Info( "IS NULL" );
			return;
		}
		HintContainer.SetClass( "active", true );

		HintContainer.Style.Position = PositionMode.Absolute;
		HintContainer.Style.Left = Length.Fraction( v3.x );
		HintContainer.Style.Top = Length.Fraction( v3.y );
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( BuyText, Cost, Type );
	}
}
