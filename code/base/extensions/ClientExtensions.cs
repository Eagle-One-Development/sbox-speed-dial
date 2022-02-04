using Sandbox;

namespace SpeedDial {
	public static class ClientExtensions {
		public static T AssignPawn<T>(this Client cl, bool respawn = true) where T : BasePlayer, new() {
			cl.Pawn?.Delete();

			var player = new T();
			cl.Pawn = player;

			if(respawn)
				player.InitialRespawn();

			return player;
		}

		public static T GetPawn<T>(this Client cl) where T : BasePlayer {
			return cl.Pawn as T;
		}
	}
}
