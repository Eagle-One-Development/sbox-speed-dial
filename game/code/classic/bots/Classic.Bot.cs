namespace SpeedDial.Classic.Bots;

public partial class ClassicBot : Bot
{
	protected ClassicBotBehaviour behaviour;

	public override void BuildInput()
	{
		Input.ClearActions();

		if ( behaviour != null )
		{
			//Input.InputDirection = behaviour.InputDirection;
			//Input.ViewAngles = behaviour.ViewAngles;
			Input.SetAction( "Attack1", behaviour.Attack1 );
			Input.SetAction( "Attack2", behaviour.Attack2 );
		}
	}

	public virtual void ApplyBehaviour<T>() where T : ClassicBotBehaviour, new()
	{
		Log.Debug( $"Bot behaviour applied | {typeof( T )}" );
		behaviour = new T
		{
			Bot = this
		};
	}

	public override void Tick()
	{
		if ( behaviour is null )
		{
			return;
		}

		behaviour.Tick();
	}
}
