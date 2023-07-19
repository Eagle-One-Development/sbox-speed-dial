namespace SpeedDial.Classic.UI;

public partial class Crosshair : Panel
{

	private readonly Panel Cross;

	private readonly Panel[] Hairs;

	public static Crosshair Current { get; private set; }

	private Vector2 Mouse;

	private float BumpScale;

	public Crosshair()
	{
		Hairs = new Panel[4];
		StyleSheet.Load("/classic/ui/crosshair/Crosshair.scss");
		Cross = Add.Panel("cross");

		BindClass("active", () => true);

		for (int i = 0; i < 4; i++)
		{
			Hairs[i] = Cross.Add.Panel( "hair" );
			Hairs[i].BindClass( "inactive", () => (Game.LocalPawn as BasePlayer).ActiveChild == null );
		}

		Current = this;
	}

	[ClientRpc]
	public static void Fire()
	{
		if (Current is null) return;
		Current.BumpScale = 1f;
	}

	[ClientRpc]
	public static void UpdateMouse(Vector2 mouse)
	{
		if (Current is null) return;
		// floor to prevent fucky pixel snapping
		Current.Mouse = new Vector2(mouse.x.Floor(), mouse.y.Floor());
	}

	public override void Tick()
	{
		Cross.Style.Left = Length.Fraction(Mouse.x / Screen.Width);
		Cross.Style.Top = Length.Fraction(Mouse.y / Screen.Height);

		for (int hair = 0; hair < 4; hair++)
		{
			PanelTransform transform = new();

			transform.AddRotation(0, 0, hair * 90f);

			float pixel = 18f + (20f * BumpScale);
			transform.AddTranslateY(Length.Pixels(pixel));

			var h = Hairs[hair];
			h.Style.Transform = transform;
			h.Style.Dirty();
		}

		BumpScale = BumpScale.LerpTo(0f, Time.Delta * 6f);
	}
}
