using System.Net;
using System.Text;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ScaleController
{
	internal class Image
	{
		private readonly ApplicationSettings _appSettings;

		public Image(HttpClient client, IOptions<ApplicationSettings> appSettings)
		{
			_appSettings = appSettings.Value;
		}

		public async Task GetPicture(Camera camera, CancellationToken cancellationToken)
		{
			if (_appSettings.CameraConfiguration == null)
				return;

			while (!cancellationToken.IsCancellationRequested)
			{
				try
				{
					string url = string.Empty;
					switch (camera.Type)
					{
						case "Hikvision":
							if (string.IsNullOrWhiteSpace(camera.IPAddress) || camera.Channel == null)
								continue;
							url = $"http://{camera.IPAddress}/ISAPI/Streaming/channels/{camera.Channel}/picture";
							break;
						case "Custom":
							if (string.IsNullOrWhiteSpace(camera.Url))
								continue;
							url = camera.Url;
							break;
					}

					HttpClientHandler handler;
					if (!string.IsNullOrWhiteSpace(camera.Username) && !string.IsNullOrWhiteSpace(camera.Password))
						handler = new HttpClientHandler { Credentials = new NetworkCredential(camera.Username, camera.Password) };
					else
						handler = new HttpClientHandler();

					// Disable HTTPS Certificate Validation
					if (camera.IgnoreSslIssues)
					{
						handler.ClientCertificateOptions = ClientCertificateOption.Manual;
						handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) =>
						{
							return true;
						};
					}

					if (url.EndsWith(".jpeg") || url.EndsWith(".jpg") || url.EndsWith(".png"))
					{
						using (HttpClient client = new HttpClient(handler))
						{
							try
							{
								HttpResponseMessage response = await client.GetAsync(url);
								response.EnsureSuccessStatusCode();

								byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

								if (camera.ShowSuccess)
									Console.WriteLine($"Image received {camera.Name}");

								await CaptureAndSendImageAsync(camera, imageBytes);
							}
							catch (Exception ex)
							{
								Console.WriteLine($"Failed to get image from Camera {camera.Name}: {ex.Message}");
							}
						}
					}
					else
					{
						using (HttpClient client = new HttpClient(handler))
						{
							try
							{
								HttpResponseMessage response = await client.GetAsync(url);

								response.EnsureSuccessStatusCode();

								byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

								if (camera.ShowSuccess) 
									Console.WriteLine($"Image received {camera.Name}");

								await CaptureAndSendImageAsync(camera, imageBytes);
							}
							catch (Exception ex)
							{
								Console.WriteLine($"Failed to get image from Camera {camera.Name}: {ex.Message}");
							}
						}
					}
				}
				catch (HttpRequestException e)
				{
					Console.WriteLine($"Request failed from Camera {camera.Name}: {e.Message}");
				}
				catch (TaskCanceledException e)
				{
					Console.WriteLine($"\nRequest was canceled or timed out for Camera{camera.Name}: {e.Message}");
				}
				await Task.Delay(_appSettings.CameraConfiguration.UpdateRate, cancellationToken);
			}
		}

		public async Task CaptureAndSendImageAsync(Camera camera, byte[] currentImageBytes)
		{
			// Load the current image
			using var currentImage = SixLabors.ImageSharp.Image.Load<Rgb24>(currentImageBytes);

			string apiUrl = $"{_appSettings.ServerUrl}/api/Image/ReceiveImage";
			try
			{
				using var inputStream = new MemoryStream(currentImageBytes);
				using var image = SixLabors.ImageSharp.Image.Load(inputStream);
				if (camera.ImageScale != null)
				{
					double scalingFactor = (camera.ImageScale ?? 100) / 100d;
					int width = (int)Math.Round(image.Width * scalingFactor);
					int height = (int)Math.Round(image.Height * scalingFactor);
					image.Mutate(i => i.Resize(width, height));
				}

				using var outputStream = new MemoryStream();
				image.Save(outputStream, new JpegEncoder());

				var request = new ReceiveImageRequest
				{
					CameraName = camera.Name,
					LocationId = _appSettings.LocationID,
					Image = Convert.ToBase64String(outputStream.ToArray()),
					OK = true,
					Message = string.Empty
				};

				Console.WriteLine($"Sending image Length {request.Image.Length}");

				using var httpClient = new HttpClient();
				var response = await httpClient.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
				if (!response.IsSuccessStatusCode)
					Console.WriteLine($"Failed to send image: {response.StatusCode}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to send image to server {apiUrl}: {ex.Message}");
			}
		}
	}
}
