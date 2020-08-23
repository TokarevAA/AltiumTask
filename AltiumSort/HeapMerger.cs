using System.Collections.Generic;
using System.IO;

namespace AltiumSort
{
	internal class HeapMerger
	{
		private readonly int _chunksCount;

		private readonly SortedList<string, (int ChunkIndex, int Count)> _heap;
		private readonly StreamReader[] _streamReaders;


		internal HeapMerger(string directoryPath)
		{
			string[] paths = Directory.GetFiles(directoryPath);

			_chunksCount = paths.Length;
			_streamReaders = new StreamReader[_chunksCount];
			_heap = new SortedList<string, (int ChunkIndex, int Count)>(LineComparer.Instance);

			for (var i = 0; i < _chunksCount; i++)
			{
				_streamReaders[i] = new StreamReader(paths[i]);
			}
		}


		internal void Merge()
		{
			using var streamWriter = new StreamWriter("sorted.txt");

			try
			{
				// Initial heap fill
				for (var i = 0; i < _chunksCount; i++)
				{
					Read(i);
				}

				// Refilling heap
				do
				{
					(int chunkIndex, int count) = _heap.Values[0];

					for (var i = 0; i < count; i++)
					{
						streamWriter.WriteLine(_heap.Keys[0]);
					}

					_heap.RemoveAt(0);
					Read(chunkIndex);
				} while (_heap.Count > 0);
			}
			finally
			{
				foreach (StreamReader streamReader in _streamReaders)
				{
					streamReader.Dispose();
				}
			}
		}


		private void Read(int readerIndex)
		{
			StreamReader reader = _streamReaders[readerIndex];
			string? rawLine = reader.ReadLine();

			if (rawLine != null)
			{
				string[] blocks = rawLine.Split(':');

				int count = int.Parse(blocks[0]);
				string line = blocks[1];

				_heap.Add(line, (readerIndex, count));
			}
		}
	}
}