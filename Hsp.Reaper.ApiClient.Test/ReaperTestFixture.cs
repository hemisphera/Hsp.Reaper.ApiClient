namespace Hsp.Reaper.ApiClient.Test;

public class ReaperTestFixture : IAsyncDisposable
{

  private static readonly Uri LocalReaperUri = new Uri("http://localhost:8080");


  public ReaperApiClient Client { get; }


  public ReaperTestFixture()
  {
    Client = new ReaperApiClient(LocalReaperUri);
  }


  public async ValueTask DisposeAsync()
  {
    await Client.DisposeAsync();
  }

}