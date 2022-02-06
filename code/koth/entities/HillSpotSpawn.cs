using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedDial;
namespace Sandbox.koth.entities {
	[Library("sd_koth_hill_spawn", Title = "Random Hill Site Spawn")]
	[Hammer.EditorModel("models/koth/ring.vmdl")]
	public partial class HillSpotSpawn : GamemodeEntity {
		public override void Spawn() {
			base.Spawn();
			Transmit = TransmitType.Always;
		}
	}
}
