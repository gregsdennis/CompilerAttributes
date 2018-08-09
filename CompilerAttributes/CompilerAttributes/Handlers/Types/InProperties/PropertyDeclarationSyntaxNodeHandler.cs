using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers.Types.InProperties
{
	internal class PropertyDeclarationSyntaxNodeHandler : SyntaxNodeHandlerBase<IPropertySymbol, PropertyDeclarationSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var propertySymbol = ((IPropertySymbol) context.ContainingSymbol).Type as INamedTypeSymbol;
			var propertySyntax = ((PropertyDeclarationSyntax) context.Node).Type;

			return CheckSymbol(propertySymbol, propertySyntax);
		}
	}
}