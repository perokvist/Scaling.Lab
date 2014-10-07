using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using RPS.Game.Domain;
using RPS.Game.ReadModel;
using Scaling.Lab.Utils;
using Treefort.Application;
using Treefort.Events;
using Treefort.Infrastructure;
using Treefort.Commanding;
using Treefort.Read;

namespace RPS.Api
{
    public static class RouteConfiguration
    {
        public const string GamesRoute = "Games";
        public const string AvailableGamesRoute = "Available";
        public const string EndedGamesRoute = "Ended";

    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Filters.Add(new LoadAttribute(100, 20));

            //Local config
            var commandDispatcher = new Dispatcher<ICommand, Task>();

            var availableGames = new AvailableGames();
            var endedGames = new EndendGames();

            //TODO log to trace
            //http://blog.amitapple.com/post/2014/06/azure-website-logging/#.VC6_jvmSyPY
            var eventPublisher = new EventPublisher(Console.WriteLine, new ProjectionEventListener(availableGames, endedGames));
            var eventStore = new PublishingEventStore(new InMemoryEventStore(() => new InMemoryEventStream()), eventPublisher);

            commandDispatcher.Register<IGameCommand>(
                command => ApplicationService.UpdateAsync<Game.Domain.Game, GameState>(
                    state => new Game.Domain.Game(state), eventStore, command, game =>
                    {
                        CpuUtils.Slow(1500);
                        return game.Handle(command);
                    }));

            var cb = new ContainerBuilder();
            cb.RegisterInstance(commandDispatcher).AsSelf().SingleInstance();
            cb.RegisterType<ReadService>().AsImplementedInterfaces();
            cb.RegisterInstance(availableGames).AsSelf().SingleInstance();
            cb.RegisterInstance(endedGames).AsSelf().SingleInstance();
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
