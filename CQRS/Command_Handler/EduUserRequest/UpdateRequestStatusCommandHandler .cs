using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduUserRequest;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduUserRequest
{
    public class UpdateRequestStatusCommandHandler : IRequestHandler<UpdateRequestStatusCommand, ApiResponse<object>>
    {
        private readonly EduBlockDataContext _dbContext;
        public UpdateRequestStatusCommandHandler(EduBlockDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<object>> Handle(UpdateRequestStatusCommand request, CancellationToken cancellationToken)
        {

            try
            {

                UserRequest userRequest = _dbContext.UserRequest.Where( ur => ur.Id == request.Guid).FirstOrDefault();

                if (userRequest != null)
                {
                    userRequest.Status = request.Status;
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null, message: "unable find request");

                }

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: userRequest, message: "Request updated successfully");

            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, data: null, message: "unable to update request");

            }
           
        }

        //private bool IsValidStatus(MessageStatus status)
        //{

        //    var validStatusValues = new[] { "Approve", "Reject", "Remark" };
        //    return validStatusValues.Contains(status.ToLower());
        //}
    }
}
