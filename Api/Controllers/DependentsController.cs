using Api.Dtos.Dependent;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private static List<Dependent> _dependents = new List<Dependent>
    {
        new Dependent { Id = 1, FirstName = "Spouse", LastName = "Morant", DateOfBirth = new DateTime(1998, 3, 3), Relationship = Relationship.Spouse },
        new Dependent { Id = 2, FirstName = "Child1", LastName = "Morant", DateOfBirth = new DateTime(2020, 6, 23), Relationship = Relationship.Child },
        new Dependent { Id = 3, FirstName = "Child2", LastName = "Morant", DateOfBirth = new DateTime(2021, 5, 18), Relationship = Relationship.Child },
        new Dependent { Id = 4, FirstName = "DP", LastName = "Jordan", DateOfBirth = new DateTime(1974, 1, 2), Relationship = Relationship.DomesticPartner }
    };

    // Helper method to map Dependent model to GetDependentDto
    private GetDependentDto MapToDependentDto(Dependent dependent)
    {
        return new GetDependentDto
        {
            Id = dependent.Id,
            FirstName = dependent.FirstName,
            LastName = dependent.LastName,
            DateOfBirth = dependent.DateOfBirth,
            Relationship = dependent.Relationship
        };
    }

    // Helper method to map GetDependentDto to Dependent model
    private Dependent MapToDependent(GetDependentDto dependentDto)
    {
        return new Dependent
        {
            Id = dependentDto.Id,
            FirstName = dependentDto.FirstName,
            LastName = dependentDto.LastName,
            DateOfBirth = dependentDto.DateOfBirth,
            Relationship = dependentDto.Relationship
        };
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public ActionResult<ApiResponse<GetDependentDto>> Get(int id)
    {
        // Find the dependent by ID
        var dependent = _dependents.FirstOrDefault(d => d.Id == id);
        if (dependent == null)
        {
            return NotFound(new ApiResponse<GetDependentDto> { Success = false, Message = "Dependent not found" });
        }

        // Map the Dependent model to the GetDependentDto
        var dependentDto = MapToDependentDto(dependent);
        return Ok(new ApiResponse<GetDependentDto> { Success = true, Data = dependentDto });
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public ActionResult<ApiResponse<List<GetDependentDto>>> GetAll()
    {
        // Map the list of Dependent models to a list of GetDependentDto
        var dependentDtos = _dependents.Select(d => MapToDependentDto(d)).ToList();
        return Ok(new ApiResponse<List<GetDependentDto>> { Success = true, Data = dependentDtos });
    }

    [SwaggerOperation(Summary = "Add a new dependent")]
    [HttpPost]
    public ActionResult<ApiResponse<GetDependentDto>> Add([FromBody] GetDependentDto dependentDto)
    {
        // Map the GetDependentDto to the Dependent model
        var dependent = MapToDependent(dependentDto);
        dependent.Id = _dependents.Count + 1;
        _dependents.Add(dependent);

        return CreatedAtAction(nameof(Get), new { id = dependent.Id }, new ApiResponse<GetDependentDto> { Success = true, Data = dependentDto });
    }

}
