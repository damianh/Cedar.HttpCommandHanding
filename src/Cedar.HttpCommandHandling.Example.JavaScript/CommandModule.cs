namespace Cedar.HttpCommandHandling.Example.JavaScript
{
    using System;
    using System.Net;

    public class CommandModule : CommandHandlerModule
    {
        public CommandModule()
        {
            For<CommandThatIsAccepted>().Handle(message
                => Console.WriteLine("Command {0} with {1}", message.CommandId, message.Command.Value));
            For<CommandThatThrowsProblemDetailsException>().Handle(_ =>
            {
                var details = new HttpProblemDetails
                {
                    Status = (int) HttpStatusCode.BadRequest,
                    Title = "A Bad Request",
                    Detail = "A command that is not accepted"
                };
                throw new HttpProblemDetailsException<HttpProblemDetails>(details);
            });
        }
    }

    public class CommandThatIsAccepted
    {
        public string Value { get; set; }
    }

    public class CommandThatThrowsProblemDetailsException
    {}
}