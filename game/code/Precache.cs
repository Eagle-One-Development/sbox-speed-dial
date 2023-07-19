namespace SpeedDial
{
	public partial class SDGame
	{
		private static void PrecacheAssets()
		{
			if ( ResourceLibrary.TryGet<StringManifest>( "data/precache.sdstrman", out var manifest ) )
			{
				Log.Info( "----PRECACHING----" );
				foreach ( var item in manifest.Manifest )
				{
					Log.Info( item );
					Precache.Add( $"{item}" );
				}
			}
			else
			{
				Log.Warning( $"Couldn't find precache string manifest!" );
			}
		}
	}
}
