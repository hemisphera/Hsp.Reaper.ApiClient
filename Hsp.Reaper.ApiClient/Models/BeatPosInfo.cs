using System.Xml;

namespace Hsp.Reaper.ApiClient;

public class BeatPosInfo
{

  internal static BeatPosInfo? Parse(string? input)
  {
    if (String.IsNullOrEmpty(input)) return null;
    var parts = input.Split('\t');
    return new BeatPosInfo
    {
      PlayState = (TransportPlayState)XmlConvert.ToInt32(parts[1]),
      TimePosition = TimeSpan.FromSeconds(XmlConvert.ToDouble(parts[2])),
      FullBeatsPosition = XmlConvert.ToDouble(parts[3]),
      MeasureCount = XmlConvert.ToInt32(parts[4]),
      BeatsInMeasure = XmlConvert.ToDouble(parts[5]),
      Numerator = XmlConvert.ToInt32(parts[6]),
      Denominator = XmlConvert.ToInt32(parts[7])
    };
  }


  public TimeSpan TimePosition { get; set; }

  public TransportPlayState PlayState { get; set; }

  public double FullBeatsPosition { get; set; }

  public int MeasureCount { get; set; }

  public double BeatsInMeasure { get; set; }

  public int Numerator { get; set; }

  public int Denominator { get; set; }

}