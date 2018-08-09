using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers.Types.InProperties
{
	internal class IndexerDeclarationSyntaxNodeHandler : SyntaxNodeHandlerBase<IPropertySymbol, IndexerDeclarationSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var propertySymbol = ((IPropertySymbol) context.ContainingSymbol).Type as INamedTypeSymbol;
			var indexerSyntax = ((IndexerDeclarationSyntax) context.Node).Type;

			return CheckSymbol(propertySymbol, indexerSyntax);
		}
	}
}