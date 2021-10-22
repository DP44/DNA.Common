using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	public abstract class MUSaveDevice : SaveDevice, IGameComponent, IUpdateable
	{
		private static string promptForCancelledMessage;

		private static string forceCancelledReselectionMessage;

		private static string promptForDisconnectedMessage;

		private static string forceDisconnectedReselectionMessage;

		private static string deviceRequiredTitle;

		private static string deviceOptionalTitle;

		private static readonly string[] deviceOptionalOptions = new string[2];

		private static readonly string[] deviceRequiredOptions = new string[1];

		public bool ForceDeviceSelection;

		public bool PromptForReselect;

		private string _containerName;

		private int updateOrder;

		private bool enabled = true;

		private bool deviceWasConnected;

		private SaveDevicePromptState state;

		private readonly SaveDevicePromptEventArgs promptEventArgs = new SaveDevicePromptEventArgs();

		private readonly SaveDeviceEventArgs eventArgs = new SaveDeviceEventArgs();

		private StorageDevice storageDevice;

		private SuccessCallback _promptCallback;

		private StorageContainer _currentContainer;

		public static string PromptForCancelledMessage
		{
			get
			{
				return MUSaveDevice.promptForCancelledMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.promptForCancelledMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string ForceCancelledReselectionMessage
		{
			get
			{
				return MUSaveDevice.forceCancelledReselectionMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.forceCancelledReselectionMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string PromptForDisconnectedMessage
		{
			get
			{
				return MUSaveDevice.promptForDisconnectedMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.promptForDisconnectedMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string ForceDisconnectedReselectionMessage
		{
			get
			{
				return MUSaveDevice.forceDisconnectedReselectionMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.forceDisconnectedReselectionMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string DeviceRequiredTitle
		{
			get
			{
				return MUSaveDevice.deviceRequiredTitle;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.deviceRequiredTitle = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string DeviceOptionalTitle
		{
			get
			{
				return MUSaveDevice.deviceOptionalTitle;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.deviceOptionalTitle = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string OkOption
		{
			get
			{
				return MUSaveDevice.deviceRequiredOptions[0];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.deviceRequiredOptions[0] = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string YesOption
		{
			get
			{
				return MUSaveDevice.deviceOptionalOptions[0];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.deviceOptionalOptions[0] = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string NoOption
		{
			get
			{
				return MUSaveDevice.deviceOptionalOptions[1];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					MUSaveDevice.deviceOptionalOptions[1] = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public bool IsReady
		{
			get
			{
				return this.storageDevice != null && this.storageDevice.IsConnected;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.enabled != value)
				{
					this.enabled = value;
					if (this.EnabledChanged != null)
					{
						this.EnabledChanged(this, null);
					}
				}
			}
		}

		public int UpdateOrder
		{
			get
			{
				return this.updateOrder;
			}
			set
			{
				if (this.updateOrder != value)
				{
					this.updateOrder = value;
					if (this.UpdateOrderChanged != null)
					{
						this.UpdateOrderChanged(this, null);
					}
				}
			}
		}

		public event EventHandler<SaveDeviceEventArgs> DeviceDisconnected;

		public event EventHandler<EventArgs> EnabledChanged;

		public event EventHandler<EventArgs> UpdateOrderChanged;

		static MUSaveDevice()
		{
			StorageSettings.ResetSaveDeviceStrings();
		}

		protected MUSaveDevice(string containerName, byte[] key) : base(key)
		{
			this._containerName = containerName;
		}

		public virtual void Initialize()
		{
		}

		public void PromptForDevice(SuccessCallback callBack)
		{
			this._promptCallback = callBack;
			if (this.state == SaveDevicePromptState.None)
			{
				this.state = SaveDevicePromptState.ShowSelector;
			}
		}

		public static void EnsureCreated(StorageContainer container, string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName))
			{
				MUSaveDevice.EnsureCreated(container, directoryName);
			}
			if (!container.DirectoryExists(path))
			{
				container.CreateDirectory(path);
			}
		}

		protected abstract void GetStorageDevice(AsyncCallback callback, SuccessCallback resultCallback);

		protected virtual void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			args.Response = (this.ForceDeviceSelection ? SaveDeviceEventResponse.Force : SaveDeviceEventResponse.Prompt);
			args.PlayerToPrompt = null;
		}

		public void Update(GameTime gameTime)
		{
			bool flag = (this.PromptForReselect || this.DeviceDisconnected != null) && this.storageDevice != null;
			bool flag2 = this.deviceWasConnected && this.storageDevice != null;
			if (flag)
			{
				flag2 = this.storageDevice.IsConnected;
			}
			if (!flag2 && this.deviceWasConnected)
			{
				this.PrepareEventArgs(this.eventArgs);
				if (this.DeviceDisconnected != null)
				{
					this.DeviceDisconnected(this, this.eventArgs);
				}
				if (this.PromptForReselect)
				{
					this.HandleEventArgResults();
				}
				else
				{
					this.state = SaveDevicePromptState.None;
				}
			}
			else if (!flag2)
			{
				try
				{
					SaveDevicePromptState saveDevicePromptState = this.state;
					if (saveDevicePromptState == SaveDevicePromptState.ShowSelector)
					{
						this.state = SaveDevicePromptState.None;
						this.GetStorageDevice(new AsyncCallback(this.StorageDeviceSelectorCallback), this._promptCallback);
					}
				}
				catch (GuideAlreadyVisibleException)
				{
				}
			}
			this.deviceWasConnected = flag2;
		}

		private void StorageDeviceSelectorCallback(IAsyncResult result)
		{
			SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
			this.storageDevice = StorageDevice.EndShowSelector(result);
			if (this.storageDevice != null && this.storageDevice.IsConnected)
			{
				try
				{
					this._currentContainer = this.OpenContainer(this._containerName);
					if (successCallback != null)
					{
						successCallback(true);
					}
					return;
				}
				catch
				{
					if (successCallback != null)
					{
						successCallback(false);
					}
					return;
				}
			}
			this.PrepareEventArgs(this.eventArgs);
			this.HandleEventArgResults();
		}

		private void ForcePromptCallback(IAsyncResult result)
		{
			Guide.EndShowMessageBox(result);
			this.state = SaveDevicePromptState.ShowSelector;
		}

		private void ReselectPromptCallback(IAsyncResult result)
		{
			int? num = Guide.EndShowMessageBox(result);
			this.state = ((num != null && num.Value == 0) ? SaveDevicePromptState.ShowSelector : SaveDevicePromptState.None);
			this.promptEventArgs.ShowDeviceSelector = (this.state == SaveDevicePromptState.ShowSelector);
			SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
			if (this.state == SaveDevicePromptState.None && successCallback != null)
			{
				successCallback(false);
			}
		}

		private void HandleEventArgResults()
		{
			this.storageDevice = null;
			switch (this.eventArgs.Response)
			{
			case SaveDeviceEventResponse.Prompt:
				this.state = (this.deviceWasConnected ? SaveDevicePromptState.PromptForDisconnected : SaveDevicePromptState.PromptForCanceled);
				return;
			case SaveDeviceEventResponse.Force:
				this.state = (this.deviceWasConnected ? SaveDevicePromptState.ForceDisconnectedReselection : SaveDevicePromptState.ForceCanceledReselection);
				return;
			default:
				this.state = SaveDevicePromptState.None;
				return;
			}
		}

		private static void ShowMessageBox(PlayerIndex? player, string title, string text, IEnumerable<string> buttons, AsyncCallback callback, object state)
		{
			if (player != null)
			{
				Guide.BeginShowMessageBox(player.Value, title, text, buttons, 0, MessageBoxIcon.None, callback, state);
				return;
			}
			Guide.BeginShowMessageBox(title, text, buttons, 0, MessageBoxIcon.None, callback, state);
		}

		private StorageContainer OpenContainer(string containerName)
		{
			IAsyncResult asyncResult = this.storageDevice.BeginOpenContainer(containerName, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			return this.storageDevice.EndOpenContainer(asyncResult);
		}

		protected override void VerifyIsReady()
		{
			if (!this.IsReady)
			{
				throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);
			}
		}

		public override void Flush()
		{
			if (this.storageDevice == null)
			{
				return;
			}
			lock (this)
			{
				if (this._currentContainer != null)
				{
					this._currentContainer.Dispose();
					try
					{
						this._currentContainer = this.OpenContainer(this._containerName);
					}
					catch
					{
						this._currentContainer = null;
					}
				}
			}
		}

		public override void DeviceDispose()
		{
			if (this._currentContainer != null)
			{
				this._currentContainer.Dispose();
				this._currentContainer = null;
			}
		}

		public override void DeleteStorage()
		{
			if (this.storageDevice == null)
			{
				return;
			}
			lock (this)
			{
				if (this._currentContainer != null)
				{
					this._currentContainer.Dispose();
					try
					{
						this.storageDevice.DeleteContainer(this._containerName);
						this._currentContainer = this.OpenContainer(this._containerName);
					}
					catch
					{
						this._currentContainer = null;
					}
				}
			}
		}

		protected override Stream DeviceOpenFile(string fileName, FileMode mode, FileAccess access, FileShare share)
		{
			return this._currentContainer.OpenFile(fileName, mode, access, share);
		}

		protected override void DeviceDeleteFile(string fileName)
		{
			this._currentContainer.DeleteFile(fileName);
		}

		protected override bool DeviceFileExists(string fileName)
		{
			return this._currentContainer.FileExists(fileName);
		}

		protected override bool DeviceDirectoryExists(string dirName)
		{
			return this._currentContainer.DirectoryExists(dirName);
		}

		protected override string[] DeviceGetDirectoryNames()
		{
			return this._currentContainer.GetDirectoryNames();
		}

		protected override string[] DeviceGetFileNames()
		{
			return this._currentContainer.GetFileNames();
		}

		protected override string[] DeviceGetFileNames(string pattern)
		{
			return this._currentContainer.GetFileNames(pattern);
		}

		protected override string[] DeviceGetDirectoryNames(string pattern)
		{
			return this._currentContainer.GetDirectoryNames(pattern);
		}

		protected override void DeviceCreateDirectory(string path)
		{
			this._currentContainer.CreateDirectory(path);
		}

		protected override void DeviceDeleteDirectory(string path)
		{
			this._currentContainer.DeleteDirectory(path);
		}
	}
}
