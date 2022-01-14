using System;
using System.Linq;
using System.Collections.Generic;

using Sandbox;

using SpeedDial.Classic.UI;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic {
	[Library("classic"), Hammer.Skip]
	public partial class ClassicGamemode : Gamemode {

		[Net] public string CurrentSoundtrack { get; set; } = "track01";

		public string[] Soundtracks { get; } = {
			"track01",
			"track02",
			"track03",
			"track03"
		};

		public void PickNewSoundtrack() {
			int index = Rand.Int(0, Soundtracks.Length);
			CurrentSoundtrack = Soundtracks[index];
		}

		public static ClassicGamemode Current => Instance as ClassicGamemode;

		protected override void OnStart() {
			SetRound(new WarmupRound());
			PickNewSoundtrack();
		}

		protected override void OnClientReady(Client client) {
			client.AssignPawn<ClassicPlayer>(true);
		}

		public override void CreateGamemodeUI() {
			GamemodeUI = new ClassicHud();
			Local.Hud = GamemodeUI;
		}

		public override bool OnClientSuicide(Client client) {
			if(client.Pawn is ClassicPlayer player) {
				player.DeathCause = ClassicPlayer.CauseOfDeath.Suicide;
			}
			return true;
		}
	}
}
