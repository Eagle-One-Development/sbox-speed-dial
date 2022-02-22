using SpeedDial.Classic.Player;
using SpeedDial.Classic.Weapons;
using SpeedDial.Classic.Drugs;
using SpeedDial.Classic.Bots;

namespace SpeedDial.Koth.Bots;

public partial class KothBotBehaviour : ClassicBotBehaviour {
	public float HillSearchRadius => 800.0f;
	public float HillRadius => 150.0f;
	private Entity hill;

	public override void Tick() {
		base.Tick();
		hill = Entity.All.Where(x => x is Entities.HillSpot).FirstOrDefault();
	}

	public override void SetInputs() {
		base.SetInputs();

		if(CurrentTarget != null && CurrentTarget.IsValid && CurrentTarget is Entities.HillSpot && Vector3.DistanceBetween(Bot.Client.Pawn.Position, CurrentTarget.Position) <= HillRadius) {
			InputDirection = Vector3.Zero;
		}
	}

	public override Vector3 EvaulatePositon(Entity target) {
		var pawn = Bot.Client.Pawn;

		// Don't go right up to the player if we have a gun
		if(target is ClassicPlayer player && pawn.ActiveChild != null) {
			return target.Position + (pawn.Position - target.Position).Normal * PlayerOrbitDistance;
		}

		return target.Position;
	}

	public override Entity EvaulateTarget() {
		Entity target = null;

		// get those entities
		var closestPlayer = GetClosestEntityInSphere<ClassicPlayer>(Bot.Client.Pawn.Position, SearchRadius, Bot.Client.Pawn);
		var closestWeapon = GetClosestEntityInSphere<Weapon>(Bot.Client.Pawn.Position, SearchRadius);
		var closestDrug = GetClosestEntityInSphere<ClassicBaseDrug>(Bot.Client.Pawn.Position, SearchRadius);

		CurrentPlayer = closestPlayer;
		CurrentWeapon = closestWeapon;

		// random variables
		var pawn = Bot.Client.Pawn as ClassicPlayer;

		bool weapon = (pawn.ActiveChild as Weapon) != null;
		int ammo = 0;
		int clip = 0;

		if(weapon) {
			ammo = (pawn.ActiveChild as Weapon).AmmoClip;
			clip = (pawn.ActiveChild as Weapon).Blueprint.ClipSize;
		}

		bool drug = pawn.ActiveDrug;

		float playerDist = closestPlayer != null ? Vector3.DistanceBetween(pawn.Position, closestPlayer.Position) : float.MaxValue;
		float weaponDist = closestWeapon != null ? Vector3.DistanceBetween(pawn.Position, closestWeapon.Position) : float.MaxValue;
		float drugDist = closestDrug != null ? Vector3.DistanceBetween(pawn.Position, closestDrug.Position) : float.MaxValue;

		// dumb logic
		// precedence is player/weapon/drug
		// choose player if weapon and drug or no weapon and closest or weapon and closer than drug
		if(closestPlayer != null && ((weapon && drug) || (!weapon && playerDist < weaponDist && playerDist < drugDist) || (weapon && (playerDist < drugDist)))) {
			target = closestPlayer;
		}
		// choose weapon if no weapon or no ammo
		else if(closestWeapon != null && (!weapon || ammo <= 0)) {
			target = closestWeapon;
		}
		// choose drug if no drug
		else if(closestDrug != null && (!drug)) {
			target = closestDrug;
		} else if(hill != null) {
			target = hill;
		}
		  // if target is null the bot will patrol/wander
		  else {
			target = null;
		}

		return target;
	}
}
