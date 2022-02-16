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

		public static T SwapPawn<T>(this Client cl) where T : BasePlayer, new() {
			// swap out pawn for spectator pawn
			var oldpawn = cl.Pawn;
			var newpawn = new T();
			newpawn.Transform = oldpawn.Transform;
			cl.Pawn = newpawn;
			newpawn.InitialRespawn();
			// get rid of old pawn
			oldpawn.Delete();

			return newpawn;
		}

		public static T GetPawn<T>(this Client cl) where T : BasePlayer {
			return cl.Pawn as T;
		}
	}
}
