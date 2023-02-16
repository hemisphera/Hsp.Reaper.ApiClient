using System.Xml;
using Hsp.Reaper.ApiClient.JobScheduler;

namespace Hsp.Reaper.ApiClient;

public class ReaperApiClient : IAsyncDisposable
{

  public Uri BaseUri { get; }

  private HttpClient Client { get; }

  private JobScheduler.JobScheduler Scheduler { get; }

  private bool CanDisposeClient { get; }


  public ReaperApiClient(Uri baseUri, HttpClient? client = null)
  {
    Client = client ?? new HttpClient();
    BaseUri = baseUri;
    Scheduler = new JobScheduler.JobScheduler();
    Scheduler.Start();
    CanDisposeClient = client == null;
  }


  private RestClient CreateRestClient(string resource, HttpMethod? method = null)
  {
    return RestClient.Create(Client)
      .Method(method ?? HttpMethod.Get)
      .Uri(new Uri(BaseUri, $"_/{resource}"));
  }


  public async Task<TransportInfo?> GetTransportInfo()
  {
    await using var rc = CreateRestClient("TRANSPORT");
    var response = await rc.GetTextResponse();
    return TransportInfo.Parse(await response.ReadLine());
  }

  public async Task<BeatPosInfo?> GetBeatPos()
  {
    const string command = "BEATPOS";
    await using var rc = CreateRestClient(command);
    var response = await rc.GetTextResponse();
    return BeatPosInfo.Parse(await response.ReadLine());
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


  public async Task<int> GetTrackCount()
  {
    const string commandText = "NTRACK";
    await using var rc = CreateRestClient(commandText);
    var response = await rc.GetTextResponse();

    var parts = await response.ReadLineAndSplit(commandText);
    return XmlConvert.ToInt32(parts[1]);
  }

  public async Task<Track[]> ListTracks()
  {
    await using var rc = CreateRestClient("TRACK");
    var response = await rc.GetTextResponse();

    var tracks = new List<Track>();
    await response.ReadUntil(line =>
    {
      var track = Track.Parse(line);
      if (track != null) tracks.Add(track);
    });

    return tracks.ToArray();
  }


  public async Task GoToMarker(Marker marker)
  {
    await GoToMarker(marker.MarkerId);
  }

  public async Task GoToMarker(int markerId)
  {
    await GoTo($"m{markerId}");
  }

  public async Task GoToRegion(Region region)
  {
    await GoToRegion(region.RegionId);
  }

  public async Task GoToRegion(int regionId)
  {
    await GoTo($"r{regionId}");
  }


  public async Task GoTo(string expression)
  {
    await using var rc = CreateRestClient($"SET/POS_STR/{expression}");
    await rc.Send();
  }

  public async Task GoTo(double seconds)
  {
    await using var rc = CreateRestClient($"SET/POS/{XmlConvert.ToString(seconds)}");
    await rc.Send();
  }

  public async Task GoTo(BeatInfo beats)
  {
    await GoTo($"{beats}");
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
    if (CanDisposeClient)
      Client.Dispose();
    Scheduler.Stop();
    await Scheduler.DisposeAsync();
  }

  public async Task RegisterCallback(TimeSpan frequency, Func<Task> callback)
  {
    await RegisterCallback(String.Empty, frequency, callback);
  }

  public async Task RegisterCallback(string name, TimeSpan frequency, Func<Task> callback)
  {
    await Scheduler.Enqueue(new ScheduledJob(frequency, callback)
    {
      Name = name
    });
  }

}