using System.Net;

namespace Hsp.Reaper.ApiClient;

internal class ApiException : Exception
{
  public ApiException(HttpStatusCode responseStatusCode, string responseStr)
  {
  }

}