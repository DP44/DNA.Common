using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class InputBinding
	{
		private enum BindableFlags
		{
			KeyFlag = 512,
			MouseFlag = 1024,
			ControllerFlag = 2048,
			TypeMask = 3584
		}

		[Flags]
		public enum Constraint
		{
			AllowKeyboard = 512,
			AllowMouse = 1024,
			AllowController = 2048,
			AllowKeyboardMouse = 1536,
			AllowAll = 3584
		}

		public enum Slot
		{
			KeyMouse1,
			KeyMouse2,
			Controller,
			Count
		}

		public enum Bindable
		{
			None,
			KeyBack = 520,
			KeyTab,
			KeyEnter = 525,
			KeyPause = 531,
			KeyCapsLock,
			KeyKana,
			KeyKanji = 537,
			KeyEscape = 539,
			KeyImeCvt,
			KeyImeNoCvt,
			KeySpace = 544,
			KeyPageUp,
			KeyPageDn,
			KeyEnd,
			KeyHome,
			KeyLeft,
			KeyUp,
			KeyRight,
			KeyDown,
			KeySelect,
			KeyPrint,
			KeyExecute,
			KeyPrntScr,
			KeyInsert,
			KeyDelete,
			KeyHelp,
			KeyD0,
			KeyD1,
			KeyD2,
			KeyD3,
			KeyD4,
			KeyD5,
			KeyD6,
			KeyD7,
			KeyD8,
			KeyD9,
			KeyA = 577,
			KeyB,
			KeyC,
			KeyD,
			KeyE,
			KeyF,
			KeyG,
			KeyH,
			KeyI,
			KeyJ,
			KeyK,
			KeyL,
			KeyM,
			KeyN,
			KeyO,
			KeyP,
			KeyQ,
			KeyR,
			KeyS,
			KeyT,
			KeyU,
			KeyV,
			KeyW,
			KeyX,
			KeyY,
			KeyZ,
			KeyLeftWin,
			KeyRightWin,
			KeyApps,
			KeySleep = 607,
			KeyNumPad0,
			KeyNumPad1,
			KeyNumPad2,
			KeyNumPad3,
			KeyNumPad4,
			KeyNumPad5,
			KeyNumPad6,
			KeyNumPad7,
			KeyNumPad8,
			KeyNumPad9,
			KeyMultiply,
			KeyAdd,
			KeySeparator,
			KeySubtract,
			KeyDecimal,
			KeyDivide,
			KeyF1,
			KeyF2,
			KeyF3,
			KeyF4,
			KeyF5,
			KeyF6,
			KeyF7,
			KeyF8,
			KeyF9,
			KeyF10,
			KeyF11,
			KeyF12,
			KeyF13,
			KeyF14,
			KeyF15,
			KeyF16,
			KeyF17,
			KeyF18,
			KeyF19,
			KeyF20,
			KeyF21,
			KeyF22,
			KeyF23,
			KeyF24,
			KeyNumLock = 656,
			KeyScroll,
			KeyLShift = 672,
			KeyRShift,
			KeyLeftCtrl,
			KeyRightCtrl,
			KeyLeftAlt,
			KeyRightAlt,
			KeyBrwseBack,
			KeyBrwseFwd,
			KeyBrwseRefresh,
			KeyBrwseStop,
			KeyBrwseSearch,
			KeyBrwseFav,
			KeyBrwseHome,
			KeyMute,
			KeyVolDown,
			KeyVolUp,
			KeyNextTrack,
			KeyPrevTrack,
			KeyStop,
			KeyPlayPause,
			KeyMail,
			KeySelectMedia,
			KeyApp1,
			KeyApp2,
			KeySemicolon = 698,
			KeyPlus,
			KeyComma,
			KeyMinus,
			KeyPeriod,
			KeyQuestion,
			KeyTilde,
			KeyChatGreen = 714,
			KeyChatOrange,
			KeyLBracket = 731,
			KeyPipe,
			KeyRBracket,
			KeyQuotes,
			Key8,
			KeyBackslash = 738,
			KeyProcess = 741,
			KeyCopy = 754,
			KeyAuto,
			KeyEnlW,
			KeyAttn = 758,
			KeyCrsel,
			KeyExsel,
			KeyEraseEof,
			KeyPlay,
			KeyZoom,
			KeyPa1 = 765,
			KeyOemClear,
			MouseButtonLeft = 1024,
			MouseButtonMiddle,
			MouseButtonRight,
			MouseButton3,
			MouseButton4,
			MouseWheelUp,
			MouseWheelDown,
			ButtonDPadUp = 2048,
			ButtonDPadDown,
			ButtonDPadLeft,
			ButtonDPadRight,
			ButtonStart,
			ButtonBack,
			ButtonLStick,
			ButtonRStick,
			ButtonLShldr,
			ButtonRShldr,
			ButtonBigButton,
			ButtonA,
			ButtonB,
			ButtonX,
			ButtonY,
			ButtonRTrigger,
			ButtonLTrigger,
			ButtonRStickUp,
			ButtonRStickDn,
			ButtonRStickRt,
			ButtonRStickLt,
			ButtonLStickUp,
			ButtonLStickDn,
			ButtonLStickRt,
			ButtonLStickLt
		}

		private enum ButtonFunction
		{
			Pressed,
			Released,
			Held
		}

		private enum FileVersion
		{
			CurrentVersion
		}

		private static Dictionary<InputBinding.Bindable, Buttons> _bindableToButtons = new Dictionary<InputBinding.Bindable, Buttons>();

		private static Dictionary<Buttons, InputBinding.Bindable> _buttonsToBindable = new Dictionary<Buttons, InputBinding.Bindable>();

		private static Dictionary<InputBinding.Bindable, string> _bindableNames = new Dictionary<InputBinding.Bindable, string>();

		private static List<Keys> _validKeys = new List<Keys>();

		private static List<Buttons> _validButtons = new List<Buttons>();

		private Dictionary<int, InputBinding.Bindable[]> _bindings = new Dictionary<int, InputBinding.Bindable[]>();

		public bool AvoidShiftTab;

		public bool Initialized;

		private InputBinding.Bindable _keySensed;

		private int _skipFrames;

		public static string KeyString(InputBinding.Bindable bindable)
		{
			return InputBinding._bindableNames[bindable];
		}

		private static string SplitBindableName(string name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (name.StartsWith("Key"))
			{
				name = name.Substring("Key".Length);
			}
			else if (name.StartsWith("Button"))
			{
				name = name.Substring("Button".Length);
			}
			bool flag = true;
			bool flag2 = true;
			foreach (char c in name)
			{
				if (!flag && char.ToUpper(c) == c)
				{
					stringBuilder.Append(' ');
				}
				else if (!flag2 && char.IsDigit(c))
				{
					stringBuilder.Append(' ');
				}
				flag2 = char.IsDigit(c);
				flag = false;
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		static InputBinding()
		{
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonDPadUp, Buttons.DPadUp);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonDPadDown, Buttons.DPadDown);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonDPadLeft, Buttons.DPadLeft);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonDPadRight, Buttons.DPadRight);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonStart, Buttons.Start);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonBack, Buttons.Back);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLStick, Buttons.LeftStick);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRStick, Buttons.RightStick);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLShldr, Buttons.LeftShoulder);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRShldr, Buttons.RightShoulder);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonBigButton, Buttons.BigButton);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonA, Buttons.A);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonB, Buttons.B);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonX, Buttons.X);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonY, Buttons.Y);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRTrigger, Buttons.RightTrigger);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLTrigger, Buttons.LeftTrigger);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRStickUp, Buttons.RightThumbstickUp);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRStickDn, Buttons.RightThumbstickDown);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRStickRt, Buttons.RightThumbstickRight);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonRStickLt, Buttons.RightThumbstickLeft);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLStickUp, Buttons.LeftThumbstickUp);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLStickDn, Buttons.LeftThumbstickDown);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLStickRt, Buttons.LeftThumbstickRight);
			InputBinding._bindableToButtons.Add(InputBinding.Bindable.ButtonLStickLt, Buttons.LeftThumbstickLeft);
			InputBinding._buttonsToBindable.Add(Buttons.DPadUp, InputBinding.Bindable.ButtonDPadUp);
			InputBinding._buttonsToBindable.Add(Buttons.DPadDown, InputBinding.Bindable.ButtonDPadDown);
			InputBinding._buttonsToBindable.Add(Buttons.DPadLeft, InputBinding.Bindable.ButtonDPadLeft);
			InputBinding._buttonsToBindable.Add(Buttons.DPadRight, InputBinding.Bindable.ButtonDPadRight);
			InputBinding._buttonsToBindable.Add(Buttons.Start, InputBinding.Bindable.ButtonStart);
			InputBinding._buttonsToBindable.Add(Buttons.Back, InputBinding.Bindable.ButtonBack);
			InputBinding._buttonsToBindable.Add(Buttons.LeftStick, InputBinding.Bindable.ButtonLStick);
			InputBinding._buttonsToBindable.Add(Buttons.RightStick, InputBinding.Bindable.ButtonRStick);
			InputBinding._buttonsToBindable.Add(Buttons.LeftShoulder, InputBinding.Bindable.ButtonLShldr);
			InputBinding._buttonsToBindable.Add(Buttons.RightShoulder, InputBinding.Bindable.ButtonRShldr);
			InputBinding._buttonsToBindable.Add(Buttons.BigButton, InputBinding.Bindable.ButtonBigButton);
			InputBinding._buttonsToBindable.Add(Buttons.A, InputBinding.Bindable.ButtonA);
			InputBinding._buttonsToBindable.Add(Buttons.B, InputBinding.Bindable.ButtonB);
			InputBinding._buttonsToBindable.Add(Buttons.X, InputBinding.Bindable.ButtonX);
			InputBinding._buttonsToBindable.Add(Buttons.Y, InputBinding.Bindable.ButtonY);
			InputBinding._buttonsToBindable.Add(Buttons.LeftThumbstickLeft, InputBinding.Bindable.ButtonLStickLt);
			InputBinding._buttonsToBindable.Add(Buttons.RightTrigger, InputBinding.Bindable.ButtonRTrigger);
			InputBinding._buttonsToBindable.Add(Buttons.LeftTrigger, InputBinding.Bindable.ButtonLTrigger);
			InputBinding._buttonsToBindable.Add(Buttons.RightThumbstickUp, InputBinding.Bindable.ButtonRStickUp);
			InputBinding._buttonsToBindable.Add(Buttons.RightThumbstickDown, InputBinding.Bindable.ButtonRStickDn);
			InputBinding._buttonsToBindable.Add(Buttons.RightThumbstickRight, InputBinding.Bindable.ButtonRStickRt);
			InputBinding._buttonsToBindable.Add(Buttons.RightThumbstickLeft, InputBinding.Bindable.ButtonRStickLt);
			InputBinding._buttonsToBindable.Add(Buttons.LeftThumbstickUp, InputBinding.Bindable.ButtonLStickUp);
			InputBinding._buttonsToBindable.Add(Buttons.LeftThumbstickDown, InputBinding.Bindable.ButtonLStickDn);
			InputBinding._buttonsToBindable.Add(Buttons.LeftThumbstickRight, InputBinding.Bindable.ButtonLStickRt);
			InputBinding._validButtons.Add(Buttons.DPadUp);
			InputBinding._validButtons.Add(Buttons.DPadDown);
			InputBinding._validButtons.Add(Buttons.DPadLeft);
			InputBinding._validButtons.Add(Buttons.DPadRight);
			InputBinding._validButtons.Add(Buttons.Start);
			InputBinding._validButtons.Add(Buttons.Back);
			InputBinding._validButtons.Add(Buttons.LeftStick);
			InputBinding._validButtons.Add(Buttons.RightStick);
			InputBinding._validButtons.Add(Buttons.LeftShoulder);
			InputBinding._validButtons.Add(Buttons.RightShoulder);
			InputBinding._validButtons.Add(Buttons.BigButton);
			InputBinding._validButtons.Add(Buttons.A);
			InputBinding._validButtons.Add(Buttons.B);
			InputBinding._validButtons.Add(Buttons.X);
			InputBinding._validButtons.Add(Buttons.Y);
			InputBinding._validButtons.Add(Buttons.RightTrigger);
			InputBinding._validButtons.Add(Buttons.LeftTrigger);
			InputBinding._validButtons.Add(Buttons.RightThumbstickUp);
			InputBinding._validButtons.Add(Buttons.RightThumbstickDown);
			InputBinding._validButtons.Add(Buttons.RightThumbstickRight);
			InputBinding._validButtons.Add(Buttons.RightThumbstickLeft);
			InputBinding._validButtons.Add(Buttons.LeftThumbstickUp);
			InputBinding._validButtons.Add(Buttons.LeftThumbstickDown);
			InputBinding._validButtons.Add(Buttons.LeftThumbstickRight);
			InputBinding._validButtons.Add(Buttons.LeftThumbstickLeft);
			InputBinding._validKeys.Add(Keys.Back);
			InputBinding._validKeys.Add(Keys.Tab);
			InputBinding._validKeys.Add(Keys.Enter);
			InputBinding._validKeys.Add(Keys.Pause);
			InputBinding._validKeys.Add(Keys.CapsLock);
			InputBinding._validKeys.Add(Keys.Kana);
			InputBinding._validKeys.Add(Keys.Kanji);
			InputBinding._validKeys.Add(Keys.Escape);
			InputBinding._validKeys.Add(Keys.ImeConvert);
			InputBinding._validKeys.Add(Keys.ImeNoConvert);
			InputBinding._validKeys.Add(Keys.Space);
			InputBinding._validKeys.Add(Keys.PageUp);
			InputBinding._validKeys.Add(Keys.PageDown);
			InputBinding._validKeys.Add(Keys.End);
			InputBinding._validKeys.Add(Keys.Home);
			InputBinding._validKeys.Add(Keys.Left);
			InputBinding._validKeys.Add(Keys.Up);
			InputBinding._validKeys.Add(Keys.Right);
			InputBinding._validKeys.Add(Keys.Down);
			InputBinding._validKeys.Add(Keys.Select);
			InputBinding._validKeys.Add(Keys.Print);
			InputBinding._validKeys.Add(Keys.Execute);
			InputBinding._validKeys.Add(Keys.PrintScreen);
			InputBinding._validKeys.Add(Keys.Insert);
			InputBinding._validKeys.Add(Keys.Delete);
			InputBinding._validKeys.Add(Keys.Help);
			InputBinding._validKeys.Add(Keys.D0);
			InputBinding._validKeys.Add(Keys.D1);
			InputBinding._validKeys.Add(Keys.D2);
			InputBinding._validKeys.Add(Keys.D3);
			InputBinding._validKeys.Add(Keys.D4);
			InputBinding._validKeys.Add(Keys.D5);
			InputBinding._validKeys.Add(Keys.D6);
			InputBinding._validKeys.Add(Keys.D7);
			InputBinding._validKeys.Add(Keys.D8);
			InputBinding._validKeys.Add(Keys.D9);
			InputBinding._validKeys.Add(Keys.A);
			InputBinding._validKeys.Add(Keys.B);
			InputBinding._validKeys.Add(Keys.C);
			InputBinding._validKeys.Add(Keys.D);
			InputBinding._validKeys.Add(Keys.E);
			InputBinding._validKeys.Add(Keys.F);
			InputBinding._validKeys.Add(Keys.G);
			InputBinding._validKeys.Add(Keys.H);
			InputBinding._validKeys.Add(Keys.I);
			InputBinding._validKeys.Add(Keys.J);
			InputBinding._validKeys.Add(Keys.K);
			InputBinding._validKeys.Add(Keys.L);
			InputBinding._validKeys.Add(Keys.M);
			InputBinding._validKeys.Add(Keys.N);
			InputBinding._validKeys.Add(Keys.O);
			InputBinding._validKeys.Add(Keys.P);
			InputBinding._validKeys.Add(Keys.Q);
			InputBinding._validKeys.Add(Keys.R);
			InputBinding._validKeys.Add(Keys.S);
			InputBinding._validKeys.Add(Keys.T);
			InputBinding._validKeys.Add(Keys.U);
			InputBinding._validKeys.Add(Keys.V);
			InputBinding._validKeys.Add(Keys.W);
			InputBinding._validKeys.Add(Keys.X);
			InputBinding._validKeys.Add(Keys.Y);
			InputBinding._validKeys.Add(Keys.Z);
			InputBinding._validKeys.Add(Keys.LeftWindows);
			InputBinding._validKeys.Add(Keys.RightWindows);
			InputBinding._validKeys.Add(Keys.Apps);
			InputBinding._validKeys.Add(Keys.Sleep);
			InputBinding._validKeys.Add(Keys.NumPad0);
			InputBinding._validKeys.Add(Keys.NumPad1);
			InputBinding._validKeys.Add(Keys.NumPad2);
			InputBinding._validKeys.Add(Keys.NumPad3);
			InputBinding._validKeys.Add(Keys.NumPad4);
			InputBinding._validKeys.Add(Keys.NumPad5);
			InputBinding._validKeys.Add(Keys.NumPad6);
			InputBinding._validKeys.Add(Keys.NumPad7);
			InputBinding._validKeys.Add(Keys.NumPad8);
			InputBinding._validKeys.Add(Keys.NumPad9);
			InputBinding._validKeys.Add(Keys.Multiply);
			InputBinding._validKeys.Add(Keys.Add);
			InputBinding._validKeys.Add(Keys.Separator);
			InputBinding._validKeys.Add(Keys.Subtract);
			InputBinding._validKeys.Add(Keys.Decimal);
			InputBinding._validKeys.Add(Keys.Divide);
			InputBinding._validKeys.Add(Keys.F1);
			InputBinding._validKeys.Add(Keys.F2);
			InputBinding._validKeys.Add(Keys.F3);
			InputBinding._validKeys.Add(Keys.F4);
			InputBinding._validKeys.Add(Keys.F5);
			InputBinding._validKeys.Add(Keys.F6);
			InputBinding._validKeys.Add(Keys.F7);
			InputBinding._validKeys.Add(Keys.F8);
			InputBinding._validKeys.Add(Keys.F9);
			InputBinding._validKeys.Add(Keys.F10);
			InputBinding._validKeys.Add(Keys.F11);
			InputBinding._validKeys.Add(Keys.F12);
			InputBinding._validKeys.Add(Keys.F13);
			InputBinding._validKeys.Add(Keys.F14);
			InputBinding._validKeys.Add(Keys.F15);
			InputBinding._validKeys.Add(Keys.F16);
			InputBinding._validKeys.Add(Keys.F17);
			InputBinding._validKeys.Add(Keys.F18);
			InputBinding._validKeys.Add(Keys.F19);
			InputBinding._validKeys.Add(Keys.F20);
			InputBinding._validKeys.Add(Keys.F21);
			InputBinding._validKeys.Add(Keys.F22);
			InputBinding._validKeys.Add(Keys.F23);
			InputBinding._validKeys.Add(Keys.F24);
			InputBinding._validKeys.Add(Keys.NumLock);
			InputBinding._validKeys.Add(Keys.Scroll);
			InputBinding._validKeys.Add(Keys.LeftShift);
			InputBinding._validKeys.Add(Keys.RightShift);
			InputBinding._validKeys.Add(Keys.LeftControl);
			InputBinding._validKeys.Add(Keys.RightControl);
			InputBinding._validKeys.Add(Keys.LeftAlt);
			InputBinding._validKeys.Add(Keys.RightAlt);
			InputBinding._validKeys.Add(Keys.BrowserBack);
			InputBinding._validKeys.Add(Keys.BrowserForward);
			InputBinding._validKeys.Add(Keys.BrowserRefresh);
			InputBinding._validKeys.Add(Keys.BrowserStop);
			InputBinding._validKeys.Add(Keys.BrowserSearch);
			InputBinding._validKeys.Add(Keys.BrowserFavorites);
			InputBinding._validKeys.Add(Keys.BrowserHome);
			InputBinding._validKeys.Add(Keys.VolumeMute);
			InputBinding._validKeys.Add(Keys.VolumeDown);
			InputBinding._validKeys.Add(Keys.VolumeUp);
			InputBinding._validKeys.Add(Keys.MediaNextTrack);
			InputBinding._validKeys.Add(Keys.MediaPreviousTrack);
			InputBinding._validKeys.Add(Keys.MediaStop);
			InputBinding._validKeys.Add(Keys.MediaPlayPause);
			InputBinding._validKeys.Add(Keys.LaunchMail);
			InputBinding._validKeys.Add(Keys.SelectMedia);
			InputBinding._validKeys.Add(Keys.LaunchApplication1);
			InputBinding._validKeys.Add(Keys.LaunchApplication2);
			InputBinding._validKeys.Add(Keys.OemSemicolon);
			InputBinding._validKeys.Add(Keys.OemPlus);
			InputBinding._validKeys.Add(Keys.OemComma);
			InputBinding._validKeys.Add(Keys.OemMinus);
			InputBinding._validKeys.Add(Keys.OemPeriod);
			InputBinding._validKeys.Add(Keys.OemQuestion);
			InputBinding._validKeys.Add(Keys.OemTilde);
			InputBinding._validKeys.Add(Keys.ChatPadGreen);
			InputBinding._validKeys.Add(Keys.ChatPadOrange);
			InputBinding._validKeys.Add(Keys.OemOpenBrackets);
			InputBinding._validKeys.Add(Keys.OemPipe);
			InputBinding._validKeys.Add(Keys.OemCloseBrackets);
			InputBinding._validKeys.Add(Keys.OemQuotes);
			InputBinding._validKeys.Add(Keys.Oem8);
			InputBinding._validKeys.Add(Keys.OemBackslash);
			InputBinding._validKeys.Add(Keys.ProcessKey);
			InputBinding._validKeys.Add(Keys.OemCopy);
			InputBinding._validKeys.Add(Keys.OemAuto);
			InputBinding._validKeys.Add(Keys.OemEnlW);
			InputBinding._validKeys.Add(Keys.Attn);
			InputBinding._validKeys.Add(Keys.Crsel);
			InputBinding._validKeys.Add(Keys.Exsel);
			InputBinding._validKeys.Add(Keys.EraseEof);
			InputBinding._validKeys.Add(Keys.Play);
			InputBinding._validKeys.Add(Keys.Zoom);
			InputBinding._validKeys.Add(Keys.Pa1);
			InputBinding._validKeys.Add(Keys.OemClear);
			foreach (InputBinding.Bindable bindable in Enum.GetValues(typeof(InputBinding.Bindable)).Cast<InputBinding.Bindable>())
			{
				InputBinding.Bindable bindable2 = bindable;
				string value;
				switch (bindable2)
				{
				case InputBinding.Bindable.KeyD1:
					value = "1";
					break;
				case InputBinding.Bindable.KeyD2:
					value = "2";
					break;
				case InputBinding.Bindable.KeyD3:
					value = "3";
					break;
				case InputBinding.Bindable.KeyD4:
					value = "4";
					break;
				case InputBinding.Bindable.KeyD5:
					value = "5";
					break;
				case InputBinding.Bindable.KeyD6:
					value = "6";
					break;
				case InputBinding.Bindable.KeyD7:
					value = "7";
					break;
				case InputBinding.Bindable.KeyD8:
					value = "8";
					break;
				default:
					switch (bindable2)
					{
					case InputBinding.Bindable.MouseButtonLeft:
						value = "Left Click";
						break;
					case InputBinding.Bindable.MouseButtonMiddle:
						value = "Middle Click";
						break;
					case InputBinding.Bindable.MouseButtonRight:
						value = "Right Click";
						break;
					case InputBinding.Bindable.MouseButton3:
						value = "Mouse Button 3";
						break;
					case InputBinding.Bindable.MouseButton4:
						value = "Mouse Button 4";
						break;
					case InputBinding.Bindable.MouseWheelUp:
						value = "Scroll Up";
						break;
					case InputBinding.Bindable.MouseWheelDown:
						value = "Scroll Down";
						break;
					default:
						value = InputBinding.SplitBindableName(bindable.ToString());
						break;
					}
					break;
				}
				InputBinding._bindableNames.Add(bindable, value);
			}
		}

		public void Clear()
		{
			this._bindings.Clear();
			this.Initialized = false;
		}

		public void SaveData(BinaryWriter writer)
		{
			int num = 0;
			foreach (InputBinding.Bindable[] array in this._bindings.Values)
			{
				for (int i = 0; i < 3; i++)
				{
					if (array[i] != InputBinding.Bindable.None)
					{
						num++;
						break;
					}
				}
			}
			writer.Write(0);
			writer.Write(num);
			foreach (KeyValuePair<int, InputBinding.Bindable[]> keyValuePair in this._bindings)
			{
				bool flag = false;
				for (int j = 0; j < 3; j++)
				{
					if (keyValuePair.Value[j] != InputBinding.Bindable.None)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					writer.Write(keyValuePair.Key);
					for (int k = 0; k < 3; k++)
					{
						writer.Write((int)keyValuePair.Value[k]);
					}
				}
			}
		}

		public void LoadData(BinaryReader reader)
		{
			InputBinding.FileVersion fileVersion = (InputBinding.FileVersion)reader.ReadInt32();
			if (fileVersion < InputBinding.FileVersion.CurrentVersion || fileVersion > InputBinding.FileVersion.CurrentVersion)
			{
				return;
			}
			this._bindings.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int key = reader.ReadInt32();
				InputBinding.Bindable[] array = new InputBinding.Bindable[3];
				for (int j = 0; j < 3; j++)
				{
					array[j] = (InputBinding.Bindable)reader.ReadInt32();
				}
				this._bindings.Add(key, array);
			}
			this.Initialized = true;
		}

		public void Bind(int function, InputBinding.Bindable keymouse1, InputBinding.Bindable keymouse2, InputBinding.Bindable controller)
		{
			this.Bind(function, InputBinding.Slot.KeyMouse1, keymouse1);
			this.Bind(function, InputBinding.Slot.KeyMouse2, keymouse2);
			this.Bind(function, InputBinding.Slot.Controller, controller);
		}

		public void Bind(int function, InputBinding.Slot slot, InputBinding.Bindable btn)
		{
			this.RemoveBinding(btn);
			InputBinding.Bindable[] array;
			if (!this._bindings.TryGetValue(function, out array))
			{
				array = new InputBinding.Bindable[3];
				array[0] = (array[1] = (array[2] = InputBinding.Bindable.None));
				this._bindings.Add(function, array);
			}
			array[(int)slot] = btn;
		}

		public InputBinding.Bindable GetBinding(int function, InputBinding.Slot slot)
		{
			InputBinding.Bindable[] array;
			this._bindings.TryGetValue(function, out array);
			if (array == null)
			{
				return InputBinding.Bindable.None;
			}
			return array[(int)slot];
		}

		public static bool IsKeyMouse(InputBinding.Bindable value)
		{
			return !InputBinding.IsController(value);
		}

		public static bool IsController(InputBinding.Bindable value)
		{
			return (value & InputBinding.Bindable.ButtonDPadUp) != InputBinding.Bindable.None;
		}

		public static bool IsMouse(InputBinding.Bindable value)
		{
			return (value & InputBinding.Bindable.MouseButtonLeft) != InputBinding.Bindable.None;
		}

		public void RemoveBinding(InputBinding.Bindable btn)
		{
			if (btn != InputBinding.Bindable.None)
			{
				foreach (InputBinding.Bindable[] array in this._bindings.Values)
				{
					for (int i = 0; i < 3; i++)
					{
						if (array[i] == btn)
						{
							array[i] = InputBinding.Bindable.None;
							return;
						}
					}
				}
			}
		}

		private InputBinding.Bindable FindFirstKeyPressed(KeyboardInput keyboard)
		{
			for (int i = 0; i < InputBinding._validKeys.Count; i++)
			{
				if (keyboard.WasKeyPressed(InputBinding._validKeys[i]))
				{
					return (InputBinding.Bindable)(InputBinding._validKeys[i] | (Keys)512);
				}
			}
			return InputBinding.Bindable.None;
		}

		private InputBinding.Bindable FindFirstButtonPressed(GameController controller)
		{
			for (int i = 0; i < InputBinding._validButtons.Count; i++)
			{
				if (controller.PressedButtons[InputBinding._validButtons[i]])
				{
					return InputBinding._buttonsToBindable[InputBinding._validButtons[i]];
				}
			}
			return InputBinding.Bindable.None;
		}

		private InputBinding.Bindable FindFirstMouseButtonPressed(MouseInput mouse)
		{
			if (mouse.LeftButtonPressed)
			{
				return InputBinding.Bindable.MouseButtonLeft;
			}
			if (mouse.RightButtonPressed)
			{
				return InputBinding.Bindable.MouseButtonRight;
			}
			if (mouse.MiddleButtonPressed)
			{
				return InputBinding.Bindable.MouseButtonMiddle;
			}
			if (mouse.XButton1Pressed)
			{
				return InputBinding.Bindable.MouseButton3;
			}
			if (mouse.XButton2Pressed)
			{
				return InputBinding.Bindable.MouseButton4;
			}
			if (mouse.WheelUpPressed)
			{
				return InputBinding.Bindable.MouseWheelUp;
			}
			if (mouse.WheelDownPressed)
			{
				return InputBinding.Bindable.MouseWheelDown;
			}
			return InputBinding.Bindable.None;
		}

		public void InitBindableSensor()
		{
			this._keySensed = InputBinding.Bindable.None;
			this._skipFrames = 2;
		}

		public InputBinding.Bindable SenseBindable(InputBinding.Slot slot, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			InputBinding.Bindable result = InputBinding.Bindable.None;
			InputBinding.Constraint constraint = InputBinding.Constraint.AllowKeyboardMouse;
			if (slot == InputBinding.Slot.Controller)
			{
				constraint = InputBinding.Constraint.AllowController;
			}
			if (this._skipFrames != 0)
			{
				this._skipFrames--;
			}
			else if (this._keySensed == InputBinding.Bindable.None)
			{
				if ((constraint & InputBinding.Constraint.AllowKeyboard) != (InputBinding.Constraint)0)
				{
					this._keySensed = this.FindFirstKeyPressed(keyboard);
				}
				if (this._keySensed == InputBinding.Bindable.None && (constraint & InputBinding.Constraint.AllowController) != (InputBinding.Constraint)0)
				{
					this._keySensed = this.FindFirstButtonPressed(controller);
				}
				if (this._keySensed == InputBinding.Bindable.None && (constraint & InputBinding.Constraint.AllowMouse) != (InputBinding.Constraint)0)
				{
					this._keySensed = this.FindFirstMouseButtonPressed(mouse);
				}
			}
			else if (this.GetButtonValue(this._keySensed, InputBinding.ButtonFunction.Released, keyboard, mouse, controller))
			{
				result = this._keySensed;
			}
			return result;
		}

		private bool GetKeyboardValue(InputBinding.Bindable btn, InputBinding.ButtonFunction fn, KeyboardInput keyboard)
		{
			Keys key = (Keys)(btn ^ (InputBinding.Bindable)512);
			bool result = false;
			switch (fn)
			{
			case InputBinding.ButtonFunction.Pressed:
				if (btn == InputBinding.Bindable.KeyTab && this.AvoidShiftTab)
				{
					result = (keyboard.WasKeyPressed(Keys.Tab) && !keyboard.IsKeyDown(Keys.LeftShift) && !keyboard.IsKeyDown(Keys.RightShift));
				}
				else
				{
					result = keyboard.WasKeyPressed(key);
				}
				break;
			case InputBinding.ButtonFunction.Released:
				result = keyboard.WasKeyReleased(key);
				break;
			case InputBinding.ButtonFunction.Held:
				result = keyboard.IsKeyDown(key);
				break;
			}
			return result;
		}

		private Trigger GetKeyboardValue(InputBinding.Bindable btn, KeyboardInput keyboard)
		{
			Keys key = (Keys)(btn ^ (InputBinding.Bindable)512);
			Trigger result = default(Trigger);
			if (btn == InputBinding.Bindable.KeyTab && this.AvoidShiftTab)
			{
				result.Pressed = (keyboard.WasKeyPressed(Keys.Tab) && !keyboard.IsKeyDown(Keys.LeftShift) && !keyboard.IsKeyDown(Keys.RightShift));
			}
			else
			{
				result.Pressed = keyboard.WasKeyPressed(key);
			}
			result.Released = keyboard.WasKeyReleased(key);
			result.Held = keyboard.IsKeyDown(key);
			return result;
		}

		private bool GetMouseValue(InputBinding.Bindable btn, InputBinding.ButtonFunction fn, MouseInput mouse)
		{
			bool result = false;
			switch (btn)
			{
			case InputBinding.Bindable.MouseButtonLeft:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.LeftButtonPressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.LeftButtonReleased;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.LeftButtonDown;
					break;
				}
				break;
			case InputBinding.Bindable.MouseButtonMiddle:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.MiddleButtonPressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.MiddleButtonReleased;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.MiddleButtonDown;
					break;
				}
				break;
			case InputBinding.Bindable.MouseButtonRight:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.RightButtonPressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.RightButtonReleased;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.RightButtonDown;
					break;
				}
				break;
			case InputBinding.Bindable.MouseButton3:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.XButton1Pressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.XButton1Released;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.XButton1Down;
					break;
				}
				break;
			case InputBinding.Bindable.MouseButton4:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.XButton2Pressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.XButton2Released;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.XButton2Down;
					break;
				}
				break;
			case InputBinding.Bindable.MouseWheelUp:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.WheelUpPressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.WheelUpReleased;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.WheelUpDown;
					break;
				}
				break;
			case InputBinding.Bindable.MouseWheelDown:
				switch (fn)
				{
				case InputBinding.ButtonFunction.Pressed:
					result = mouse.WheelDownPressed;
					break;
				case InputBinding.ButtonFunction.Released:
					result = mouse.WheelDownReleased;
					break;
				case InputBinding.ButtonFunction.Held:
					result = mouse.WheelDownDown;
					break;
				}
				break;
			}
			return result;
		}

		private Trigger GetMouseValue(InputBinding.Bindable btn, MouseInput mouse)
		{
			Trigger result = default(Trigger);
			switch (btn)
			{
			case InputBinding.Bindable.MouseButtonLeft:
				result.Pressed = mouse.LeftButtonPressed;
				result.Released = mouse.LeftButtonReleased;
				result.Held = mouse.LeftButtonDown;
				break;
			case InputBinding.Bindable.MouseButtonMiddle:
				result.Pressed = mouse.MiddleButtonPressed;
				result.Released = mouse.MiddleButtonReleased;
				result.Held = mouse.MiddleButtonDown;
				break;
			case InputBinding.Bindable.MouseButtonRight:
				result.Pressed = mouse.RightButtonPressed;
				result.Released = mouse.RightButtonReleased;
				result.Held = mouse.RightButtonDown;
				break;
			case InputBinding.Bindable.MouseButton3:
				result.Pressed = mouse.XButton1Pressed;
				result.Released = mouse.XButton1Released;
				result.Held = mouse.XButton1Down;
				break;
			case InputBinding.Bindable.MouseButton4:
				result.Pressed = mouse.XButton2Pressed;
				result.Released = mouse.XButton2Released;
				result.Held = mouse.XButton2Down;
				break;
			case InputBinding.Bindable.MouseWheelUp:
				result.Pressed = mouse.WheelUpPressed;
				result.Released = mouse.WheelUpReleased;
				result.Held = mouse.WheelUpDown;
				break;
			case InputBinding.Bindable.MouseWheelDown:
				result.Pressed = mouse.WheelDownPressed;
				result.Released = mouse.WheelDownReleased;
				result.Held = mouse.WheelDownDown;
				break;
			}
			return result;
		}

		private bool GetControllerValue(InputBinding.Bindable btn, InputBinding.ButtonFunction fn, GameController controller)
		{
			Buttons btn2 = InputBinding._bindableToButtons[btn];
			bool result = false;
			switch (fn)
			{
			case InputBinding.ButtonFunction.Pressed:
				result = controller.GetButtonPressed(btn2);
				break;
			case InputBinding.ButtonFunction.Released:
				result = controller.GetButtonPressed(btn2);
				break;
			case InputBinding.ButtonFunction.Held:
				result = controller.GetButtonHeld(btn2);
				break;
			}
			return result;
		}

		private Trigger GetControllerValue(InputBinding.Bindable btn, GameController controller)
		{
			Trigger result = default(Trigger);
			Buttons btn2 = InputBinding._bindableToButtons[btn];
			result.Pressed = controller.GetButtonPressed(btn2);
			result.Released = controller.GetButtonPressed(btn2);
			result.Held = controller.GetButtonHeld(btn2);
			return result;
		}

		private bool GetButtonValue(InputBinding.Bindable btn, InputBinding.ButtonFunction fn, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			bool result = false;
			int num = (int)(btn & (InputBinding.Bindable)3584);
			if (num != 512)
			{
				if (num != 1024)
				{
					if (num == 2048)
					{
						result = this.GetControllerValue(btn, fn, controller);
					}
				}
				else
				{
					result = this.GetMouseValue(btn, fn, mouse);
				}
			}
			else
			{
				result = this.GetKeyboardValue(btn, fn, keyboard);
			}
			return result;
		}

		private Trigger GetButtonValue(InputBinding.Bindable btn, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			Trigger result = default(Trigger);
			int num = (int)(btn & (InputBinding.Bindable)3584);
			if (num != 512)
			{
				if (num != 1024)
				{
					if (num == 2048)
					{
						result = this.GetControllerValue(btn, controller);
					}
				}
				else
				{
					result = this.GetMouseValue(btn, mouse);
				}
			}
			else
			{
				result = this.GetKeyboardValue(btn, keyboard);
			}
			return result;
		}

		private bool GetButtonValue(int function, InputBinding.ButtonFunction fn, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			InputBinding.Bindable[] array;
			if (this._bindings.TryGetValue(function, out array))
			{
				for (int i = 0; i < 3; i++)
				{
					if (this.GetButtonValue(array[i], fn, keyboard, mouse, controller))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool GetFunctionPressed(int function, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			return this.GetButtonValue(function, InputBinding.ButtonFunction.Pressed, keyboard, mouse, controller);
		}

		public bool GetFunctionReleased(int function, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			return this.GetButtonValue(function, InputBinding.ButtonFunction.Released, keyboard, mouse, controller);
		}

		public bool GetFunctionHeld(int function, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			return this.GetButtonValue(function, InputBinding.ButtonFunction.Held, keyboard, mouse, controller);
		}

		public Trigger GetFunction(int function, KeyboardInput keyboard, MouseInput mouse, GameController controller)
		{
			Trigger trigger = default(Trigger);
			InputBinding.Bindable[] array;
			if (this._bindings.TryGetValue(function, out array))
			{
				for (int i = 0; i < 3; i++)
				{
					trigger = (this.GetButtonValue(array[i], keyboard, mouse, controller) | trigger);
				}
			}
			return trigger;
		}
	}
}
