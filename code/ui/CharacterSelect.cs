using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Threading.Tasks;
using SpeedDial.Weapons;
using SpeedDial.Player;



namespace SpeedDial.UI{
    public class CharacterSelect : Panel{

		public Image portrait;
		public Label title;

		public Label description;

		public Label startLoad;

		private int currentIndex;

		private float translate;

        public CharacterSelect(){
			StyleSheet.Load("/ui/CharacterSelect.scss"); 
			portrait = Add.Image("/ui/portraits/default.png","portrait");
			
			title = portrait.Add.Label("PLAYER NAME","title");
			description = portrait.Add.Label("Default character is so cool. He spent his days doing crime while not doing the time.", "description");
			startLoad = description.Add.Label("Abilities: NONE\nWeapon: NONE", "loadout");

			currentIndex = 0;
		}

		public override void Tick(){
			base.Tick();

			SpeedDialPlayerCharacter character = SpeedDialGame.Instance.characters[currentIndex];

			title.Text = character.name + " " + currentIndex.ToString();
			description.Text = character.description;
			portrait.SetTexture(character.portrait);


			var transform = new PanelTransform();
			transform.AddTranslateY( Length.Percent( translate ) );

			translate = translate.LerpTo(0f,Time.Delta * 8f);

			portrait.Style.Transform = transform;
			portrait.Style.Dirty();

			if(Local.Client != null){
				bool Q = Local.Client.Input.Pressed(InputButton.Menu);
				bool E = Local.Client.Input.Pressed(InputButton.Use);

				

				if(Q){
					currentIndex++;
					translate = 100f;
					if(currentIndex > SpeedDialGame.Instance.characters.Count - 1){
						currentIndex = 0;
					}
				}

				if(E){
					currentIndex--;
					translate = 100f;
					if(currentIndex < 0){
						currentIndex = SpeedDialGame.Instance.characters.Count - 1;
					}
				}
			}
			

		}
    }

}
