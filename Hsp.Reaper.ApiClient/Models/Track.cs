using System.Xml;

namespace Hsp.Reaper.ApiClient;

public class Track
{

  internal static Track? Parse(string? line)
  {
    if (String.IsNullOrEmpty(line)) return null;
    var parts = line.Split('\t');
    var track = new Track
    {
      TrackNumber = int.Parse(parts[1]),
      Name = parts[2],
      Volume = parts[4],
      Pan = parts[5],
      LastMeterPeak = XmlConvert.ToDouble(parts[6]) / 10,
      LastMeterPos = XmlConvert.ToDouble(parts[7]) / 10,
      Width = parts[8],
      PanMode = parts[9],
      SendCount = XmlConvert.ToInt32(parts[10]),
      ReceiveCount = XmlConvert.ToInt32(parts[11]),
      HardwareOutCount = XmlConvert.ToInt32(parts[12]),
      Color = parts[13]
    };
    track.ParseFlags(parts[3]);
    return track;
  }


  public int TrackNumber { get; set; }

  public string Name { get; set; }

  public string Volume { get; set; }

  public string Pan { get; set; }

  public double LastMeterPeak { get; set; }

  public double LastMeterPos { get; set; }

  public string Width { get; set; }

  public string PanMode { get; set; }

  public int SendCount { get; set; }

  public int ReceiveCount { get; set; }

  public int HardwareOutCount { get; set; }

  public string Color { get; set; }


  public bool Folder { get; set; }

  public bool Selected { get; set; }

  public bool HasFx { get; set; }

  public bool Muted { get; set; }

  public bool Soloed { get; set; }

  public bool SoloedInPlace { get; set; }

  public bool RecordArmed { get; set; }

  public bool RecordMonitoringOn { get; set; }

  public bool RecordMonitoringAuto { get; set; }



  private void ParseFlags(string part)
  {
    var tempInt = XmlConvert.ToInt32(part);
    Folder = (tempInt & 1) == 1;
    Selected = (tempInt & 2) == 2;
    HasFx = (tempInt & 4) == 4;
    Muted = (tempInt & 8) == 8;
    Soloed = (tempInt & 16) == 16;
    SoloedInPlace = (tempInt & 32) == 32;
    RecordArmed = (tempInt & 64) == 64;
    RecordMonitoringOn = (tempInt & 128) == 128;
    RecordMonitoringAuto = (tempInt & 256) == 256;
  }


  public override string ToString()
  {
    return $"{TrackNumber}: {Name}";
  }

}