using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Sandbox;

namespace SpeedDial {
	[Library]
	[Hammer.Skip]
	public partial class GamemodeEntity<T> : Entity where T : Entity {

		[Property(Title = "Active Gamemodes"), FGDType("flags")]
		public Gamemodes ActiveGamemodes { get; set; }

		public bool Enabled { get; set; }

		[Input]
		public async void Enable() {
			OnEnabled();
			await OnEntityEnabled.Fire(this);
		}

		protected Output OnEntityEnabled { get; set; }
		public virtual void OnEnabled() { }

		[Input]
		public async void Disable() {
			OnDisabled();
			await OnEntityDisabled.Fire(this);
		}
		
		protected Output OnEntityDisabled { get; set; }
		public virtual void OnDisabled() { }

		[Flags] // THIS HAS TO LINE UP WITH THE ENUM IN GAMEMODE.CS
		public enum Gamemodes {
			Classic = 0,
			Koth = 1,
			Dodgeball = 2
		}
	}
}
