using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SpeedDial.Classic.UI {
	public partial class ScreenHints : Panel {
		public static ScreenHints Current;
		private bool Active;
		private TimeSince TimeSinceActive;
		private readonly Panel Banner;
		private readonly Label Title;
		private readonly Label Extra;
		private bool FireExtra;
		private bool FireBanner;

		public ScreenHints() {
			Current = this;

			StyleSheet.Load("/classic/ui/screenhints/ScreenHints.scss");
			AddClass("screenhints");

			Banner = Add.Panel("banner");
			Banner.Add.Panel("right");
			Banner.Add.Panel("left");

			Title = Add.Label("IF YOU READ THIS", "title");
			Extra = Add.Label("+YOU ARE HANDSOME.", "extra");


			Banner.BindClass("visible", () => Active && FireBanner);
			Title.BindClass("visible", () => Active && TimeSinceActive > 0.3f - (FireBanner ? 0 : 0.3f));
			Extra.BindClass("visible", () => Active && FireExtra && TimeSinceActive > 0.7f - (FireBanner ? 0 : 0.3f));
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
