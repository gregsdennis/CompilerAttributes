using System.Collections.Generic;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal interface ISyntaxNodeHandler
	{
		bool Handles(SyntaxNodeAnalysisContext context);
		IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context);
	}
}