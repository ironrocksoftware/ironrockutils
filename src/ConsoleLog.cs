
using System;

namespace IronRockUtils
{
	public class ConsoleLog
	{
		public static string logFolder = AppDomain.CurrentDomain.BaseDirectory;
		public static string outFile = null;
		public static bool dualOutput = false;

		public static string TimeStamp()
		{
			DateTime T = DateTime.Now;
			return T.Year+"-"+T.Month+"-"+T.Day+"_"+T.Hour+T.Minute+T.Second;
		}

		public static void WriteLineToFile (string file, string str)
		{
			System.IO.File.AppendAllText(logFolder + file, str + "\r\n");
		}

		public static void SetOut (string filename)
		{
			outFile = filename;
		}

		public static void WriteLine (string line)
		{
			if (outFile != null)
				WriteLineToFile (outFile, line);

			if (outFile == null || (outFile != null && dualOutput))
				Console.WriteLine (line);
		}
	}
}
