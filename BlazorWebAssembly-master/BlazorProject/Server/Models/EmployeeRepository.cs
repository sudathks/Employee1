using BlazorProject.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.Json;
using WebPush;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorProject.Server.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly IDepartmentRepository departmentRepository;

        public EmployeeRepository(AppDbContext appDbContext, IDepartmentRepository departmentRepository)
        {
            this.appDbContext = appDbContext;
            this.departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await appDbContext.Employees.Include(e => e.Department).ToListAsync();
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            if (employee.DepartmentId == 0)
            {
                throw new Exception("Employee DepartmentId cannot be ZERO");
            }
            else
            {
                Department department = await this.departmentRepository
                    .GetDepartment(employee.DepartmentId);
                if (department == null)
                {
                    throw new Exception($"Invalid Employee DepartmentId {employee.DepartmentId}");
                }
                employee.Department = department;
            }

            var result = await appDbContext.Employees.AddAsync(employee);
            await appDbContext.SaveChangesAsync();
            return result.Entity;

            var subscription = await appDbContext.NotificationSubscriptions.Where(e => e.UserId == GetUserId()).SingleOrDefaultAsync();
            if (subscription != null)
            {
                _ = TrackAndSendNotificationsAsync(employee, subscription);
            }
        }

        private string GetUserId()
        {
            return "1";
            //return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private static async Task TrackAndSendNotificationsAsync(Employee employee, NotificationSubscription subscription)
        {
            await SendNotificationAsync(employee, subscription, "your changes have been added sucessfully!");
        }

        private static async Task SendNotificationAsync(Employee employee, NotificationSubscription subscription, string message)
        {
            // For a real application, generate your own
            var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
            var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
            var vapidDetails = new VapidDetails("mailto:<someone@example.com>", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            try
            {
                var payload = JsonSerializer.Serialize(new
                {
                    message,
                    url = $"/vehicle/{employee.EmployeeId}",
                });
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error sending push notification: " + ex.Message);
            }
        }

        public async Task DeleteEmployee(Guid employeeId)
        {
            var result = await appDbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (result != null)
            {
                appDbContext.Employees.Remove(result);
                await appDbContext.SaveChangesAsync();
            }
        }

        public async Task<Employee> GetEmployee(Guid employeeId)
        {
            return await appDbContext.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await appDbContext.Employees
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<EmployeeDataResult> GetEmployees
            (int skip = 0, int take = 5, string orderBy = "EmployeeId")
        {
            EmployeeDataResult result = new EmployeeDataResult()
            {
                Employees = appDbContext.Employees.OrderBy(orderBy).Skip(skip).Take(take),
                Count = await appDbContext.Employees.CountAsync()
            };

            return result;
        }

        public async Task<IEnumerable<Employee>> Search(string name, Gender? gender)
        {
            IQueryable<Employee> query = appDbContext.Employees;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName.Contains(name)
                            || e.LastName.Contains(name));
            }

            if (gender != null)
            {
                query = query.Where(e => e.Gender == gender);
            }

            return await query.ToListAsync();
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            var result = await appDbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

            if (result != null)
            {
                result.FirstName = employee.FirstName;
                result.LastName = employee.LastName;
                result.Email = employee.Email;
                result.DateOfBrith = employee.DateOfBrith;
                result.Gender = employee.Gender;
                if (employee.DepartmentId != 0)
                {
                    result.DepartmentId = employee.DepartmentId;
                }
                else if (employee.Department != null)
                {
                    result.DepartmentId = employee.Department.DepartmentId;
                }
                //result.PhotoPath = employee.PhotoPath;

                await appDbContext.SaveChangesAsync();

                return result;
            }

            return null;
        }
    }
}