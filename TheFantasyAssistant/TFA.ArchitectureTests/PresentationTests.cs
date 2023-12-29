using TFA.ArchitectureTests.Common;

namespace TFA.ArchitectureTests;

public class PresentationTests
{
    [Fact]
    public void Presentation_Should_Not_HaveDependencyOnOuterProjects()
    {
        Assembly assembly = typeof(Presentation.AssemblyReference).Assembly;
        string[] otherProjects =
        [
            Namespaces.InfrastructureNamespace,
            Namespaces.ApiNamespace,
            Namespaces.ClientNamespace
        ];

        TestResult shouldNotReference = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        shouldNotReference.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Api_Should_Not_ReferenceClient()
    {
        Assembly assembly = typeof(Api.AssemblyReference).Assembly;

        TestResult shouldNotReference = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(Namespaces.ClientNamespace)
            .GetResult();

        shouldNotReference.IsSuccessful.Should().BeTrue();
    }

    //[Fact]
    //public void Client_Should_Not_ReferenceApi()
    //{
    //    Assembly assembly = typeof(Client.AssemblyReference).Assembly;

    //    TestResult shouldNotReference = Types
    //        .InAssembly(assembly)
    //        .ShouldNot()
    //        .HaveDependencyOn(Namespaces.ApiNamespace)
    //        .GetResult();

    //    shouldNotReference.IsSuccessful.Should().BeTrue();
    //}
}
