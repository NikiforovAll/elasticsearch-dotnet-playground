using System.Threading.Tasks;
using Analyzer.Test;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace Nall.NEST.MigtarionAnalyzer.Test;

[TestClass]
public class UseElasticsearchNetInsteadOfNestTests
{
    [TestMethod]
    public async Task TestNestUsage()
    {
        var test =
            @"
                using Nest;
                public class ElasticsearchService
                {
                    private readonly {|#0:IElasticClient|} _client;

                    public ElasticsearchService({|#1:IElasticClient|} client)
                    {
                        _client = client;
                    }
                }";

        var expected1 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(0)
            .WithArguments("IElasticClient");

        var expected2 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(1)
            .WithArguments("IElasticClient");

        await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    [TestMethod]
    public async Task TestNestUsageAsClass()
    {
        var test =
            @"
                using Nest;
                public class ElasticsearchService
                {
                    private readonly {|#0:ElasticClient|} _client;

                    public ElasticsearchService({|#1:ElasticClient|} client)
                    {
                        _client = client;
                    }
                }";

        var expected1 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(0)
            .WithArguments("ElasticClient");

        var expected2 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(1)
            .WithArguments("ElasticClient");

        await VerifyCS.VerifyAnalyzerAsync(test, expected1, expected2);
    }

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public async Task TestCodeFix()
    {
        var test = """
            using Nest;

            public class ElasticsearchService
            {
                private readonly {|#0:IElasticClient|} _client;
            }
            """;

        var fixtest = """
            using Elastic.Clients.Elasticsearch;

            public class ElasticsearchService
            {
                private readonly ElasticsearchClient _client;
            }
            """;

        var expected = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(0)
            .WithArguments("IElasticClient");

        await VerifyCS.VerifyCodeFixAsync(test, [expected], fixtest);
    }

    [TestMethod]
    public async Task TestCodeFixPreservesWhitespace()
    {
        var test = """
            using Nest;

            public class ElasticsearchService
            {
                private readonly    {|#0:IElasticClient|}    _client;

                public ElasticsearchService(   {|#1:IElasticClient|}   client)
                {
                    _client = client;
                }
            }
            """;

        var fixtest = """
            using Elastic.Clients.Elasticsearch;

            public class ElasticsearchService
            {
                private readonly    ElasticsearchClient    _client;

                public ElasticsearchService(   ElasticsearchClient   client)
                {
                    _client = client;
                }
            }
            """;

        var expected1 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(0)
            .WithArguments("IElasticClient");

        var expected2 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(1)
            .WithArguments("IElasticClient");

        await VerifyCS.VerifyCodeFixAsync(test, [expected1, expected2], fixtest);
    }

    [TestMethod]
    public async Task TestCodeFixWithSettings()
    {
        var test = """
            using Nest;

            public class ElasticsearchService
            {
                private readonly {|#0:ElasticClient|} _client;

                public ElasticsearchService()
                {
                    var settings = new ConnectionSettings(new Uri("https://elastic:elastic@127.0.0.1:9200/"));
                    _client = new {|#1:ElasticClient|}(settings);
                }
            }
            """;

        var fixtest = """
            using Elastic.Clients.Elasticsearch;

            public class ElasticsearchService
            {
                private readonly ElasticsearchClient _client;

                public ElasticsearchService()
                {
                    var settings = new ElasticsearchClientSettings(new Uri("https://elastic:elastic@127.0.0.1:9200/"));
                    _client = new ElasticsearchClient(settings);
                }
            }
            """;

        var expected1 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(0)
            .WithArguments("ElasticClient");

        var expected2 = VerifyCS
            .Diagnostic("ELS001")
            .WithLocation(1)
            .WithArguments("ElasticClient");

        await VerifyCS.VerifyCodeFixAsync(test, [expected1, expected2], fixtest);
    }
}

public static class VerifyCS
{
    private static readonly MetadataReference s_nestReference = MetadataReference.CreateFromFile(
        typeof(IElasticClient).Assembly.Location
    );
    private static readonly MetadataReference s_elasticReference = MetadataReference.CreateFromFile(
        typeof(Elastic.Clients.Elasticsearch.ElasticsearchClient).Assembly.Location
    );

    public static DiagnosticResult Diagnostic(string diagnosticId) =>
        CSharpAnalyzerVerifier<UseElasticsearchNetInsteadOfNest, MSTestVerifier>.Diagnostic(
            diagnosticId
        );

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test { TestCode = source };
        test.TestState.AdditionalReferences.Add(s_nestReference);
        test.TestState.AdditionalReferences.Add(s_elasticReference);
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync();
    }

    public static Task VerifyCodeFixAsync(
        string source,
        DiagnosticResult[] expected,
        string fixedSource
    )
    {
        var test = new Test { TestCode = source, FixedCode = fixedSource };
        test.TestState.AdditionalReferences.Add(s_nestReference);
        test.TestState.AdditionalReferences.Add(s_elasticReference);
        test.ExpectedDiagnostics.AddRange(expected);
        // we need this becaues of
        // error CS1705: Assembly 'Elastic.Clients.Elasticsearch' with identity 'Elastic.Clients.Elasticsearch, Version=8.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d' uses 'System.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' which has a higher version than referenced assembly 'System.Runtime' with identity 'System.Runtime, Version=4.2.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
        test.CompilerDiagnostics = CompilerDiagnostics.None;
        return test.RunAsync();
    }

    private class Test
        : CSharpCodeFixTest<
            UseElasticsearchNetInsteadOfNest,
            UseElasticsearchNetInsteadOfNestCodeFixProvider,
            MSTestVerifier
        >
    {
        public Test()
        {
            SolutionTransforms.Add(
                (solution, projectId) =>
                {
                    var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                    compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                        compilationOptions.SpecificDiagnosticOptions.SetItems(
                            CSharpVerifierHelper.NullableWarnings
                        )
                    );
                    solution = solution.WithProjectCompilationOptions(
                        projectId,
                        compilationOptions
                    );

                    return solution;
                }
            );
        }
    }
}
