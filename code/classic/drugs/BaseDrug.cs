using SpeedDial.Classic.Entities;
using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;

namespace SpeedDial.Classic.Drugs;

public partial class ClassicBaseDrug : ModelEntity
{
	public virtual string WorldModel { get; }
	public virtual string DrugName { get; }
	public virtual string DrugDescription { get; }
	public virtual DrugType DrugType { get; }
	public virtual string Icon { get; }
	public virtual string PickupSound { get; }
	public virtual string ParticleName { get; }
	public virtual Color HighlightColor => new( 1, 1, 1, 1 );
	public TimeSince TimeSinceSpawned { get; set; }
	public BasePickupTrigger PickupTrigger { get; protected set; }
	public ClassicDrugSpawn DrugSpawn { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "drug" ); // so players touch it as a trigger but not as a solid

		SetModel( WorldModel );

		this.SetGlowState( true, HighlightColor );

		PickupTrigger = new();
		PickupTrigger.Position = Position;
		PickupTrigger.Parent = this;
		PickupTrigger.ResetInterpolation();
		PickupTrigger.EnableTouchPersists = true;
		PickupTrigger.EnableTouch = true;
		PickupTrigger.SetTriggerSize( 25 );

		// workaround with spawning
		PickupTrigger.EnableAllCollisions = false;

		TimeSinceSpawned = 0;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		GlowUtil.SetGlow( this, true, HighlightColor );
	}

	public void Taken( ClassicPlayer player )
	{
		if ( DrugSpawn is not null )
		{
			DrugSpawn.DrugTaken();
			DrugSpawn = null;
		}

		player.ActiveDrug = true;
		player.DrugType = DrugType;
		player.TimeSinceDrugTaken = 0;

		Effect( player );

		Delete();
	}

	public virtual void Effect( ClassicPlayer player )
	{
		var particle = Particles.Create( ParticleName );
		if ( particle is not null )
		{
			particle.SetForward( 0, Vector3.Up );
			particle.SetEntityBone( 0, player, player.GetBoneIndex( "head" ), Transform.Zero, true );
			player.DrugParticles = particle;
		}

		BasePlayer.SoundFromScreen( To.Single( player.Client ), PickupSound );

		ScreenHints.FireEvent( To.Single( player.Client ), $"{DrugName}", $"{DrugDescription}", false );
	}

	[Event.Tick.Server]
	public void ServerTick()
	{
		if ( TimeSinceSpawned >= 0.5f )
		{
			PickupTrigger.EnableAllCollisions = true;
		}
	}

	[Event.Frame]
	public void Frame()
	{
		if ( SceneObject is null ) return;
		SceneObject.Rotation = SceneObject.Rotation.RotateAroundAxis( Vector3.Up, Time.Delta * 20f );
		SceneObject.Position += Vector3.Up * MathF.Sin( Time.Now ) * 7 * Time.Delta;
	}

	[Event.Tick]
	public void Tick()
	{
		if ( PickupTrigger is null ) return;
		if ( Debug.Enabled )
		{
			DebugOverlay.Sphere( Position, 5, Color.Green, Time.Delta, false );
			DebugOverlay.Sphere( PickupTrigger.Position, 10, Color.Red, Time.Delta, false );
			DebugOverlay.Line( Position, PickupTrigger.Position, Color.Yellow, Time.Delta, false );
		}
	}

	public static TypeDescription GetRandomSpawnableType()
	{
		var types = TypeLibrary.GetDescriptions<ClassicBaseDrug>();
		return types.Random();
	}
}

public enum DrugType
{
	Polvo,
	Leaf,
	Ollie,
	Ritindi
}
