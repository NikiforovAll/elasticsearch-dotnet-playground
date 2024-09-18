#pragma warning disable CS8321 // Local function is declared but never used

using System.Text.Json;
using Elastic.Transport;
using Nest;

// ElasticClient -> ElasticsearchClient
// IElasticClient -> ElasticsearchClient
// ConnectionSettings -> ElasticsearchClientSettings

var settings = new ConnectionSettings(new Uri("https://elastic:elastic@127.0.0.1:9200/"))
    .DisableDirectStreaming()
    .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

IElasticClient client = new ElasticClient(settings);

var nodeInfo = await client.Nodes.InfoAsync();

Console.WriteLine(
    JsonSerializer.Serialize(nodeInfo, new JsonSerializerOptions { WriteIndented = true })
);

static IElasticClient Create1(ConnectionSettings settings)
{
    return new ElasticClient(settings);
}
static ElasticClient Create2(ConnectionSettings settings)
{
    return new ElasticClient(settings);
}
