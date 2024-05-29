
namespace EduBlock.Model.DTO;

public class PaginationDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}


public class UniversityPaginationDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public Guid StudentId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
