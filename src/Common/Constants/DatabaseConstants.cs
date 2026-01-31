namespace ICMarket.Common.Constants;

/// <summary>
/// Constants for database table names, index names, column max lengths, and connection string keys.
/// </summary>
public static class DatabaseConstants
{
	public static class Tables
	{
		public const string BlockchainData = "BlockchainData";
	}

	public static class Indexes
	{
		public const string BlockchainDataNameCreatedAt = "IX_BlockchainData_Name_CreatedAt";
	}

	public static class ColumnLengths
	{
		public const int Name = 50;
		public const int Hash = 256;
		public const int Time = 100;
		public const int LatestUrl = 500;
		public const int PreviousHash = 256;
		public const int PreviousUrl = 500;
		public const int LastForkHash = 256;
	}

	public static class Configuration
	{
		public const string DefaultConnection = "DefaultConnection";
	}
}