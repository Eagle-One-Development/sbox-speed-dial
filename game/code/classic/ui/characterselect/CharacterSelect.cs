using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI;

public partial class CharacterSelect
{
	public static CharacterSelect Current { get; private set; }
	// bindings for HTML
	public string HeaderTitle => "SELECT A CRIMINAL";
	public string SelectHeader => $" TO SELECT";
	public string PromptLeft => $"< {(Input.UsingController ? "" : Input.GetButtonOrigin( "Menu" ).ToUpper())}";
	public string PromptRight => $"{(Input.UsingController ? "" : Input.GetButtonOrigin( "Use" ).ToUpper())} >";

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

	private Panel Progress { get; set; }
	private Panel ProgressBar { get; set; }

	private int startIndex = 0;
	public static int SelectedIndex = 0;

	public CharacterSelect()
	{
		Current = this;
	}

	private Sound MenuSound;

	[ClientRpc]
	public static void ForceState( bool state )
	{
		if ( Current is null ) return;
		Current.SetState( state );
	}

	private void ToggleMenu()
	{
		TimeSinceToggled = 0;
		SetState( !Open );
	}

	private void SetState( bool state )
	{
		if ( !state )
		{
			if ( Open != state )
			{
				Sound.FromScreen( "select_confirm" );
				MenuSound.Stop();
				ClassicPlayer.FadeSoundtrack( To.Single( Game.LocalClient ), 1 );
			}
			Open = state;
		}
		else
		{
			if ( Open != state )
			{
				Sound.FromScreen( "tape_stop" );
				MenuSound = Sound.FromScreen( "tape_noise" );
				ClassicPlayer.FadeSoundtrack( To.Single( Game.LocalClient ), 0.3f );
			}

			// make sure equipped character is the selected one
			SelectedIndex = (Game.LocalPawn as ClassicPlayer).CharacterIndex;
			// character is outside of the first 4 characters
			if ( SelectedIndex > 3 )
			{
				// adjust start index so that selected index is at the right edge
				startIndex = SelectedIndex - 3;
			}
			Open = state;
		}
	}

	public override void Tick()
	{
		// handle open/close
		if ( Toggle && TimeSinceToggled > 0.3f && !Gamemode.Instance.Ending )
		{
			ToggleMenu();
		}
		SetClass( "open", Open );

		if ( !IsVisible || !Open )
			return;

		// progress bar at the bottom
		Progress.Style.Width = Length.Fraction( 1 / (float)Character.All.Count * 4 );
		Progress.Style.Left = Length.Fraction( 1 / (float)Character.All.Count * startIndex );
		ProgressBar.SetClass( "hidden", Character.All.Count <= 4 );

		if ( TimeSinceToggled > 0.1f )
		{

			if ( Left )
			{
				// don't bump when we hit the end
				if ( SelectedIndex > 0 )
				{
					Sound.FromScreen( "select_click" );
					PromptLeftScale += 0.3f;
				}

				SelectedIndex--;
				SelectedIndex = SelectedIndex.Clamp( 0, Character.All.Count - 1 );
				// reached left end of current lineup, shove start index to the left
				if ( SelectedIndex < startIndex )
				{
					startIndex--;
				}
			}
			if ( Right )
			{
				// don't bump when we hit the end
				if ( SelectedIndex < Character.All.Count - 1 )
				{
					Sound.FromScreen( "select_click" );
					PromptRightScale += 0.3f;
				}

				SelectedIndex++;
				SelectedIndex = SelectedIndex.Clamp( 0, Character.All.Count - 1 );
				// reached right end of current lineup, shove start index to the right
				if ( SelectedIndex > startIndex + 3 )
				{
					startIndex++;
				}
			}

			if ( Select )
			{
				ClassicPlayer.SetCharacter( SelectedIndex );
				Open = false;
				MenuSound.Stop();
				Sound.FromScreen( "select_confirm" );
				ClassicPlayer.FadeSoundtrack( To.Single( Game.LocalClient ), 1 );
				return;
			}
		}

		// set panel indices, start index is first panel on the left
		Character1.Index = startIndex;
		Character2.Index = startIndex + 1;
		Character3.Index = startIndex + 2;
		Character4.Index = startIndex + 3;

		// clamp scale
		PromptLeftScale = PromptLeftScale.Clamp( 0, 1.5f );
		PromptRightScale = PromptRightScale.Clamp( 0, 1.5f );

		// apply scale left
		PanelTransform transformLeft = new();
		transformLeft.AddScale( PromptLeftScale );
		PromptLeftLabel.Style.Transform = transformLeft;

		// apply scale right
		PanelTransform transformRight = new();
		transformRight.AddScale( PromptRightScale );
		PromptRightLabel.Style.Transform = transformRight;

		// lerp to base scale
		PromptLeftScale = PromptLeftScale.LerpTo( 1, Time.Delta * 7f );
		PromptRightScale = PromptRightScale.LerpTo( 1, Time.Delta * 7f );
	}

	private bool Left;
	private bool Right;
	private bool Select;
	private bool Toggle;


	[GameEvent.Client.BuildInput]
	public void BuildInput()
	{
		Left = Input.UsingController ? Input.Pressed( "SlotPrev" ) || Input.Pressed( "Slot1" ) : Input.Pressed( "Menu" );
		Right = Input.UsingController ? Input.Pressed( "SlotNext" ) || Input.Pressed( "Slot2" ) : Input.Pressed( "Use" );
		Select = Input.Pressed( "Jump" );
		Toggle = Input.Pressed( "Duck" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( PromptLeft, PromptRight );
	}
}