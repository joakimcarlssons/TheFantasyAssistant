using TFA.ArchitectureTests.Common;

namespace TFA.ArchitectureTests;

public class ApplicationTests
{
    private readonly Assembly Assembly = typeof(Application.AssemblyReference).Assembly;

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOuterProjects()
    {
        string[] otherProjects =
        [
            Namespaces.InfrastructureNamespace,
            Namespaces.PresentationNamespace,
            Namespaces.ApiNamespace,
            Namespaces.ClientNamespace
        ];

        TestResult shouldNotReference = Types
            .InAssembly(Assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        shouldNotReference.IsSuccessful.Should().BeTrue();
    }

    //[Fact]
    //public void Handlers_Should_Have_DependencyOnDomain()
    //{
    //    TestResult testResult = Types
    //        .InAssembly(Assembly)
    //        .That()
    //        .HaveNameEndingWith("Handler")
    //        .Should()
    //        .HaveDependencyOn(Namespaces.DomainNamespace)
    //        .GetResult();

    //    testResult.IsSuccessful.Should().BeTrue();
    //}
}
