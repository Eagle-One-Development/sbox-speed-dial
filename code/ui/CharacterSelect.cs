using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;



namespace SpeedDial.UI {
	public class CharacterSelect : Panel {

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

		public CharacterSelect() {
			StyleSheet.Load("/ui/CharacterSelect.scss");
			backPortrait = Add.Image("materials/ui/portraits/default.png", "backportrait");
			portrait = Add.Image("materials/ui/portraits/default.png", "portrait");

			AddClass("active");
			title = portrait.Add.Label("PLAYER NAME", "title");
			description = portrait.Add.Label("Default character is so cool. He spent his days doing crime while not doing the time.", "description");
			startLoad = description.Add.Label("Abilities: NONE\nWeapon: NONE", "loadout");

			Current = this;
		}

		public override void Tick() {
			base.Tick();

			BaseSpeedDialCharacter character = SpeedDialGame.Instance.characters[currentIndex];

			title.Text = character.Name;
			description.Text = character.Description;
			portrait.SetTexture(character.Portrait);

			string wep = Library.GetAttribute(character.Weapon).Title;
			startLoad.Text = $"Weapon: {wep}\nAbility: NONE";

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
			portrait.Style.Dirty();
			backPortrait.Style.Dirty();

			if(Host.IsClient) {
				if(!open) {
					SetClass("active", false);
				} else {
					SetClass("active", true);
				}
			}
		}

		[Event("buildinput")]
		public void ProcessClientInput(InputBuilder input) {
			bool Q = input.Pressed(InputButton.Menu);
			bool E = input.Pressed(InputButton.Use);
			bool space = input.Pressed(InputButton.Jump);

			if(input.Pressed(InputButton.Score)) {
				open = !open;
			}

			if(open) {
				if(E) {
					currentIndex++;
					translate = 100f;
					translate2 = 0f;
					right = true;
					if(currentIndex > SpeedDialGame.Instance.characters.Count - 1) {
						currentIndex = 0;
					}
				}

				if(Q) {
					right = false;
					currentIndex--;
					//Log.Info(currentIndex.ToString());
					translate2 = 100f;
					translate = 0f;

					if(currentIndex < 0) {
						currentIndex = SpeedDialGame.Instance.characters.Count - 1;
					}
				}

				if(space) {
					string[] s = { currentIndex.ToString() };
					//Log.Info(s[0].ToString());
					ConsoleSystem.Run("set_character", s);
					open = false;
				}
			}
		}
	}
}
