using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DNA.Threading
{
	public class TaskScheduler
	{
		private class ScheduledTask : Task
		{
			private ParameterizedThreadStart _paramCallback;
			private ThreadStart _callback;
			private object _state;

			public ScheduledTask(ParameterizedThreadStart callback, object state)
			{
				this._paramCallback = callback;
				this._state = state;
			}

			public ScheduledTask(ThreadStart callback) => 
				this._callback = callback;

			public bool DoWork()
			{
				base.Status = TaskStatus.InProcess;
				
				if (Debugger.IsAttached)
				{
					if (this._paramCallback == null)
					{
						this._callback();
					}
					else
					{
						this._paramCallback(this._state);
					}
				}
				else
				{
					try
					{
						if (this._paramCallback == null)
						{
							this._callback();
						}
						else
						{
							this._paramCallback(this._state);
						}
					}
					catch (Exception exception)
					{
						base.Exception = exception;
						base.Status = TaskStatus.Failed;
						return false;
					}
				}
				
				base.Status = TaskStatus.Compelete;
				return true;
			}
		}

		public class ExceptionEventArgs : EventArgs
		{
			public Exception InnerException;

			public ExceptionEventArgs(Exception e) => 
				this.InnerException = e;
		}

		private Queue<TaskScheduler.ScheduledTask> _taskQueue = 
			new Queue<TaskScheduler.ScheduledTask>();
		
		private Thread _queueWorkerThread;
		private bool _runThread = true;
		private AutoResetEvent _event = new AutoResetEvent(false);

		public void Exit()
		{
			// We don't want to exit a thread while it's running.
			if (!this.ThreadRunning)
			{
				return;
			}

			Thread queueWorkerThread = this._queueWorkerThread;
			this._runThread = false;
			this._event.Set();

			if (Thread.CurrentThread == queueWorkerThread)
			{
				return;
			}

			queueWorkerThread.Join();
		}

		private void ExecutionThread(object state)
		{
			TaskScheduler.ScheduledTask scheduledTask = (TaskScheduler.ScheduledTask)state;
			
			if (scheduledTask.DoWork() || this.ThreadException == null)
			{
				return;
			}

			this.ThreadException((object)this, 
				new TaskScheduler.ExceptionEventArgs(scheduledTask.Exception));
		}

		private void StartWorkerQueue()
		{
			if (this._queueWorkerThread == null)
			{
				this._queueWorkerThread = new Thread(new ThreadStart(this.QueueWorker));
				this._queueWorkerThread.Name = "TaskSchedulerQueueWorker";
				this._queueWorkerThread.IsBackground = true;
				this._queueWorkerThread.Priority = ThreadPriority.BelowNormal;
				this._queueWorkerThread.Start();
			}
			
			this._event.Set();
		}

		public bool ThreadRunning => 
			this._queueWorkerThread != null && this._runThread;

		private void QueueWorker()
		{
			while (this._runThread)
			{
				this._event.WaitOne();
				
				while (this._taskQueue.Count > 0)
				{
					TaskScheduler.ScheduledTask scheduledTask;
				
					lock (this._taskQueue)
					{
						scheduledTask = this._taskQueue.Dequeue();
					}
				
					this.ExecutionThread((object)scheduledTask);
				}
			}
			
			this._queueWorkerThread = (Thread)null;
			this._runThread = true;
		}

		public event EventHandler<TaskScheduler.ExceptionEventArgs> ThreadException;

		public Task QueueUserWorkItem(ThreadStart callBack)
		{
			lock (this._taskQueue)
			{
				TaskScheduler.ScheduledTask scheduledTask = 
					new TaskScheduler.ScheduledTask(callBack);
				
				this._taskQueue.Enqueue(scheduledTask);
				this.StartWorkerQueue();
				return (Task)scheduledTask;
			}
		}

		public Task QueueUserWorkItem(ParameterizedThreadStart callBack, object state)
		{
			lock (this._taskQueue)
			{
				TaskScheduler.ScheduledTask scheduledTask = 
					new TaskScheduler.ScheduledTask(callBack, state);
				
				this._taskQueue.Enqueue(scheduledTask);
				this.StartWorkerQueue();
				return (Task)scheduledTask;
			}
		}

		public Task DoUserWorkItem(ThreadStart callBack)
		{
			TaskScheduler.ScheduledTask scheduledTask = 
				new TaskScheduler.ScheduledTask(callBack);
			
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecutionThread), 
										 (object)scheduledTask);
			
			return (Task)scheduledTask;
		}

		public Task DoUserWorkItem(ParameterizedThreadStart callBack, object state)
		{
			TaskScheduler.ScheduledTask scheduledTask = 
				new TaskScheduler.ScheduledTask(callBack, state);
			
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecutionThread), 
										 (object)scheduledTask);
			
			return (Task)scheduledTask;
		}
	}
}
