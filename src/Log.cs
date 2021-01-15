
using System;
using System.Windows.Forms;

namespace IronRockUtils
{
	public class Log
	{
		public static string logFolder = AppDomain.CurrentDomain.BaseDirectory;
		public static bool stdout = false;

		public static string TimeStamp()
		{
			DateTime T = DateTime.Now;
			return T.Year+"-"+T.Month+"-"+T.Day+"_"+T.Hour+T.Minute+T.Second;
		}

		public static void write (string str)
		{
			if (stdout)
			{
				Console.WriteLine(str);
				return;
			}

			try {
				System.IO.File.AppendAllText(logFolder + "log.txt", TimeStamp() + ": " + str + "\r\n");
			}
			catch (Exception e) {
				try {
					System.IO.File.AppendAllText(logFolder + "log_"+TimeStamp()+".txt", TimeStamp() + ": " + str + "\r\n");
				}
				catch (Exception e1) {
				}
			}
		}

		public static void write (string file, string str)
		{
			try {
				System.IO.File.AppendAllText(logFolder + file + ".txt", TimeStamp() + ": " + str + "\r\n");
			}
			catch (Exception e) {
				try {
					System.IO.File.AppendAllText(logFolder + file + "_" + TimeStamp() + ".txt", TimeStamp() + ": " + str + "\r\n");
				}
				catch (Exception e1) {
				}
			}
		}
	}
}
