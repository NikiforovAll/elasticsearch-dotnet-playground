using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Nall.NEST.MigtarionAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseElasticsearchNetInsteadOfNest : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ELS001";

        private static readonly LocalizableString s_title = "Use ElasticsearchClient";
        private static readonly LocalizableString s_messageFormat =
            "Use ElasticsearchClient instead of IElasticClient";
        private static readonly LocalizableString s_description =
            "NEST is deprecated. Use the official Elasticsearch.Net client instead.";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor s_rule = new DiagnosticDescriptor(
            DiagnosticId,
            s_title,
            s_messageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: s_description
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(s_rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.IdentifierName);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var identifierNode = (IdentifierNameSyntax)context.Node;

            if (identifierNode.Identifier.Text != "IElasticClient")
            {
                return;
            }

            //var symbolInfo = context.SemanticModel.GetSymbolInfo(identifierNode);
            //if (symbolInfo.Symbol == null)
            //{
            //    return;
            //}
            //
            //var containingNamespace = symbolInfo.Symbol.ContainingNamespace;
            //if (containingNamespace == null || containingNamespace.ToString() != "Nest")
            //{
            //    return;
            //}

            var diagnostic = Diagnostic.Create(s_rule, identifierNode.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
