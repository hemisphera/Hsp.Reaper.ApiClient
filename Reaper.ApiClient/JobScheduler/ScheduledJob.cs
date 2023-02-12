namespace Reaper.Api.Client;

public class ScheduledJob
{

  public DateTime NextExecution { get; private set; }

  public TimeSpan? Frequency { get; }

  public Func<Task> Callback { get; }

  public bool IsReady => !Running && NextExecution <= DateTime.Now;

  public int RunCount { get; private set; }

  public bool Running { get; private set; }




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
      await Callback();
      if (Frequency != null)
        NextExecution = DateTime.Now.Add(Frequency.Value);
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