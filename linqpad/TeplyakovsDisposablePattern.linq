<Query Kind="Statements">
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>Microsoft.Win32.SafeHandles</Namespace>
</Query>

// Define other methods and classes here
public class ProperComplexResourceHolder : IDisposable {
	//unmanaged resource
	private IntPtr _buffer;
	
	//managed resource
	private SafeHandle _handle;

	public ProperComplexResourceHolder() {
		_buffer = AllocateBuffer();
		_handle = new SafeWaitHandle(IntPtr.Zero, true);
	}

	public void Dispose() {
		DisposeNativeResources();
		DisposeManagedResources();
		GC.SuppressFinalize(this);
	}

	~ProperComplexResourceHolder() {
		DisposeNativeResources();
	}

	protected virtual void DisposeNativeResources() {
		ReleaseBuffer(_buffer);
	}

	protected virtual void DisposeManagedResources() {
		_handle?.Dispose();
	}

	private IntPtr AllocateBuffer() {
	//...
		throw new NotImplementedException();
	}

	private void ReleaseBuffer(IntPtr buffer) {
	//...
	}
}