using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CompilerAttributes
{
	internal static class SyntaxNodeHelpers
	{
		public static IEnumerable<IdenifierLocationResult> CheckSymbol(this SyntaxNode syntax, ISymbol symbol)
		{
			if (syntax is SimpleNameSyntax identifier) return CheckSymbol(identifier, symbol);

			Debug.WriteLine($"{syntax.GetType()} - {syntax.GetText()}");
			return Enumerable.Empty<IdenifierLocationResult>();
		}

		private static IEnumerable<IdenifierLocationResult> CheckSymbol(SimpleNameSyntax syntax, ISymbol symbol)
		{
			if (symbol == null || symbol.Name != syntax?.Identifier.Text)
				return Enumerable.Empty<IdenifierLocationResult>();

			return CheckSymbolCore(syntax, symbol, nameof(GeneratesWarningAttribute), DiagnosticSeverity.Warning)
				.Union(CheckSymbolCore(syntax, symbol, nameof(GeneratesErrorAttribute), DiagnosticSeverity.Error));
		}

		private static IEnumerable<IdenifierLocationResult> CheckSymbolCore(SyntaxNode syntax, ISymbol symbol,
		                                                                    string searchForAttribute, DiagnosticSeverity severity)
		{
			var attributes = GetAttributes(symbol, searchForAttribute);

			foreach (var (attribute, id, message) in attributes)
			{
				if (attribute == null) continue;
				yield return new IdenifierLocationResult
					{
						Name = symbol.Name,
						Location = Location.Create(syntax.SyntaxTree, syntax.Span),
						Id = id ?? 0,
						Message = message,
						Severity = severity
					};
			}
		}

		private static IEnumerable<(AttributeData Attribute, int? Id, string Message)> GetAttributes(ISymbol symbol, string searchForAttribute)
		{
			return symbol.GetAttributes()
			             .Select(t =>
				             {
					             var generatorAttribute = t.AttributeClass.GetAttributes()
					                                       .FirstOrDefault(a => a.AttributeClass.Name == searchForAttribute &&
					                                                            a.AttributeClass.ContainingNamespace.Name == nameof(CompilerAttributes));

					             if (generatorAttribute == null) return (Attribute: null, Id: 0, Message: null);


								 var attributeArgs = generatorAttribute.ConstructorArguments
					                                                   .Select(a => a.Value)
					                                                   .ToList();

					             //var id = (int?) attributeArgs?[0];
					             int? id = 0;
					             var message = (string) attributeArgs[0];

					             return (Attribute: t, Id: id, Message: message);
				             })
			             .Where(t => t.Attribute != null);
		}
	}
}