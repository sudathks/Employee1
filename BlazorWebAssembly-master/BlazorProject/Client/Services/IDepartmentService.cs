using BlazorProject.Shared;

namespace BlazorProject.Client.Services
{
    public interface IDepartmentService 
    {
        Task<IEnumerable<Department>> GetAllDepartments();
        Task<Department> GetDepartment(int departmentId);
    }
}
