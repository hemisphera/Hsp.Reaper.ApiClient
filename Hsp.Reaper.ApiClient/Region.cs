using System.Xml;

namespace Hsp.Reaper.ApiClient;

public class Region
{

  public static Region Parse(string line)
  {
    var parts = line.Split('\t');
    return new Region
    {
      Name = parts[1],
      RegionId = int.Parse(parts[2]),
      Start = TimeSpan.FromSeconds(XmlConvert.ToDouble(parts[3])),
      End = TimeSpan.FromSeconds(XmlConvert.ToDouble(parts[4]))
    };
  }

  public TimeSpan Start { get; set; }

  public TimeSpan End { get; set; }

  public TimeSpan Duration => End.Subtract(Start);

  public string Name { get; set; }

  public int RegionId { get; set; }


  public override string ToString()
  {
    return $"{RegionId}: {Name}";
  }

}