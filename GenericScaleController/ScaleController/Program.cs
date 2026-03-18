using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Serilog;
using Serilog.Extensions.Logging;
using System.Threading.Tasks;

namespace ScaleController
{
	public class Program
	{
		static async Task Main(string[] args)
		{
			var builder = new HostBuilder()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.SetBasePath(Directory.GetCurrentDirectory());
					config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
					config.AddEnvironmentVariables();
				})
				.ConfigureServices((hostingContext, services) =>
				{
					services.Configure<ApplicationSettings>(hostingContext.Configuration.GetSection("ApplicationSettings"));
					services.AddHttpClient();
					services.AddTransient<Image>();
					services.AddTransient<ScaleConnection>();
					services.AddLogging(configure => configure.AddConsole());
					services.AddSingleton<ILoggerFactory>(services =>
					{
						var logger = new LoggerConfiguration()
							.ReadFrom.Configuration(hostingContext.Configuration)
							.CreateLogger();
						return new SerilogLoggerFactory(logger, true);
					});
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
					logging.AddConsole();
				});

			var host = builder.Build();
			var cameraService = host.Services.GetRequiredService<Image>();
			var scaleService = host.Services.GetRequiredService<ScaleConnection>();
			var applicationSettings = host.Services.GetRequiredService<IOptions<ApplicationSettings>>().Value;
			var cts = new CancellationTokenSource();

			var tasks = new List<Task>();
			if (applicationSettings.CameraConfiguration != null)
			{
				foreach (var camera in applicationSettings.CameraConfiguration.Cameras)
					tasks.Add(Task.Run(() => cameraService.GetPicture(camera, cts.Token)));
			}

			if (applicationSettings.ScaleConfiguration != null)
			{
				tasks.AddRange(
				[
					Task.Run(() => scaleService.GetWeight(cts.Token)),
					Task.Run(() => scaleService.SendWeightToServer(cts.Token))
				]);

				await Task.WhenAll(tasks);
			}

			await host.RunAsync();
		}
	}
}
