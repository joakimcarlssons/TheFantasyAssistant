using TFA.ArchitectureTests.Common;

namespace TFA.ArchitectureTests;

public class ClientTests
{
    private readonly Assembly Assembly = typeof(Client.AssemblyReference).Assembly;

    [Fact]
    public void Client_Should_Only_HaveDependencyOnDomainProject()
    {
        string[] nonDependencyProjects =
            [
                Namespaces.ApplicationNamespace,
                Namespaces.InfrastructureNamespace,
                Namespaces.InfrastructureNamespace,
                Namespaces.ApiNamespace
            ];

        TestResult shouldNotReference = Types
            .InAssembly(Assembly)
            .ShouldNot()
            .HaveDependencyOnAll(nonDependencyProjects)
            .GetResult();

        shouldNotReference.IsSuccessful.Should().BeTrue();
    }
}
