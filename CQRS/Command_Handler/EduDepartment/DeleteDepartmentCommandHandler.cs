using Edu_Block.DAL;
using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduDepartment;
using MediatR;
using System.Data;
using System.Data.SqlClient;

namespace Edu_Block_dev.CQRS.Command_Handler.EduDepartment
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, ApiResponse<object>>
    {
        private readonly IRepository<Department> _departmentReppository;
        private readonly EduBlockDataContext _context;
        private readonly IConfiguration _configuration;

        public DeleteDepartmentCommandHandler(EduBlockDataContext context, IRepository<Department> departmentReppository, IConfiguration configuration)
        {
            _departmentReppository = departmentReppository;
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<object>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            try {
                Department department = await _context.Departments.FindAsync(request.Guid);
                var Students = _context.DepartmentStudents.Where( ds => ds.DepartmentId == department.Id).Select( u => u.StudentId ).ToList();
                var UniversityUsers = _context.UniversityDepartmentUsers.Where(ds => ds.DepartmentId == department.Id).Select(u => u.UniversityUserId).ToList();

                if (department == null)
                {
                    return new ApiResponse<object>(System.Net.HttpStatusCode.NotFound, data: null , message: "Unable to delete Department because department doesn't exists");
                }

                department.loginStatus = false;
                department.IsDeleted = true;
                await _departmentReppository.UpdateAsync( department.Id, department );
                using (var connection = new SqlConnection(_configuration.GetSection("ConnectionStrings:defaultconnection").Value))
                {
                    connection.Open();


                    if (Students != null && Students.Count() > 0)
                    {
                        try
                        {
                            string inClause = string.Join(",", Students.Select((id, index) => $"@id{index}"));
                            string query = $@"
                                        UPDATE [edublock].[dbo].[User]
                                        SET loginStatus = 0, IsDeleted = 1
                                        WHERE Id IN ({inClause});
                                        ";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.CommandType = CommandType.Text;

                                for (int i = 0; i < Students.Count; i++)
                                {
                                    command.Parameters.AddWithValue($"@id{i}", Students[i]);
                                }

                                int rowsAffected = command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message, message: "Unable to delete Department");

                        }
                    }


                    if (UniversityUsers != null && UniversityUsers.Count() > 0)
                    {
                        try
                        {
                            string inClause = string.Join(",", UniversityUsers.Select((id, index) => $"@id{index}"));
                            string query = $@"
                                        UPDATE [edublock].[dbo].[UniversityUsers]
                                        SET loginStatus = 0, IsDeleted = 1
                                        WHERE Id IN ({inClause});
                                        ";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.CommandType = CommandType.Text;

                                for (int i = 0; i < UniversityUsers.Count; i++)
                                {
                                    command.Parameters.AddWithValue($"@id{i}", UniversityUsers[i]);
                                }

                                int rowsAffected = command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message, message: "Unable to delete Department");

                        }
                    }

                    connection.Close();
                }

                return new ApiResponse<object>(System.Net.HttpStatusCode.OK, data: null, message: "Department deleted successfully");
            }
            catch (Exception ex) {
                return new ApiResponse<object>(System.Net.HttpStatusCode.InternalServerError, error: ex.Message , message: "Unable to delete Department");

            }
        }
    }
}