using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduUniversity
{
    public class GetUniversitiesQuery : IRequest<ApiResponse<object>>
    {
        public UniversityPaginationDTO UniversityPaginationDTO { get; set;}

        public GetUniversitiesQuery(UniversityPaginationDTO universityPaginationDTO)
        {
            UniversityPaginationDTO = universityPaginationDTO;
        }
    }
    public class GetUniversitiesByStudentQuery : IRequest<ApiResponse<object>>
    {
        public UniversityPaginationDTO UniversityPaginationDTO { get; set; }

        public GetUniversitiesByStudentQuery(UniversityPaginationDTO universityPaginationDTO)
        {
            UniversityPaginationDTO = universityPaginationDTO;
        }
    }

    public class GetUniversityByUniqueIdQuery : IRequest<UserWithProfile<University>>
    {
        public Guid _uniqueId { get; set; }

        public GetUniversityByUniqueIdQuery(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }
}