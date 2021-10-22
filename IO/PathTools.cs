using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DNA.IO
{
	public static class PathTools
	{
		private static Regex HasQuestionMarkRegEx = new Regex("\\?", RegexOptions.Compiled);

		private static Regex IlegalCharactersRegex = new Regex("[\\/:<>|\"]", RegexOptions.Compiled);

		private static Regex CatchExtentionRegex = new Regex("^\\s*.+\\.([^\\.]+)\\s*$", RegexOptions.Compiled);

		private static string NonDotCharacters = "[^.]*";

		public static string ReplaceInvalidChars(string orginalPath)
		{
			return PathTools.ReplaceInvalidChars(orginalPath, '_');
		}

		public static string ReplaceInvalidChars(string orginalPath, char replaceChar)
		{
			StringBuilder stringBuilder = new StringBuilder(orginalPath);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				char c = stringBuilder[i];
				if (c == '?' || c == '*')
				{
					stringBuilder[i] = replaceChar;
				}
				else
				{
					foreach (char c2 in Path.GetInvalidPathChars())
					{
						if (c == c2)
						{
							stringBuilder[i] = replaceChar;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetTempFolderPath()
		{
			string text = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(text);
			return text;
		}

		public static string GetTempFilePath()
		{
			return Path.Combine(Path.GetTempPath(), PathTools.GetTempFileName());
		}

		public static string GetTempFileName()
		{
			return Guid.NewGuid().ToString() + ".tmp";
		}

		public static string[] GetFileNames(string[] paths)
		{
			string[] array = new string[paths.Length];
			for (int i = 0; i < paths.Length; i++)
			{
				array[i] = Path.GetFileName(paths[i]);
			}
			return array;
		}

		public static string RootDirectory(string path)
		{
			int num = path.IndexOf(Path.DirectorySeparatorChar);
			if (num < 0)
			{
				return "";
			}
			if (num == 0)
			{
				path = path.Substring(1);
				return PathTools.RootDirectory(path);
			}
			string text = path.Substring(0, num);
			if (text == ".")
			{
				return PathTools.RootDirectory(path.Substring(num));
			}
			if (text.Length > 1 && text[1] == ':')
			{
				return PathTools.RootDirectory(path.Substring(num));
			}
			return text;
		}

		private static void InnerGetFiles(string dirname, string pattern, List<string> files)
		{
			foreach (string dirname2 in Directory.EnumerateDirectories(dirname))
			{
				PathTools.InnerGetFiles(dirname2, pattern, files);
			}
			foreach (string item in Directory.EnumerateFiles(dirname, pattern))
			{
				files.Add(item);
			}
		}

		public static string[] RecursivelyGetFiles(string dirname, string pattern)
		{
			List<string> list = new List<string>();
			PathTools.InnerGetFiles(dirname, pattern, list);
			return list.ToArray();
		}

		public static Regex FilePatternToRegex(string pattern)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException();
			}
			pattern = pattern.Trim();
			if (pattern.Length == 0)
			{
				throw new ArgumentException("Pattern is empty.");
			}
			if (PathTools.IlegalCharactersRegex.IsMatch(pattern))
			{
				throw new ArgumentException("Patterns contains ilegal characters.");
			}
			bool flag = PathTools.CatchExtentionRegex.IsMatch(pattern);
			bool flag2 = false;
			if (PathTools.HasQuestionMarkRegEx.IsMatch(pattern))
			{
				flag2 = true;
			}
			else if (flag)
			{
				flag2 = (PathTools.CatchExtentionRegex.Match(pattern).Groups[1].Length != 3);
			}
			string text = Regex.Escape(pattern);
			text = "^" + Regex.Replace(text, "\\\\\\*", ".*");
			text = Regex.Replace(text, "\\\\\\?", ".");
			if (!flag2 && flag)
			{
				text += PathTools.NonDotCharacters;
			}
			text += "$";
			return new Regex(text, RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		public static bool IsBelow(string root, string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(root);
			for (DirectoryInfo directoryInfo2 = new DirectoryInfo(path); directoryInfo2 != null; directoryInfo2 = directoryInfo2.Parent)
			{
				if (directoryInfo2.FullName == directoryInfo.FullName)
				{
					return true;
				}
			}
			return false;
		}

		public static string MakeRelative(string root, string path)
		{
			root = root.TrimEnd(new char[]
			{
				Path.DirectorySeparatorChar
			});
			path = path.TrimEnd(new char[]
			{
				Path.DirectorySeparatorChar
			});
			DirectoryInfo directoryInfo = new DirectoryInfo(root);
			DirectoryInfo directoryInfo2 = new DirectoryInfo(path);
			Stack<DirectoryInfo> stack = new Stack<DirectoryInfo>();
			for (DirectoryInfo directoryInfo3 = directoryInfo; directoryInfo3 != null; directoryInfo3 = directoryInfo3.Parent)
			{
				stack.Push(directoryInfo3);
			}
			Stack<DirectoryInfo> stack2 = new Stack<DirectoryInfo>();
			for (DirectoryInfo directoryInfo3 = directoryInfo2; directoryInfo3 != null; directoryInfo3 = directoryInfo3.Parent)
			{
				stack2.Push(directoryInfo3);
			}
			if (stack2.Count < stack.Count)
			{
				throw new IOException("path must be under root");
			}
			while (stack.Count > 0 && stack.Peek().FullName == stack2.Peek().FullName)
			{
				stack.Pop();
				stack2.Pop();
			}
			if (stack.Count > 0)
			{
				throw new IOException("path must be under root");
			}
			string text = "";
			while (stack2.Count > 0)
			{
				text = Path.Combine(text, stack2.Pop().Name);
			}
			return text;
		}
	}
}
