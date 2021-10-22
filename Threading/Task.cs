using System;

namespace DNA.Threading 
{
	public abstract class Task 
	{
		private TaskStatus _status;
		private Exception _exception;

		public TaskStatus Status 
		{
			get => 
				this._status;
			
			protected set => 
				this._status = value;
		}

		public Exception Exception
		{
			get => 
				this._exception;
			
			protected set => 
				this._exception = value;
		}

		private bool Failed => 
			this.Status == TaskStatus.Failed;

		private bool Completed => 
			this.Status == TaskStatus.Failed || this.Status == TaskStatus.Compelete;
	}
}
