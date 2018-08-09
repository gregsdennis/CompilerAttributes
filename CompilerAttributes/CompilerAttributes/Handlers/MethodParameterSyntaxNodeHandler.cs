using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes.Handlers
{
	internal class MethodParameterSyntaxNodeHandler : SyntaxNodeHandlerBase<IMethodSymbol, ParameterSyntax>
	{
		public override IEnumerable<SyntaxNodeHandledResult> TryHandle(SyntaxNodeAnalysisContext context)
		{
			var methodSymbol = (IMethodSymbol)context.ContainingSymbol;
			var parameterSyntax = (ParameterSyntax)context.Node;

			var typeSymbol = methodSymbol.Parameters.Single(p => p.Name == parameterSyntax.Identifier.Text).Type as INamedTypeSymbol;
			var typeSyntax = parameterSyntax.Type;

			return CheckType(typeSymbol, typeSyntax);
		}
	}

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
				var typeSymbol = constraintSymbol as INamedTypeSymbol;
				if (typeSymbol == null) continue;

				// TODO: this doesn't account for typeSymbol being a generic type with the target as a parameter
				var typeSyntax = parameterSyntax.Constraints.OfType<TypeConstraintSyntax>().First(c => typeSymbol.Name == (c.Type as IdentifierNameSyntax)?.Identifier.Text).Type;

				results.AddRange(CheckType(typeSymbol, typeSyntax));
			}

			return results;
		}
	}
}