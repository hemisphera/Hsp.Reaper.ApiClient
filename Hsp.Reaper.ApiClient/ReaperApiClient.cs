using Hsp.Reaper.ApiClient.JobScheduler;

namespace Hsp.Reaper.ApiClient;

public class ReaperApiClient : IAsyncDisposable
{

  public Uri BaseUri { get; }

  private HttpClient Client { get; }

  private JobScheduler.JobScheduler Scheduler { get; }


  public ReaperApiClient(Uri baseUri)
  {
    Client = new HttpClient();
    BaseUri = baseUri;
    Scheduler = new JobScheduler.JobScheduler();
    Scheduler.Start();
  }


  private RestClient CreateRestClient(string resource, HttpMethod? method = null)
  {
    return RestClient.Create(Client)
      .Method(method ?? HttpMethod.Get)
      .Uri(new Uri(BaseUri, $"_/{resource}"));
  }


  public async Task<TransportInfo> GetTransportInfo()
  {
    await using var rc = CreateRestClient("TRANSPORT");
    var response = await rc.GetTextResponse();
    return TransportInfo.Parse(await response.ReadLine());
  }

  public async Task<Marker[]> ListMarkers()
  {
    await using var rc = CreateRestClient("MARKER");
    var response = await rc.GetTextResponse();

    var markers = new List<Marker>();
    await response.ReadLineAndTest("MARKER_LIST");
    await response.ReadUntil("MARKER_LIST_END", line => markers.Add(Marker.Parse(line)));

    return markers.ToArray();
  }

  public async Task<Region[]> ListRegions()
  {
    await using var rc = CreateRestClient("REGION");
    var response = await rc.GetTextResponse();

    var regions = new List<Region>();
    await response.ReadLineAndTest("REGION_LIST");
    await response.ReadUntil("REGION_LIST_END", line => regions.Add(Region.Parse(line)));

    return regions.ToArray();
  }

  public async Task GoToMarker(Marker marker)
  {
    await GoToMarker(marker.MarkerId);
  }

  public async Task GoToMarker(int markerId)
  {
    await using var rc = CreateRestClient($"SET/POS_STR/m{markerId}");
    await rc.Send();
  }

  public async Task GoToRegion(Region region)
  {
    await GoToRegion(region.RegionId);
  }

  public async Task GoToRegion(int regionId)
  {
    await using var rc = CreateRestClient($"SET/POS_STR/r{regionId}");
    await rc.Send();
  }

  public async Task SendAction(int actionId)
  {
    await RestClient.Create(Client)
      .Method(HttpMethod.Get)
      .Uri(new Uri(BaseUri, $"_/{actionId}"))
      .Send();
  }

  public async Task Play()
  {
    await SendAction(1007);
  }

  public async Task Stop()
  {
    await SendAction(40667);
  }

  public async Task TogglePause()
  {
    await SendAction(1008);
  }


  public async ValueTask DisposeAsync()
  {
    Client.Dispose();
    await Task.CompletedTask;
  }


  public async Task RegisterCallback(TimeSpan frequency, Func<Task> callback)
  {
    await Scheduler.Enqueue(new ScheduledJob(frequency, callback));
  }

}