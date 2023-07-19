using SpeedDial.Koth.Entities;


namespace SpeedDial.Koth.UI;

public partial class KothHillPositionElement
{
	public Image HillIcon { get; set; }

	public Panel DivPanel { get; set; }

	public override void Tick()
	{
		base.Tick();

		HillIcon.SetClass( "active", Entity.All.OfType<HillSpot>().Any() );

		if ( Entity.All.OfType<HillSpot>().Any() )
		{
			var ent = Entity.All.OfType<HillSpot>().First();

			//All of this is so incredibly cursed, for the record, it's caused by issues with sciscorring on panels 
			//where translate(-50%) has been applied. 
			var screenPos = (ent.Position + (Vector3.Forward * 16f) - (Vector3.Right * 8f)).ToScreen();

			HillIcon.Style.Left = Length.Fraction( screenPos.x.Clamp( 0f, 1f - (Screen.Height * 0.05f / Screen.Width) ) );
			HillIcon.Style.Top = Length.Fraction( screenPos.y.Clamp( 0f, 0.95f ) );
		}
	}
}
