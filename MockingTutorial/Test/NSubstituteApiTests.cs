using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Service;
using WebApi;
using Xunit;

namespace Test;

public class NSubstituteApiTests
{
    private readonly ITodoService _substituteTodoService;
    private readonly INotificationService _notificationService;
    
    public NSubstituteApiTests()
    {
        _substituteTodoService = Substitute.For<ITodoService>();
        _notificationService = Substitute.For<INotificationService>();
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

        _substituteTodoService.GetAllTodos().Returns(expectedTodos);

       
        var sut = new TodoController(_substituteTodoService, _notificationService);

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

        _substituteTodoService.GetAllTodos().Returns(expectedTodos);

        var sut = new TodoController(_substituteTodoService, _notificationService);

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
        _substituteTodoService.When(x => x.Delete(1)).Do(x => throw new Exception(errorMessage));

        // or less specific way (regardless of id arg) is:
        // NSubstitute
        _substituteTodoService.When(x => x.Delete(Arg.Any<int>())).Do(x => throw new Exception(errorMessage));

        var sut = new TodoController(_substituteTodoService,_notificationService);

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
        var result = new TodoController(_substituteTodoService, _notificationService).DeleteTodoItem(1);
        
        // Assert
        _notificationService.Received().NotifyUserTaskCompleted(1,1);
    }
    

    #endregion
}
