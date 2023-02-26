namespace Pdsr.Queue.SampleWebApp;

/// <summary>
/// Some message sample
/// </summary>
public class SampleMessage
{
    public SampleMessage(string item)
    {
        Item = item;
    }
    public string Item { get;  }
}
