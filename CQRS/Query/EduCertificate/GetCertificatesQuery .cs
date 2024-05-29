using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduCertificate
{
    public class GetCertificatesQuery : IRequest<List<Certificate>>
    {
        public CommanUser User;
        public GetCertificatesQuery(CommanUser user)
        {
            User = user;
        }
    }

    public class GetCertificateResponseQuery : IRequest<ApiResponse<object>>
    {
        public CommanUser User;
        public PaginationUniversityUserDTO PaginationUniversityUserDTO;
        public GetCertificateResponseQuery(CommanUser user, PaginationUniversityUserDTO paginationUniversityUserDTO)
        {
            User = user;
            PaginationUniversityUserDTO = paginationUniversityUserDTO;
        }
    }

    public class GetCertificatesQueryById : IRequest<Certificate>
    {
        public Guid CertificateId;
        public GetCertificatesQueryById(Guid _certificateId)
        {
            CertificateId = _certificateId;
        }

    }

    public class GetCertificateUrlQuery : IRequest<string>
    {
        public Guid CertificateId;
        public GetCertificateUrlQuery(Guid _certificateId)
        {
            CertificateId = _certificateId;
        }
    }

    public class CertificateExistsQuery : IRequest<bool>
    {
        public Guid CertificateId;
        public CertificateExistsQuery(Guid _certificateId)
        {
            CertificateId = _certificateId;
        }

    }

}
