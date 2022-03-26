namespace SpeedDial.Classic.Bots;

public partial class ClassicBot : Bot
{
	protected ClassicBotBehaviour behaviour;

	public override void BuildInput( InputBuilder builder )
	{
		builder.Clear();

		if ( behaviour != null )
		{
			builder.InputDirection = behaviour.InputDirection;
			builder.ViewAngles = behaviour.ViewAngles;
			builder.SetButton( InputButton.Attack1, behaviour.Attack1 );
			builder.SetButton( InputButton.Attack2, behaviour.Attack2 );
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
