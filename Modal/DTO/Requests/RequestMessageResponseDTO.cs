
using Edu_Block.DAL.EF;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class RequestMessageResponseDTO
    {
        public RequestMessage RequestMessages { get; set; }
        public CommanUser CommanUser { get; set; }
        public UserRequest UserRequest { get; set; }
    }
}