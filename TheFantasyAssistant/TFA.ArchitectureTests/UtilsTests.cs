namespace TFA.ArchitectureTests;

public class UtilsTests
{
    private readonly Assembly Assembly = typeof(Utils.AssemblyReference).Assembly;

    [Fact]
    public void Utils_ShouldNot_HaveDependenciesOnOtherProjects()
    {
        TestResult testResult = Types
            .InAssembly(Assembly)
            .ShouldNot()
            .HaveDependencyOnAny()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }
}
