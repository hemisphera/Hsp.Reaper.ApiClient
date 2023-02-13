using System.Xml;

namespace Hsp.Reaper.ApiClient;

public class TransportInfo
{

  internal static TransportInfo? Parse(string? line)
  {
    if (String.IsNullOrEmpty(line)) return null;
    var parts = line.Split('\t');
    return new TransportInfo
    {
      State = (TransportPlayState)int.Parse(parts[1]),
      TimePosition = TimeSpan.FromSeconds(XmlConvert.ToDouble(parts[2])),
      BeatPosition = BeatInfo.Parse(parts[4])
    };
  }

  public TransportPlayState State { get; set; }

  public TimeSpan TimePosition { get; set; }

  public BeatInfo BeatPosition { get; set; }


  public override string ToString()
  {
    return $"{State}: {TimePosition}";
  }

}