namespace Cedar.HttpCommandHandling
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xunit;
    using Xunit.Extensions;

    public class CommandHandlingTests : IUseFixture<CommandHandlingFixture>
    {
        private CommandHandlingFixture _fixture;

        [Fact]
        public void When_put_valid_command_then_should_not_throw()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                Func<Task> act = () => client.PutCommand(new TestCommand(), Guid.NewGuid());

                act.ShouldNotThrow();
            }
        }

        [Fact]
        public async Task When_put_valid_command_then_shoule_receive_the_command()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                var commandId = Guid.NewGuid();
                await client.PutCommand(new TestCommand(), commandId);
                var receivedCommand = _fixture.ReceivedCommands.Last();
                var commandMessage = (CommandMessage<TestCommand>)receivedCommand;

                commandMessage.Command.Should().BeOfType<TestCommand>();
                commandMessage.Command.Should().NotBeNull();
                commandMessage.CommandId.Should().Be(commandId);
                commandMessage.User.Should().NotBeNull();
            }
        }

        [Fact]
        public void When_put_command_whose_handler_throws_standard_exception_then_should_throw()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                Func<Task> act = () => client.PutCommand(new TestCommandWhoseHandlerThrowsStandardException(), Guid.NewGuid());

                act.ShouldThrow<HttpRequestException>();
            }
        }

        [Fact]
        public void When_put_command_whose_handler_throws_http_problem_details_exception_then_should_throw()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                Func<Task> act = () => client.PutCommand(new TestCommandWhoseHandlerThrowProblemDetailsException(), Guid.NewGuid());

                var exception = act.ShouldThrow<HttpProblemDetailsException>().And;

                exception.ProblemDetails.Should().NotBeNull();
                exception.ProblemDetails.Instance.Should().NotBeNull();
                exception.ProblemDetails.Detail.Should().NotBeNull();
                exception.ProblemDetails.Title.Should().NotBeNull();
                exception.ProblemDetails.Type.Should().NotBeNull();
            }
        }

        [Fact]
        public void When_put_command_whose_handler_throws_extended_http_problem_details_exception_then_should_throw()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                Func<Task> act = () => client.PutCommand(new TestCommandWhoseHandlerThrowExtendedProblemDetailsException(), Guid.NewGuid());

                var exception = act.ShouldThrow<HttpProblemDetailsException>().And;

                exception.ProblemDetails.Should().NotBeNull();

                dynamic problemDetails = exception.ProblemDetails;
                ((object)problemDetails.Instance).Should().NotBeNull();
                ((object)problemDetails.Detail).Should().NotBeNull();
                ((object)problemDetails.Title).Should().NotBeNull();
                ((object)problemDetails.Type).Should().NotBeNull();
                //((object)problemDetails.Extension).Should().NotBeNull();
            }
        }

        [Fact]
        public void When_put_command_whose_handler_throws_exception_mapped_to_http_problem_details_exception_then_should_throw()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                Func<Task> act = () => client.PutCommand(new TestCommandWhoseHandlerThrowsExceptionThatIsConvertedToProblemDetails(), Guid.NewGuid());

                var exception = act.ShouldThrow<HttpProblemDetailsException>().And;

                exception.ProblemDetails.Should().NotBeNull();
                exception.ProblemDetails.Instance.Should().BeNull();
                exception.ProblemDetails.Detail.Should().NotBeNull();
                exception.ProblemDetails.Title.Should().NotBeNull();
                exception.ProblemDetails.Type.Should().NotBeNull();
            }
        }

        [Fact]
        public void When_command_endpoint_is_not_found_then_should_throw()
        {
            using (var client = _fixture.CreateHttpClient())
            {
                Func<Task> act = () => client.PutCommand(new TestCommand(), Guid.NewGuid(), "notfoundpath");

                act.ShouldThrow<HttpRequestException>();
            }
        }

        [Theory]
        [InlineData("text/html")]
        [InlineData("text/html+unsupported")]
        public async Task When_request_MediaType_does_not_have_a_valid_serialization_then_should_get_Unsupported_Media_Type(string mediaType)
        {
            using (var client = _fixture.CreateHttpClient())
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Put,
                    Guid.NewGuid().ToString())
                {
                    Content = new StringContent("text")
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                var response = await client.SendAsync(request);

                response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
            }
        }

        public void SetFixture(CommandHandlingFixture data)
        {
            _fixture = data;
        }
    }
}