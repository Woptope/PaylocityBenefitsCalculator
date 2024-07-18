using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly PaycheckCalculator _paycheckCalculator;

    public EmployeesController(PaycheckCalculator paycheckCalculator)
    {
        _paycheckCalculator = paycheckCalculator;
    }

    // Mock data for demonstration; in a real application, this would come from a database
    private static readonly List<Employee> _employees = new()
        {
            new Employee
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new Employee
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<Dependent>
                {
                    new Dependent
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new Dependent
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new Dependent
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new Employee
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<Dependent>
                {
                    new Dependent
                    {
                        Id = 4,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    }
                }
            }
        };



    // Helper method to map Employee model to GetEmployeeDto
    private GetEmployeeDto MapToEmployeeDto(Employee employee)
    {
        return new GetEmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Salary = employee.Salary,
            DateOfBirth = employee.DateOfBirth,
            Dependents = employee.Dependents.Select(d => new GetDependentDto
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                DateOfBirth = d.DateOfBirth,
                Relationship = d.Relationship
            }).ToList()
        };
    }

    // Helper method to map GetEmployeeDto to Employee model
    private Employee MapToEmployee(GetEmployeeDto employeeDto)
    {
        return new Employee
        {
            Id = employeeDto.Id,
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Salary = employeeDto.Salary,
            DateOfBirth = employeeDto.DateOfBirth,
            Dependents = employeeDto.Dependents.Select(d => new Dependent
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                DateOfBirth = d.DateOfBirth,
                Relationship = d.Relationship
            }).ToList()
        };
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public ActionResult<ApiResponse<GetEmployeeDto>> Get(int id)
    {
        // Find the employee by ID
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound(new ApiResponse<GetEmployeeDto> { Success = false, Message = "Employee not found" });
        }

        // Map the Employee model to the GetEmployeeDto
        var employeeDto = MapToEmployeeDto(employee);
        return Ok(new ApiResponse<GetEmployeeDto> { Success = true, Data = employeeDto });
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public ActionResult<ApiResponse<List<GetEmployeeDto>>> GetAll()
    {
        // Map the list of Employee models to a list of GetEmployeeDto
        var employeeDtos = _employees.Select(e => MapToEmployeeDto(e)).ToList();
        return Ok(new ApiResponse<List<GetEmployeeDto>> { Success = true, Data = employeeDtos });
    }

    [SwaggerOperation(Summary = "Add a new employee")]
    [HttpPost]
    public ActionResult<ApiResponse<GetEmployeeDto>> Add([FromBody] GetEmployeeDto employeeDto)
    {
        // Map the GetEmployeeDto to the Employee model
        var employee = MapToEmployee(employeeDto);
        employee.Id = _employees.Count + 1;
        _employees.Add(employee);

        return CreatedAtAction(nameof(Get), new { id = employee.Id }, new ApiResponse<GetEmployeeDto> { Success = true, Data = employeeDto });
    }

    [SwaggerOperation(Summary = "Calculate paycheck for employee by id")]
    [HttpGet("{id}/paycheck")]
    public ActionResult<ApiResponse<decimal>> CalculatePaycheck(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound(new ApiResponse<decimal> { Success = false, Message = "Employee not found" });
        }

        var paycheckAmount = _paycheckCalculator.CalculatePaycheck(employee);
        return Ok(new ApiResponse<decimal> { Success = true, Data = paycheckAmount });
    }
}
