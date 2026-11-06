namespace Wolfgang.Etl.Csv.Tests.Unit.TestModels;

public record PersonRecord
{
    public string FirstName { get; set; } = string.Empty;



    public string LastName { get; set; } = string.Empty;



    public int Age { get; set; }
}
