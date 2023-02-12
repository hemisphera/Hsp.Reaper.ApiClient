using System.Xml;

namespace Reaper.Api.Client;

public class Marker
{

  internal static Marker Parse(string line)
  {
    var parts = line.Split('\t');
    return new Marker
    {
      MarkerName = parts[1],
      MarkerId = int.Parse(parts[2]),
      Position = TimeSpan.FromSeconds(XmlConvert.ToDouble(parts[3]))
    };
  }

  public int MarkerId { get; set; }

  public string MarkerName { get; set; }

  public TimeSpan Position { get; set; }


  public override string ToString()
  {
    return $"{MarkerId}: {MarkerName}";
  }

}