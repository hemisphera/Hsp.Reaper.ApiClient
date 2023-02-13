namespace Hsp.Reaper.ApiClient;

public struct BeatInfo
{

  public static BeatInfo Parse(string line)
  {
    var parts = line.Split('.');
    return new BeatInfo
    {
      Measure = int.Parse(parts[0]),
      Beat = int.Parse(parts[1]),
      Frames = int.Parse(parts[2])
    };
  }

  public int Beat { get; set; }

  public int Measure { get; set; }

  public int Frames { get; set; }


  public override string ToString()
  {
    return $"{Measure}.{Beat}.{Frames}";
  }

}