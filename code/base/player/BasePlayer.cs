//CREDIT: Modified from Espionage.Engine by Jake Wooshito
namespace SpeedDial;

public partial class BasePlayer : AnimatedEntity
{

	public virtual float RespawnTime => 1;
	public virtual string ModelPath { get; set; } = "models/citizen/citizen.vmdl";
	[Net] public float MaxHealth { get; set; } = 100;
	[Net] public TimeSince TimeSinceDied { get; set; }
	[Net] public BaseCarriable LastActiveChild { get; set; }
	[Net] public BaseCarriable ActiveChild { get; set; }

	[Net, Predicted] public Rotation EyeRotation { get; set; }
	[Net, Predicted] public Vector3 EyeLocalPosition { get; set; }
	public Vector3 EyePosition => AimRay.Position;

	public override Ray AimRay => new( EyeLocalPosition + Position, EyeRotation.Forward );

	public override void Simulate( IClient cl )
	{
		if ( LifeState == LifeState.Dead )
		{
			if ( TimeSinceDied > RespawnTime && Game.IsServer && CanRespawn() )
			{
				Respawn();
			}
		}

		SimulateActiveChild( cl, ActiveChild );
		GetActiveController()?.Simulate( cl, this, GetActiveAnimator() );
	}

	public virtual bool CanRespawn()
	{
		return true;
	}

	[ClientRpc]
	public static void SoundFromEntity( string name, Entity entity )
	{
		Sound.FromEntity( name, entity );
	}

	[ClientRpc]
	public static void SoundFromScreen( string name )
	{
		Sound.FromScreen( name );
	}

	[ClientRpc]
	public static void SoundFromScreen( string name, float x, float y )
	{
		Sound.FromScreen( name, x, y );
	}

	[ClientRpc]
	public static void SoundFromWorld( string name, Vector3 position )
	{
		Sound.FromWorld( name, position );
	}

	public virtual void SimulateActiveChild( IClient client, BaseCarriable child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( LastActiveChild.IsAuthority )
		{
			LastActiveChild.Simulate( client );
		}
	}

	public virtual void OnActiveChildChanged( BaseCarriable previous, BaseCarriable next )
	{
		previous?.ActiveEnd( this, previous.Owner != this );
		next?.ActiveStart( this );
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		GetActiveController()?.FrameSimulate( cl, this, GetActiveAnimator() );
	}

	public virtual void CreateHull()
	{
		Tags.Add( "player" );
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );


		EnableHitboxes = true;
	}

	[ClientInput]
	public Rotation InputRotation { get; set; }

	[ClientInput]
	public Angles InputViewAngles { get; set; }

	[ClientInput]
	public float InputForward { get; set; }

	[ClientInput]
	public float InputLeft { get; set; }

	public override void BuildInput()
	{
		if ( Input.StopProcessing )
			return;

		// rotation
		InputRotation = InputViewAngles.ToRotation();

		// forward and left input from wasd
		InputForward = Input.AnalogMove.x;
		InputLeft = Input.AnalogMove.y;

		ActiveChild?.BuildInput();

		GetActiveController()?.BuildInput();

		if ( Input.StopProcessing )
			return;

		GetActiveAnimator()?.BuildInput();
	}

	//
	// Pawn States
	//

	public virtual void InitialRespawn()
	{
		Respawn();
		// call round stuff after respawn to potentially override stuff in it
		SDGame.Current.ActiveGamemode?.ActiveRound?.OnPawnJoined( this );
	}

	public virtual void Respawn()
	{
		Game.AssertServer();

		SetModel( ModelPath );

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;

		CreateHull();
		ResetInterpolation();

		SDGame.Current.PawnRespawned( this );
		SDGame.Current.MoveToSpawnpoint( this );
		SDGame.Current.ActiveGamemode?.ActiveRound?.OnPawnRespawned( this );
	}

	public override void OnKilled()
	{
		LifeState = LifeState.Dead;
		SDGame.Current.PawnKilled( this, LastRecievedDamage );
	}

	public DamageInfo LastRecievedDamage { get; set; }

	public override void TakeDamage( DamageInfo info )
	{
		// If this pawn is allowed to take damage. Then take damage
		if ( Game.IsServer && SDGame.Current.PawnDamaged( this, ref info ) )
		{
			LastRecievedDamage = info;
			base.TakeDamage( info );
		}
	}

	/// <summary>
	///  called before the pawn gets cleaned up upon a client disconnect
	/// </summary>
	public virtual void OnClientDisconnected() { }

	//
	// Controller
	//

	[Net, Predicted]
	public PawnController Controller { get; set; }

	[Net]
	public PawnController DevController { get; set; }

	public virtual PawnController GetActiveController()
	{
		return DevController ?? Controller;
	}

	//
	// Animator
	//

	[Net, Predicted]
	protected PawnAnimator Animator { get; set; }

	public virtual PawnAnimator GetActiveAnimator()
	{
		return Animator;
	}

	//
	// Camera
	//

	public CameraMode Camera
	{
		get => Components.Get<CameraMode>();
		set
		{
			var current = Camera;
			if ( current == value ) return;

			Components.RemoveAny<CameraMode>();
			Components.Add( value );
		}
	}

	//
	// Team
	//

	public Team Team
	{
		get => Components.Get<Team>();
		set
		{
			var current = Team;
			if ( current == value ) return;

			Components.RemoveAny<Team>();
			Components.Add( value );
		}
	}
}
