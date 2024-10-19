using System.Text.Json;
using System.IO;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;
using System.Text.Json.Nodes;


static void Dump(ElasticsearchResponse response)
{
    response.DebugInformation.DisplayAs("application/json");
}

static void DumpResponse(ElasticsearchResponse response)
{
    ToJson(Response(response)).DisplayAs("application/json");
}

static object Response(ElasticsearchResponse response)
{
    return TryParsePayload(GetResponseFromDebugInformation(response.DebugInformation));
}

static void DumpRequest(ElasticsearchResponse response)
{
    ToJson(Request(response)).DisplayAs("application/json");
}

static object Request(ElasticsearchResponse response) 
{
    return TryParsePayload(GetRequestFromDebugInformation(response.DebugInformation));
}

static object TryParsePayload(string payload)
{
    try
    {
        return JsonDocument.Parse(payload.Trim());
    }
    catch
    {
        return payload;
    }
}

static string ToJson(object paylod)
{
    return Indent(prettyJson(System.Text.Json.JsonSerializer.Serialize(paylod)));

    static string prettyJson(string json)
    {
        try
        {
            var jsonDocument = JsonDocument.Parse(json);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string prettyJson = JsonSerializer.Serialize(jsonDocument, options);

            return prettyJson;
        }
        catch (Exception)
        {
            return json;
        }
    }
}

static string Indent(string json)
{
    json = JsonNode.Parse(json).ToJsonString(new JsonSerializerOptions
    {
        WriteIndented = true,
        // Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
    return json;
}

static string GetRequestFromDebugInformation(string paylod)
{
    var request = paylod
        .Split(new[] { "# Request:" }, StringSplitOptions.None)[1]
        .Split(new[] { "# Response:" }, StringSplitOptions.None)[0];

    return request;
}

static string GetResponseFromDebugInformation(string paylod)
{
    var response = paylod.Split(new[] { "# Response:" }, StringSplitOptions.None)[1];

    return response;
}
