using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Headless;
using Xunit;
using AvaloniaTest.Helpers;
using Xunit.Abstractions;
namespace Tests.UITest { 
public class ClockTest
{
    private readonly ITestOutputHelper output;

    public ClockTest(ITestOutputHelper output)
    {
        this.output = output;
    }


    [Fact]
    public void AddTask_ShouldAddNewTask()
    {
        Action testTask = () => { Console.WriteLine("Test task excuted"); };
        TimeSpan interval = TimeSpan.FromSeconds(1);
        Clock.Instance.AddTask("TestTask", testTask, interval);

        var tasks = Clock.Instance.Tasks;

        Assert.True(tasks.ContainsKey("TestTask"));
        Assert.Equal(interval, tasks["TestTask"].Interval);
    }

    [Fact]
    public void RemoveTask_ShouldRemoveTask()
    {
        // Arrange
        var clock = Clock.Instance;
        Action testTask = () => Console.WriteLine("Task executed");
        TimeSpan interval = TimeSpan.FromSeconds(1);
        clock.AddTask("TestTask", testTask, interval);

        // Act
        clock.RemoveTask("TestTask");

        // Assert
        var tasks = clock.Tasks;
        Assert.False(tasks.ContainsKey("TestTask"));
    }


    [AvaloniaFact]
    public async void Should_Type_Text_Into_TextBox()
    {
        // Setup controls:
       // var textBox = new TextBox();
       // var window = new Window { Content = textBox };

        // Open window:
      //  window.Show();

        // Focus text box:
       // textBox.Focus();

        // Simulate text input:
      //  window.KeyTextInput("Hello World");
        var clock = Clock.Instance;
        var initialTime = clock.CurrentTime;
        await Task.Delay(1000);
        var updatedTime = clock.CurrentTime;
        Assert.NotEqual(initialTime, updatedTime);
      }

        [AvaloniaFact]
        public async Task AddTask_ExecutesTaskAtInterval()
        {
            var clock = Clock.Instance;
            bool taskExecuted = false;

            // Dodajemy zadanie do wykonania co 1 sekundę
            clock.AddTask("TestTask", () =>
            {
                taskExecuted = true;
            }, TimeSpan.FromSeconds(1));

            // Czekamy 2 sekundy, aby upewnić się, że zadanie zostało wykonane
            await Task.Delay(2000); // Poczekaj 2 sekundy

            // Assert
            Assert.True(taskExecuted, "Task was not executed after the specified interval.");
        }


}
}