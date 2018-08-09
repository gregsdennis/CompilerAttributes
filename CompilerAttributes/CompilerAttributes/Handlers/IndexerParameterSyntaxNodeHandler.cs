using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal class IndexerParameterSyntaxNodeHandler : SyntaxNodeHandlerBase<IPropertySymbol, ParameterSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var propertySymbol = (IPropertySymbol) context.ContainingSymbol;
			var parameterSyntax = (ParameterSyntax) context.Node;

			var typeSymbol = propertySymbol.Parameters.Single(p => p.Name == parameterSyntax.Identifier.Text).Type as INamedTypeSymbol;
			var typeSyntax = parameterSyntax.Type;

			return CheckType(typeSymbol, typeSyntax);
		}
	}
}