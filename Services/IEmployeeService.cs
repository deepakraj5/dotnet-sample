using SampleDotNet.Models;
using SampleDotNet.Models.Entities;

namespace SampleDotNet.Services
{
    public interface IEmployeeService
    {
        List<Employee> GetEmployees();
        Employee GetEmployeeById(Guid id);
        Employee AddEmployee(AddEmployeeDto addEmployeeDto);
        Employee UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployeeDto);
        void DeleteEmployee(Guid id);
    }
}
