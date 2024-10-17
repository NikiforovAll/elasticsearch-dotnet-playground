using System.Text.Json;
using System.IO;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;


static object DumpGetRequest(ElasticsearchResponse response)
{
    return DumpGetRequest(response.DebugInformation);
}

static object DumpGetRequest(string debugInformation) 
{
    return TryParsePayload(GetRequestFromDebugInformation(debugInformation));
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
    return prettyJson(System.Text.Json.JsonSerializer.Serialize(paylod));

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
