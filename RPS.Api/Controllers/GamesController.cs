using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RPS.Game.ReadModel;
using Treefort.Application;
using Treefort.Commanding;
using Treefort.Common;
using RPS.Game.Domain;

namespace RPS.Api.Controllers
{

    [RoutePrefix("api/Games")]
    public class GamesController : ApiController
    {
        private readonly Dispatcher<ICommand, Task> _dispatcher;
        private readonly IReadService _readService;

        public GamesController(Dispatcher<ICommand, Task> dispatcher , IReadService readService)
        {
            _dispatcher = dispatcher;
            _readService = readService;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create(Game.ReadModel.CreateGameCommand input)
        {
            var gameId = Guid.NewGuid();

            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            Move move;
            if (!Enum.TryParse(input.Move, true, out move))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid move");

            var command = new Game.Domain.CreateGameCommand(gameId, input.PlayerName, input.GameName, move);
            _dispatcher.Dispatch(command);

            return Request.CreateResponse(HttpStatusCode.Accepted)
                .Tap(message => message.Headers.Location = new Uri(Url.Link(RouteConfiguration.AvailableGamesRoute, new { id = gameId })));
        }

        [HttpPut]
        [Route("available/{id:Guid}")]
        public async Task<HttpResponseMessage> Move(Guid id, Game.ReadModel.MakeMoveCommand input)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            Move move;
            if (!Enum.TryParse(input.Move, true, out move))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid move");

            var command = new Game.Domain.MakeMoveCommand(id, move, input.PlayerName);
            _dispatcher.Dispatch(command);
            return Request.CreateResponse(HttpStatusCode.Accepted).Tap(
                    r => r.Headers.Location = new Uri(Url.Link(RouteConfiguration.EndedGamesRoute, new { id })));
        }


        [Route("available/{id:Guid}", Name = RouteConfiguration.AvailableGamesRoute)]
        public IHttpActionResult GetAvailableGame(Guid id)
        {
            var game = _readService
                .AvailableGames
                .SingleOrDefault(x => x.GameId == id);

            if (game != null)
                return Ok(game);
            return NotFound();
        }

        [Route("available")]
        public IEnumerable<Game.ReadModel.Game> GetAvailableGames()
        {
            return _readService
                .AvailableGames
                .Reverse();
        }

        [Route("ended/{id:Guid}", Name = RouteConfiguration.EndedGamesRoute)]
        public IHttpActionResult GetEndedGame(Guid id)
        {
            var game = _readService
                .EndedGames
                .SingleOrDefault(x => x.GameId == id);

            if (game != null)
                return Ok(game);

            return NotFound();
        }

        [Route("ended")]
        public IEnumerable<EndedGame> GetEndedGames()
        {
            return _readService
                .EndedGames
                .Reverse();
        }

    }

}
