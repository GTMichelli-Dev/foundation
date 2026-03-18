using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleController
{
	public static class StreamReaderExtensions
	{
		/// <summary>
		/// Reads characters from the current stream until the delimiter sequence is found and returns the data as a string.
		/// </summary>
		/// <param name="delimiter">The sequence of characters to read to.</param>
		/// <returns>The contents of the input stream up to the delimiter, or <see langword="null"/> if the end of the input stream is reached.</returns>
		/// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than <see cref="int.MaxValue"/>.</exception>
		/// <exception cref="ObjectDisposedException">The stream reader has been disposed.</exception>
		/// <exception cref="InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
		public static string? ReadToDelimiter(this StreamReader self, string delimiter)
		{
			StringBuilder sb = new StringBuilder();
			bool found = false;

			while (!found && !self.EndOfStream)
			{
				for (int i = 0; i < delimiter.Length; i++)
				{
					char c = (char)self.Read();
					sb.Append(c);

					if (c != delimiter[i])
					{
						int io = delimiter.IndexOf(c);
						if (io < i)
							i = io;
						else
							break;
					}

					if (i == delimiter.Length - 1)
					{
						sb.Remove(sb.Length - delimiter.Length, delimiter.Length);
						found = true;
						break;
					}
				}
			}
			if (found == false)
				return null;
			return sb.ToString();
		}

		/// <summary>
		/// Reads characters asynchronously from the current stream until the delimiter sequence is found and returns the data as a string.
		/// </summary>
		/// <param name="delimiter">The sequence of characters to read to.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the <c>TResult</c> parameter contains the contents of the input stream up to the 
		/// delimiter, or <see langword="null"/> if the end of the input stream is reached.</returns>
		/// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than <see cref="int.MaxValue"/>.</exception>
		/// <exception cref="ObjectDisposedException">The stream reader has been disposed.</exception>
		/// <exception cref="InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
		public static async Task<string?> ReadToDelimiterAsync(this StreamReader self, string delimiter)
		{
			StringBuilder sb = new StringBuilder();
			bool found = false;

			while (!found && !self.EndOfStream)
			{
				for (int i = 0; i < delimiter.Length; i++)
				{
					char[] chars = new char[1];
					await self.ReadAsync(chars);
					char c = chars[0];
					sb.Append(c);

					if (c != delimiter[i])
					{
						int io = delimiter.IndexOf(c);
						if (io < i)
							i = io;
						else
							break;
					}

					if (i == delimiter.Length - 1)
					{
						sb.Remove(sb.Length - delimiter.Length, delimiter.Length);
						found = true;
						break;
					}
				}
			}

			if (found == false)
				return null;
			return sb.ToString();
		}
	}
}
