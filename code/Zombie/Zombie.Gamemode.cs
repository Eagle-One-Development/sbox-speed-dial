using System;
using SpeedDial.Zombie;

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


	}
}
