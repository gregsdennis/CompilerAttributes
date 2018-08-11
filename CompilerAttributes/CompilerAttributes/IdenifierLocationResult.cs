using Microsoft.CodeAnalysis;

namespace CompilerAttributes
{
	internal class IdenifierLocationResult
	{
		public Location Location { get; set; }
		public string Name { get; set; }
		public string Message { get; set; }
		public int Id { get; set; }
		public DiagnosticSeverity Severity { get; set; }
	}
}