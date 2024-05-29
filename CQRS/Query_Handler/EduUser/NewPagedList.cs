using EduBlock.Model.DTO;

namespace Edu_Block_dev.CQRS.Query_Handler.EduUser
{
    internal class NewPagedList
    {
        private IQueryable<StudentDetailsDTO> studentDetailsDTOList;
        private int page;
        private int pageSize;
        private int toatlCount;
        private bool v1;
        private bool v2;

        public NewPagedList(IQueryable<StudentDetailsDTO> studentDetailsDTOList, int page, int pageSize, int toatlCount, bool v1, bool v2)
        {
            this.studentDetailsDTOList = studentDetailsDTOList;
            this.page = page;
            this.pageSize = pageSize;
            this.toatlCount = toatlCount;
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}