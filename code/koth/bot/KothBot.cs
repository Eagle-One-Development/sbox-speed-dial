using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.Bot;

namespace SpeedDial.Koth.Bot {
	public partial class KothBot : ClassicBot {
		
		[ServerCmd("sd_bot_koth")]
		internal static void SpawnCustomBot() {
			Host.AssertServer();

			_ = new KothBot();
		}

		public override void Tick() {
			if (behaviour is null) {
				behaviour = new KothBotBehaviour();
				behaviour.Bot = this;
			}

			behaviour.Tick();
		}
	}
}
