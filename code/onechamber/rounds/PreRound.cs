using System;
using System.Linq;

using Sandbox;

using SpeedDial.OneChamber.Player;

namespace SpeedDial.OneChamber.Rounds {
	public partial class OneChamberPreRound : TimedRound {
		public override TimeSpan RoundDuration => TimeSpan.FromSeconds(11);
		private OneChamberGamemode onechamber => Game.Current.ActiveGamemode as OneChamberGamemode;
		public override string RoundText => "Round starting...";

		protected override void OnStart() {
			base.OnStart();

			onechamber.SetState(GamemodeState.Preparing);

			if(Host.IsServer) {
				onechamber.PickNewSoundtrack();
				// clear kills list to clear domination info
				onechamber.Kills.Clear();
			}

			foreach(var client in Client.All) {
				client.AssignPawn<OneChamberPlayer>();

				var pawn = client.Pawn as OneChamberPlayer;

				pawn.StopSoundtrack(To.Single(client), true);
				pawn.PlaySoundtrack(To.Single(client));

				pawn.Frozen = true;

				Log.Debug("pre round");
			}

			// reset stuff from warmup etc
			Game.Current.ActiveGamemode?.CallResetEvent();
		}

		protected override void OnFinish() {
			base.OnFinish();
			Game.Current.ActiveGamemode?.ChangeRound(new OneChamberGameRound());
		}

		public override void OnPawnJoined(BasePlayer pawn) {
			base.OnPawnJoined(pawn);
			if(pawn is OneChamberPlayer player) {
				player.PlaySoundtrack(To.Single(player.Client));
				player.Frozen = true;
			}
		}
	}
}
