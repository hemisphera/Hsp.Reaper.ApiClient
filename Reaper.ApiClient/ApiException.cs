using System.Net;

namespace Reaper.Api.Client;

internal class ApiException : Exception
{
  public ApiException(HttpStatusCode responseStatusCode, string responseStr)
  {
  }

}