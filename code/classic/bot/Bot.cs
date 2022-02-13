using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Bot {
	public partial class ClassicBot : Sandbox.Bot {

		private ClassicBotBehaviour behaviour;

		public Entity CurrentTarget { get; private set; }
		
		[AdminCmd("sd_bot", Help = "Spawn my custom bot.")]
		internal static void SpawnCustomBot() {
			Host.AssertServer();

			// Create an instance of your custom bot.
			_ = new ClassicBot();
		}

		public override void BuildInput(InputBuilder builder) {
			builder.Clear();

			if (behaviour != null) {
				builder.InputDirection = behaviour.InputVelocity;
				builder.ViewAngles = behaviour.ViewAngles;
				builder.SetButton(InputButton.Attack1, behaviour.Attack1);
				builder.SetButton(InputButton.Attack2, behaviour.Attack2);
			}
		}

		public override void Tick() {
			if (behaviour is null) {
				behaviour = new ClassicBotBehaviour();
				behaviour.Bot = this;
			}

			behaviour.Tick();
		}
	}
}
