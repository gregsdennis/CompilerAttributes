using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal class VariableSymbolHandler : SyntaxNodeHandlerBase<IRangeVariableSymbol, VariableDeclarationSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var fieldType = ((IFieldSymbol)context.ContainingSymbol).Type;
			var attribute = fieldType.GetAttributes()
				.FirstOrDefault(t => t.AttributeClass.Name == CompilerAttributesAnalyzer.ExperimentalAttributeName);

			if (attribute == null) return Enumerable.Empty<SyntaxNodeHandledResult>();

			var fieldTypeIdentifier = ((VariableDeclarationSyntax)context.Node).Type;

			return new[]
			{
				new SyntaxNodeHandledResult
				{
					Name = fieldType.Name,
					Location = Location.Create(fieldTypeIdentifier.SyntaxTree, fieldTypeIdentifier.Span)
				}
			};
		}
	}
}