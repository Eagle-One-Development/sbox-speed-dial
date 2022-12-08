namespace SpeedDial.Zombie.Entities;

[HammerEntity, Library( "sd_zombie_barricade" ), Title( "Barricade" )]
[Solid, AutoApplyMaterial( "materials/tools/toolsgeneric.vmat" )]
public class Barricade : BuyZone
{
	public override string GetBuyText()
	{
		return "Repair Barricade";
	}
}
