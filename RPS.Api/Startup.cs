using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using RPS.Game.Domain;
using RPS.Game.Domain.Public;
using RPS.Game.ReadModel;
using Scaling.Lab.Utils;
using Treefort.Application;
using Treefort.Events;
using Treefort.Infrastructure;
using Treefort.Commanding;
using Treefort.Read;

[assembly: OwinStartup(typeof(RPS.Api.Startup))]

namespace RPS.Api
{
    public static class RouteConfiguration
    {
        public const string GamesRoute = "Games";
        public const string AwailableGamesRoute = "Awailable";
        public const string EndedGamesRoute = "Ended";

    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Local config
            var commandDispatcher = new Dispatcher<ICommand, Task>();

            var awailableGames = new AwailableGames();
            var endedGames = new EndendGames();

            //TODO log to trace
            //http://blog.amitapple.com/post/2014/06/azure-website-logging/#.VC6_jvmSyPY
            var eventPublisher = new EventPublisher(Console.WriteLine, new ProjectionEventListener(awailableGames, endedGames));
            var eventStore = new PublishingEventStore(new InMemoryEventStore(() => new InMemoryEventStream()), eventPublisher);

            commandDispatcher.Register<IGameCommand>(
                command => ApplicationService.UpdateAsync<Game.Domain.Game, GameState>(
                    state => new Game.Domain.Game(state), eventStore, command, game =>
                    {
                        CpuUtils.Slow(500);
                        return game.Handle(command);
                    }));

            var bus = new ApplicationServer(commandDispatcher.Dispatch, new ConsoleLogger());

            var cb = new ContainerBuilder();
            cb.RegisterInstance(bus).AsImplementedInterfaces();
            cb.RegisterType<ReadService>().AsImplementedInterfaces().SingleInstance();
            cb.RegisterInstance(awailableGames).AsSelf();
            cb.RegisterInstance(endedGames).AsSelf();
            cb.RegisterApiControllers(Assembly.GetExecutingAssembly());

            config.DependencyResolver = new AutofacWebApiDependencyResolver(cb.Build());

            app.UseWebApi(config);

            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Welcome to RPS APi.");
            });
        }

    }
}
