namespace Cedar.CommandHandling.Http.TestHost
{
    using System;
    using System.Diagnostics;
    using Cedar.CommandHandling;
    using Cedar.CommandHandling.Http;
    using Cedar.CommandHandling.Http.TypeResolution;
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.Hosting;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    internal class Program
    {
        private static void Main()
        {
            var resolver = new CommandHandlerResolver(new CommandModule());

            var commandMediaTypeMap = new CommandMediaTypeMap(new CommandMediaTypeWithQualifierVersionFormatter())
            {
                { typeof(CommandThatIsAccepted).Name, typeof(CommandThatIsAccepted) },
                { typeof(CommandThatThrowsProblemDetailsException).Name, typeof(CommandThatThrowsProblemDetailsException) }
            };

            var settings = new CommandHandlingSettings(resolver, commandMediaTypeMap);

            var commandHandlingMiddleware = CommandHandlingMiddleware.HandleCommands(settings);

            using(WebApp.Start("http://localhost:8080",
                app =>
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = new PathString(""),
                        FileSystem = new PhysicalFileSystem(@"..\..\wwwroot")
                    });
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        RequestPath = new PathString("/cedarjs"),
                        FileSystem = new PhysicalFileSystem(@"..\..\..\Cedar.CommandHandling.Http.Js")
                    });
                    app.Map("/test/commands", commandsApp => commandsApp.Use(commandHandlingMiddleware));
                }))
            {
                Process.Start("http://localhost:8080/index.html");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}