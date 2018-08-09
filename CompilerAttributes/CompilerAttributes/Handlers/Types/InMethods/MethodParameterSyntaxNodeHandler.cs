using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers.Types.InMethods
{
	internal class MethodParameterSyntaxNodeHandler : SyntaxNodeHandlerBase<IMethodSymbol, ParameterSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var methodSymbol = (IMethodSymbol)context.ContainingSymbol;
			var parameterSyntax = (ParameterSyntax)context.Node;

			var typeSymbol = methodSymbol.Parameters.Single(p => p.Name == parameterSyntax.Identifier.Text).Type as INamedTypeSymbol;
			var typeSyntax = parameterSyntax.Type;

			return CheckSymbol(typeSymbol, typeSyntax);
		}
	}
}