using BlazorProject.Server.Models;
using BlazorProject.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IDepartmentRepository departmentRepository;

        public EmployeesController(IEmployeeRepository employeeRepository, IDepartmentRepository _departmentRepository)
        {
            this.employeeRepository = employeeRepository;
            this.departmentRepository = _departmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllEmployees()
        {
            try
            {
                return Ok(await employeeRepository.GetAllEmployees());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search(string name, Gender? gender)
        {
            try
            {
                var result = await employeeRepository.Search(name, gender);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
            }
        }

        //[HttpGet]
        //public async Task<ActionResult> GetEmployees(int skip = 0, int take = 5, string orderBy = "EmployeeId")
        //{
        //    try
        //    {
        //        return Ok(await employeeRepository.GetEmployees(skip, take, orderBy));
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            "Error retrieving data from the database");
        //    }
        //}

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Employee>> GetEmployee(Guid id)
        {
            try
            {
                var result = await employeeRepository.GetEmployee(id);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            var createdEmployee = new Employee();
            try
            {
                if (employee == null)
                    return BadRequest();

                var emp = await employeeRepository.GetEmployeeByEmail(employee.Email);

                if (emp != null)
                {
                    ModelState.AddModelError("Email", "Employee email already in use");
                    return BadRequest(ModelState);
                }

                createdEmployee = await employeeRepository.AddEmployee(employee);

                return CreatedAtAction(nameof(GetEmployee),
                    new { id = createdEmployee.EmployeeId }, createdEmployee);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new employee record");
            }
            finally
            {
                Console.WriteLine(createdEmployee);
            }
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(Guid id, Employee employee)
        {
            try
            {
                if (id != employee.EmployeeId)
                    return BadRequest("Employee ID mismatch");

                var employeeToUpdate = await employeeRepository.GetEmployee(id);

                if (employeeToUpdate == null)
                {
                    return NotFound($"Employee with Id = {id} not found");
                }

                return await employeeRepository.UpdateEmployee(employee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating employee record");
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var employeeToDelete = await employeeRepository.GetEmployee(id);

                if (employeeToDelete == null)
                {
                    return NotFound($"Employee with Id = {id} not found");
                }

                await employeeRepository.DeleteEmployee(id);

                return Ok($"Employee with Id = {id} deleted");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting employee record");
            }
        }

        //[HttpGet("department/{id:int}")]
        //public async Task<Department> GetDepartmentName(int id)
        //{
        //    Department department = new Department();
        //    if (id == 0)
        //    {
        //        throw new Exception("Employee DepartmentId cannot be ZERO");
        //    }
        //    else
        //    {
        //        department = await this.departmentRepository
        //            .GetDepartment(id);
        //        if (department == null)
        //        {
        //            throw new Exception($"Invalid Employee DepartmentId {id}");
        //        }
        //    }
        //    return department;
        //}
    }
}