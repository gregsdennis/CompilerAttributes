using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal class FieldSymbolHandler : SyntaxNodeHandlerBase<IFieldSymbol, FieldDeclarationSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var fieldType = ((IFieldSymbol)context.ContainingSymbol).Type as INamedTypeSymbol;
			var fieldTypeIdentifier = ((FieldDeclarationSyntax)context.Node).Declaration.Type;

			return CheckType(fieldType, fieldTypeIdentifier);
		}
	}
}