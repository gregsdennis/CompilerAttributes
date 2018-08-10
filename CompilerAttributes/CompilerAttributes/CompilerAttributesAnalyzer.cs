using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CompilerAttributesAnalyzer : DiagnosticAnalyzer
	{
		private const string Category = "Features";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

		private static readonly DiagnosticDescriptor Rule =
			new DiagnosticDescriptor("ATT0001", Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.IdentifierName);
		}

		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var symbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;

			var results = SyntaxNodeHelpers.CheckSymbol(symbol, context.Node);

			foreach (var result in results)
			{
				var rule = new DiagnosticDescriptor("ATT0001", Title, result.Message, Category, DiagnosticSeverity.Warning, true, Description);
				var diagnostic = Diagnostic.Create(rule, result.Location, result.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
