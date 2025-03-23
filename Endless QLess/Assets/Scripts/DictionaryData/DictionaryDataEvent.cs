public abstract record class DictionaryDataEvent;
public sealed record class DictionaryLogEvent(string Message) : DictionaryDataEvent;
public sealed record class DictionaryLoadStarted : DictionaryDataEvent
{
    public static readonly DictionaryLoadStarted Instance = new();
}
public sealed record class DictionaryLoadProgress(float Percentage) : DictionaryDataEvent;
public sealed record class DictionaryLoadComplete : DictionaryDataEvent
{
    public static readonly DictionaryLoadComplete Instance = new();
}