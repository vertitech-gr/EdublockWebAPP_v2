using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using EduBlock.Model.DTO;
using MediatR;

namespace Edu_Block_dev.CQRS.Commands.EduAvailableSubscription
{
    public class AvailableSubscriptionCommand : IRequest<AvailableSubscription>
    {
        public AvailableSubscriptionDto availableSubscriptionDto { get; set; }
        public CommanUser User { get; set; }

        public AvailableSubscriptionCommand(AvailableSubscriptionDto _availableSubscriptionDto, CommanUser user)
        {
            availableSubscriptionDto = _availableSubscriptionDto;
            User = user;
        }
    }

    public class EditSubscriptionCommand : IRequest<ApiResponse<object>>
    {
        public EditSubscriptionDto _editSubscriptionDto { get; set; }
        public CommanUser _user { get; set; }

        public EditSubscriptionCommand(EditSubscriptionDto editSubscriptionDto, CommanUser user)
        {
            _editSubscriptionDto = editSubscriptionDto;
            _user = user;
        }
    }

    public class DeleteAvailableSubscriptionCommand : IRequest<ApiResponse<object>>
    {
        public Guid Guid { get; set; }
        public CommanUser User { get; set; }

        public DeleteAvailableSubscriptionCommand(Guid guid, CommanUser user)
        {
            Guid = guid;
            User = user;
        }
    }
}
