namespace SpeedDial;

public partial class InputGlyph
{
	public Image Glyph { get; set; }
	public InputAction Button { get; set; }

	protected bool IsSet = false;

	public override void SetProperty( string name, string value )
	{
		base.SetProperty( name, value );

		if ( name == "btn" )
		{
			//SetButton( Enum.Parse<InputButton>( value, true ) );
		}
	}

	public void SetButton( InputAction button )
	{
		Button = button;
		IsSet = true;
	}

	public override void Tick()
	{
		base.Tick();

		if ( IsSet )
		{
			Texture glyphTexture = Input.GetGlyph( Button.Name, InputGlyphSize.Medium, GlyphStyle.Knockout.WithNeutralColorABXY() );

			Glyph.Texture = glyphTexture;
			Glyph.Style.AspectRatio = (float)glyphTexture.Width / glyphTexture.Height;
		}
	}
}
