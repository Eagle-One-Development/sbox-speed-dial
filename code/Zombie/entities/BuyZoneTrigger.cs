namespace SpeedDial.Zombie.Entities;

public class BuyZoneTrigger : ModelEntity
{


	private BuyZone buyZone { get; set; }
	public BuyZone BuyZoneEntity => buyZone;

	/// <summary>
	/// Entities with these tags can activate this trigger.
	/// </summary>
	[Property(), Title( "Activation Tags" ), DefaultValue( "player" )]
	public TagList ActivationTags { get; set; } = "player";

	/// <summary>
	/// Whether this entity is enabled or not.
	/// </summary>
	[Property]
	public bool Enabled { get; protected set; } = true;

	/// <summary>
	/// List of entities currently within this trigger's bounds.
	/// </summary>
	public IEnumerable<Entity> TouchingEntities => touchingEntities;

	/// <summary>
	/// A convenience property containing number of entities currently within this trigger's bounds.
	/// </summary>
	public int TouchingEntityCount => touchingEntities.Count;

	readonly List<Entity> touchingEntities = new();

	// Used for when an entity enters the trigger while it is disabled, and then the trigger gets enabled
	readonly List<Entity> touchingEntitiesWhileDisabled = new();

	public BuyZoneTrigger()
	{
	}

	public BuyZoneTrigger( BuyZone buyZone )
	{
		this.buyZone = buyZone;
		Transform = buyZone.Transform;
		SetupPhysicsFromOBB( PhysicsMotionType.Static, buyZone.BuyZoneMins, buyZone.BuyZoneMaxs );
	}

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "buyzone", "trigger" );
		ActivationTags.Clear();
		ActivationTags.Add( "player" );
		EnableAllCollisions = false;
		EnableTouch = true;
		EnableTraceAndQueries = true;

		Transmit = TransmitType.Never;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other.IsWorld )
			return;

		AddToucher( other );
	}

	// This is to make sure we can add a toucher after they have entered the trigger but we were on a cooldown or something (trigger_multiple's wait param)
	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other.IsWorld )
			return;

		AddToucher( other );
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other.IsWorld )
			return;

		if ( touchingEntitiesWhileDisabled.Contains( other ) )
		{
			touchingEntitiesWhileDisabled.Remove( other );
		}

		if ( touchingEntities.Contains( other ) )
		{
			touchingEntities.Remove( other );
			OnTouchEnd( other );
		}
	}

	void AddToucher( Entity toucher )
	{
		if ( !toucher.IsValid() )
			return;

		if ( !Enabled )
		{
			// We don't care about the filter because we will pass these entities to StartTouch
			if ( !touchingEntitiesWhileDisabled.Contains( toucher ) )
			{
				touchingEntitiesWhileDisabled.Add( toucher );
			}

			return;
		}

		if ( touchingEntities.Contains( toucher ) )
			return;

		if ( !PassesTriggerFilters( toucher ) )
			return;

		bool anyoneTouching = touchingEntities.Count > 0;

		touchingEntities.Add( toucher );
		OnTouchStart( toucher );
	}


	/// <summary>
	///	An entity that passes PassesTriggerFilters has started touching the trigger
	/// </summary>
	public virtual void OnTouchStart( Entity toucher )
	{
		if ( !Enabled || toucher is not BasePlayer player ) return;

		buyZone?.OnEntered( player );
	}

	/// <summary>
	///	An entity that started touching this trigger has stopped touching
	/// </summary>
	public virtual void OnTouchEnd( Entity toucher )
	{
		if ( !Enabled || toucher is not BasePlayer player ) return;

		buyZone?.OnExited( player );
	}

	/// <summary>
	///	Determine if an entity should be allowed to touch this trigger
	/// </summary>
	public virtual bool PassesTriggerFilters( Entity other )
	{
		if ( other is not ModelEntity )
		{
			return false;
		}

		if ( other.Tags.HasAny( ActivationTags ) || ActivationTags.Contains( "*" ) )
		{
			return true;
		}

		return false;
	}
	public bool TryBuyZone( BasePlayer player )
	{
		if ( !Enabled ) return false;

		buyZone?.Buy( player );

		return buyZone.HoldUse;
	}
}
