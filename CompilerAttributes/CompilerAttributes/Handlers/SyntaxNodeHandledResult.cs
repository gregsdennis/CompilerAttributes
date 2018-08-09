using Microsoft.CodeAnalysis;

namespace CompilerAttributes.Handlers
{
	internal class SyntaxNodeHandledResult
	{
		public Location Location { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
	}
}