using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using OctoshiftCLI.AdoToGithub;
using OctoshiftCLI.AdoToGithub.Commands;
using Xunit;

namespace OctoshiftCLI.Tests.AdoToGithub.Commands
{
    public class CreateTeamCommandTests
    {
        [Fact]
        public void Should_Have_Options()
        {
            var command = new CreateTeamCommand(null, null);
            Assert.NotNull(command);
            Assert.Equal("create-team", command.Name);
            Assert.Equal(4, command.Options.Count);

            TestHelpers.VerifyCommandOption(command.Options, "github-org", true);
            TestHelpers.VerifyCommandOption(command.Options, "team-name", true);
            TestHelpers.VerifyCommandOption(command.Options, "idp-group", false);
            TestHelpers.VerifyCommandOption(command.Options, "verbose", false);
        }

        [Fact]
        public async Task Happy_Path()
        {
            var githubOrg = "FooOrg";
            var teamName = "foo-team";
            var idpGroup = "foo-group";
            var teamMembers = new List<string>() { "dylan", "dave" };
            var idpGroupId = 42;
            var teamSlug = "foo-slug";

            var mockGithub = new Mock<GithubApi>(null, null);
            mockGithub.Setup(x => x.GetTeamMembers(githubOrg, teamName).Result).Returns(teamMembers);
            mockGithub.Setup(x => x.GetIdpGroupId(githubOrg, idpGroup).Result).Returns(idpGroupId);
            mockGithub.Setup(x => x.GetTeamSlug(githubOrg, teamName).Result).Returns(teamSlug);

            var mockGithubApiFactory = new Mock<GithubApiFactory>(null, null, null);
            mockGithubApiFactory.Setup(m => m.Create()).Returns(mockGithub.Object);

            var command = new CreateTeamCommand(new Mock<OctoLogger>().Object, mockGithubApiFactory.Object);
            await command.Invoke(githubOrg, teamName, idpGroup);

            mockGithub.Verify(x => x.CreateTeam(githubOrg, teamName));
            mockGithub.Verify(x => x.RemoveTeamMember(githubOrg, teamName, teamMembers[0]));
            mockGithub.Verify(x => x.RemoveTeamMember(githubOrg, teamName, teamMembers[1]));
            mockGithub.Verify(x => x.AddEmuGroupToTeam(githubOrg, teamSlug, idpGroupId));
        }
    }
}
