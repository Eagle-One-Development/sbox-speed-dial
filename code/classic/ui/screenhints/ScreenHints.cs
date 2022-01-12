using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class ScreenHints : Panel {
		public static ScreenHints Current { get; private set; }
		private bool Active;
		private TimeSince TimeSinceActive;
		public Panel Banner { get; set; }
		public Label Title { get; set; }
		public Label Extra { get; set; }
		private bool FireExtra;
		private bool FireBanner;

		public ScreenHints() {
			Current = this;

			Banner.BindClass("visible", () => Active && FireBanner);
			Title.BindClass("visible", () => Active && TimeSinceActive > 0.05f - (FireBanner ? 0 : 0.05f));
			Extra.BindClass("visible", () => Active && FireExtra && TimeSinceActive > 0.5f - (FireBanner ? 0 : 0.05f));
		}

		public override void Tick() {

			if(TimeSinceActive > 1.5f) {
				Active = false;
				FireExtra = false;
			}

			// if you're here to find a way to forcefully hide the current 
			// animation if a new one is played while it's still running... good luck
		}

		[ClientRpc]
		public static void FireEvent(string title, string extra, bool banner) {
			Current.Title.Text = title;
			Current.Extra.Text = $"{extra}";
			Current.FireBanner = banner;
			Current.FireExtra = true;

			Current.Active = true;
			Current.TimeSinceActive = 0;
		}

		[ClientRpc]
		public static void FireEvent(string title, bool banner) {
			Current.Title.Text = title;
			Current.FireExtra = false;
			Current.FireBanner = banner;

			Current.Active = true;
			Current.TimeSinceActive = 0;
		}

		[ClientRpc]
		public static void FireEvent(string title, string extra) {
			Current.Title.Text = title;
			Current.Extra.Text = $"{extra}";
			Current.FireExtra = true;
			Current.FireBanner = true;

			Current.Active = true;
			Current.TimeSinceActive = 0;
		}

		[ClientRpc]
		public static void FireEvent(string title) {
			Current.Title.Text = title;
			Current.FireExtra = false;
			Current.FireBanner = true;

			Current.Active = true;
			Current.TimeSinceActive = 0;
		}
	}
}
