namespace Wolfgang.Etl.Csv.Examples.RecordValidation;

// Captures the most recent progress report so the final CurrentInvalidItemCount can be read after the run.
internal sealed class LastValueProgress<T> : IProgress<T>
{
    public T? Value { get; private set; }

    public void Report(T value) => Value = value;
}
