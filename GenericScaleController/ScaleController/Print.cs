using System.Diagnostics;

namespace ScaleController
{
	public static class Print
	{
		public static void ConvertBase64ToPDF(string base64EncodedPDF, string outputPath)
		{
			try
			{
				// Convert Base64 string to byte[]
				byte[] fileBytes = Convert.FromBase64String(base64EncodedPDF);

				// Write the bytes to a file
				File.WriteAllBytes(outputPath, fileBytes);

				Console.WriteLine("PDF file saved to: " + outputPath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error converting Base64 to PDF: " + ex.Message);
			}
		}

		/// <summary>
		/// Prints a specified number of copies of a PDF file using the CUPS system.
		/// </summary>
		/// <param name="printerName">The name of the printer.</param>
		/// <param name="TicketAsBase64">The path to the PDF file to print.</param>
		/// <param name="paperSize"></param>
		/// <param name="copies">The number of copies to print.</param>
		public static void PrintDocument(string? printerName, string TicketAsBase64, string paperSize, int copies)
		{
			if (string.IsNullOrWhiteSpace(printerName))
				return;

			string tempDirectory = Path.GetTempPath(); // Gets the system's temporary directory
			string tempFileName = Path.GetRandomFileName(); // Generates a unique file name
			string filePath = Path.ChangeExtension(Path.Combine(tempDirectory, tempFileName), ".pdf"); // Combines them with a .pdf extension
			try
			{
				ConvertBase64ToPDF(TicketAsBase64, filePath);
				var options = (string.IsNullOrEmpty(paperSize)) ? "" : $"-o media={paperSize}";

				// Construct the command to send the print job
				string command = $"lp -d {printerName} {options} -n{copies} \"{filePath}\"";
				Console.WriteLine($"Command: {command}");

				// Setup the process with the ProcessStartInfo class
				ProcessStartInfo startInfo = new ProcessStartInfo()
				{
					FileName = "/bin/bash",
					Arguments = $"-c \"{command}\"",
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				Process process = new Process() { StartInfo = startInfo };

				// Start the process
				process.Start();

				// Read the output (which includes any command execution messages)
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();

				// Wait for the command to finish executing
				process.WaitForExit();

				if (!string.IsNullOrEmpty(output))
					Console.WriteLine($"Output: {output}");

				if (!string.IsNullOrEmpty(error))
					Console.WriteLine($"Error: {error}");
			}
			catch (Exception e)
			{
				Console.WriteLine($"An exception occurred: {e.Message}");
			}
			finally
			{
				try
				{
					File.Delete(filePath);
				}
				catch (Exception e2)
				{
					Console.WriteLine($"Error Deleting File {filePath} " + e2.Message);
				}
			}
		}

		public static class PrinterConfiguration
		{
			public static List<string> GetSupportedPaperSizes(string printerName)
			{
				List<string> paperSizes = new List<string>();

				try
				{
					// Prepare the command to execute
					string command = $"lpoptions -p {printerName} -l";

					// Setup the process with the ProcessStartInfo class
					ProcessStartInfo startInfo = new ProcessStartInfo()
					{
						FileName = "/bin/bash",
						Arguments = $"-c \"{command}\"",
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true,
					};

					Process process = new Process() { StartInfo = startInfo };

					// Start the process
					process.Start();

					// Read the output from the command
					while (!process.StandardOutput.EndOfStream)
					{
						string line = process.StandardOutput.ReadLine() ?? "";

						// Look for the line that starts with "*Media" or "Media" which lists the paper sizes
						if (line.StartsWith("Media") || line.StartsWith("*Media"))
						{
							// Example output: Media/Media Size: Letter A4 *Legal...
							// Split the line by spaces and take each value as a paper size
							string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

							// Skip the first two parts ("Media/Media" and "Size:") and iterate over the remaining parts
							for (int i = 2; i < parts.Length; i++)
							{
								string paperSize = parts[i].Trim();

								// Some paper size names might be prefixed with '*', indicating the default size. Remove the '*'.
								if (paperSize.StartsWith("*"))
									paperSize = paperSize.Substring(1);

								paperSizes.Add(paperSize);
							}

							// Since we've found the line we were looking for, we can break out of the loop
							break;
						}
					}

					// Wait for the command to finish executing
					process.WaitForExit();
				}
				catch (Exception e)
				{
					Console.WriteLine($"An error occurred: {e.Message}");
				}

				return paperSizes;
			}

			public static void SetPrinterPaperSize(string printerName, string paperSize)
			{
				try
				{
					// Prepare the command to execute
					string command = $"/usr/sbin/lpadmin -p {printerName} -o media={paperSize}";

					// Setup the process with the ProcessStartInfo class
					ProcessStartInfo startInfo = new ProcessStartInfo()
					{
						FileName = "/bin/bash",
						Arguments = $"-c \"{command}\"",
						RedirectStandardOutput = true,
						UseShellExecute = false,
						CreateNoWindow = true,
					};

					Process process = new Process() { StartInfo = startInfo };

					// Start the process
					process.Start();

					// Wait for the command to finish executing
					process.WaitForExit();

					// Check the exit code. If 0, the command was successful
					if (process.ExitCode == 0)
						Console.WriteLine($"Successfully set paper size to {paperSize} for printer {printerName}.");
					else
						Console.WriteLine($"Failed to set paper size for printer {printerName}. Exit code: {process.ExitCode}");
				}
				catch (Exception e)
				{
					Console.WriteLine($"An error occurred: {e.Message}");
				}
			}
		}
	}
}
