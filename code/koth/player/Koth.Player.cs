using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.UI;
using SpeedDial.Koth.Entities;
using SpeedDial.Classic.Drugs;

namespace SpeedDial.Koth.Player {
	public partial class KothPlayer : ClassicPlayer {

		[Net]
		public TimeSince TimeSinceCircleScore { get; set; }

		public override void Spawn() {
			base.Spawn();
			EnableTouchPersists = true;
		}
		public override void AwardKill(ClassicPlayer killed) {
			// only award during main round (not warmup)
			if(!Gamemode.Instance.Running) return;
			ScorePanel.AwardKill();

			// add to current combo
			Client.AddInt("combo", 1);

			// if combo is bigger than max combo, we have a new max combo
			if(Client.GetValue("combo", 0) > Client.GetValue("maxcombo", 0)) {
				Client.SetValue("maxcombo", Client.GetValue("combo", 0));
			}
		}

		public override void Touch(Entity other) {
			base.Touch(other);
			if(other is HillSpot) {

				if(TimeSinceCircleScore > 1f) {
					//Every Second add 10 score multiplied by our current combo
					int score = 10 * Math.Max(Client.GetValue("combo", 1),1);
					Client.AddInt("score", score);
					WorldHints.AddHint(To.Single(Client), $"+{score} pts", EyePos, 1.5f);
					TimeSinceCircleScore = 0f;
				}
			}
		}
	}
}
