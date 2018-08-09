using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal abstract class SyntaxNodeHandlerBase<TSymbol, TSyntax> : ISyntaxNodeHandler
		where TSymbol :	ISymbol
		where TSyntax : SyntaxNode
	{
		public virtual bool Handles(SyntaxNodeAnalysisContext context)
		{
			return context.ContainingSymbol is TSymbol && context.Node is TSyntax;
		}

		public abstract IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context);

		protected static IEnumerable<SyntaxNodeHandledResult> CheckType(INamedTypeSymbol symbol, TypeSyntax syntax)
		{
			if (symbol == null) return Enumerable.Empty<SyntaxNodeHandledResult>();

			var attribute = symbol.GetAttributes()
				.FirstOrDefault(t => t.AttributeClass.Name == CompilerAttributesAnalyzer.ExperimentalAttributeName);
			var results = new List<SyntaxNodeHandledResult>();

			if (attribute != null)
				results.Add(new SyntaxNodeHandledResult
				{
					Name = symbol.Name,
					Location = Location.Create(syntax.SyntaxTree, syntax.Span)
				});

			if (symbol is INamedTypeSymbol namedSymbol && namedSymbol.IsGenericType)
			{
				var genericArgumentSymbols = namedSymbol.TypeArguments;
				var genericArgumentSyntaxes = syntax.DescendantNodes().OfType<TypeSyntax>();

				var zipped = genericArgumentSymbols.Zip(genericArgumentSyntaxes,
					(sym, syn) => new {sym, syn});

				foreach (var arg in zipped)
				{
					results.AddRange(CheckType(arg.sym as INamedTypeSymbol, arg.syn));
				}
			}

			return results;
		}
	}
}