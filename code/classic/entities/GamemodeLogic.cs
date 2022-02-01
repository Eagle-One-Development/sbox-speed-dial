using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sandbox;

namespace SpeedDial {
	[Library("sd_gamemode_logic", Description = "Used to handle Gamemode logic. Will fire gamemode start, end and reset event outputs on its Active Gamemodes.")]
	public partial class GamemodeLogic : GamemodeEntity<Entity> {
		protected Output OnGamemodeReset { get; set; }
		public async void GamemodeReset(GamemodeIdentity gamemode) {
			if(ActiveGamemodes.HasFlag((Gamemodes)(int)gamemode)) {
				await OnGamemodeReset.Fire(this);
			}
		}

		protected Output OnGamemodeStarted { get; set; }
		public async void GamemodeStart(GamemodeIdentity gamemode) {
			if(ActiveGamemodes.HasFlag((Gamemodes)(int)gamemode)) {
				await OnGamemodeStarted.Fire(this);
			}
		}

		protected Output OnGamemodeEnded { get; set; }
		public async void GamemodeEnd(GamemodeIdentity gamemode) {
			if(ActiveGamemodes.HasFlag((Gamemodes)(int)gamemode)) {
				await OnGamemodeEnded.Fire(this);
			}
		}
	}
}
