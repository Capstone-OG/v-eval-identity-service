using Application.Common.Interfaces.Repositories;
using Domain.Entities.Profiles;
using Infrastructure.Persistence;

namespace Infrastructure.Persistence.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }
}
