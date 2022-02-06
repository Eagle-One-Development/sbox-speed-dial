using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial.Koth.Entities {
	public class HillSpot : ModelEntity {

		public List<BasePlayer> TouchingPlayers = new();

		public override void Spawn() {
			base.Spawn();
			SetModel("models/koth/ring.vmdl");
			Transmit = TransmitType.Always;
			CollisionGroup = CollisionGroup.Trigger;
			SetupPhysicsFromModel(PhysicsMotionType.Static);
		}

		public override void StartTouch(Entity other) {
			Log.Info("STARTING TOUCH");
			if(other is BasePlayer player)
				TouchingPlayers.Add(player);
		}

		public override void EndTouch(Entity other) {
			if(other is BasePlayer player)
				TouchingPlayers.Remove(player);
		}
	}
}
