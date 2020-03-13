
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace IronRockUtils
{
	public class Utils
	{
		public static MD5 md5 = null;

		// Calculates an MD5 hash from the given data and returns it.
		public static string getMd5 (string data)
		{
			if (md5 == null) md5 = MD5.Create();

			return BitConverter.ToString(md5.ComputeHash(Encoding.GetEncoding(1252).GetBytes(data))).Replace("-", "").ToLower();
		}

		// Waits for a file to be available (not locked).
		public static bool waitFile (string fullPath, int seconds)
		{
			int tries = seconds*1000 / 250;

			while (tries-- > 0)
			{
				try
				{
		            FileStream fs = new FileStream (fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		            fs.ReadByte ();
		            fs.Close();
		            return true;
		        }
		        catch (IOException) {
					System.Threading.Thread.Sleep (250);
		        }
		    }

		    return false;
		}
		
		public static string Run (string filename, string arguments)
		{
			using (Process process = new Process())
			{
				process.StartInfo.FileName = filename;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
			
				StringBuilder output = new StringBuilder();
			
				using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
				{
					process.OutputDataReceived += (sender, e) =>
					{
						if (e.Data == null)
							outputWaitHandle.Set();
						else
							output.AppendLine(e.Data);
					};

					process.Start();

					process.BeginOutputReadLine();
					process.WaitForExit();
					outputWaitHandle.WaitOne();

					return output.ToString();
				}
			}
		}
	}
}
