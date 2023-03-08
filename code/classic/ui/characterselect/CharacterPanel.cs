using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.Player;

namespace SpeedDial.Classic.UI;

public partial class CharacterPanel
{
	public Image Portrait { get; set; }
	public Label CharacterName { get; set; }
	public Label Description { get; set; }
	public Image Weapon { get; set; }
	public int Index;

	public override void Tick()
	{
		if ( !IsVisible )
			return;

		SetClass( "selected", Index == CharacterSelect.SelectedIndex );

		// redundancy clamp
		Index = Index.Clamp( 0, Character.All.Count - 1 );

		var character = Character.All.ElementAtOrDefault( Index );
		Portrait.Texture = character.PortraitTexture;
		CharacterName.Text = $"{character.CharacterName.ToUpper()}";
		Description.Text = $"{character.Description.ToUpper()}";
		var wepblueprint = WeaponBlueprint.GetBlueprint( character.WeaponClass );
		Weapon.Texture = wepblueprint?.IconTexture;
	}
}
