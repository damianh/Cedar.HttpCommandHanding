namespace Cedar.HttpCommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class CommandHandlingFixture
    {
        private readonly Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>> _midFunc;
        private readonly List<object> _receivedCommands = new List<object>();

        public CommandHandlingFixture()
        {
            var module = new CommandHandlerModule();
            module.For<TestCommand>()
                .Handle(commandMessage =>
                {
                    _receivedCommands.Add(commandMessage);
                });
            module.For<TestCommandWhoseHandlerThrowsStandardException>()
                .Handle(_ => { throw new InvalidOperationException(); });
            module.For<TestCommandWhoseHandlerThrowProblemDetailsException>()
                .Handle((_, __) =>
                {
                    var problemDetails = new HttpProblemDetails(HttpStatusCode.BadRequest)
                    {
                        Type = new Uri("http://localhost/type"),
                        Detail = "You done goof'd",
                        Instance = new Uri("http://localhost/errors/1"),
                        Title = "Jimmies Ruslted"
                    };
                    throw new HttpProblemDetailsException(problemDetails);
                });
            module.For<TestCommandWhoseHandlerThrowExtendedProblemDetailsException>()
                .Handle((_, __) =>
                {
                    var extendedProblemDetails = new ExtendedHttpProblemDetails(HttpStatusCode.BadRequest)
                    {
                        Type = new Uri("http://localhost/type"),
                        Detail = "You done goof'd",
                        Instance = new Uri("http://localhost/errors/1"),
                        Title = "Jimmies Ruslted",
                        Extension = "Some more data"
                    };
                    throw new HttpProblemDetailsException(extendedProblemDetails);
                });
            module.For<TestCommandWhoseHandlerThrowsExceptionThatIsConvertedToProblemDetails>()
               .Handle((_, __) => { throw new ApplicationException("Custom application exception"); });

            var handlerResolver = new CommandHandlerResolver(module);
            var commandHandlingSettings = new CommandHandlingSettings(handlerResolver)
            {
                CreateProblemDetails = CreateProblemDetails
            };

            _midFunc = CommandHandlingMiddleware.HandleCommands(commandHandlingSettings);
        }

        public List<object> ReceivedCommands
        {
            get { return _receivedCommands; }
        }

        private static HttpProblemDetails CreateProblemDetails(Exception ex)
        {
            var applicationExcepion = ex as ApplicationException;
            if(applicationExcepion != null)
            {
                return new HttpProblemDetails(HttpStatusCode.BadRequest)
                {
                    Title = "Application Exception",
                    Detail = applicationExcepion.Message,
                    Type = new Uri("urn:ApplicationException")
                };
            }
            return null;
        }

        public HttpClient CreateHttpClient()
        {
            return _midFunc.CreateEmbeddedClient();
        }
    }

    public class TestCommand { }

    public class TestCommandWithoutHandler { }

    public class TestCommandWhoseHandlerThrowsStandardException { }

    public class TestCommandWhoseHandlerThrowProblemDetailsException { }

    public class TestCommandWhoseHandlerThrowExtendedProblemDetailsException {}

    public class TestCommandWhoseHandlerThrowsExceptionThatIsConvertedToProblemDetails { }

    public class ExtendedHttpProblemDetails : HttpProblemDetails
    {
        public ExtendedHttpProblemDetails(HttpStatusCode status): base(status)
        {}

        public string Extension { get; set; }

        public override HttpProblemDetailsDto GetDto()
        {
            return new ExtendedHttpProblemDetailsDto
            {
                Detail = Detail,
                Status = (int)Status,
                Instance = Instance != null ? Instance.ToString() : null,
                Title = Title,
                Type = Type != null ? Type.ToString() : null,
                Extension = Extension
            };
        }
    }

    public class ExtendedHttpProblemDetailsDto : HttpProblemDetailsDto
    {
        public string Extension { get; set; }
    }
}