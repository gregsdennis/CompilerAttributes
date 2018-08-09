using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers.Types.InMethods
{
	internal class MethodTypeParameterConstraintSyntaxNodeHandler : SyntaxNodeHandlerBase<IMethodSymbol, TypeParameterConstraintClauseSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var methodSymbol = (IMethodSymbol)context.ContainingSymbol;
			var parameterSyntax = (TypeParameterConstraintClauseSyntax)context.Node;

			var results = new List<SyntaxNodeHandledResult>();

			var constraintSymbols = methodSymbol.TypeParameters.Single(p => p.Name == parameterSyntax.Name.Identifier.Text).ConstraintTypes;

			foreach (var constraintSymbol in constraintSymbols)
			{
				if (!(constraintSymbol is INamedTypeSymbol typeSymbol)) continue;

				var typeSyntax = parameterSyntax.Constraints.OfType<TypeConstraintSyntax>()
					.FirstOrDefault(c => ContainsMatch(c.Type, typeSymbol.Name))?.Type;
				if (typeSyntax == null) continue;

				results.AddRange(CheckSymbol(typeSymbol, typeSyntax));
			}

			return results;
		}

		private static bool ContainsMatch(TypeSyntax type, string name)
		{
			if (type is IdentifierNameSyntax identifier)
				return name == identifier.Identifier.Text;

			if (type is GenericNameSyntax generic)
				return name == generic?.Identifier.Text ||
				       generic.TypeArgumentList.Arguments.Any(a => ContainsMatch(a, name));

			return false;
		}
	}
}