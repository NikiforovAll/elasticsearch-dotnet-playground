using System.Text.Json;
using System.IO;
using Nest;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;

static void ReportAndAssert(
    string testCaseName,
    IResponse nestResponse,
    ElasticsearchResponse elasticResponse)
{
    ReportAndAssert(testCaseName, nestResponse.DebugInformation, elasticResponse.DebugInformation);
}

static void ReportAndAssert(string testCaseName, string nestResponse, string elasticResponse)
{
    display($"Test case: '{testCaseName}' 🚀");
    display(nestResponse);
    display(elasticResponse);

    var file1 = $"../report/{testCaseName}.nest.txt";
    var file2 = $"../report/{testCaseName}.net.txt";
    var (payload1, payload2) = (GetRequestResponseString(nestResponse), GetRequestResponseString(elasticResponse));

    System.IO.File.WriteAllText(file1, payload1);
    System.IO.File.WriteAllText(file2, payload2);

    try
    {
        Xunit.Assert.Equal(payload1.Substring(0, payload1.IndexOf("Response:")), payload2.Substring(0, payload2.IndexOf("Response:")));

        display($"'{testCaseName}' passed ✅");
    }
    catch (Xunit.Sdk.XunitException ex)
    {
        display(ex.Message);
        display($"'{testCaseName}' failed ❌");
    }
}

static void DumpResponse(IResponse response)
{
    DumpResponseCore(response.DebugInformation);
}
static void DumpResponse(ElasticsearchResponse response)
{
    DumpResponseCore(response.DebugInformation);
}

static void DumpResponseCore(string debugInformation)
{
    display(GetGeneralInfoFromDebugInformation(debugInformation));
    display("⚙️❔Request:");
    display(TryParsePayload(GetRequestFromDebugInformation(debugInformation)));
    display("⚙️🟰Response:");
    display(TryParsePayload(GetResponseFromDebugInformation(debugInformation)));
}

static object TryParsePayload(string payload)
{
    try
    {
        return JsonDocument.Parse(payload);
    }
    catch
    {
        return string.Empty;
    }
}

static string GetRequestResponseString(string paylod)
{
    var request = GetRequestFromDebugInformation(paylod);

    var response = GetResponseFromDebugInformation(paylod);

    var resultPayload = $"""
        Request:
        {prettyJson(request)}
        Response:
        {prettyJson(response)}
        """;

    return resultPayload;

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

static string GetGeneralInfoFromDebugInformation(string paylod)
{
    var response = paylod.Split(new[] { "# Request:" }, StringSplitOptions.None)[0];

    return response;
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
