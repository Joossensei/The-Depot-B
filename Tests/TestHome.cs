using NUnit.Framework;
using project_depotv1.Classes;
using ReservationSystem;

namespace project_depotv1.Tests;

[TestFixture]
public class ProgramManagerTests
{
    private ConsoleManagerStub _consoleManager = null;

    [SetUp]
    public void SetUp()
    {
        _consoleManager = new ConsoleManagerStub();
        ProgramManger.start(Program.getStartScreen());
    }

    [TearDown]
    public void TearDown()
    {
        
    }
    
    [TestCase("")]
    public void RunWithInputAs1AndName(string name)
    {
        _consoleManager.UserInputs.Enqueue("1");
        Assert.AreEqual("1", "1");
    }
}