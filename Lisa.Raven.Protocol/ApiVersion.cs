namespace Lisa.Raven.Protocol
{
	public class ApiVersion
	{
		public ApiVersion(int major, int minor)
		{
			Major = major;
			Minor = minor;
		}

		public int Major { get; set; }
		public int Minor { get; set; }

		public bool Is(int major, int minor)
		{
			return Major == major && Minor == minor;
		}
	}
}