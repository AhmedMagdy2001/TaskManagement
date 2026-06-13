using FluentAssertions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;

public class TaskTests
{
    [Fact]
    public void Task_Should_Start_As_Pending()
    {
        var task = new TaskItem
        {
            Title = "New Task",
            Status = TasksStatus.Pending
        };

        task.Status.Should().Be(TasksStatus.Pending);
    }
}