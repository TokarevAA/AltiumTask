using System;
using System.Diagnostics;
using System.IO;

namespace AltiumGen
{
	internal static class Program
	{
		private static readonly string[] _words =
		{
			"apple",
			"something",
			"something something something",
			"cherry is the best",
			"banana is yellow",
			"but tomato not yellow",
			"it's actually a berry btw",
			"anyway someone totally not"
		};


		private static int Main()
		{
			long totalBytes = GetMegabytesCount() * 1024 * 1024;
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			GenerateFile(totalBytes);
			stopwatch.Stop();

			Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}(ms)");

			return 0;
		}


		private static void GenerateFile(long totalBytes)
		{
			long writtenBytes = 0;
			var random = new Random();

			using var fileStream = new FileStream("out.txt", FileMode.Create, FileAccess.Write);
			using var streamWriter = new StreamWriter(fileStream);

			while (writtenBytes < totalBytes)
			{
				int number = random.Next(short.MaxValue / 2);

				string word = _words[random.Next(_words.Length)];
				string line = string.Concat(number, ". ", word);

				streamWriter.WriteLine(line);
				writtenBytes += line.Length + 2;
			}
		}

		private static long GetMegabytesCount()
		{
			Console.Write("Enter file size (megabytes): ");

			while (true)
			{
				string input = Console.ReadLine();

				if (long.TryParse(input, out long result) && result >= 1)
				{
					return result;
				}

				Console.WriteLine("Only integer values greater than 0!");
			}
		}
	}
}