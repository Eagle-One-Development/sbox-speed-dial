namespace SpeedDial.Zombie.Entities;


[HammerEntity, Library( "sd_zombie_wallbuyzone" ), Title( "Wall Buy Zone" )]
[EditorModel( "models/weapons/pistol/prop_pistol.vmdl" )]
public class WallBuyZone : BuyZone
{
	public override string GetBuyText()
	{
		return "Buy Weapon";
	}
}
