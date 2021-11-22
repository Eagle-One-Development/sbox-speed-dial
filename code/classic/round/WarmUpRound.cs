using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic {
	public class WarmUpRound : BaseRound {
		public override string RoundName => "WarmUp";
		public override int RoundDuration => -1;


		public override void OnSecond() {
			if(Host.IsServer) {
				//TODO: When each team has a minimum of one player, start the round
			}
		}

		protected override void OnStart() {

			var players = Client.All;
			foreach(var p in players.ToArray()) {

				if(p.Pawn is SpeedDialPlayer sp) {
					sp.Freeze = false;
					sp.Client.SetValue("killcombo", 0);
					sp.Client.SetValue("score", 0);
					sp.Client.SetValue("maxcombo", 0);
				}
			}
		}
		//private bool _roundStarted;
	}
}
