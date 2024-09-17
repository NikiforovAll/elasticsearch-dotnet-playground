using System.Text.Json;
using Elastic.Transport;
using Nest;

var settings = new ConnectionSettings(new Uri("https://elastic:elastic@127.0.0.1:9200/"))
    .DisableDirectStreaming()
    .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

var client = new ElasticClient(settings);

var nodeInfo = await client.Nodes.InfoAsync();

Console.WriteLine(
    JsonSerializer.Serialize(nodeInfo, new JsonSerializerOptions { WriteIndented = true })
);
