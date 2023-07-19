namespace SpeedDial;

public static class EntityExtension
{
	public static void Kill( this Entity ent )
	{
		ent.TakeDamage( DamageInfo.Generic( float.MaxValue ) );
	}

	public static bool Alive( this Entity ent )
	{
		return ent.LifeState == LifeState.Alive;
	}

	// it's here cause fuck u that's why
	public static bool HasAttribute<T>( this Type type, bool inherit = false ) where T : Attribute
	{
		return type.IsDefined( typeof( T ), inherit );
	}
}
