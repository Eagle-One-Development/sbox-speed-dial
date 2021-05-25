using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace SpeedDial {
	public class PostRound : BaseRound {
		public override string RoundName => "Post Round";
		public override int RoundDuration => 5;

		protected override void OnFinish() {
			Log.Info("Finished Game Round");

		}

		protected override void OnTimeUp() {
			Log.Info("Game Round Round Up!");


			base.OnTimeUp();
		}
	}
}
