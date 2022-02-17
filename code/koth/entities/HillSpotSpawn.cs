using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedDial;
using Sandbox;
namespace SpeedDial.Koth.Entities {
	[Library("sd_koth_hill_spawn", Title = "Random Hill Site Spawn")]
	[Hammer.EditorModel("models/koth/ring.vmdl")]
	public partial class HillSpotSpawn : GamemodeEntity {
		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
		}

		[SpeedDialEvent.Gamemode.Reset]
		public void HandleGamemodeReset(GamemodeIdentity ident) {
			if(ident == GamemodeIdentity.Koth) {
				Enable();
			} else {
				Disable();
			}
		}
	}
}
