using TFA.ArchitectureTests.Common;

namespace TFA.ArchitectureTests;

public class InfrastructureTests
{
    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        Assembly assembly = typeof(Infrastructure.AssemblyReference).Assembly;
        string[] otherProjects = new[]
        {
            Namespaces.PresentationNamespace,
            Namespaces.ApiNamespace,
            Namespaces.ClientNamespace
        };

        TestResult shouldNotReference = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        shouldNotReference.IsSuccessful.Should().BeTrue();
    }
}
