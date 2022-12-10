using System;

namespace SpeedDial.Zombie.Rounds;

public class ZombieRound : Round
{
	protected ZombieGamemode GameMode => SDGame.Current.ActiveGamemode as ZombieGamemode;

}
