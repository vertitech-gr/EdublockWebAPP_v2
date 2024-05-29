using System;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block.DAL.EF;
using EduBlock.Model.DTO;
using MediatR;
using AutoMapper;
using Edu_Block.DAL;
using Edu_Block_dev.CQRS.Commands.EduUser;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
	public class UpdateUserLoginStatusHandler : IRequestHandler<UpdateUserLoginStatus, ApiResponse<object>>
    {
        private readonly IRepository<User> _userRepository;

        public UpdateUserLoginStatusHandler(IMapper mapper, IRepository<User> userRepository)
		{
            _userRepository = userRepository;
        }
        public async Task<ApiResponse<object>> Handle(UpdateUserLoginStatus request, CancellationToken cancellationToken)
        {
            try
            {
                User existingUser = await _userRepository.FindAsync(u => u.Email == request.email);
                if (existingUser == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "user not found");
                }
                if (request.type && !existingUser.loginStatus)
                {
                    existingUser.loginStatus = true;
                    await _userRepository.UpdateAsync(existingUser.Id, existingUser);
                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "activated successfully");
                }
                else if (!request.type && existingUser.loginStatus)
                {
                    existingUser.loginStatus = false;
                    await _userRepository.UpdateAsync(existingUser.Id, existingUser);
                    return new ApiResponse<object>(System.Net.HttpStatusCode.OK, message: "deactivated successfully");
                }
                else
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.BadRequest, message: "Invalid Request");
                }
            } catch (Exception ex)
            {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, message: $"An error occurred: {ex.Message}");
            }
        }


    }
}

