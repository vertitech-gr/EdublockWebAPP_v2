using Edu_Block.DAL.EF;
using Edu_Block_dev.CQRS.Commands.EduEmployer;
using Edu_Block_dev.DAL.EF;
using Edu_Block_dev.Modal.DTO;
using MediatR;

public class EmployerCommandHandler : IRequestHandler<EmployerCommand, EmployerDTO>
{
    private readonly EduBlockDataContext _context;

    public EmployerCommandHandler(EduBlockDataContext context)
    {
        _context = context;
    }

    public async Task<EmployerDTO> Handle(EmployerCommand request, CancellationToken cancellationToken)
    {
        var employerEntity = new Employer
        {
            Name = request.employerDTO.Name,
            Email = request.employerDTO.Email,
            Address = request.employerDTO.Address,
            Industry = request.employerDTO.Industry,
            SpecificIndustry = request.employerDTO.SpecificIndustry,
            Password = request.employerDTO.Password
        };

        _context.Add(employerEntity);
        await _context.SaveChangesAsync();

        return request.employerDTO;
    }
}
