using System;

namespace CompilerAttributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GeneratesWarningAttribute : Attribute
	{
		public string Message { get; }

		public GeneratesWarningAttribute(string message)
		{
			Message = message;
		}
	}
}