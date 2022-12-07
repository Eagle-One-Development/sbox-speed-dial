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
			Host.AssertServer();
			var spawnpoints = All.Where( ( s ) => s is SpawnPoint );
			Entity optimalSpawn = spawnpoints.ToList()[0];
			float optimalDistance = 0;
			foreach ( var spawn in spawnpoints )
			{
				float smallestDistance = 999999;
				foreach ( var player in All.Where( ( p ) => p is BasePlayer ) )
				{
					var distance = Vector3.DistanceBetween( spawn.Position, player.Position );
					if ( distance < smallestDistance )
					{
						smallestDistance = distance;
					}
				}
				if ( smallestDistance > optimalDistance )
				{
					optimalSpawn = spawn;
					optimalDistance = smallestDistance;
				}
			}
			pawn.Transform = optimalSpawn.Transform;
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
