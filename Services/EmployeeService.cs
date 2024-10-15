using SampleDotNet.Data;
using SampleDotNet.Models;
using SampleDotNet.Models.Entities;

namespace SampleDotNet.Services
{
    public class EmployeeService: IEmployeeService
    {

        public readonly ApplicationDBContext dBContext;

        public EmployeeService(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public Employee AddEmployee(AddEmployeeDto addEmployeeDto)
        {
            var employeeEntity = new Employee()
            {
                Name = addEmployeeDto.Name,
                Email = addEmployeeDto.Email,
                Phone = addEmployeeDto.Phone,
                Salary = addEmployeeDto.Salary
            };

            dBContext.Employees.Add(employeeEntity);
            dBContext.SaveChanges();
                
            return employeeEntity;
        }

        public void DeleteEmployee(Guid id)
        {
            var employee = dBContext.Employees.Find(id);

            if (employee is null)
            {
                throw new KeyNotFoundException();
            }

            dBContext.Employees.Remove(employee);
            dBContext.SaveChanges();
        }

        public Employee GetEmployeeById(Guid id)
        {
            var employee = dBContext.Employees.Find(id);

            if (employee is null)
            {
                throw new KeyNotFoundException();
            }

            return employee;
        }

        public List<Employee> GetEmployees()
        {
            return dBContext.Employees.ToList();
        }

        public Employee UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = dBContext.Employees.Find(id);

            if (employee is null)
            {
                throw new KeyNotFoundException();
            }

            employee.Name = updateEmployeeDto.Name;
            employee.Email = updateEmployeeDto.Email;
            employee.Phone = updateEmployeeDto.Phone;
            employee.Salary = updateEmployeeDto.Salary;

            dBContext.SaveChanges();

            return employee;
        }
    }
}
