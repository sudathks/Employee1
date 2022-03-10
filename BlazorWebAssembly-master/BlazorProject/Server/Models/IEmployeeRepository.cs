using BlazorProject.Shared;

namespace BlazorProject.Server.Models
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> Search(string name, Gender? gender);
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<EmployeeDataResult> GetEmployees(int skip, int take, string orderBy);
        Task<Employee> GetEmployee(Guid employeeId);
        Task<Employee> GetEmployeeByEmail(string email);
        Task<Employee> AddEmployee(Employee employee);
        Task<Employee> UpdateEmployee(Employee employee);
        Task DeleteEmployee(Guid employeeId);
    }
}
