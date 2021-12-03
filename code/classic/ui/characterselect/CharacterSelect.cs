using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class CharacterSelect : Panel {
		public static CharacterSelect Current;
		// bindings for HTML
		public string HeaderTitle => "SELECT A CRIMINAL";
		public string SelectHeader => $"[{Input.GetKeyWithBinding("+iv_jump").ToUpper()}] TO SELECT";
		public string PromptLeft => $"<{Input.GetKeyWithBinding("+iv_menu").ToUpper()}";
		public string PromptRight => $"{Input.GetKeyWithBinding("+iv_use").ToUpper()}>";

		public bool Open = false;
		private TimeSince TimeSinceToggled;

		private Label PromptLeftLabel { get; set; }
		private Label PromptRightLabel { get; set; }
		private float PromptLeftScale;
		private float PromptRightScale;

		private CharacterPanel Character1 { get; set; }
		private CharacterPanel Character2 { get; set; }
		private CharacterPanel Character3 { get; set; }
		private CharacterPanel Character4 { get; set; }

		private int startIndex = 0;
		public static int SelectedIndex = 0;

		public CharacterSelect() {
			Current = this;
		}

		private Sound MenuSound;

		[ClientRpc]
		public static void ForceState(bool state) {
			Current.SetState(state);
		}

		private void ToggleMenu() {
			Open = !Open;
			TimeSinceToggled = 0;
			SetState(Open);
		}

		private void SetState(bool state) {
			if(!state) {
				MenuSound.Stop();
				Sound.FromScreen("select_confirm");
			} else {
				Sound.FromScreen("tape_stop");
				MenuSound = Sound.FromScreen("tape_noise");
				SelectedIndex = (Local.Pawn as ClassicPlayer).CharacterIndex;
				// character is outside of the first 4 characters
				if(SelectedIndex > 3) {
					startIndex = SelectedIndex - 3;
				}
			}
		}

		public override void Tick() {
			TickInput();

			// handle open/close
			if(Toggle && TimeSinceToggled > 0.3f) {
				ToggleMenu();
			}
			SetClass("open", Open);

			if(!IsVisible)
				return;

			if(TimeSinceToggled > 0.1f) {
				if(Left || Right) {
					Sound.FromScreen("select_click");
				}

				if(Left) {
					PromptLeftScale += 0.3f;
					SelectedIndex--;
					SelectedIndex = SelectedIndex.Clamp(0, Character.All.Count - 1);
					if(SelectedIndex < startIndex) {
						startIndex--;
					}
				} else if(Right) {
					PromptRightScale += 0.3f;
					SelectedIndex++;
					SelectedIndex = SelectedIndex.Clamp(0, Character.All.Count - 1);
					if(SelectedIndex > startIndex + 3) {
						startIndex++;
					}
				}

				if(Select) {
					ClassicPlayer.SetCharacter(SelectedIndex);
					Open = false;
					MenuSound.Stop();
					Sound.FromScreen("select_confirm");
					return;
				}
			}

			Character1.Index = startIndex;
			Character2.Index = SetIndex(startIndex, 1);
			Character3.Index = SetIndex(startIndex, 2);
			Character4.Index = SetIndex(startIndex, 3);

			// clamp scale
			PromptLeftScale = PromptLeftScale.Clamp(0, 1.5f);
			PromptRightScale = PromptRightScale.Clamp(0, 1.5f);

			// apply scale left
			PanelTransform transformLeft = new();
			transformLeft.AddScale(PromptLeftScale);
			PromptLeftLabel.Style.Transform = transformLeft;

			// apply scale right
			PanelTransform transformRight = new();
			transformRight.AddScale(PromptRightScale);
			PromptRightLabel.Style.Transform = transformRight;

			// lerp to base scale
			PromptLeftScale = PromptLeftScale.LerpTo(1, Time.Delta * 7f);
			PromptRightScale = PromptRightScale.LerpTo(1, Time.Delta * 7f);
		}

		private static int SetIndex(int index, int add) {
			if(index > Character.All.Count - 1) {
				index = 0;
			}
			return index + add;
		}

		private bool Select;
		private bool Left;
		private bool Right;
		private bool Toggle;
		private TimeSince TimeSinceInputProcessed;

		// this is stupid but for some reason Input.Pressed is called multiple times in ui, probably a tick vs frame thing
		private void TickInput() {
			if(Select)
				Select = false;
			if(Left)
				Left = false;
			if(Right)
				Right = false;
			if(Toggle)
				Toggle = false;

			// wait a tick
			if(TimeSinceInputProcessed < Global.TickInterval) return;

			Select = Input.Pressed(InputButton.Jump);
			Left = Input.Pressed(InputButton.Menu);
			Right = Input.Pressed(InputButton.Use);
			Toggle = Input.Pressed(InputButton.Duck);

			TimeSinceInputProcessed = 0;
		}
	}
}
