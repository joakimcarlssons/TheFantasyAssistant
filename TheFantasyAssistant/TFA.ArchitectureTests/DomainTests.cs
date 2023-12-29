using TFA.ArchitectureTests.Common;

namespace TFA.ArchitectureTests;

public class DomainTests
{
    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOuterProjects()
    {
        Assembly assembly = typeof(Domain.AssemblyReference).Assembly;
        string[] otherProjects = new[]
        {
                Namespaces.ApplicationNamespace,
                Namespaces.InfrastructureNamespace,
                Namespaces.PresentationNamespace,
                Namespaces.ApiNamespace,
                Namespaces.ClientNamespace
            };

        TestResult testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }
}
