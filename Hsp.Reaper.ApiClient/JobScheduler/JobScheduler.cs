namespace Hsp.Reaper.ApiClient.JobScheduler;

public class JobScheduler : IAsyncDisposable
{

  private List<ScheduledJob> Jobs { get; } = new();

  private SemaphoreSlim JobLock { get; } = new(1, 1);


  private CancellationTokenSource Token { get; set; }


  public JobScheduler()
  {
  }


  public async ValueTask DisposeAsync()
  {
    Stop();
    await WaitForAllRunning();
  }


  public async Task Enqueue(ScheduledJob job)
  {
    await JobLock.WaitAsync();
    Jobs.Add(job);
    JobLock.Release();
  }

  public void Start()
  {
    Stop();
    Token = new CancellationTokenSource();
    Task.Run(ProcessLoop, Token.Token);
  }

  private async Task ProcessLoop()
  {
    while (true)
    {
      await Task.Delay(50, Token.Token);
      Token.Token.ThrowIfCancellationRequested();

      ScheduledJob[] tasksToRun;
      try
      {
        await JobLock.WaitAsync();
        var tasksToRemove = Jobs.Where(j => !j.Running && j.RunCount > 0 && j.Frequency == null);
        foreach (var task in tasksToRemove)
          Jobs.Remove(task);

        tasksToRun = Jobs.Where(j => j.IsReady).ToArray();
      }
      finally
      {
        JobLock.Release();
      }

      foreach (var task in tasksToRun)
        task.Execute();
    }
  }

  public void Stop()
  {
    if (Token == null) return;
    Token.Cancel();
  }

  public async Task WaitForAllRunning()
  {
    var runningTasks = Jobs.Where(j => j.Running).ToArray();
    await Task.WhenAll(runningTasks.Select(t => t.Wait()));
  }

}