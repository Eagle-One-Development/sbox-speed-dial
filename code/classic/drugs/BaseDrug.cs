using SpeedDial.Classic.Player;
using SpeedDial.Classic.UI;
using SpeedDial.Classic.Entities;

namespace SpeedDial.Classic.Drugs;

public partial class ClassicBaseDrug : ModelEntity {
	public virtual string WorldModel { get; }
	public virtual string DrugName { get; }
	public virtual string DrugDescription { get; }
	public virtual DrugType DrugType { get; }
	public virtual string Icon { get; }
	public virtual string PickupSound { get; }
	public virtual string ParticleName { get; }
	public virtual Color HighlightColor => new(1, 1, 1, 1);
	public TimeSince TimeSinceSpawned { get; set; }
	public BasePickupTrigger PickupTrigger { get; protected set; }
	public ClassicDrugSpawn DrugSpawn { get; set; }

	public override void Spawn() {
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
		SetInteractsAs(CollisionLayer.Debris); // so player movement doesn't walk into it

		SetModel(WorldModel);

		MoveType = MoveType.None;

		this.SetGlowState(true, HighlightColor);

		PickupTrigger = new();
		PickupTrigger.Position = Position;
		PickupTrigger.Parent = this;
		PickupTrigger.ResetInterpolation();
		PickupTrigger.EnableTouchPersists = true;
		PickupTrigger.EnableTouch = true;
		PickupTrigger.SetTriggerSize(25);

		// workaround with spawning
		PickupTrigger.EnableAllCollisions = false;

		TimeSinceSpawned = 0;
	}

	public void Taken(ClassicPlayer player) {
		if(DrugSpawn is not null) {
			DrugSpawn.DrugTaken();
			DrugSpawn = null;
		}

		player.ActiveDrug = true;
		player.DrugType = DrugType;
		player.TimeSinceDrugTaken = 0;

		Effect(player);

		Delete();
	}

	public virtual void Effect(ClassicPlayer player) {
		var particle = Particles.Create(ParticleName);
		if(particle is not null) {
			particle.SetForward(0, Vector3.Up);
			particle.SetEntityBone(0, player, player.GetBoneIndex("head"), Transform.Zero, true);
			player.DrugParticles = particle;
		}

		BasePlayer.SoundFromScreen(To.Single(player.Client), PickupSound);

		ScreenHints.FireEvent(To.Single(player.Client), $"{DrugName}", $"{DrugDescription}", false);
	}

	[Event.Tick.Server]
	public void ServerTick() {
		if(TimeSinceSpawned >= 0.5f) {
			PickupTrigger.EnableAllCollisions = true;
		}
	}

	[Event.Frame]
	public void Frame() {
		if(SceneObject is null) return;
		SceneObject.Rotation = SceneObject.Rotation.RotateAroundAxis(Vector3.Up, Time.Delta * 20f);
		SceneObject.Position += Vector3.Up * MathF.Sin(Time.Now) * 7 * Time.Delta;
	}

	[Event.Tick]
	public void Tick() {
		if(PickupTrigger is null) return;
		Debug.Sphere(Position, 5, Color.Green, 0.01f, false);
		Debug.Sphere(PickupTrigger.Position, 10, Color.Red, 0.01f, false);
		Debug.Line(Position, PickupTrigger.Position, Color.Yellow, 0.01f, false);
	}

	public static Type GetRandomSpawnableType() {
		var types = Library.GetAll<ClassicBaseDrug>();
		return types.Random();
	}
}

public enum DrugType {
	Polvo,
	Leaf,
	Ollie,
	Ritindi
}
