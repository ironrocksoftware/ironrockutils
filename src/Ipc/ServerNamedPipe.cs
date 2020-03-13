
using System;
using System.Threading;
using System.IO;

namespace IronRockUtils.Ipc
{
	public sealed class ServerNamedPipe : IDisposable
	{
		internal Thread PipeThread;
		internal ServerPipeConnection PipeConnection;
		internal bool Listen = true;
		internal DateTime LastAction;
		private bool disposed = false;
		private PipeManager manager;

		private void PipeListener()
		{
			CheckIfDisposed();
			try
			{
				Listen = manager.Listen;

				while (Listen)
				{
					LastAction = DateTime.Now;
					string request = PipeConnection.Read();
					LastAction = DateTime.Now;
					if (request.Trim() != "") {
						PipeConnection.Write(manager.HandleRequest(request));
					}

					PipeConnection.Disconnect();
					if (Listen) Connect();
					manager.WakeUp();

					LastAction = DateTime.Now;
				}
			} 
			catch (System.Threading.ThreadAbortException ex) { ex = ex != null ? ex : null; }
			catch (System.Threading.ThreadStateException ex) { ex = ex != null ? ex : null; }
			catch (Exception ex) { 
				Log.write(ex.ToString());
			}
			finally {
				this.Close();
			}
		}

		internal void Connect() {
			CheckIfDisposed();
			PipeConnection.Connect();
		}

		internal void Close() {
			CheckIfDisposed();
			this.Listen = false;
			manager.RemoveServerChannel(this.PipeConnection.NativeHandle);
			this.Dispose();
		}

		internal void Start() {
			CheckIfDisposed();
			PipeThread.Start();
		}

		private void CheckIfDisposed() {
			if(this.disposed) {
				throw new ObjectDisposedException("ServerNamedPipe");
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if(!this.disposed) {
				PipeConnection.Dispose();
				if (PipeThread != null) {
					try {
						PipeThread.Abort();
					} 
					catch (System.Threading.ThreadAbortException ex) { ex = ex != null ? ex : null; }
					catch (System.Threading.ThreadStateException ex) { ex = ex != null ? ex : null; }
					catch (Exception ex) {
						Log.write(ex.ToString());
					}
				}
			}
			disposed = true;         
		}

		~ServerNamedPipe() {
			Dispose(false);
		}

		internal ServerNamedPipe(PipeManager manager, string name, uint outBuffer, uint inBuffer, int maxReadBytes, bool secure)
		{
			this.manager = manager;
			PipeConnection = new ServerPipeConnection(name, outBuffer, inBuffer, maxReadBytes, secure);
			PipeThread = new Thread(new ThreadStart(PipeListener));
			PipeThread.IsBackground = true;
			PipeThread.Name = "Pipe Thread " + this.PipeConnection.NativeHandle.ToString();
			LastAction = DateTime.Now;
		}
	};
}
