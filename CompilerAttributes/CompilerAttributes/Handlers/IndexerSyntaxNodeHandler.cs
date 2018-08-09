using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal class IndexerSyntaxNodeHandler : SyntaxNodeHandlerBase<IPropertySymbol, IndexerDeclarationSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var propertySymbol = ((IPropertySymbol) context.ContainingSymbol).Type as INamedTypeSymbol;
			var indexerSyntax = ((IndexerDeclarationSyntax) context.Node).Type;

			return CheckType(propertySymbol, indexerSyntax);
		}
	}
}