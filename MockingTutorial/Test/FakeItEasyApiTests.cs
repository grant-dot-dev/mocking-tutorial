using System.Runtime.InteropServices.JavaScript;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Service;
using WebApi;
using Xunit;

namespace Test;

public class FakeItEasyApiTests
{
    private readonly ITodoService _fakeTodoService;
    private readonly INotificationService _fakeNotificationService;

    public FakeItEasyApiTests()
    {
        _fakeTodoService = A.Fake<ITodoService>();
        _fakeNotificationService = A.Fake<INotificationService>();
    }

    #region Get

    [Fact]
    public void GetAll_ReturnsExpectedData()
    {
        // Arrange
        var expectedTodos = new List<Todo>
        {
            new() { Id = 1, Description = "Task 1", IsCompleted = false },
            new() { Id = 2, Description = "Task 2", IsCompleted = true }
        };

        // setup the mock service and Get Endpoint mocked return value
        A.CallTo(() => _fakeTodoService.GetAllTodos()).Returns(expectedTodos);
        var sut = new TodoController(_fakeTodoService, _fakeNotificationService);

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

        A.CallTo(() => _fakeTodoService.GetAllTodos()).Returns(expectedTodos);

        var sut = new TodoController(_fakeTodoService, _fakeNotificationService);

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
        A.CallTo(() => _fakeTodoService.Delete(1)).Throws(new Exception(errorMessage));

        // or less specific way (regardless of id arg) is:
        A.CallTo(() => _fakeTodoService.Delete(A<int>._)).Throws(new Exception(errorMessage));

        var sut = new TodoController(_fakeTodoService, _fakeNotificationService);

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
        A.CallTo(() => _fakeTodoService.Delete(A<int>._)).DoesNothing();
        
        // Act
        var result = new TodoController(_fakeTodoService, _fakeNotificationService).DeleteTodoItem(1);
        
        // Assert
        A.CallTo(() => _fakeNotificationService.NotifyUserTaskCompleted(1,1)).MustHaveHappened(1, Times.Exactly);
    }

    #endregion
}
