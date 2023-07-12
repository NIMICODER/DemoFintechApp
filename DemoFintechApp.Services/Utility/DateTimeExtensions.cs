namespace DemoFintechApp.Services.Utility
{
	public static class DateTimeExtensions
	{
		public static long ToTimeStamp(this DateTime dateInstance)
		{
			DateTime epochDateTime = new DateTime(1970, 1, 1);
			return (long)(dateInstance - epochDateTime).TotalMilliseconds;
		}
	}

}
