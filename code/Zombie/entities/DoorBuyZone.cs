namespace SpeedDial.Zombie.Entities;

[HammerEntity, Library( "sd_zombie_doorbuyzone" ), Title( "Door Buy Zone" )]
public partial class DoorBuyZone : BuyZone
{
	[Property, Net]
	public string DoorName { get; set; } = "door";
	public override string GetBuyText()
	{
		return $"Open {DoorName}";
	}
}
