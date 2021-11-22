using Sandbox;

using SpeedDial.Base.UI;

namespace SpeedDial.Base {
	public abstract class BaseGamemode : Entity {

		public static BaseGamemode Current { get; protected set; }

		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
			Current = this;
		}

		public virtual void GamemodeInitializeUI() {
			_ = new BaseHud();
		}

		public virtual void GamemodeSimulate(Client cl) { }

		public virtual void GamemodeClientJoined(Client client) { }

		public virtual void GamemodeDoPlayerNoclip(Client player) { }

		public virtual void GamemodeDoPlayerSuicide(Client cl) {
			if(cl.Pawn == null) return;

			cl.Pawn.Kill();
		}

		public virtual void GamemodeOnKilled(Client client, Entity pawn) { }

		public virtual void GamemodePostLevelLoaded() { }

		public virtual void GamemodeClientSpawn() { }
	}
}
