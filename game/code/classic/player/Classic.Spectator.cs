namespace SpeedDial.Classic.Player;

public partial class ClassicSpectator : ClassicPlayer
{

	public override void InitialRespawn()
	{
		Respawn();
	}

	public override void Respawn()
	{
		Game.AssertServer();

		SetModel( ModelPath );
		EnableDrawing = false;

		Controller = new OneChamberSpectatorController();
		Camera = new OneChamberSpectatorCamera();
		//Animator = new ClassicAnimator();

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;

		CreateHull();
		ResetInterpolation();
	}

	public override void CreateHull()
	{
		// no physics
	}

	public override void TakeDamage( DamageInfo info )
	{
		return;
	}

	public override void Simulate( IClient cl )
	{
		SimulateActiveChild( cl, ActiveChild );
		GetActiveController()?.Simulate( cl, this, GetActiveAnimator() );
	}
}
