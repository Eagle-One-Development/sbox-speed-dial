using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;

namespace SpeedDial.Classic.Bots;

public partial class ClassicBotBehaviour
{
	public ClassicBot Bot { get; set; }

	public NavSteer Steer;
	public Draw Draw => Draw.Once;

	#region Randomized Variables
	private readonly float turnSpeed = Rand.Float( 10f, 25f ); // lower is slower
	private readonly float sinAimSpeed = Rand.Float( 10.0f, 15.0f );
	private readonly float accuracy = Rand.Float( 0.0f, 10.0f ); // lower is more accurate

	#endregion

	#region Inputs
	public bool Attack1 { get; set; }
	public bool Attack2 { get; set; }
	public Angles ViewAngles { get; set; }
	public Vector3 InputDirection { get; set; }
	#endregion

	public Entity CurrentTarget { get; protected set; }
	public Entity CurrentPlayer { get; protected set; }
	public Entity CurrentWeapon { get; protected set; }
	public Vector3 InputVelocity { get; protected set; }

	#region Static Variables
	public float UpdateInterval => 1.0f;
	public float SearchRadius => 400.0f;
	public float MinWanderRadius => 1000;
	public float MaxWanderRadius => 10000;
	public float PlayerOrbitDistance => 200;
	#endregion

	private TimeSince timeSinceUpdate;
	private Vector3 lastPos;

	public virtual void Tick()
	{
		if ( Debug.Bots )
		{
			DebugOverlay.Sphere( Bot.Client.Pawn.Position, SearchRadius, Color.Magenta );
			DebugOverlay.Text( $"{Bot.GetType().Name}\nFake Client Name: {Bot.Client.Name}\nCurrent Target: {(CurrentTarget != null ? CurrentTarget : "null")}", Bot.Client.Pawn.Position, CurrentTarget != null ? Color.Yellow : Color.White, 0, 1000 );
		}

		if ( Bot.Client.Pawn is null || Bot.Client.Pawn.LifeState == LifeState.Dead || (Bot.Client.Pawn as ClassicPlayer).Frozen ) return;

		SetInputs();

		// Reevaulate our target every interval
		if ( timeSinceUpdate > UpdateInterval )
		{
			CurrentTarget = EvaulateTarget();
			timeSinceUpdate = 0f;
		}

		if ( Steer != null )
		{
			if ( CurrentTarget != null && CurrentTarget.IsValid )
			{
				if ( Debug.Bots ) DebugOverlay.Sphere( EvaulatePositon( CurrentTarget ), 30f, Color.Green );
				Steer.Target = EvaulatePositon( CurrentTarget );
			}
			else if ( Steer.Path.IsEmpty || Bot.Client.Pawn.Position.AlmostEqual( lastPos, 0.1f ) )
			{
				// Wander
				var t = NavMesh.GetPointWithinRadius( Bot.Client.Pawn.Position, MinWanderRadius, MaxWanderRadius );
				Steer.Target = t.HasValue ? t.Value : Bot.Client.Pawn.Position;
			}

			Steer.Tick( Bot.Client.Pawn.Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
			}

			if ( Debug.Bots )
			{
				Steer.DebugDrawPath();
			}
		}
		else
		{
			Steer = new NavSteer();
		}

		lastPos = Bot.Client.Pawn.Position;
	}

	/// <summary>
	/// Decide inputs
	/// </summary>
	public virtual void SetInputs()
	{
		var pawn = Bot.Client.Pawn as BasePlayer;

		Attack1 = CurrentTarget is ClassicPlayer;

		Attack2 = (pawn.ActiveChild is not Weapon weapon) ||
			((weapon != null) && (weapon.AmmoClip <= 0));

		var targetView = CurrentPlayer != null && CurrentPlayer.IsValid ? Rotation.LookAt( (CurrentPlayer.Position - Bot.Client.Pawn.Position).Normal, Vector3.Up ).Angles() :
				Rotation.LookAt( InputVelocity, Vector3.Up ).Angles();
		if ( CurrentTarget is ClassicPlayer ) targetView += new Angles( 0, MathF.Sin( Time.Now * sinAimSpeed ) * accuracy, 0 );
		ViewAngles = Angles.Lerp( ViewAngles, targetView, Time.Delta * turnSpeed );
		InputDirection = InputVelocity;
	}

