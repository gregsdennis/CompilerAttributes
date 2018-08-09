using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal class MethodSymbolHandler : SyntaxNodeHandlerBase<IMethodSymbol, MethodDeclarationSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var methodSymbol = (IMethodSymbol)context.ContainingSymbol;
			var methodSyntax = (MethodDeclarationSyntax) context.Node;

			var returnSymbol = methodSymbol.ReturnType as INamedTypeSymbol;
			var returnSyntax = methodSyntax.ReturnType;

			return CheckType(returnSymbol, returnSyntax);
		}
	}

}