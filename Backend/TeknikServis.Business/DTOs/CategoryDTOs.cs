using TeknikServis.Domain.Enums;

namespace TeknikServis.Business.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CategoryType Type { get; set; }
    public string TypeName { get; set; } = null!;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string? ParentName { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
    public int ProductCount { get; set; }
}

public class CreateCategoryRequest
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CategoryType Type { get; set; } = CategoryType.Product;
    public int SortOrder { get; set; } = 0;
}

public class UpdateCategoryRequest
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CategoryType Type { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CategoryTreeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public CategoryType Type { get; set; }
    public List<CategoryTreeDto> Children { get; set; } = new();
}
