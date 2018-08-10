using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CompilerAttributes
{
	internal static class SyntaxNodeHelpers
	{
		public static IEnumerable<IdenifierLocationResult> CheckSymbol(ISymbol symbol, SyntaxNode syntax)
		{
			if (symbol == null || symbol.Name != (syntax as IdentifierNameSyntax)?.Identifier.Text)
				return Enumerable.Empty<IdenifierLocationResult>();

			var attributes = GetAttributes(symbol);

			var results = new List<IdenifierLocationResult>();

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

			foreach (var (attribute, id, message) in attributes)
			{
				if (attribute == null) continue;
				results.Add(new IdenifierLocationResult
					{
						Name = symbol.Name,
						Location = Location.Create(syntax.SyntaxTree, syntax.Span),
						Id = id ?? 0,
						Message = message
					});
			}

			return results;
		}

		private static IEnumerable<(AttributeData Attribute, int? Id, string Message)> GetAttributes(ISymbol symbol)
		{
			var attributes = symbol.GetAttributes()
			                       .Select(t =>
				                       {
					                       var generatorAttribute = t.AttributeClass.GetAttributes()
					                                                 .FirstOrDefault(a => a.AttributeClass.Name == nameof(GeneratesWarningAttribute) &&
					                                                                      a.AttributeClass.ContainingNamespace.Name == nameof(CompilerAttributes));
					                       var attributeArgs = generatorAttribute?.ConstructorArguments
					                                                             .Select(a => a.Value)
					                                                             .ToList();

										   //var id = (int?) attributeArgs?[0];
					                       int? id = 0;
										   var message = (string) attributeArgs?[0];

					                       return (Attribute: t, Id: id, Message: message);
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