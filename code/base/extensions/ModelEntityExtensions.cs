namespace SpeedDial;

public static class ModelEntityExtensions
{
	public static void SetGlowState( this ModelEntity ent, bool state, Color col )
	{
		Host.AssertServer();

		GlowUtil.SetGlow( ent, state, col );
	}
}

public static partial class GlowUtil
{
	[ClientRpc]
	public static void SetGlow( Entity ent, bool state, Color color )
	{
		Host.AssertClient();

		//var glow = ent.Components.GetOrCreate<Glow>();
		//glow.Enabled = state;
		//glow.Color = color;
		//glow.Width = 0.3f;
		//glow.ObscuredColor = Color.White.WithAlpha( 0 ); // ugh
	}
}
