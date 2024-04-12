namespace Hsp.Reaper.ApiClient;

internal class ScheduledJob
{

  private static readonly TimeSpan Precision = TimeSpan.FromMilliseconds(50);

  public DateTime NextExecution { get; private set; }

  public TimeSpan? Frequency { get; }

  public Func<Task> Callback { get; }

  public bool IsReady => !Running && NextExecution <= DateTime.Now;

  public int RunCount { get; private set; }

  public bool Running { get; private set; }

  public string Name { get; set; }



  public ScheduledJob(DateTime execution, Func<Task> callback)
  {
    NextExecution = execution;
    Callback = callback;
    Frequency = null;
  }

  public ScheduledJob(DateTime execution, TimeSpan frequency, Func<Task> callback)
    : this(execution, callback)
  {
    Frequency = frequency;
    if (Frequency != null)
      if (Frequency.Value.TotalMilliseconds < Precision.TotalMilliseconds)
        throw new NotSupportedException($"The minimum frequency is {Precision}");
  }

  public ScheduledJob(TimeSpan frequency, Func<Task> callback)
    : this(DateTime.Now, frequency, callback)
  {
  }


  public async Task Execute()
  {
    Running = true;
    RunCount += 1;
    try
    {
      if (Frequency != null)
      {
        // round next execution to compensate for any delays or execution laziness
        var dateTime = DateTime.Now.Add(Frequency.Value);
        var halfIntervalTicks = (Precision.Ticks + 1) >> 1;
        NextExecution = dateTime.AddTicks(halfIntervalTicks - (dateTime.Ticks + halfIntervalTicks) % Precision.Ticks); 
      }
      await Callback();
    }
    finally
    {
      Running = false;
    }
  }

  public async Task Wait()
  {
    while (Running)
      await Task.Delay(1000);
  }

}