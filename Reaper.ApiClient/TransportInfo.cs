using System.Xml;

namespace Reaper.Api.Client;

public class TransportInfo
{

  internal static TransportInfo Parse(string line)
  {
    var parts = line.Split('\t');
    return new TransportInfo
    {
      State = (TransportPlayState)int.Parse(parts[1]),
      TimePosition = TimeSpan.FromSeconds(XmlConvert.ToDouble(parts[2])),
      BeatPosition = parts[4]
    };
  }

  public TransportPlayState State { get; set; }

  public TimeSpan TimePosition { get; set; }

  public string BeatPosition { get; set; } = string.Empty;


  public override string ToString()
  {
    return $"{State}: {TimePosition}";
  }

}