using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.Dock;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.UserCommand
{
    public class CreateDockHandlerCommand : IRequest<CreateHandlerResponse>
    {
        public CreateDockHandlerCommand()
        {
        }
    }
    public class CreateDockIssuerCommand : IRequest<CreateIssuerResponse>
    {
        public IssuerDTO issuerDTO;
        public CreateDockIssuerCommand(IssuerDTO issuerDTO)
        {
            this.issuerDTO = issuerDTO;
        }
    }

    public class IssuerProfileCommand : IRequest<CreateHandlerResponse>
    {
        public CreateHandlerResponse CreateHandlerResponse;
        public IssuerDTO issuerDTO;
        public IssuerProfileCommand(CreateHandlerResponse createHandlerResponse, IssuerDTO issuerDTO)
        {
            this.issuerDTO = issuerDTO;
            this.CreateHandlerResponse = createHandlerResponse;
        }
    }

    public class IssuerCredentialCommand : IRequest<ApiResponse<object>>
    {
        public IssuerCredentialRequest _issuerCredentialRequest;
        public IssuerCredentialCommand(IssuerCredentialRequest issuerCredentialRequest)
        {
            this._issuerCredentialRequest = issuerCredentialRequest;
        }
    }

    public class RevokeCredentialCommand : IRequest<ApiResponse<object>>
    {
        public Guid Guid;
        public RevokeCredentialCommand(Guid guid)
        {
            Guid = guid;
        }
    }

    public class ResendCredentialCommand : IRequest<ApiResponse<object>>
    {
        public Guid Guid;
        public ResendCredentialCommand(Guid guid)
        {
            Guid = guid;
        }
    }

    public class VerifyResourceCommand : IRequest<ShareVerificationResponse>
    {
        public VerifyResourceDTO _verifyResourceDTO;
        public CommanUser User;

        public VerifyResourceCommand(VerifyResourceDTO verifyResourceDTO, CommanUser user)
    {
        this._verifyResourceDTO = verifyResourceDTO;
            this.User = user;
    }
}


}