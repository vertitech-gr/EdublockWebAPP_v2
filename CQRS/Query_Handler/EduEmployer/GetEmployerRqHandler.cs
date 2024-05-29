using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Query.EduEmployeeRequest;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;
using Edu_Block_dev.Helpers;
using System.Linq.Expressions;

namespace Edu_Block_dev.CQRS.Query_Handler.EduEmployer
{
    public class GetEmployerRqHandler : IRequestHandler<GetEmployerRq, ApiResponse<object>>
    {
        private readonly IRepository<Request> _request;
        private readonly IRepository<User> _user;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<UserProfile> _userProfile;
        private readonly IRepository<Share> _share;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Role> _roleRepository;

        public GetEmployerRqHandler(IRepository<Role> roleRepository, EduBlockDataContext context, IRepository<Employer> employerRepository, IRepository<Share> share, IConfiguration configuration, IRepository<User> user, IRepository<UserProfile> userProfile, IRepository<Request> request, IMediator mediator)
        {
            _request = request;
            _mediator = mediator;
            _user = user;
            _configuration = configuration;
            _userProfile = userProfile;
            _share = share;
            _employerRepository = employerRepository;
            _roleRepository = roleRepository;
        }

        public async Task<ApiResponse<object>> Handle(GetEmployerRq request, CancellationToken cancellationToken)
        {
            Employer employer = null;
            UserProfile userProfile = null;
            Role role = await _roleRepository.FindAsync(r => r.NormalizedName == Authorization.Role.ADMIN.ToString());

            if (request.User == null)
            {
                if(request.PaginationStatusGuidDTO.guid != Guid.Empty)
                {
                    employer = await _employerRepository.FindAsync(e => e.Id == request.PaginationStatusGuidDTO.guid);
                    userProfile = await _userProfile.FindAsync(u => u.UserID == employer.Id);

                }else
                {
                    employer = null;
                }
            }
            else
            {
                if (request.PaginationStatusGuidDTO.guid != Guid.Empty)
                {
                    employer = await _employerRepository.FindAsync(e => e.Id == request.PaginationStatusGuidDTO.guid);
                }
                else
                {
                    employer = await _employerRepository.FindAsync(e => e.Id == request.User.Id);
                }

                if (employer != null)
                {
                    userProfile = await _userProfile.FindAsync(u => u.UserID == employer.Id);
                }
            }

            List<Request> req = new List<Request>();
            if (employer == null)
            {
                req = (await _request.GetAllAsync()).ToList();
            }
            else
            { 
                req = (await _request.FindAllAsync((e) => e.SenderId == userProfile.Id )).ToList();
            }
            int chargebleAmount = _configuration.GetValue<int>("PaymentConfig:CertificateCharge");
            List<EmployerRequestDTO> employerRequestDTOs = new List<EmployerRequestDTO>();
            decimal amountToDeduct = 0;
            foreach (var item in req)
            {
                EmployerRequestDTO employerRequestDTO = new EmployerRequestDTO();
                Share share = await _share.FindAsync((e) => e.RequsetId == item.Id);
                UserProfile receiverProfile = await _userProfile.FindAsync((e) => e.Id == item.ReceiverId);
                User receiver = await _user.FindAsync((e) => e.Id == receiverProfile.UserID);
                if (share != null && share.Type == ShareType.Envelope)
                {
                    List<CertificateView> certificateDetails = await _mediator.Send(new EnvelopeRequestGroupQuery(share.ResourceId));
                    amountToDeduct = certificateDetails.Count * chargebleAmount;
                }
                if (share != null && share.Type == ShareType.Certificate)
                {
                    amountToDeduct = chargebleAmount;
                }
                employerRequestDTO.Amount = amountToDeduct;
                employerRequestDTO.CreatedTime = item.CreatedAt;
                employerRequestDTO.RequestItem = item;
                employerRequestDTO.Name = receiver == null ? string.Empty : receiver.Name;
                employerRequestDTO.Email = receiver == null ? string.Empty : receiver.Email;
                employerRequestDTO.Token = (share != null && share.Token != string.Empty) ? share.Token : string.Empty;
                employerRequestDTOs.Add(employerRequestDTO);
            }

            IQueryable<EmployerRequestDTO> employerRequests = employerRequestDTOs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PaginationStatusGuidDTO.SearchTerm))
            {
                employerRequests = employerRequests.Where(p =>
                    p.Name.ToLower().Contains(request.PaginationStatusGuidDTO.SearchTerm.ToLower()) ||
                     p.RequestItem.SenderId.ToString().Contains(request.PaginationStatusGuidDTO.SearchTerm.ToLower()) ||
                    ((string)p.Email).ToLower().Contains(request.PaginationStatusGuidDTO.SearchTerm.ToLower()) ||
                    (p.RequestItem.CreatedAt.ToString()).ToLower().Contains(request.PaginationStatusGuidDTO.SearchTerm.ToLower())
                    );
            }
            if (request.PaginationStatusGuidDTO.SortOrder?.ToLower() == "desc")
            {
                employerRequests = employerRequests.OrderByDescending(GetSortProperty(request));
            }
            else
            {
                employerRequests = employerRequests.OrderBy(GetSortProperty(request));

            }

            if( request.PaginationStatusGuidDTO.Status == RequestStatus.All)
            {

            }
            else
            {
                employerRequests = employerRequests.Where( er => er.RequestItem.Status == request.PaginationStatusGuidDTO.Status) ;

            }


            var employerRequestList = await PagedList<EmployerRequestDTO>.CreateAsync(
                            employerRequests,
                            request.PaginationStatusGuidDTO.Page,
            request.PaginationStatusGuidDTO.PageSize);

            return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: employerRequestList, message: "Employer Request list");
        }

        private static Expression<Func<EmployerRequestDTO, object>> GetSortProperty(GetEmployerRq request)
        {
            return request.PaginationStatusGuidDTO.SortColumn?.ToLower() switch
            {
                "name" => erd => erd.Name,
                "email" => erd => erd.Email,
                "CreatedAt" => erd => erd.CreatedTime,
                _ => erd => erd.CreatedTime
            };
        }
    }
}