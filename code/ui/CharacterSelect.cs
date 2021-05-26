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
		public Label title;

		public Label description;

		public Label startLoad;


		private int currentIndex = 0;

		private float translate;

		public bool open = false;

		public static CharacterSelect Current;

		public CharacterSelect() {
			StyleSheet.Load("/ui/CharacterSelect.scss");
			portrait = Add.Image("/ui/portraits/default.png", "portrait");

			title = portrait.Add.Label("PLAYER NAME", "title");
			description = portrait.Add.Label("Default character is so cool. He spent his days doing crime while not doing the time.", "description");
			startLoad = description.Add.Label("Abilities: NONE\nWeapon: NONE", "loadout");

			Current = this;
		}

		public override void Tick() {
			base.Tick();

			BaseSpeedDialCharacter character = SpeedDialGame.Instance.characters[currentIndex];

			title.Text = character.name;
			description.Text = character.description;
			portrait.SetTexture(character.portrait);

			string wep = Library.GetAttribute(character.weapon).Title;
			startLoad.Text = $"Weapon: {wep}\nAbility: NONE";

			var transform = new PanelTransform();
			transform.AddTranslateX(Length.Percent(-translate));

			translate = translate.LerpTo(0f, Time.Delta * 10f);

			portrait.Style.Transform = transform;
			portrait.Style.Dirty();

			if(Host.IsClient) {
				if(!open) {
					Style.Opacity = 0f;
				} else {
					Style.Opacity = 1f;


				}
			}
			Style.Dirty();


		}


		[Event("buildinput")]
		public void ProcessClientInput(InputBuilder input) {
			bool Q = input.Pressed(InputButton.Menu);
			bool E = input.Pressed(InputButton.Use);
			bool space = input.Pressed(InputButton.Jump);

			if(input.Pressed(InputButton.Score)) {
				open = !open;
				Log.Info(open.ToString());
			}
			if(open) {
				if(E) {
					currentIndex++;
					translate = 100f;

					if(currentIndex > SpeedDialGame.Instance.characters.Count - 1) {
						currentIndex = 0;
					}

				}

				if(Q) {
					currentIndex--;
					Log.Info(currentIndex.ToString());
					translate = 100f;
					if(currentIndex < 0) {
						currentIndex = SpeedDialGame.Instance.characters.Count - 1;
					}
				}
			}
		}

	}

}
