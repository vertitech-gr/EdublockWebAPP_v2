using AutoMapper;
using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using Edu_Block_dev.DAL.EF;
using MediatR;
using Edu_Block_dev.CQRS.Commands.EduShareLink;
using Edu_Block_dev.CQRS.Commands.EduShareCredential;
using Edu_Block_dev.Modal.DTO;
using Edu_Block_dev.CQRS.Query.EduShareLink;
using EduBlock.Model.DTO;
using Edu_Block_dev.CQRS.Query.EduEnvelope;
using Edu_Block_dev.CQRS.Commands.EduEnvelope;
using Microsoft.AspNetCore.Mvc;
using Request = Edu_Block_dev.DAL.EF.Request;
using Edu_Block_dev.Modal.Enum;

namespace Edu_Block_dev.CQRS.Command_Handler.EduShareLink
{
    public class ShareCommandHandler : IRequestHandler<ShareCommand, IActionResult>
    {
        private readonly EduBlockDataContext _context;
        private readonly IMediator _mediator;
        private readonly Util _util;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IRepository<Employer> _employerRepository;


        public ShareCommandHandler(EduBlockDataContext context, IRepository<User> userRepository, IConfiguration configuration, IMediator mediator, Util util, IMapper mapper, IRepository<UserProfile> userProfileRepository, IRepository<Employer> employerRepository)
        {
            _mediator = mediator;
            _userProfileRepository = userProfileRepository;
            _context = context;
            _util = util;
            _mapper = mapper;
            _configuration = configuration;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _employerRepository = employerRepository;
        }

        public async Task<IActionResult> Handle(ShareCommand request, CancellationToken cancellationToken)
        {
            CommanUser user = request.User;

            if (request.shareDTO.Type == ShareType.Envelope)
            {
                var envelopeInfo = await _context.Envelopes.FindAsync(request.shareDTO.ResourceId);

                if (envelopeInfo == null || envelopeInfo.CreatedBy != user.UserProfile.Id)
                {
                    return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.NotFound, message: $"Envelope with ID {request.shareDTO.ResourceId} not found or you don't have permission."));
                }
            }
            else if (request.shareDTO.Type == ShareType.Certificate)
            {
                var certificateInfo = await _context.Certificates.FindAsync(request.shareDTO.ResourceId);

                if (certificateInfo == null || certificateInfo.CreatedBy != user.UserProfile.Id)
                {
                    return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.NotFound, message: $"Certificate with ID {request.shareDTO.ResourceId} not found or you don't have permission."));
                }

            }

            string email = "";
            Employer existingUser = await _employerRepository.FindAsync(u => u.Email == request.shareDTO.EmailId);

            if (existingUser == null)
            {
                email = request.shareDTO.EmailId;
            }
            else
            {
                email = existingUser.Email;
            }


            if (request.shareDTO.Type == ShareType.Envelope && request.shareDTO.Credentials != null && request.shareDTO.Credentials.Count() > 0)
            {
                EnvelopResponseDTO envelopResponseDTO = await _mediator.Send(new EnvelopQueryById(request.shareDTO.ResourceId));

                //List<Guid> missingGuids = request.shareDTO.Credentials.Except(envelopResponseDTO.Credentials.Select(cv => cv.Id)).ToList();

                //List<Guid> additionalGuids = envelopResponseDTO.Credentials.Where(cert => !request.shareDTO.Credentials.Contains(cert.Id)).ToList().Select(s => s.Id).ToList<Guid>();

                var envelopeInfo = await _context.Envelopes.FindAsync(request.shareDTO.ResourceId);


                //if ((missingGuids != null && missingGuids.Count > 0) || (additionalGuids != null && additionalGuids.Count > 0))
                {
                    EnvelopeDTOC envelopeDTOC = new EnvelopeDTOC()
                    {
                        //Name = email.Split('@')[0],
                        Name = envelopeInfo.Name,
                        Credentials = request.shareDTO.Credentials
                    };

                    var envelopeResult = await _mediator.Send(new EnvelopeCommand(envelopeDTOC, request.User, EnvelopeShareType.SHARE));
                    request.shareDTO.ResourceId = (envelopeResult.Data as Envelope).Id;
                }
            }

            Request sharedRequest = await _context.Requests.FindAsync(request.shareDTO.RequestId);
            if (sharedRequest != null)
            {
                sharedRequest.Status = RequestStatus.Shared;
                _context.Requests.Update(sharedRequest);
                await _context.SaveChangesAsync();

            }

            var shareEntity = _mapper.Map<Share>(request.shareDTO);

            shareEntity.SenderId = user.UserProfile.Id;
            shareEntity.CreatedBy = user.UserProfile.Id;
            shareEntity.Token = _util.GenerateAndConvertToSHA(Guid.NewGuid().ToString());

            int expiryTimeInSeconds = _configuration.GetValue<int>("ShareConfig:ExpiryTimeInSeconds");
            shareEntity.ExpiryDate = DateTime.UtcNow.AddDays(30);
            var baseUrl = _configuration.GetSection("DashboardUrl:Url").Value;
            var verificationUrl = $"{baseUrl}/{shareEntity.Token}";
            _context.Shares.Add(shareEntity);
            await _context.SaveChangesAsync();

            var shareDetailsQuery = new ShareByUniqueIdQuery(shareEntity.Id);
            ShareRespnseDto shareDetails = await _mediator.Send(shareDetailsQuery);

            foreach (Guid credentialGuid in request.shareDTO.Credentials)
            {
                var shareCredentialCommand = new ShareCredentialCommand(new ShareCredentialDTO
                {
                    CredentialId = credentialGuid,
                    ShareId = shareDetails.Id
                });
                var shareCredentialResult = await _mediator.Send(shareCredentialCommand);
            }

            var subject = "Document-------- EduBlock";
            var content = $"Click on this link to view Certificate/Envelope: {verificationUrl}";
            if (!request.shareDTO.QR)
            {
                _util.SendSimpleMessage(subject, email, content, "View Certificate", verificationUrl);
            }

            return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.OK ,data: verificationUrl, message: "Link Created successfully"));
        }
    }
}
