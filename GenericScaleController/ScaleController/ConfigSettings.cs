namespace ScaleController
{
	public class ScaleConfiguration
	{
		public int ScaleID { get; set; }
		public int UpdateRate { get; set; }

		public int AxleingThreshold { get; set; } = 1000;
		public int AxleingDelay { get; set; } = 10000;

		public string Type { get; set; } = string.Empty;
		public string Connection { get; set; } = string.Empty;

		// Ethernet connection settings
		public string? IPAddress { get; set; }
		public int? Port { get; set; }

		// Serial connection settings
		public string? PortName { get; set; }
		public int? BaudRate { get; set; }

		public string EolCharacter { get; set; } = string.Empty;

		public string? ZeroCommand { get; set; }
		public string? TareCommand { get; set; }
		public string? ClearCommand { get; set; }
	}

	public class CameraConfiguration
	{
		public int UpdateRate { get; set; }

		public List<Camera> Cameras { get; set; } = new List<Camera>();
	}

	public class Camera
	{
		public string Name { get; set; } = string.Empty;
		public string Type { get; set; } = string.Empty;
		public string? IPAddress { get; set; } = string.Empty;
		public string? Url { get; set; } = string.Empty;
		public string? Username { get; set; }
		public string? Password { get; set; }
		public int? Channel { get; set; }
		public int? ImageScale { get; set; }
		public bool IgnoreSslIssues { get; set; }
		public bool ShowSuccess { get; set; }
		public bool Enabled { get; set; }
	}

	public class IOConfiguration
	{
		public string? PrinterName { get; set; }
		public int? RedGpioPin { get; set; } = 5;
		public int? GrnGpioPin { get; set; } = 6;
		public bool? InvertGpioPins { get; set; } = false;
	}

	public class ApplicationSettings
	{
		public string ServerUrl { get; set; } = string.Empty;
		public int LocationID { get; set; }

		public ScaleConfiguration? ScaleConfiguration { get; set; }
		public CameraConfiguration? CameraConfiguration { get; set; }
		public IOConfiguration? IOConfiguration { get; set; }
	}
}
