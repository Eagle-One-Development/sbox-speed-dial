using System;

namespace SpeedDial.Zombie.Rounds
{
	public class ZombieRound : Round
	{
		protected ZombieGamemode GameMode => Game.Current.ActiveGamemode as ZombieGamemode;

	}
}
