using AutoMapper;
using Edu_Block.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Edu_Block.DAL.EF;
using EduBlock.Model.DTO;
using Edu_Block_dev.CQRS.Query.EduUser;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    public class GetUserDetailsQueryHandler
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public GetUserDetailsQueryHandler(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetUserDetailsQuery query)
        {
            var email = query.User.FindFirst(ClaimTypes.Email)?.Value;
            var result = await _userRepository.FindAsync(u => u.Email == email);
            if (result == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            var userDetailsDto = _mapper.Map<UserDetailsDTO>(result);
            return new OkObjectResult(new ApiResponse<object>( System.Net.HttpStatusCode.OK, data: new { user = userDetailsDto }, message: "User details retrieved successfully"));
        }

        public async Task<IActionResult> Handle(GetUserByEmailQuery query)
        {
            var result = await _userRepository.FindAsync(u => u.Email == query.Email);
            if (result == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            var userDetailsDto = _mapper.Map<UserDetailsDTO>(result);
            return new OkObjectResult(new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: new { user = userDetailsDto }, message: "User details retrieved successfully"));
        }
    }
}