	/// <summary>
	/// Decide where we want to go based on our target
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	public virtual Vector3 EvaulatePositon( Entity target )
	{
		var pawn = Bot.Client.Pawn as BasePlayer;

		// Don't go right up to the player if we have a gun
		return target is ClassicPlayer player && pawn.ActiveChild != null
			? target.Position + ((pawn.Position - target.Position).Normal * PlayerOrbitDistance)
			: target.Position;
	}

	/// <summary>
	/// Choose what the bot should move to; the main decision making process. Override this for different gamemodes ande write your own logic
	/// </summary>
	/// <returns></returns>
	public virtual Entity EvaulateTarget()
	{
		Entity target = null;

		// get those entities
		var closestPlayer = GetClosestEntityInSphere<ClassicPlayer>( Bot.Client.Pawn.Position, SearchRadius, Bot.Client.Pawn );
		var closestWeapon = GetClosestEntityInSphere<Weapon>( Bot.Client.Pawn.Position, SearchRadius );
		var closestDrug = GetClosestEntityInSphere<ClassicBaseDrug>( Bot.Client.Pawn.Position, SearchRadius );

		CurrentPlayer = closestPlayer;
		CurrentWeapon = closestWeapon;

		// random variables
		var pawn = Bot.Client.Pawn as ClassicPlayer;

		bool weapon = (pawn.ActiveChild as Weapon) != null;
		int ammo = 0;
		int clip = 0;

		if ( weapon )
		{
			ammo = (pawn.ActiveChild as Weapon).AmmoClip;
			clip = (pawn.ActiveChild as Weapon).Blueprint.ClipSize;
		}

		bool drug = pawn.ActiveDrug;

		float playerDist = closestPlayer != null ? Vector3.DistanceBetween( pawn.Position, closestPlayer.Position ) : float.MaxValue;
		float weaponDist = closestWeapon != null ? Vector3.DistanceBetween( pawn.Position, closestWeapon.Position ) : float.MaxValue;
		float drugDist = closestDrug != null ? Vector3.DistanceBetween( pawn.Position, closestDrug.Position ) : float.MaxValue;

		// dumb logic
		// precedence is player/weapon/drug
		// choose player if weapon and drug or no weapon and closest or weapon and closer than drug
		if ( closestPlayer != null && ((weapon && drug) || (!weapon && playerDist < weaponDist && playerDist < drugDist) || (weapon && (playerDist < drugDist))) )
		{
			target = closestPlayer;
		}
		// choose weapon if no weapon or no ammo
		else
		{
			target = closestWeapon != null && (!weapon || ammo <= 0) ? closestWeapon : closestDrug != null && (!drug) ? closestDrug : (Entity)null;
		}

		return target;
	}

	/// <summary>
	/// Finds the nearest entity within a sphere radius
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="position">Position of the sphere</param>
	/// <param name="radius">Radius of the sphere</param>
	/// <param name="ignore">Entities to ignore in the search</param>
	public static T GetClosestEntityInSphere<T>( Vector3 position, float radius, params Entity[] ignore ) where T : Entity
	{
		List<Entity> ents = Entity.FindInSphere( position, radius ).Where( x => x is T && !ignore.Contains( x ) ).ToList();
		Entity closestEnt = null;

		float smallestDist = 999999;
		foreach ( var ent in ents )
		{
			var dist = Vector3.DistanceBetween( position, ent.Position );
			if ( dist < smallestDist )
			{
				smallestDist = dist;
				closestEnt = ent;
			}
		}

		return closestEnt as T;
	}
}
