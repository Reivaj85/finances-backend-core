using Finances.Domain.Common;

namespace Finances.Domain.Categories;

public sealed class Category : Entity<CategoryId>
{
    private Category(CategoryId id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; }

    public static Result<Category> Create(CategoryId id, string name)
    {
        return Result<Category>.Success(new Category(id, name.Trim()));
    }
}
