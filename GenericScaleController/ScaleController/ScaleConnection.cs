using System.Collections;
using System.Device.Gpio;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using Timer = System.Timers.Timer;

namespace ScaleController
{
	internal class ScaleConnection
	{
		private readonly ApplicationSettings _appSettings;
		private readonly ScaleConfiguration _scaleConfiguration;

		private ScaleData scale;

		private bool zeroScale = false;
		private bool tareScale = false;
		private bool clearScale = false;

		private bool resetAxles = false;
		private bool finishAxles = false;

		private int lastWeight = 0;

		private bool axleing = false;
		private int currentAxle = 1;

		private Timer axleUpdateTimer;

		public ScaleConnection(HttpClient client, IOptions<ApplicationSettings> appSettings)
		{
			_appSettings = appSettings.Value;
			if (_appSettings.ScaleConfiguration == null)
				throw new ArgumentNullException("ScaleConfiguration node in appsettings.json is null");
			_scaleConfiguration = _appSettings.ScaleConfiguration;

			scale = new ScaleData()
			{
				LocationID = _appSettings.LocationID,
				ScaleID = _scaleConfiguration.ScaleID,
				Status = "E",
				GrossWt = 0,
				Axle1Wt = null,
				Axle2Wt = null,
				Axle3Wt = null,
				Axle4Wt = null,
				RedIndicatorStatus = false,
				GrnIndicatorStatus = false
			};

			SetLightStatus(false, true);

			axleUpdateTimer = new Timer(_scaleConfiguration.AxleingDelay);
			axleUpdateTimer.Elapsed += AxleUpdateTimer_Elapsed;
		}

		public void SetScaleError(string error)
		{
			scale.Status = error;
			scale.GrossWt = 0;
		}

