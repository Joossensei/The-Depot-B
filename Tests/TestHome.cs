using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace project_depotv1.Tests;

[TestFixture]
public class programManagerTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void Test()
    {
        Assert.AreEqual("1", "1");
        Assert.Pass();
    }
}