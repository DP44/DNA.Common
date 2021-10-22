using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DNA.Text
{
	public class CommandLineArgs
	{
		protected delegate void ArgumentHandler(string flag, List<string> arguments);

		protected class ArgumentDefinition
		{
			public string Flag;
			public int NumArgs;
			public string Description;
			public string Sample;
			public CommandLineArgs.ArgumentHandler Handler;
			public StringComparison ComparisonType;

			public ArgumentDefinition(string flag, int numargs, CommandLineArgs.ArgumentHandler handler, string description, string sample, StringComparison comparisonType)
			{
				this.Flag = flag;
				this.NumArgs = numargs;
				this.Description = description;
				this.Sample = sample;
				this.Handler = handler;
				this.ComparisonType = comparisonType;
			}

			public bool Matches(string flag)
			{
				return this.Flag != null && this.Flag.Equals(flag, this.ComparisonType);
			}

			public bool EquivilentTo(CommandLineArgs.ArgumentDefinition def)
			{
				return this.Flag != null && def.Flag != null && (this.Flag.Equals(def.Flag, this.ComparisonType) || this.Flag.Equals(def.Flag, def.ComparisonType));
			}

			public override string ToString()
			{
				string text = (this.Flag == null) ? "null" : this.Flag;
				return $"{text}: {this.Description} ({this.NumArgs.ToString()})"
				// return string.Concat(new string[] { text, ": ", this.Description, " (", this.NumArgs.ToString(), ")" });
			}
		}

		protected const int cAllRemainingArgs = -1;
		protected const int cArgsUpToNextFlag = -2;

		private List<CommandLineArgs.ArgumentDefinition> _argumentDefinitions = new List<CommandLineArgs.ArgumentDefinition>();
		public bool ShowUsage = true;
		public string ErrorString;
		public static string ApplicationPath;
		private static Dictionary<Type, CommandLineArgs> _commandLineCache;
		private static List<CommandLineArgs.ArgumentDefinition> _registeredArgs;
		private static List<string> _sCommandLineTokens;

		protected virtual void ValidateSettings() {}

		private static List<string> TokenizeCommandLine()
		{
			List<string> list = new List<string>();
			string commandLine = Environment.CommandLine;
			
			if (commandLine == null || commandLine.Length == 0)
			{
				return list;
			}
			
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;

			StringBuilder stringBuilder = new StringBuilder();
			
			foreach (char c in commandLine)
			{
				if (flag)
				{
					stringBuilder.Append(c);
					flag = false;
				}
				else if (flag2)
				{
					if ((flag3 && c == '"') || (!flag3 && c == '\''))
					{
						flag2 = false;
						flag4 = true;
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else if (c == '\\' && CommandLineArgs.ApplicationPath != null)
				{
					flag = true;
				}
				else if (c == '"')
				{
					flag2 = true;
					flag3 = true;
				}
				else if (c == '\'')
				{
					flag2 = true;
					flag3 = false;
				}
				else if (!char.IsWhiteSpace(c))
				{
					stringBuilder.Append(c);
					flag4 = false;
				}
				else
				{
					string text2 = stringBuilder.ToString();
					
					if (CommandLineArgs.ApplicationPath == null)
					{
						if (flag4 || File.Exists(text2) || File.Exists(text2 + ".exe"))
						{
							CommandLineArgs.ApplicationPath = text2;
							stringBuilder.Length = 0;
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
						stringBuilder.Length = 0;
					}

					flag4 = false;
				}
			}
			
			if (CommandLineArgs.ApplicationPath == null)
			{
				CommandLineArgs.ApplicationPath = stringBuilder.ToString();
			}
			else if (stringBuilder.Length != 0)
			{
				list.Add(stringBuilder.ToString());
			}

			return list;
		}

		public static void ProcessArguments()
		{
			if (CommandLineArgs._commandLineCache != null)
			{
				return;
			}
			
			CommandLineArgs._sCommandLineTokens = CommandLineArgs.TokenizeCommandLine();
			CommandLineArgs._commandLineCache = new Dictionary<Type, CommandLineArgs>();
			CommandLineArgs._registeredArgs = new List<CommandLineArgs.ArgumentDefinition>();
			
			Type typeFromHandle = typeof(CommandLineArgs);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					Type[] types = assembly.GetTypes();
					
					foreach (Type type in types)
					{
						if (type.IsSubclassOf(typeFromHandle) && !type.Equals(typeFromHandle))
						{
							CommandLineArgs args = Activator.CreateInstance(type) as CommandLineArgs;
							if (args != null)
							{
								args.RegisterArguments();
							}

							CommandLineArgs._commandLineCache[type] = args;
						}
					}
				}
				catch (Exception)
				{
					// Swallow exceptions.
				}
			}

			foreach (CommandLineArgs args in CommandLineArgs._commandLineCache.Values)
			{
				if (args != null)
				{
					args.ParseArguments();
				}
			}
		}

		public static T Get<T>() where T : CommandLineArgs
		{
			CommandLineArgs args = null;
			
			if (args._commandLineCache == null)
			{
				throw new Exception("Command line requested but arguments have not been processed");
			}
			
			if (!args._commandLineCache.TryGetValue(typeof(T), out commandLineArgs))
			{
				throw new Exception("Command line type " + typeof(T).Name + " referenced but does not exist");
			}

			return commandLineArgs as T;
		}

		protected int AddFlag(string flag, int numargs, CommandLineArgs.ArgumentHandler handler, string description = "", string sample = "", StringComparison compType = StringComparison.OrdinalIgnoreCase)
		{
			this._argumentDefinitions.Add(new CommandLineArgs.ArgumentDefinition(flag, numargs, handler, description, sample, compType));
			return this._argumentDefinitions.Count;
		}

		public virtual string GetErrorUsageAndDescription()
		{
			StringBuilder errorDescription = new StringBuilder();
			
			if (this.ErrorString != null)
			{
				errorDescription.Append(this.ErrorString).Append("\n").Append("\n");
			}
			
			this.GetUsageLine(errorDescription);
			errorDescription.Append("\n\n");
			string text = this.UsagePreamble;
			
			if (text != null)
			{
				errorDescription.Append(text);
			}
			
			this.GetUsageDescription(errorDescription);
			errorDescription.Append("\n");
			text = this.UsageEpilogue;
			
			if (text != null)
			{
				errorDescription.Append(text);
			}
			
			errorDescription.Append("\n");
			return errorDescription.ToString();
		}

		public virtual string GetUsageLine()
		{
			StringBuilder usage = new StringBuilder();
			this.GetUsageLine(usage);
			return usage.ToString();
		}

		public virtual void GetUsageLine(StringBuilder sb)
		{
			string text = null;
			
			if (!string.IsNullOrEmpty(CommandLineArgs.ApplicationPath))
			{
				text = Path.GetFileName(CommandLineArgs.ApplicationPath);
			}
			
			if (string.IsNullOrEmpty(text))
			{
				text = "usage:";
			}
			
			sb.Append(text + " ");
			CommandLineArgs.ArgumentDefinition argumentDefinition = null;
			
			foreach (CommandLineArgs.ArgumentDefinition argumentDefinition2 in this._argumentDefinitions)
			{
				if (argumentDefinition2.Flag != null)
				{
					if (!string.IsNullOrEmpty(argumentDefinition2.Sample))
					{
						sb.Append(" ").Append(argumentDefinition2.Sample);
					}
				}
				else
				{
					argumentDefinition = argumentDefinition2;
				}
			}

			if (argumentDefinition != null && !string.IsNullOrEmpty(argumentDefinition.Sample))
			{
				sb.Append(" ").Append(argumentDefinition.Sample);
			}
		}

		public virtual string GetUsageDescription()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.GetUsageDescription(stringBuilder);
			return stringBuilder.ToString();
		}

		public virtual void GetUsageDescription(StringBuilder sb)
		{
			CommandLineArgs.ArgumentDefinition argumentDefinition = null;
			
			foreach (CommandLineArgs.ArgumentDefinition argumentDefinition2 in this._argumentDefinitions)
			{
				if (argumentDefinition2.Flag != null)
				{
					if (!string.IsNullOrEmpty(argumentDefinition2.Description))
					{
						sb.Append("\t").Append(argumentDefinition2.Flag).Append(" - ").Append(argumentDefinition2.Description).Append("\n");
					}
				}
				else
				{
					argumentDefinition = argumentDefinition2;
				}
			}

			if (argumentDefinition != null && !string.IsNullOrEmpty(argumentDefinition.Sample))
			{
				sb.Append("\t").Append(argumentDefinition.Description).Append("\n");
			}
		}

		protected virtual string UsagePreamble
		{
			get
			{
				return null;
			}
		}

		protected virtual string UsageEpilogue
		{
			get
			{
				return null;
			}
		}

		protected virtual void HandleUncaughtArg(string arg)
		{
			this.ShowUsage = true;
			this.ErrorString = "Unknown argument: " + arg;
		}

		protected virtual void RegisterArguments()
		{
			foreach (CommandLineArgs.ArgumentDefinition argumentDefinition in this._argumentDefinitions)
			{
				if (argumentDefinition.Flag != null)
				{
					bool flag = false;
					int i = 0;
					
					while (i < CommandLineArgs._registeredArgs.Count)
					{
						if (CommandLineArgs._registeredArgs[i].EquivilentTo(argumentDefinition))
						{
							if (CommandLineArgs._registeredArgs[i].NumArgs != argumentDefinition.NumArgs)
							{
								CommandLineArgs._registeredArgs.RemoveAt(i);
								break;
							}
							
							flag = true;
							break;
						}
						else
						{
							i++;
						}
					}
					
					if (!flag)
					{
						CommandLineArgs._registeredArgs.Add(argumentDefinition);
					}
				}
			}
		}

		protected virtual bool IsAFlag(string val)
		{
			foreach (CommandLineArgs.ArgumentDefinition argumentDefinition in CommandLineArgs._registeredArgs)
			{
				if (argumentDefinition.Matches(val))
				{
					return true;
				}
			}

			return false;
		}

		protected virtual bool CheckFlag(List<CommandLineArgs.ArgumentDefinition> definitions, List<string> args, bool runHandlers)
		{
			bool result = false;
		
			foreach (CommandLineArgs.ArgumentDefinition argumentDefinition in definitions)
			{
				if (argumentDefinition.Matches(args[0]))
				{
					result = true;
					string text = args[0];
					args.RemoveAt(0);
					
					if (argumentDefinition.NumArgs == -1)
					{
						if (runHandlers)
						{
							argumentDefinition.Handler(text, args);
							break;
						}
						
						break;
					}
					else if (argumentDefinition.NumArgs == -2)
					{
						List<string> list = null;
						
						while (args.Count != 0 && !this.IsAFlag(args[0]))
						{
							if (runHandlers)
							{
								if (list == null)
								{
									list = new List<string>();
								}
							
								list.Add(args[0]);
							}
							
							args.RemoveAt(0);
						}
						
						if (runHandlers)
						{
							argumentDefinition.Handler(text, list);
							break;
						}
						
						break;
					}
					else
					{
						List<string> list2 = null;
						
						if (argumentDefinition.NumArgs > args.Count)
						{
							if (runHandlers)
							{
								this.ShowUsage = true;
								this.ErrorString = "Not enough arguments for " + text + " flag";
								break;
							}
							
							break;
						}
						else
						{
							if (argumentDefinition.NumArgs > 0)
							{
								if (runHandlers)
								{
									list2 = new List<string>();
									
									for (int i = 0; i < argumentDefinition.NumArgs; i++)
									{
										list2.Add(args[i]);
									}
								}
								
								args.RemoveRange(0, argumentDefinition.NumArgs);
							}
							
							if (runHandlers)
							{
								argumentDefinition.Handler(text, list2);
								break;
							}
						
							break;
						}
					}
				}
			}
			
			return result;
		}

		// TODO: Reverse engineer control flow.
		protected virtual bool ParseArguments()
		{
			this.ShowUsage = false;
			List<string> list = new List<string>(CommandLineArgs._sCommandLineTokens);
			CommandLineArgs.ArgumentDefinition argumentDefinition = null;
			
			using (List<CommandLineArgs.ArgumentDefinition>.Enumerator enumerator = this._argumentDefinitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CommandLineArgs.ArgumentDefinition argumentDefinition2 = enumerator.Current;
					
					if (argumentDefinition2.Flag == null)
					{
						argumentDefinition = argumentDefinition2;
						break;
					}
				}
		
				goto IL_AE;
			}
		
		IL_50:
			if (!this.CheckFlag(this._argumentDefinitions, list, true))
			{
				if (argumentDefinition != null)
				{
					argumentDefinition.Handler(list[0], null);
					list.RemoveAt(0);
				}
				else if (!this.CheckFlag(CommandLineArgs._registeredArgs, list, false))
				{
					this.HandleUncaughtArg(list[0]);
					list.RemoveAt(0);
				}
			}
			
			if (this.ShowUsage)
			{
				return false;
			}
		
		IL_AE:
			if (list == null || list.Count <= 0)
			{
				this.ValidateSettings();
				return !this.ShowUsage;
			}
		
			goto IL_50;
		}
	}
}
