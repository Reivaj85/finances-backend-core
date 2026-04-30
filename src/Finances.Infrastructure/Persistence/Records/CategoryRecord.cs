namespace Finances.Infrastructure.Persistence.Records;

public sealed class CategoryRecord
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
