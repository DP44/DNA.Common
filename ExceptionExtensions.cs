using System;
using System.Reflection;

internal static class ExceptionExtensions 
{
	public static void PreserveStackTrace(this Exception exception) 
	{
		MethodInfo method = typeof(Exception).GetMethod(
			"InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
		
		method.Invoke(exception, null);
	}
}
