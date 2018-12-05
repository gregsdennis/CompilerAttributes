using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CompilerAttributes
{
	/// <summary>
	/// Provides analysis that generates compiler output for attributes, similarly as it does for the <see cref="ObsoleteAttribute"/>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CompilerAttributesAnalyzer : DiagnosticAnalyzer
	{
		private const string Category = "Features";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

		private static readonly DiagnosticDescriptor WarningRule =
			new DiagnosticDescriptor("ATT0001", Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);
		private static readonly DiagnosticDescriptor ErrorRule =
			new DiagnosticDescriptor("ATT0002", Title, MessageFormat, Category, DiagnosticSeverity.Error, true, Description);
		private static readonly DiagnosticDescriptor InfoRule =
			new DiagnosticDescriptor("ATT0003", Title, MessageFormat, Category, DiagnosticSeverity.Info, true, Description);

		/// <summary>
		/// Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.
		/// </summary>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(WarningRule, ErrorRule, InfoRule);

		/// <summary>
		/// Called once at session start to register actions in the analysis context.
		/// </summary>
		/// <param name="context"></param>
		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode,
			                                 SyntaxKind.IdentifierName,
			                                 SyntaxKind.GenericName);
		}

		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var symbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol;
			var results = context.Node.CheckSymbol(symbol);

			foreach (var result in results)
			{
				DiagnosticDescriptor rule;
				switch (result.Severity)
				{
					case DiagnosticSeverity.Hidden:
						continue;
					case DiagnosticSeverity.Info:
						rule = new DiagnosticDescriptor("ATT0003", Title, result.Message, Category, DiagnosticSeverity.Info, true, Description);
						break;
					case DiagnosticSeverity.Warning:
						rule = new DiagnosticDescriptor("ATT0002", Title, result.Message, Category, DiagnosticSeverity.Warning, true, Description);
						break;
					case DiagnosticSeverity.Error:
						rule = new DiagnosticDescriptor("ATT0001", Title, result.Message, Category, DiagnosticSeverity.Error, true, Description);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				var diagnostic = Diagnostic.Create(rule, result.Location, result.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
