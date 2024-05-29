using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;

namespace EduBlock.Model.DTO;

public class PaginationGuidDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid guid { get; set; }
}

public class PaginationStatusGuidDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public RequestStatus Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid guid { get; set; }
}

public class PaginationUniversityGuidDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid guid { get; set; }
    public Guid UniversityID { get; set; }
}

public class PaginationUniversityUserDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public Guid Guid { get; set; }
    public Guid? SenderId { get; set; }
    public Guid UniversityId { get; set; }
    public Guid DepartmentId { get; set; }
}

public class PaginationReceivedRequestDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public MessageStatus Status { get; set; }
    public Guid Guid { get; set; }
    public Guid? SenderId { get; set; }
    public Guid UniversityId { get; set; }
    public Guid DepartmentId { get; set; }
}

public class PaginationUniversityDepartmentSchemaDTO
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid Guid { get; set; }
    public Guid UniversityId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid SchemaId { get; set; }
}