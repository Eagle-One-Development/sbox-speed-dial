using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using System.Linq;

using SpeedDial.Weapons;
using SpeedDial.Player;
using SpeedDial.Settings;

namespace SpeedDial.UI {
	public partial class CharacterSelect : Panel {

		public Image portrait;
		public Image backPortrait;
		public Label title;

		public Label description;

		public Label startLoad;


		private int currentIndex = 0;

		private float translate;
		private float translate2;

		public bool open = false;

		public static CharacterSelect Current;
		public bool right;

		public Panel bars;
		public Panel bar1;
		public Panel bar2;

		Sound tapeSound;

		public Label time;
		public Label date;
		public Label pause;

		public Label leftPrompt;
		public Label rightPrompt;
		public Label spacePrompt;

		private float rightScale;
		private float leftScale;
		private float middleScale;

		public SettingsMenu SettingsPanel;

		[ClientRpc]
		public static void SetInitialIndex(int index) {
			Current.currentIndex = index;
		}


		public CharacterSelect() {
			StyleSheet.Load("/ui/CharacterSelect.scss");
			backPortrait = Add.Image("materials/ui/portraits/default.png", "backportrait");
			portrait = Add.Image("materials/ui/portraits/default.png", "portrait");

			AddClass("active");
			title = portrait.Add.Label("PLAYER NAME", "title");
			description = portrait.Add.Label("Default character is so cool. He spent his days doing crime while not doing the time.", "description");
			startLoad = description.Add.Label("Abilities: NONE\nWeapon: NONE", "loadout");

			bars = Add.Panel("bars");
			bar1 = bars.Add.Panel("bar");
			bar2 = bars.Add.Panel("bar");
			bar1.SetClass("top", true);
			bar2.SetClass("bottom", true);

			time = bar1.Add.Label("00:00:00", "timer");

			date = bar2.Add.Label("00:00:00", "dater");


			var panel = Add.Panel("portrait2");
			leftPrompt = panel.Add.Label("< Q", "prompt");
			spacePrompt = panel.Add.Label("<SPACE>", "prompt");
			rightPrompt = panel.Add.Label("E >", "prompt");
			leftPrompt.SetClass("left", true);
			rightPrompt.SetClass("right", true);
			spacePrompt.SetClass("middle", true);

			SettingsPanel = AddChild<SettingsMenu>();

			Current = this;


		}

		public override void Tick() {
			base.Tick();

			Character character = Character.All.ElementAtOrDefault(currentIndex);

			title.Text = character.CharacterName;
			description.Text = character.Description;
			portrait.Texture = character.PortraitTexture;

			DateTime dt = DateTime.Now.AddYears(-28);

			string s = dt.ToString(@"tt hh:mm");

			s += "\n";

			s += dt.ToString(@"MMM. dd yyyy");

			time.Text = TimeSpan.FromSeconds(Time.Now).ToString(@"hh\:mm\:ss");
			date.Text = s;

			leftScale = leftScale.LerpTo(0, Time.Delta * 8f);
			rightScale = rightScale.LerpTo(0, Time.Delta * 8f);
			middleScale = middleScale.LerpTo(0, Time.Delta * 8f);

			PanelTransform rightBump = new PanelTransform();
			PanelTransform leftBump = new PanelTransform();
			PanelTransform middleBump = new PanelTransform();
			rightBump.AddScale(1f + 0.5f * rightScale);
			leftBump.AddScale(1f + 0.5f * leftScale);
			middleBump.AddScale(1f + 0.5f * middleScale);


			rightPrompt.Style.Transform = rightBump;
			leftPrompt.Style.Transform = leftBump;
			spacePrompt.Style.Transform = middleBump;

			string wep = Library.GetAttribute(character.WeaponClass).Title;
			startLoad.Text = $"Weapon: {wep}";

			var transform = new PanelTransform();

			var transform2 = new PanelTransform();
			if(right) {
				translate = translate.LerpTo(0f, Time.Delta * 10f);
				translate2 = translate2.LerpTo(200f, Time.Delta * 2f);

				transform.AddTranslateX(Length.Percent(-translate));
				transform2.AddTranslateX(Length.Percent(-translate2));
				float anim = 1 - (translate / 100f);
				transform2.AddScale(1 - 0.1f * anim);
				transform2.AddTranslateY(Length.Pixels(190f * anim));
			} else {
				translate = translate.LerpTo(200f, Time.Delta * 2f);
				translate2 = translate2.LerpTo(0f, Time.Delta * 10f);

				transform.AddTranslateX(Length.Percent(-translate2));
				transform2.AddTranslateX(Length.Percent(-translate));
				float anim = 1 - (translate / 100f);
				float anim2 = 1 - (translate / 200f);
				transform2.AddScale(1f);
				transform.AddTranslateY(Length.Pixels(190f * anim2));
				transform.AddScale(0.9f + 0.1f * (1 - anim));
			}



			backPortrait.Style.Transform = transform2;
			portrait.Style.Transform = transform;

			if(Host.IsClient) {
				if(!open) {
					SetClass("active", false);
					bar1.SetClass("active", false);
					bar2.SetClass("active", false);
				} else {
					SetClass("active", true);
					bar1.SetClass("active", true);
					bar2.SetClass("active", true);
				}
			}

			if(SpeedDialGame.Instance.Round is GameRound || SpeedDialGame.Instance.Round is PreRound) {

			} else {
				SetClass("active", false);
			}
		}
		public void ToggleOpen() {
			open = !open;
			if(open) {
				(Local.Pawn as SpeedDialPlayer).FadeSoundtrack(0.3f);
				Sound.FromScreen("tape_stop");
				tapeSound = Sound.FromScreen("tape_noise");
				SettingsPanel.ReloadSettings();
			} else {
				(Local.Pawn as SpeedDialPlayer).FadeSoundtrack(1);
				tapeSound.Stop();
				var sound = Sound.FromScreen("select_confirm");
				SettingsManager.SaveSettings();
			}
		}
		[Event.BuildInput]
		public void ProcessClientInput(InputBuilder input) {
			if(SpeedDialGame.Instance.Round is GameRound || SpeedDialGame.Instance.Round is PreRound) {

			} else {
				return;
			}

			bool Q = input.Pressed(InputButton.Menu);
			bool E = input.Pressed(InputButton.Use);
			bool space = input.Pressed(InputButton.Jump);

			if(input.Pressed(InputButton.Duck)) {
				ToggleOpen();
			}

			if(open) {
				if(E) {
					currentIndex++;
					translate = 100f;
					translate2 = 0f;
					rightScale = 1f;
					right = true;
					backPortrait.Texture = portrait.Texture;
					if(currentIndex > Character.All.Count - 1) {
						currentIndex = 0;
					}
					var sound = Sound.FromScreen("select_click");
				}

				if(Q) {
					right = false;
					currentIndex--;
					translate2 = 100f;
					translate = 0f;
					backPortrait.Texture = portrait.Texture;
					if(currentIndex < 0) {
						currentIndex = Character.All.Count - 1;
					}
					leftScale = 1f;
					var sound = Sound.FromScreen("select_click");
				}

				if(space) {
					string[] s = { currentIndex.ToString() };
					(Local.Pawn as SpeedDialPlayer).FadeSoundtrack(1);
					ConsoleSystem.Run("set_character", s);
					open = false;
					middleScale = 2f;
					tapeSound.Stop();
					var sound = Sound.FromScreen("select_confirm");
				}
			}
		}
	}
}
