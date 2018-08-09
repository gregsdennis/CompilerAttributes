using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using CompilerAttributes.Handlers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CompilerAttributesAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "CompilerAttributes";
		public const string ExperimentalAttributeName = "ExperimentalAttribute";
		private const string Category = "Features";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		private static readonly IList<ISyntaxNodeHandler> Handlers = typeof(CompilerAttributesAnalyzer).GetTypeInfo()
			.Assembly
			.DefinedTypes
			.Where(t => typeof(ISyntaxNodeHandler).GetTypeInfo().IsAssignableFrom(t) &&
						!t.IsAbstract && !t.IsInterface)
			.Select(t => Activator.CreateInstance(t.AsType()))
			.Cast<ISyntaxNodeHandler>()
			.ToList();

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode,
				SyntaxKind.MethodDeclaration,
				SyntaxKind.PropertyDeclaration,
				SyntaxKind.FieldDeclaration,
				SyntaxKind.TypeParameter,
				SyntaxKind.TypeParameterConstraintClause,
				SyntaxKind.IndexerDeclaration,
				SyntaxKind.Parameter,
				SyntaxKind.ClassDeclaration);
		}

		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var handlers = Handlers.Where(h => h.Handles(context));

			var results = handlers.SelectMany(h => h.TryHandle(context));

			foreach (var result in results)
			{
				var diagnostic = Diagnostic.Create(Rule, result.Location, result.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
