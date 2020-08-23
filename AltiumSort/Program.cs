using System;
using System.Diagnostics;
using System.IO;

namespace AltiumSort
{
	public static class Program
	{
		private static readonly string _tmpDirectory = Utils.GetGuidString();


		private static int Main()
		{
			string inputFilePath = GetInputFilePath();
			using var streamReader = new StreamReader(inputFilePath);

			try
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();

				var splitter = new Splitter(_tmpDirectory);
				splitter.Split(streamReader);

				long splitElapsedMs = stopwatch.ElapsedMilliseconds;
				PrintElapsed("Split elapsed", splitElapsedMs);

				var sorter = new Sorter(_tmpDirectory);
				sorter.Sort();

				long sortElapsedMs = stopwatch.ElapsedMilliseconds - splitElapsedMs;
				PrintElapsed("Sort elapsed", sortElapsedMs);

				var merger = new HeapMerger(_tmpDirectory);
				merger.Merge();

				long mergeElapsedMs = stopwatch.ElapsedMilliseconds - sortElapsedMs - splitElapsedMs;
				PrintElapsed("Merge elapsed", mergeElapsedMs);

				stopwatch.Stop();
				PrintElapsed("Total elapsed", stopwatch.ElapsedMilliseconds);
			}
			finally
			{
				Directory.Delete(_tmpDirectory, true);
			}

			return 0;
		}


		private static string GetInputFilePath()
		{
			Console.Write("Enter file path: ");

			while (true)
			{
				string input = Console.ReadLine();

				if (File.Exists(input))
				{
					return input;
				}

				Console.WriteLine($"File {input} doesn't exist!");
			}
		}

		private static void PrintElapsed(string message, long elapsedMs)
		{
			Console.WriteLine($"{message}: {elapsedMs.ToString().PadLeft(10)}(ms)");
		}
	}
}