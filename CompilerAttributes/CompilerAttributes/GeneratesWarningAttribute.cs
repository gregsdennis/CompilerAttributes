using System;

namespace CompilerAttributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class GeneratesWarningAttribute : Attribute
	{
		//public int Id { get; }
		public string Message { get; }

		public GeneratesWarningAttribute(string message)
		{
			//Id = code;
			Message = message;
		}
	}
}