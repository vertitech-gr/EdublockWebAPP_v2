using System.Net;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.DAL.EF;
using MediatR;

namespace Edu_Block_dev.CQRS.Command_Handler.EduRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ApiResponse<object>>
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public DeleteRoleCommandHandler(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        
        public async Task<ApiResponse<object>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Role existingRole  =  await _roleRepository.FindAsync(r => r.Id ==  request.Guid) ;
                if(existingRole == null)
                {
                    return new ApiResponse<object>(HttpStatusCode.InternalServerError, error: "Role doesn't exists", message: "Role doesn't exists.");
                }
                await _roleRepository.DeleteAsync(request.Guid);
                return new ApiResponse<object>(HttpStatusCode.OK, message: "Role deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(HttpStatusCode.InternalServerError, message: "Unable to delete role.");
            }
        }
    }
}