using System.IO;

namespace AltiumSort
{
	internal class Splitter
	{
		private readonly string _outDirectory;


		private int _chunkNumber;


		internal Splitter(string outDirectory)
		{
			_chunkNumber = 0;
			_outDirectory = outDirectory;
		}


		internal void Split(TextReader reader)
		{
			var linesCount = 0;

			string? line;

			Directory.CreateDirectory(_outDirectory);

			var streamWriter = new StreamWriter(GetChunkFileName());

			while ((line = reader.ReadLine()) != null)
			{
				++linesCount;
				streamWriter.WriteLine(line);

				if (linesCount > 4096 * 512 && reader.Peek() >= 0)
				{
					streamWriter.Dispose();
					streamWriter = new StreamWriter(GetChunkFileName());

					linesCount = 0;
				}
			}

			streamWriter.Dispose();
		}


		private string GetChunkFileName()
		{
			return $"{_outDirectory}/{Utils.CHUNK_FILE_PREFIX}{_chunkNumber++}.txt";
		}
	}
}