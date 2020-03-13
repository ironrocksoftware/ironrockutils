﻿
using System;
using System.Collections;
using System.Threading;
using System.Web;
using System.IO;
using System.Configuration;
using System.Diagnostics;




namespace IronRockUtils.Ipc
{
	/// <summary>
	/// Description of PipeManager.
	/// </summary>
	public sealed class PipeManager : IChannelManager
	{
		public Hashtable Pipes;

		private uint NumberPipes = 8;
		private uint OutBuffer = 512;
		private uint InBuffer = 512;
		private int MAX_READ_BYTES = 1048576;
		private bool _listen = true;

		public bool Listen {
			get {
				return _listen;
			}
			set {
				_listen=value;
			}
		}

		private int numChannels = 0;
		private Hashtable _pipes = new Hashtable();
		private Thread MainThread;
		private string PipeName;
		private ManualResetEvent Mre;
		private const int PIPE_MAX_STUFFED_TIME = 5000;

		public object SyncRoot = new object();
		public delegate string RequestHandler (string request);
		public RequestHandler handler;

		public PipeManager (string PipeName, RequestHandler handler)
		{
			this.PipeName = PipeName;
			this.handler = handler;
		}

		public void Initialize()
		{
			Pipes = Hashtable.Synchronized(_pipes);
			Mre = new ManualResetEvent(false);
			MainThread = new Thread(new ThreadStart(Start));
			MainThread.IsBackground = true;
			MainThread.Name = PipeName + "_Thread";
			MainThread.Start();
			Thread.Sleep(1000);
		}

		public Thread GetThread ()
		{
			return MainThread;
		}

		public string HandleRequest(string request)
		{
			return this.handler (request);
		}

		private void Start()
		{
			try
			{
				while (_listen)
				{
					int[] keys = new int[Pipes.Keys.Count];
					Pipes.Keys.CopyTo(keys,0);

					foreach (int key in keys) {
						ServerNamedPipe serverPipe = (ServerNamedPipe)Pipes[key];
						if (serverPipe != null && DateTime.Now.Subtract(serverPipe.LastAction).Milliseconds > PIPE_MAX_STUFFED_TIME && serverPipe.PipeConnection.GetState() != InterProcessConnectionState.WaitingForClient)
						{
							serverPipe.Listen = false;
							serverPipe.PipeThread.Abort();
							RemoveServerChannel(serverPipe.PipeConnection.NativeHandle);
						}
					}

					if (numChannels <= NumberPipes)
					{
						ServerNamedPipe pipe = new ServerNamedPipe(this, PipeName, OutBuffer, InBuffer, MAX_READ_BYTES, false);
						try {
							pipe.Connect();
							pipe.LastAction = DateTime.Now;
							System.Threading.Interlocked.Increment(ref numChannels);
							pipe.Start();
							Pipes.Add(pipe.PipeConnection.NativeHandle, pipe);
						}
						catch (InterProcessIOException ex) {
							Log.write(ex.Message);
							RemoveServerChannel(pipe.PipeConnection.NativeHandle);
							pipe.Dispose();
						}
					}
					else {
						Mre.Reset();
						Mre.WaitOne(1000, false);
					}
				}
			}
			catch {
				// Log exception
			}
		}

		public void Stop()
		{
			_listen = false;
			Mre.Set();

			try
			{
				int[] keys = new int[Pipes.Keys.Count];
				Pipes.Keys.CopyTo(keys,0);
				foreach (int key in keys) {
					((ServerNamedPipe)Pipes[key]).Listen = false;
				}
				int i = numChannels * 3;
				for (int j = 0; j < i; j++) {
					StopServerPipe();
				}
				Pipes.Clear();
				Mre.Close();
				Mre = null;
			} 
			catch {	
				// Log exception
			}
		}

		public void WakeUp()
		{
			if (Mre != null) {
				Mre.Set();
			}
		}

		private void StopServerPipe()
		{
			try {
				ClientPipeConnection pipe = new ClientPipeConnection(PipeName);
				if (pipe.TryConnect()) {
					pipe.Close();
				}
			} catch {	
				// Log exception
			}
		}

		public void RemoveServerChannel(object param)
		{
			int handle = (int)param;
			System.Threading.Interlocked.Decrement(ref numChannels);
			Pipes.Remove(handle);
			this.WakeUp();
		}
	}
}
