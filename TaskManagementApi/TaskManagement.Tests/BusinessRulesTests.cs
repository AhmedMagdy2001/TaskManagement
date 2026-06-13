using Xunit;
using FluentAssertions;
using TaskManagement.Domain.Entities;

public class BusinessRulesTests
{
    [Fact]
    public void Should_Detect_Duplicate_Task_Title_Same_Day()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Title = "Test Task",
                CreatedAt = DateTime.UtcNow
            }
        };

        var newTaskTitle = "Test Task";

        var isDuplicate = tasks.Any(x =>
            x.Title == newTaskTitle &&
            x.CreatedAt.Date == DateTime.UtcNow.Date);

        isDuplicate.Should().BeTrue();
    }
}