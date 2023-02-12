using System.Text;
using System.Web;
using System.Xml;

namespace Reaper.Api.Client;

internal class RestClient : IAsyncDisposable
{

  private HttpClient Client { get; }

  private bool ClientIsDisposable { get; }

  public static RestClient Create(HttpClient? client = null)
  {
    return new RestClient(client);
  }


  private HttpRequestMessage Request { get; }


  private RestClient(HttpClient? client = null)
  {
    Request = new HttpRequestMessage();
    Client = client ?? new HttpClient();
    ClientIsDisposable = client == null;
  }


  public RestClient Method(HttpMethod method)
  {
    Request.Method = method;
    return this;
  }

  public RestClient Uri(Uri uri)
  {
    Request.RequestUri = uri;
    return this;
  }

  public RestClient JsonPayload(object payload)
  {
    //Request.Content = new StringContent(JsonConvert.SerializeObject(payload, BareJsonSerializerSettings.Instance), Encoding.UTF8, "application/json");
    return this;
  }

  public RestClient StreamPayload(Stream stream)
  {
    Request.Content = new StreamContent(stream);
    return this;
  }

  public RestClient Header(string name, string value)
  {
    Request.Headers.Add(name, value);
    return this;
  }

  public RestClient HeaderIfNotNullOrEmpty(string name, string value)
  {
    if (String.IsNullOrEmpty(value)) return this;
    return Header(name, value);
  }

  public RestClient ApiKey(string value)
  {
    if (String.IsNullOrEmpty(value)) return this;
    Request.Headers.Add("X-EOS-ApiKey", value);
    return this;
  }

  public RestClient QueryParam<T>(string name, T value)
  {
    if (EqualityComparer<T>.Default.Equals(value, default)) return this;
    QueryParam(name, $"{value}");
    return this;
  }

  public RestClient QueryParam(string name, DateTime? date)
  {
    if (date == null) return this;
    return QueryParam(name, XmlConvert.ToString(date.Value, XmlDateTimeSerializationMode.Local));
  }

  public RestClient QueryParam(string name, bool? value)
  {
    if (value == null) return this;
    QueryParam(name, $"{(value == true ? "true" : "false")}");
    return this;
  }

  public RestClient QueryParam(string name, string value)
  {
    if (String.IsNullOrEmpty(value)) return this;

    var ub = new UriBuilder(Request.RequestUri);
    var p = HttpUtility.ParseQueryString(ub.Query);
    p.Set(name, value);
    ub.Query = p.ToString();
    Request.RequestUri = ub.Uri;

    return this;
  }

  public RestClient QueryParamIf(bool condition, string name, string value)
  {
    if (!condition) return this;
    return QueryParam(name, value);
  }


  private async Task<HttpResponseMessage> GetResponse()
  {
    var response = await Client.SendAsync(Request);
    return response;
  }


  /*
  public async Task<T> GetJsonResponse<T>()
  {
    var response = await GetResponse();
    var responseStr = await response.Content.ReadAsStringAsync();
    if (String.IsNullOrEmpty(responseStr))
      responseStr = $"{response.StatusCode}";
    if (!response.IsSuccessStatusCode)
      throw new ApiException(response.StatusCode, responseStr);

    var token = JToken.Parse(responseStr);
    return token.ToObject<T>(BareJsonSerializerSettings.Serializer);
  }
  */

  public async Task<Stream> GetBinaryResponse()
  {
    var response = await GetResponse();
    if (!response.IsSuccessStatusCode)
    {
      var responseStr = await response.Content.ReadAsStringAsync();
      throw new ApiException(response.StatusCode, responseStr);
    }
    return await response.Content.ReadAsStreamAsync();
  }

  public async Task Send()
  {
    var r = await GetTextResponse();
  }

  public async Task<ApiResponseReader> GetTextResponse()
  {
    var response = await GetResponse();
    var responseStr = await response.Content.ReadAsStringAsync();
    if (!response.IsSuccessStatusCode)
      throw new ApiException(response.StatusCode, responseStr);
    return new ApiResponseReader(responseStr);
  }

  public async ValueTask DisposeAsync()
  {
    if (ClientIsDisposable)
      Client.Dispose();
  }

}