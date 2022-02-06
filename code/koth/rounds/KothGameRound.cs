using System;
using System.Linq;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Koth.Player;
using SpeedDial.Koth.UI;
using SpeedDial.Koth;
using SpeedDial.Classic.UI;
using SpeedDial.Koth.Entities;

namespace SpeedDial.Koth.Rounds {
	public partial class KothGameRound : TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromMinutes(5);
		private KothGamemode koth => Game.Current.ActiveGamemode as KothGamemode;
		public override string RoundText => "";

		protected override void OnStart() {
			base.OnStart();

			koth.SetState(GamemodeState.Running);

			foreach(var client in Client.All.Where(x => x.Pawn is KothPlayer)) {
				var pawn = client.Pawn as KothPlayer;

				pawn.Frozen = false;
			}

			CreateHill();

			// start climax track 10 seconds before round ends
			_ = PlayClimaxMusic((int)RoundDuration.TotalSeconds - 10);
		}

		public void CreateHill() {
			if(Entity.All.OfType<HillSpotSpawn>().Any()) {
				var targetHill = Entity.All.OfType<HillSpotSpawn>().Random();

				var hill = new HillSpot();
				hill.Position = targetHill.Position;
				hill.Rotation = targetHill.Rotation;

			}
		}

		protected override void OnFinish() {
			base.OnFinish();

			foreach(var client in Client.All.Where(x => x.Pawn is KothPlayer)) {
				WinScreen.UpdatePanels(To.Single(client));
			}
		}

		private async Task PlayClimaxMusic(int delay) {
			await GameTask.DelaySeconds(delay);
			foreach(var client in Client.All.Where(x => x.Pawn is KothPlayer)) {
				var pawn = client.Pawn as KothPlayer;
				pawn.PlayRoundendClimax(To.Single(client));
			}
		}

		public override void OnPawnJoined(BasePlayer pawn) {
			base.OnPawnJoined(pawn);
			if(pawn is KothPlayer player) {
				player.PlaySoundtrack(To.Single(player.Client));
			}
		}
	}
}
