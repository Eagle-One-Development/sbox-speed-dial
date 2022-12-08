namespace SpeedDial.Zombie.Entities;

[HammerEntity, Category( "Speed-Dial Zombie" ), Library( "sd_zombie_buyzone" ), Title( "Buy Zone" )]
[BoundsHelper( nameof( BuyZoneMins ), nameof( BuyZoneMaxs ) )]
public partial class BuyZone : GamemodeEntity
{
	[Property, DefaultValue( "-64 -5 -64" )]
	public Vector3 BuyZoneMins { get; set; }

	[Property, DefaultValue( "64 64 64" )]
	public Vector3 BuyZoneMaxs { get; set; }

	public Output OnBought { get; set; }

	[Property, Net]
	public int Cost { get; set; } = 250;

	[Property]
	public bool ExpireOnUse { get; set; } = false;

	[Property, Net]
	public bool HoldUse { get; set; } = false;

	[Property]
	public float Cooldown { get; set; } = 0.5f;

	public bool Expired { get; set; }


	public BuyZoneTrigger BuyZoneTrigger { get; set; }
	TimeSince LastBuy;

	[SpeedDialEvent.Gamemode.Reset]
	private void HandleGamemodeReset( GamemodeIdentity ident )
	{
		if ( ident != GamemodeIdentity.Zombie )
		{
			Disable();
			BuyZoneTrigger?.Delete();
			Expired = false;
		}
		else
		{
			Enable();
			Reset();
		}

	}

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	protected virtual void Reset()
	{
		Expired = false;
		BuyZoneTrigger?.Delete();
		BuyZoneTrigger = new BuyZoneTrigger( this );

	}

	public virtual string GetBuyText()
	{
		return $"Buy :______";
	}

	public virtual void Buy( BasePlayer player )
	{
		if ( LastBuy < Cooldown || (player.Client.GetInt( "score", 0 ) < Cost && Cost > 0) ) return;
		Log.Info( "Buy" );

		player.Client.SetInt( "score", player.Client.GetInt( "score", 0 ) - Cost );
		if ( player.Client.GetInt( "score", 0 ) < 0 )
		{
			player.Client.SetInt( "score", 0 );
			return;
		}
		LastBuy = 0;
		OnBoughtItem( player );
	}


	public virtual void OnEntered( BasePlayer player )
	{

	}

	public virtual void OnExited( BasePlayer player )
	{

	}

	public virtual void OnBoughtItem( BasePlayer player )
	{
		if ( ExpireOnUse )
		{
			Expired = true;
			BuyZoneTrigger?.Delete();
		}

		_ = OnBought.Fire( player );
	}

}
