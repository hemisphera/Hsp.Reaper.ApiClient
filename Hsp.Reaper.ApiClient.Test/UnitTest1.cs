namespace Hsp.Reaper.ApiClient.Test;

public class UnitTest1 : IClassFixture<ReaperTestFixture>
{

  private ReaperTestFixture Fixture { get; }

  public UnitTest1(ReaperTestFixture fixture)
  {
    Fixture = fixture;
  }


  [Fact]
  public async Task GoToMarker()
  {
    var cl = Fixture.Client;
    var markers = await cl.ListMarkers();
    Assert.NotEmpty(markers);
    Assert.Equal("Marker One", markers.First().MarkerName);

    var ti = await cl.GetTransportInfo();
    await cl.GoToMarker(markers.Last());
    ti = await cl.GetTransportInfo();
  }

  [Fact]
  public async Task ReadTracks()
  {
    var cl = Fixture.Client;
    var trackCount = await cl.GetTrackCount();
    Assert.Equal(4, trackCount);
    var tracks = await cl.ListTracks();
    Assert.Equal(trackCount + 1, tracks.Length); // master is included
    for (var i = 1; i <= 3; i++)
    {
      Assert.Equal(i == 1, tracks[i].Selected);
      Assert.Equal(i == 2, tracks[i].Muted);
      Assert.Equal(i == 3, tracks[i].Soloed);
    }
  }

  [Fact]
  public async Task GeatBeatPos()
  {
    var cl = Fixture.Client;
    var regions = await cl.ListRegions();
    await cl.GoToRegion(regions.First(r => r.Name == "Second Song"));
    var pos = await cl.GetBeatPos();
    Assert.NotNull(pos);
    Assert.Equal(4, pos.Numerator);
    Assert.Equal(4, pos.Denominator);

    await cl.GoToRegion(regions.First(r => r.Name == "Third Song"));
    pos = await cl.GetBeatPos();
    Assert.NotNull(pos);
    Assert.Equal(5, pos.Numerator);
    Assert.Equal(8, pos.Denominator);
  }

}