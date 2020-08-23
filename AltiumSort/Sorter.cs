using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AltiumSort
{
	internal class Sorter
	{
		private readonly string _tmpDirectory;


		internal Sorter(string tmpDirectory)
		{
			_tmpDirectory = tmpDirectory;
		}


		internal void Sort()
		{
			string[] paths = Directory.GetFiles(_tmpDirectory);

			Parallel.ForEach(
				paths,
				path =>
				{
					using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
					{
						using var streamReader = new StreamReader(fileStream);

						IEnumerable<(string, int)> entries = CountingSort(streamReader);
						var newPath = path.Replace(Utils.CHUNK_FILE_PREFIX, "sorted");

						using var streamWriter = new StreamWriter(newPath);

						foreach ((string key, int count) in entries)
						{
							streamWriter.Write(count);
							streamWriter.Write(':');
							streamWriter.WriteLine(key);
						}
					}

					File.Delete(path);
				});
		}


		private IEnumerable<(string, int)> CountingSort(TextReader reader)
		{
			var entriesByKey = new Dictionary<string, int>();

			string? line;

			while ((line = reader.ReadLine()) != null)
			{
				if (!entriesByKey.TryGetValue(line, out _))
				{
					entriesByKey[line] = 0;
				}

				++entriesByKey[line];
			}

			string[] sortedKeys = entriesByKey.Keys.ToArray();
			Array.Sort(sortedKeys, LineComparer.Instance);

			foreach (string key in sortedKeys)
			{
				yield return (key, entriesByKey[key]);
			}
		}
	}
}