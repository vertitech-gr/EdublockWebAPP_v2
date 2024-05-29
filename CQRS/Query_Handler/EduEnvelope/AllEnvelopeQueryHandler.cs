using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEnvelope
{
    public class AllEnvelopeQueryHandler : IRequestHandler<AllEnvelopeQuery, List<EnvelopResponseDTO>>
    {
        private readonly IRepository<Envelope> _envelope;
        private readonly IMediator _mediator;

        public AllEnvelopeQueryHandler(EduBlockDataContext context, IRepository<Envelope> envelope, IMediator mediator)
        {
            _envelope = envelope;
            _mediator = mediator;
        }
        public async Task<List<EnvelopResponseDTO>> Handle(AllEnvelopeQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Envelope> _envelopes = await _envelope.FindAllAsync((e) => e.UserProfileId == request.Userid);

            //if( request.Type == Modal.Enum.EnvelopeShareType.CREATE)
            {
                _envelopes = _envelopes.Where(e => e.Type == Modal.Enum.EnvelopeShareType.CREATE);
            }

            List<EnvelopResponseDTO> listEnvelop = new List<EnvelopResponseDTO>();

            foreach (Envelope enve in _envelopes)
            {
                List<CertificateView> certificateDetails = await _mediator.Send(new EnvelopeRequestGroupQuery(enve.Id));

                var employeeResponseDTO = new EnvelopResponseDTO
                {
                    Name = enve.Name,
                    Id = enve.Id,
                    Status = RequestStatus.Pending,
                    Credentials = certificateDetails,
                    IsDeletedAt = enve.IsDeletedAt,
                    CreatedAt = enve.CreatedAt,
                    UpdatedAt = enve.UpdatedAt,
                    CreatedBy = enve.CreatedBy,
                    UpdatedBy = enve.UpdatedBy
                };
                listEnvelop.Add(employeeResponseDTO);
            }



            switch (request.SortBy.ToLower())
            {
                case "name":
                    return request.OrderBy == SortingOrder.Ascending ?
                    listEnvelop.OrderBy(envelop => envelop.Name).ToList() :
                        listEnvelop.OrderByDescending(envelop => envelop.Name).ToList();
                case "createdat":
                    return request.OrderBy == SortingOrder.Ascending ?
                    listEnvelop.OrderBy(envelop => envelop.CreatedAt).ToList() :
                        listEnvelop.OrderByDescending(envelop => envelop.CreatedAt).ToList();
                default:
                    return listEnvelop.OrderBy(envelop => envelop.Name).ToList();
            }
        }
    }
}
