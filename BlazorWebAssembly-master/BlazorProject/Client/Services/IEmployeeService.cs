using BlazorProject.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorProject.Client.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> Search(string name, Gender? gender);
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<EmployeeDataResult> GetEmployees(int skip, int take, string orderBy);
        Task<Employee> GetEmployee(Guid employeeId);
        Task<Employee> GetEmployeeByEmail(string email);
        Task<Employee> AddEmployee(Employee employee);
        Task<Employee> UpdateEmployee(Employee employee);
        Task DeleteEmployee(Guid employeeId);
        //Task<Department> GetDepartmentName(int id);
    }
}