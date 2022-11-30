using System;
using SpeedDial.Zombie;
using SpeedDial.Zombie.Player;
using SpeedDial.Zombie.Rounds;

namespace SpeedDial.Zombie
{
	[Title( "Zombie Mode" )]
	public class ZombieGamemode : Gamemode
	{
		public override GamemodeIdentity Identity => GamemodeIdentity.Zombie;
		public override void CreateGamemodeUI()
		{
			Hud.SetGamemodeUI( new Hud_Zombie() );
		}

		public override void MoveToSpawnpoint( BasePlayer pawn )
		{
			pawn.Position = new Vector3( 0, 0, 100 );
		}

		protected override void OnStart()
		{
			ChangeRound( new ZombiePreRound() );
		}

		protected override void OnClientReady( Client client )
		{
			client.AssignPawn<ZombiePlayer>( true );
		}


	}
}
