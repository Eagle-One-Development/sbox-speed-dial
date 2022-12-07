namespace SpeedDial.Zombie.Entities;

[HammerEntity, Category( "Speed-Dial Zombie" )]
[BoundsHelper( nameof( BuyZoneMins ), nameof( BuyZoneMaxs ) )]
public partial class ZombieBuyZone : GamemodeEntity
{
	[Property, DefaultValue( "-64 -5 -64" )]
	public Vector3 BuyZoneMins { get; set; }

	[Property, DefaultValue( "64 64 64" )]
	public Vector3 BuyZoneMaxs { get; set; }

	public Output OnBought { get; set; }

	[Property]
	public int Cost { get; set; } = 250;
}
