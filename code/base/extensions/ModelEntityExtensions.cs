namespace SpeedDial;

public static class ModelEntityExtensions
{
	public static void SetGlowState( this ModelEntity ent, bool state, Color col )
	{
		var glow = ent.Components.GetOrCreate<Glow>();
		glow.Enabled = state;
		glow.Color = col;
		glow.Width = 1;
	}
}
