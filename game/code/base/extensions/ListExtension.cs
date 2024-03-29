namespace SpeedDial;

public static class ListExtension
{
	/// <summary>
	/// Selects a random member contained within the given list.
	/// </summary>
	/// <remarks>
	/// Throws InvalidOperationException if the provided list
	/// is empty.
	/// </remarks>
	public static T Random<T>( this IEnumerable<T> list )
	{
		return !list.Any()
			? throw new ArgumentException( "Cannot select a random member of an empty list!", nameof( list ) )
			: list.ElementAt( Game.Random.Int( 0, list.Count() - 1 ) );
	}

	public static bool Random<T>( this IEnumerable<T> list, out T item )
	{
		try
		{
			item = list.Random();
			return true;
		}
		catch ( InvalidOperationException )
		{
			item = default;
			return false;
		}
	}
}
