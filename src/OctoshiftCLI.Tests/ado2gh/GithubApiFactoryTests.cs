using System.Net.Http;
using FluentAssertions;
using Moq;
using OctoshiftCLI.AdoToGithub;
using Xunit;

namespace OctoshiftCLI.Tests.AdoToGithub.Commands
{
    public class GithubApiFactoryTests
    {
        private const string GH_PAT = "GH_PAT";

        [Fact]
        public void Create_Should_Create_Github_Api_With_Github_Pat()
        {
            // Arrange
            var environmentVariableProviderMock = new Mock<EnvironmentVariableProvider>(null);
            environmentVariableProviderMock.Setup(m => m.GithubPersonalAccessToken()).Returns(GH_PAT);

            using var httpClient = new HttpClient();

            // Act
            var factory = new GithubApiFactory(null, httpClient, environmentVariableProviderMock.Object);
            var result = factory.Create();

            // Assert
            result.Should().NotBeNull();
            httpClient.DefaultRequestHeaders.Authorization.Parameter.Should().Be(GH_PAT);
            httpClient.DefaultRequestHeaders.Authorization.Scheme.Should().Be("Bearer");
        }
    }
}
