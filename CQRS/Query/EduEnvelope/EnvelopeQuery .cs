using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.Modal.Enum;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query.EduEnvelope
{
    public class EnvelopeQuery : IRequest<Envelope>
    {
        public string _name { get; }
        public EnvelopeQuery(string name)
        {
            _name = name;
        }
    }


    public class GetEmployeeByUniqueIdQuery : IRequest<UserWithProfile<Employer>>
    {
        public Guid _uniqueId { get; set; }

        public GetEmployeeByUniqueIdQuery(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }

    public class GetEmployeeByProfileIdQuery : IRequest<UserWithProfile<Employer>>
    {
        public Guid _uniqueId { get; set; }

        public GetEmployeeByProfileIdQuery(Guid uniqueId)
        {
            _uniqueId = uniqueId;
        }
    }


    public class AllEnvelopeQuery : IRequest<List<EnvelopResponseDTO>>
    {
        public Guid Userid { get; }
        public string SortBy { get; set; }
        public SortingOrder OrderBy { get; set; }
        public EnvelopeShareType Type { get; set; }

        public AllEnvelopeQuery(Guid userid, string sortBy, SortingOrder orderBy, EnvelopeShareType type)
        {
            Userid = userid;
            SortBy = sortBy;
            OrderBy = orderBy;
            Type = type;
        }
    }


}

