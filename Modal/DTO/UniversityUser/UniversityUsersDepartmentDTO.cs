﻿
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityUsersDepartmentDTO
    {
        public Role Role { get; set; }
        public UniversityUser UniversityUser { get; set; }
        public List<UniversityDepartmentUser> UniversityDepartmentUsers { get; set; }
    }
}