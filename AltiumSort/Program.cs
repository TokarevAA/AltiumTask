using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AltiumSort
{
	public class Program
	{
		private const string _CHUNK_FILE_NAME = "chunk";


		private static readonly string _tmpDirectory = Guid.NewGuid().ToString("N");


		private static int Main()
		{
			string path = GetFilePath();

			using var streamReader = new StreamReader(path);

			try
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();

				Split(streamReader);

				long splitElapsedMs = stopwatch.ElapsedMilliseconds;
				PrintElapsed("Split elapsed", splitElapsedMs);

				Sort();

				long sortElapsedMs = stopwatch.ElapsedMilliseconds - splitElapsedMs;
				PrintElapsed("Sort elapsed", sortElapsedMs);

				HeapMerge();

				long mergeElapsedMs = stopwatch.ElapsedMilliseconds - sortElapsedMs - splitElapsedMs;
				PrintElapsed("Merge elapsed", mergeElapsedMs);

				stopwatch.Stop();
				PrintElapsed("Total elapsed", stopwatch.ElapsedMilliseconds);
			}
			finally
			{
				string[] tmpPaths = Directory.GetFiles(_tmpDirectory);

				foreach (string tmpPath in tmpPaths)
				{
					File.Delete(tmpPath);
				}

				Directory.Delete(_tmpDirectory);
			}

			return 0;
		}


		private static string GetFilePath()
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

		private static void Split(TextReader reader)
		{
			var number = 0;
			var linesCount = 0;

			string line;

			Directory.CreateDirectory(_tmpDirectory);

			var streamWriter = new StreamWriter(GetChunkFileName(number++));

			while ((line = reader.ReadLine()) != null)
			{
				++linesCount;
				streamWriter.WriteLine(line);

				if (linesCount > 4096 * 512 && reader.Peek() >= 0)
				{
					streamWriter.Dispose();
					streamWriter = new StreamWriter(GetChunkFileName(number++));

					linesCount = 0;
				}
			}

			streamWriter.Dispose();
		}

		private static string GetChunkFileName(int number)
		{
			return $"{_tmpDirectory}/{_CHUNK_FILE_NAME}{number}.txt";
		}

		private static void Sort()
		{
			string[] paths = Directory.GetFiles(_tmpDirectory);

			Parallel.ForEach(
				paths,
				path =>
				{
					string[] contents = File.ReadAllLines(path);
					Array.Sort(contents, LineComparer.Instance);

					string newPath = path.Replace(_CHUNK_FILE_NAME, "sorted");
					File.WriteAllLines(newPath, contents);
					File.Delete(path);
				});
		}

		private static void HeapMerge()
		{
			string[] paths = Directory.GetFiles(_tmpDirectory);
			int chunks = paths.Length;

			var streamReaders = new StreamReader[chunks];
			using var streamWriter = new StreamWriter("sorted.txt");

			try
			{
				for (var i = 0; i < chunks; i++)
				{
					streamReaders[i] = new StreamReader(paths[i]);
				}

				var heap = new SortedList<string, int>(LineComparer.Instance);

				// Initial heap fill
				for (var i = 0; i < chunks; i++)
				{
					string line = streamReaders[i].ReadLine();

					if (line != null)
					{
						heap.Add(line, i);
					}
				}

				// Refilling heap
				do
				{
					int chunkIndex = heap.Values[0];

					streamWriter.WriteLine(heap.Keys[0]);
					heap.RemoveAt(0);

					string line = streamReaders[chunkIndex].ReadLine();

					if (line != null)
					{
						heap.Add(line, chunkIndex);
					}
				} while (heap.Count > 0);
			}
			finally
			{
				foreach (StreamReader streamReader in streamReaders)
				{
					streamReader.Dispose();
				}
			}
		}
	}
}