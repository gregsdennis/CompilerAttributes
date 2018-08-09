using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal abstract class SyntaxNodeHandlerBase<TSymbol, TSyntax> : ISyntaxNodeHandler
		where TSymbol : ISymbol
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
			}

			foreach (var attribute in attributes)
			{
				if (attribute.Attribute != null)
					results.Add(new SyntaxNodeHandledResult
						{
							Name = symbol.Name,
							Location = Location.Create(syntax.SyntaxTree, syntax.Span),
							Message = attribute.Message
						});
			}

			return results;
		}

		private static IEnumerable<(AttributeData Attribute, string Message)> GetAttributes(ISymbol symbol)
		{
			var attributes = symbol.GetAttributes()
			                       .Select(t =>
				                       {
					                       var generatorAttribute = t.AttributeClass.GetAttributes()
					                                                 .FirstOrDefault(a => a.AttributeClass.Name == nameof(GeneratesWarningAttribute) &&
					                                                                      a.AttributeClass.ContainingNamespace.Name == nameof(CompilerAttributes));

					                       var message = (string) generatorAttribute?.ConstructorArguments
					                                                                .FirstOrDefault()
					                                                                .Value;

					                       return (Attribute: t, Message: message);
				                       })
			                       .Where(t => t.Attribute != null);

			// TODO: refactor this into an extension method
			if (!(symbol is INamedTypeSymbol namedSymbol && namedSymbol.IsGenericType)) return attributes;

			var genericArgumentSymbols = namedSymbol.TypeArguments;
			attributes = attributes.Union(genericArgumentSymbols.SelectMany(GetAttributes));

			return attributes;
		}
	}
}