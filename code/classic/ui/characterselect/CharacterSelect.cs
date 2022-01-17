using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using SpeedDial.Classic.Player;
using SpeedDial.Classic.Rounds;

namespace SpeedDial.Classic.UI {
	[UseTemplate]
	public partial class CharacterSelect : Panel {
		public static CharacterSelect Current { get; private set; }
		// bindings for HTML
		public string HeaderTitle => "SELECT A CRIMINAL";
		public string SelectHeader => $"[{Input.GetButtonOrigin(InputButton.Jump).ToUpper()}] TO SELECT";
		public string PromptLeft => $"< {(Input.UsingController ? "" : Input.GetButtonOrigin(InputButton.Menu).ToUpper())}";
		public string PromptRight => $"{(Input.UsingController ? "" : Input.GetButtonOrigin(InputButton.Use).ToUpper())} >";

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
			TimeSinceToggled = 0;
			SetState(!Open);
		}

		private void SetState(bool state) {
			if(!state) {
				if(Open != state) {
					Sound.FromScreen("select_confirm");
					MenuSound.Stop();
					(Local.Pawn as ClassicPlayer).FadeSoundtrack(1);
				}
				Open = state;
			} else {
				if(Open != state) {
					Sound.FromScreen("tape_stop");
					MenuSound = Sound.FromScreen("tape_noise");
					(Local.Pawn as ClassicPlayer).FadeSoundtrack(0.3f);
				}

				// make sure equipped character is the selected one
				SelectedIndex = (Local.Pawn as ClassicPlayer).CharacterIndex;
				// character is outside of the first 4 characters
				if(SelectedIndex > 3) {
					// adjust start index so that selected index is at the right edge
					startIndex = SelectedIndex - 3;
				}
				Open = state;
			}
		}

		public override void Tick() {
			// handle open/close
			if(Toggle && TimeSinceToggled > 0.3f && ClassicGamemode.Current.ActiveRound is not PostRound) {
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
					// reached left end of current lineup, shove start index to the left
					if(SelectedIndex < startIndex) {
						startIndex--;
					}
				}
				if(Right) {
					PromptRightScale += 0.3f;
					SelectedIndex++;
					SelectedIndex = SelectedIndex.Clamp(0, Character.All.Count - 1);
					// reached right end of current lineup, shove start index to the right
					if(SelectedIndex > startIndex + 3) {
						startIndex++;
					}
				}

				if(Select) {
					ClassicPlayer.SetCharacter(SelectedIndex);
					Open = false;
					MenuSound.Stop();
					Sound.FromScreen("select_confirm");
					(Local.Pawn as ClassicPlayer).FadeSoundtrack(1);
					return;
				}
			}

			// set panel indices, start index is first panel on the left
			Character1.Index = startIndex;
			Character2.Index = startIndex + 1;
			Character3.Index = startIndex + 2;
			Character4.Index = startIndex + 3;

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

		private bool Left;
		private bool LeftReleased = true;
		private bool Right;
		private bool RightReleased = true;
		private bool Select;
		private bool SelectReleased = true;
		private bool Toggle;
		private bool ToggleReleased = true;


		[Event.BuildInput]
		public void BuildInput(InputBuilder input) {
			Left = input.UsingController ? input.Pressed(InputButton.SlotPrev) || input.Pressed(InputButton.Slot1) : input.Pressed(InputButton.Menu) && LeftReleased;
			Right = input.UsingController ? input.Pressed(InputButton.SlotNext) || input.Pressed(InputButton.Slot2) : input.Pressed(InputButton.Use) && RightReleased;
			Select = input.Pressed(InputButton.Jump) && SelectReleased;
			Toggle = input.Pressed(InputButton.Duck) && ToggleReleased;
		}
	}
}