		public async Task SendWeightToServer(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					string jsonData = JsonConvert.SerializeObject(scale, Formatting.Indented);
					Console.WriteLine(jsonData);

					var client = new HttpClient();
					var response = await client.PostAsync($"{_appSettings.ServerUrl}/api/ScaleData/UpdateScaleData", new StringContent(jsonData, Encoding.UTF8, "application/json"));
					if (response.IsSuccessStatusCode)
					{
						var responseData = JsonConvert.DeserializeObject<UpdateLocationScaleDataResponse>(await response.Content.ReadAsStringAsync());
						if (_appSettings.IOConfiguration != null && !string.IsNullOrWhiteSpace(responseData?.PrintDocument))
							Print.PrintDocument(_appSettings.IOConfiguration.PrinterName, responseData.PrintDocument, string.Empty, 1);

						if (responseData?.ZeroScale == true)
							zeroScale = true;
						if (responseData?.TareScale == true)
							tareScale = true;
						if (responseData?.ClearScale == true)
							clearScale = true;

						if (responseData?.ResetAxles == true)
							resetAxles = true;
						if (responseData?.FinishAxles == true)
							finishAxles = true;
					}
					else
						Console.WriteLine($"Failed to send weight to server {_appSettings.ServerUrl}: {response.StatusCode}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to send weight to server {_appSettings.ServerUrl}:  {ex.Message}");
				}
				await Task.Delay(_scaleConfiguration.UpdateRate);
			}
		}

		public async Task GetWeight(CancellationToken cancellationToken)
		{
			var errorCount = 0;

			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					TcpClient? tcpClient = null;
					NetworkStream? tcpStream = null;
					SerialPort? serialPort = null;
					switch (_scaleConfiguration.Connection)
					{
						case ConnectionType.ETHERNET:
							if (string.IsNullOrWhiteSpace(_scaleConfiguration.IPAddress) || _scaleConfiguration.Port == null)
								continue;
							tcpClient = new TcpClient(_scaleConfiguration.IPAddress ?? string.Empty, _scaleConfiguration.Port ?? -1);
							tcpStream = tcpClient.GetStream();
							break;
						case ConnectionType.SERIAL:
							if (string.IsNullOrWhiteSpace(_scaleConfiguration.PortName) || _scaleConfiguration.BaudRate == null)
								continue;
							serialPort = new SerialPort(_scaleConfiguration.PortName, _scaleConfiguration.BaudRate ?? 9600);

							if (serialPort.IsOpen)
								serialPort.Close();
							serialPort.ReadTimeout = 2000;
							serialPort.WriteTimeout = 2000;
							serialPort.NewLine = _scaleConfiguration.EolCharacter;
							serialPort.Open();
							break;
					}

					try
					{
						string? weight = null;
						switch (_scaleConfiguration.Connection)
						{
							case ConnectionType.ETHERNET:
								if (tcpClient == null || tcpStream == null)
								{
									errorCount++;
									continue;
								}
								StreamReader dataStream = new StreamReader(tcpStream);
								weight = await dataStream.ReadToDelimiterAsync(_scaleConfiguration.EolCharacter);
								break;
							case ConnectionType.SERIAL:
								if (serialPort == null)
								{
									errorCount++;
									continue;
								}
								weight = serialPort.ReadLine();
								break;
						}

						if (string.IsNullOrWhiteSpace(weight))
						{
							errorCount++;
							continue;
						}
						string status = string.Empty;

						int scaleWt = 0;
						int scaleWtDiff = 0;
						switch (_scaleConfiguration.Type)
						{
							case ScaleType.CONDEC:
								weight = weight.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\x02", string.Empty).ToUpper();
								scale.Status = weight.Substring(8);
								scale.GrossWt = scaleWt = int.Parse(weight.Substring(0, 8));
								break;
							case ScaleType.IQ355:
								weight = weight.Replace("\0", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\x02", string.Empty).ToUpper();
								scale.Status = weight.Substring(8);
								scale.GrossWt = scaleWt = int.Parse(weight.Substring(0, 8).Replace(" ", "0"));
								break;
							case ScaleType.MT236:
							case ScaleType.MT780:
								if (weight.Length != 16)
								{
									errorCount++;
									continue;
								}
								status = weight.Substring(1, 2);
								var statusBits = new BitArray(new byte[] { (byte)status[1] });
								bool motion = statusBits[3]; // Bit 3 indicates motion
								bool neg = statusBits[1]; // Bit 1 indicates negative weight
								bool net = statusBits[0]; // Bit 0 indicates Net mode
								weight = weight.Substring(4, 6).Trim();

								if (neg)
									weight = $"-{weight}";

								scale.Status = $"L{(net ? "N" : "G")}{(motion ? "M" : string.Empty)}";
								scale.GrossWt = scaleWt = int.Parse(weight);
								break;
						}
						scaleWtDiff = scaleWt - lastWeight;

						// Begin Axle Weighing Code
						if (resetAxles)
						{
							Console.WriteLine("Resetting Axle Weights");
							resetAxles = false;
							ResetAxleing();
						}
						if (finishAxles)
						{
							Console.WriteLine("Finishing Axle Weights");
							finishAxles = false;
							FinishAxleing();
						}

						if (scaleWt > 2000 && !axleing)
						{
							axleing = true;
							currentAxle = 1;

							lastWeight = 0;
							scale.Axle1Wt = null;
							scale.Axle2Wt = null;
							scale.Axle3Wt = null;
							scale.Axle4Wt = null;

							axleUpdateTimer.Start();
							SetLightStatus(false, false);
						}
						else if (axleUpdateTimer.Enabled && scale.Status.EndsWith("M"))
						{
							axleUpdateTimer.Stop();
							axleUpdateTimer.Start();

							if (scale.GrossWt >= lastWeight + (_scaleConfiguration.AxleingThreshold / 2))
								SetLightStatus(false, false);
						}
						// End Axle Weighing Code

						errorCount = 0;

						if (zeroScale && !string.IsNullOrWhiteSpace(_scaleConfiguration.ZeroCommand))
						{
							Console.WriteLine("Sending Scale Zero Command");
							zeroScale = false;
							await SpamCommand(tcpStream, serialPort, 3, _scaleConfiguration.ZeroCommand);
						}

						if (tareScale && !string.IsNullOrWhiteSpace(_scaleConfiguration.TareCommand))
						{
							Console.WriteLine("Sending Scale Tare Command");
							tareScale = false;
							await SpamCommand(tcpStream, serialPort, 3, _scaleConfiguration.TareCommand);
						}

						if (clearScale && !string.IsNullOrWhiteSpace(_scaleConfiguration.ClearCommand))
						{
							Console.WriteLine("Sending Scale Clear Command");
							clearScale = false;
							await SpamCommand(tcpStream, serialPort, 3, _scaleConfiguration.ClearCommand);
						}
					}
					catch (Exception ex)
					{
						errorCount++;
						Console.WriteLine($"Failed to get weight from scale: {ex.Message}");
					}
					finally
					{
						if (tcpClient != null)
							tcpClient.Close();
						if (serialPort != null)
							serialPort.Close();
					}
				}
				catch (Exception ex)
				{
					errorCount++;
					Console.WriteLine($"Failed to get weight from scale: {ex.Message}");
				}

				if (errorCount > 3)
					SetScaleError("E");

				await Task.Delay(_scaleConfiguration.UpdateRate);
			}
		}

		public async Task SpamCommand(NetworkStream? stream, SerialPort? port, int maxResends, string command)
		{
			int i = 1;
			while (i <= maxResends)
			{
				i++;
				
				if (stream != null)
					await stream.WriteAsync(ASCIIEncoding.ASCII.GetBytes(command));
				else if (port != null)
					port.Write(command);

				if (i < maxResends)
					await Task.Delay(300);
			}
		}

		private void AxleUpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			if (scale.GrossWt < 500)
			{
				ResetAxleing();
				SetLightStatus(false, true);
			}
			else if (scale.GrossWt > lastWeight + _scaleConfiguration.AxleingThreshold)
			{
				lastWeight = scale.GrossWt;

				switch (currentAxle)
				{
					case 1:
						scale.Axle1Wt = scale.GrossWt;

						break;
					case 2:
						scale.Axle2Wt = scale.GrossWt - scale.Axle1Wt;
						break;
					case 3:
						scale.Axle3Wt = scale.GrossWt - scale.Axle1Wt - scale.Axle2Wt;
						break;
					case 4:
						scale.Axle4Wt = scale.GrossWt - scale.Axle1Wt - scale.Axle2Wt - scale.Axle3Wt;
						FinishAxleing();
						break;
				}

				currentAxle++;
				SetLightStatus(false, true);
			}
			else if (scale.GrossWt > lastWeight)
				SetLightStatus(false, false);
		}

		private void ResetAxleing()
		{
			axleUpdateTimer.Stop();
			currentAxle = 1;

			lastWeight = 0;
			scale.Axle1Wt = null;
			scale.Axle2Wt = null;
			scale.Axle3Wt = null;
			scale.Axle4Wt = null;

			axleing = false;
		}

		private void FinishAxleing()
		{
			axleUpdateTimer.Stop();
		}

		private void SetLightStatus(bool redLight, bool grnLight)
		{
			if (_appSettings.IOConfiguration == null || _appSettings.IOConfiguration.RedGpioPin == null || _appSettings.IOConfiguration.GrnGpioPin == null)
				return;

			try
			{
				int redPin = _appSettings.IOConfiguration.RedGpioPin ?? -1;
				int grnPin = _appSettings.IOConfiguration.GrnGpioPin ?? -1;

				using GpioController controller = new GpioController();
				controller.OpenPin(redPin, PinMode.Output);
				controller.OpenPin(grnPin, PinMode.Output);
				bool invertPins = _appSettings.IOConfiguration.InvertGpioPins ?? false;
				var redPinValue = new PinValuePair(redPin, redLight ? PinValue.High : PinValue.Low);
				var grnPinValue = new PinValuePair(grnPin, grnLight ? PinValue.High : PinValue.Low);
				if (invertPins)
				{
					redPinValue = new PinValuePair(redPin, redLight ? PinValue.Low : PinValue.High);
					grnPinValue = new PinValuePair(grnPin, grnLight ? PinValue.Low : PinValue.High);
				}

				controller.Write(
				[
					redPinValue,
					grnPinValue
				]);

				scale.RedIndicatorStatus = redLight;
				scale.GrnIndicatorStatus = grnLight;
			}
			catch (PlatformNotSupportedException ex)
			{
				Console.WriteLine($"Failed to set light status, are you running this on something that isn't a Pi?");
			}
		}
	}

	public struct ScaleType
	{
		public const string CONDEC = "Condec";
		public const string IQ355 = "IQ355";
		public const string MT780 = "MT780";
		public const string MT236 = "MT236";
	}

	public struct ConnectionType
	{
		public const string ETHERNET = "Ethernet";
		public const string SERIAL = "Serial";
	}
}
