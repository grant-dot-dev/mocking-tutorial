using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Mvc;
using Rhino.Mocks;
using Service;
using WebApi;
using Xunit;

namespace Test;

public class RhinoMocksApiTests
{
    private readonly ITodoService _mockTodoService;
    private readonly INotificationService _mockNotificationService;

    public RhinoMocksApiTests()
    {
        _mockTodoService = MockRepository.GenerateMock<ITodoService>();
        _mockNotificationService = MockRepository.GenerateMock<INotificationService>();
    }

    #region Get

    [Fact]
    public void GetAll_ReturnsExpectedData()
    {
        // Arrange
        var expectedTodos = new List<Todo>
        {
            new Todo { Id = 1, Description = "Task 1", IsCompleted = false },
            new Todo { Id = 2, Description = "Task 2", IsCompleted = true }
        };

        _mockTodoService.Stub(s => s.GetAllTodos()).Return(expectedTodos);

        var sut = new TodoController(_mockTodoService, _mockNotificationService);

        // Act
        var response = sut.GetAllTodoItems();

        // Assert
        var okObjectResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(expectedTodos);
        okObjectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public void GetAll_ReturnsEmptyArray_WhenNoItems()
    {
        // Arrange
        var expectedTodos = new List<Todo>();

        _mockTodoService.Stub(s => s.GetAllTodos()).Return(expectedTodos);

        var sut = new TodoController(_mockTodoService, _mockNotificationService);

        // Act
        var response = sut.GetAllTodoItems();

        // Assert
        var okObjectResult = response.Should().BeOfType<OkObjectResult>().Subject;
        okObjectResult.Value.Should().BeEquivalentTo(expectedTodos);
        okObjectResult.StatusCode.Should().Be(200);
    }

    #endregion

    #region Delete

    [Fact]
    public void Delete_Returns500_AndErrorMessageThrown_WhenExceptionThrown()
    {
        // Arrange
        const string errorMessage = "Failed to delete item id doesn't exist";
        _mockTodoService.Stub(s => s.Delete(1)).Throw(new Exception(errorMessage));

        // or less specific way (regardless of id arg) is:
        _mockTodoService.Stub(s => s.Delete(Arg<int>.Is.Anything)).Throw(new Exception(errorMessage));

        var sut = new TodoController(_mockTodoService, _mockNotificationService);

        // Act
        var response = sut.DeleteTodoItem(1);

        // cast necessary to access relevant properties
        var okObjectResult = (ObjectResult)response;

        // Assert
        okObjectResult.StatusCode.Should().Be(500);
        (okObjectResult.Value as ProblemDetails)?.Detail.Should().Be(errorMessage);
    }
    
    [Fact]
    public void DeleteAPI_CallsNotificationService_WithTaskId_AndUserId()
    {
        // Act
        var result = new TodoController(_mockTodoService, _mockNotificationService).DeleteTodoItem(1);
        
        // Assert
        _mockNotificationService.AssertWasCalled(x=>x.NotifyUserTaskCompleted(1,1));
    }

    #endregion
}
