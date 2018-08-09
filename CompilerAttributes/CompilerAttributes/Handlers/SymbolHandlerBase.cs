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

		protected static IEnumerable<SyntaxNodeHandledResult> CheckSymbol(ISymbol symbol, SyntaxNode syntax)
		{
			if (symbol == null) return Enumerable.Empty<SyntaxNodeHandledResult>();

			var attributes = GetAttributes(symbol);

			var results = new List<SyntaxNodeHandledResult>();

			foreach (var attribute in attributes)
			{
				if (symbol is INamedTypeSymbol namedSymbol && namedSymbol.IsGenericType)
				{
					var genericArgumentSymbols = namedSymbol.TypeArguments;
					var genericArgumentSyntaxes = syntax.DescendantNodes().OfType<TypeSyntax>();

					var zipped = genericArgumentSymbols.Zip(genericArgumentSyntaxes,
						(sym, syn) => new {sym, syn});

					foreach (var arg in zipped)
					{
						results.AddRange(CheckSymbol(arg.sym as INamedTypeSymbol, arg.syn));
					}

					continue;
				}

				if (attribute != null)
					results.Add(new SyntaxNodeHandledResult
					{
						Name = symbol.Name,
						Location = Location.Create(syntax.SyntaxTree, syntax.Span)
					});
			}

			return results;
		}

		private static IEnumerable<AttributeData> GetAttributes(ISymbol symbol)
		{
			var attributes = symbol.GetAttributes()
				.Where(t =>
					t.AttributeClass.GetAttributes()
						.Any(a => a.AttributeClass.Name == nameof(GeneratesWarningAttribute) &&
						          a.AttributeClass.ContainingNamespace.Name == nameof(CompilerAttributes)));

			// TODO: refactor this into an extension method
			if (!(symbol is INamedTypeSymbol namedSymbol && namedSymbol.IsGenericType)) return attributes;

			var genericArgumentSymbols = namedSymbol.TypeArguments;
			attributes = attributes.Union(genericArgumentSymbols.SelectMany(GetAttributes));

			return attributes;
		}
	}
}