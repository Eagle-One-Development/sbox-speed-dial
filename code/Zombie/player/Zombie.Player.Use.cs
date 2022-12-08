using SpeedDial.Zombie.Entities;

namespace SpeedDial.Zombie.Player;

public partial class ZombiePlayer
{
	[Net]
	public BuyZone CurrentBuyZone { get; set; }
	private bool WaitForRelease;
	protected void SimulateUse()
	{
		var tr = Trace.Ray( AimRay, 0f ).WithAnyTags( "buyzone" ).Run();
		if ( tr.Hit && tr.Entity is BuyZoneTrigger buyZone && buyZone.Enabled )
		{
			CurrentBuyZone = buyZone.BuyZoneEntity;

			if ( Input.Down( InputButton.Use ) && Host.IsServer && !WaitForRelease )
			{
				if ( !buyZone.TryBuyZone( this ) )
				{
					WaitForRelease = true;
				}
			}
		}
		else
		{
			CurrentBuyZone = null;
		}
		if ( Input.Released( InputButton.Use ) && Host.IsServer )
		{
			WaitForRelease = false;
		}
	}
}
