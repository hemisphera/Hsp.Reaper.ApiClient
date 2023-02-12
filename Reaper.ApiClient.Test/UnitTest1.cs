namespace Reaper.Api.Client.Test
{
  public class UnitTest1
  {

    [Fact]
    public async Task Test1()
    {
      var cl = new ReaperApiClient(new Uri("http://localhost:8080"));
      var markers = await cl.ListMarkers();

      var ti = await cl.GetTransportInfo();
      //await cl.GoTo(markers.Last());
      ti = await cl.GetTransportInfo();
    }

  }
}