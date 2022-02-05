using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpeedDial.Classic.Player;

namespace SpeedDial.OneChamber.Player {
	public partial class OneChamberSpectator : ClassicPlayer {

		public override void InitialRespawn() {
			Respawn();
		}

		public override void Respawn() {
			Host.AssertServer();

			SetModel(ModelPath);
			EnableDrawing = false;

			Controller = new OneChamberSpectatorController();
			Camera = new OneChamberSpectatorCamera();
			Animator = new ClassicAnimator();

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;

			CreateHull();
			ResetInterpolation();
		}

		public override void CreateHull() {
			MoveType = MoveType.MOVETYPE_WALK;
		}

		public override void TakeDamage(DamageInfo info) {
			return;
		}

		public override void Simulate(Client cl) {
			SimulateActiveChild(cl, ActiveChild);
			GetActiveController()?.Simulate(cl, this, GetActiveAnimator());
		}
	}
}
