namespace Hsp.Reaper.ApiClient;

internal class ApiResponseReader
{

  private StringReader Reader { get; }


  public ApiResponseReader(string content)
  {
    Reader = new StringReader(content);
  }


  public async Task<string?> ReadLine()
  {
    return await Reader.ReadLineAsync() ?? null;
  }

  public async Task<string?> ReadLineAndTest(string expectedValue)
  {
    var line = await ReadLine();
    TestValue(line, expectedValue);
    return line;
  }

  private static void TestValue(string? value, string expectedValue)
  {
    if (value != expectedValue) 
      throw new InvalidOperationException($"'{expectedValue}' was expected but '{value}' was found.");
  }

  public async Task ReadUntil(Action<string> action)
  {
    await ReadUntil(null, action);
  }

  public async Task ReadUntil(string? expectedLine, Action<string> action)
  {
    string? nextLine;
    do
    {
      nextLine = await ReadLine();
      if (nextLine != expectedLine && !String.IsNullOrEmpty(nextLine))
        action(nextLine);
    } while (nextLine != expectedLine);
  }

  public async Task<string[]> ReadLineAndSplit(string? expectedFirstToken = null)
  {
    var line = await ReadLine();
    if (String.IsNullOrEmpty(line)) return Array.Empty<string>();
    var parts = line.Split('\t');
    if (!String.IsNullOrEmpty(expectedFirstToken))
      TestValue(parts[0], expectedFirstToken);
    return parts;
  }

}