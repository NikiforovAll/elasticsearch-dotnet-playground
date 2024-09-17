using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nall.NEST.MigtarionAnalyzer
{
    [
        ExportCodeFixProvider(
            LanguageNames.CSharp,
            Name = nameof(UseElasticsearchNetInsteadOfNestCodeFixProvider)
        ),
        Shared
    ]
    public class UseElasticsearchNetInsteadOfNestCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(UseElasticsearchNetInsteadOfNest.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context
                .Document.GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var declaration = root.FindToken(diagnosticSpan.Start)
                .Parent.AncestorsAndSelf()
                .OfType<IdentifierNameSyntax>()
                .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use ElasticsearchClient instead of IElasticClient",
                    createChangedDocument: c =>
                        ReplaceWithElasticsearchClientAsync(context.Document, declaration, c),
                    equivalenceKey: nameof(UseElasticsearchNetInsteadOfNestCodeFixProvider)
                ),
                diagnostic
            );
        }

        private async Task<Document> ReplaceWithElasticsearchClientAsync(
            Document document,
            IdentifierNameSyntax identifierName,
            CancellationToken cancellationToken
        )
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var semanticModel = await document
                .GetSemanticModelAsync(cancellationToken)
                .ConfigureAwait(false);

            // Replace the using directive
            var oldUsingDirective = root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .FirstOrDefault(u => u.Name.ToString() == "Nest");

            if (oldUsingDirective != null)
            {
                var newUsingDirective = SyntaxFactory.UsingDirective(
                    SyntaxFactory.ParseName("Elastic.Clients.Elasticsearch")
                ).WithTriviaFrom(oldUsingDirective); // Preserve trivia
                root = root.ReplaceNode(oldUsingDirective, newUsingDirective);
            }
            else
            {
                // If there's no existing Nest using directive, add the new one
                var firstUsingDirective = root.DescendantNodes()
                    .OfType<UsingDirectiveSyntax>()
                    .FirstOrDefault();
                if (firstUsingDirective != null)
                {
                    var newUsingDirective = SyntaxFactory.UsingDirective(
                        SyntaxFactory.ParseName("Elastic.Clients.Elasticsearch")
                    );
                    root = root.InsertNodesBefore(firstUsingDirective, new[] { newUsingDirective });
                }
            }

            // Replace IElasticClient with ElasticsearchClient
            var nodesToReplace = root.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(node => node.Identifier.ValueText == "IElasticClient");

            root = root.ReplaceNodes(
                nodesToReplace,
                (node, _) => SyntaxFactory.IdentifierName("ElasticsearchClient").WithTriviaFrom(node) // Preserve trivia
            );

            return document.WithSyntaxRoot(root);
        }
    }
}
