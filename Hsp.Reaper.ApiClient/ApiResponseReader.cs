namespace Hsp.Reaper.ApiClient;

internal class ApiResponseReader
{

  private StringReader Reader { get; }


  public ApiResponseReader(string content)
  {
    Reader = new StringReader(content);
  }


  public async Task<string> ReadLine()
  {
    return await Reader.ReadLineAsync() ?? String.Empty;
  }

  public async Task<string> ReadLineAndTest(string expectedValue)
  {
    var line = await ReadLine();
    if (line != expectedValue) throw new InvalidOperationException($"'{expectedValue}' was expected.");
    return line;
  }

  public async Task ReadUntil(string expectedLine, Action<string> action)
  {
    string nextLine;
    do
    {
      nextLine = await ReadLine();
      if (nextLine != expectedLine)
        action(nextLine);
    } while (nextLine != expectedLine);
  }

}