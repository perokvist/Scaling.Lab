using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RPS.Game.ReadModel;
using Treefort.Commanding;
using Treefort.Common;
using RPS.Game.Domain;

namespace RPS.Api.Controllers
{

    [RoutePrefix("api/Games")]
    public class GamesController : ApiController
    {
        private readonly ICommandBus _commandBus;
        private readonly IReadService _readService;

        public GamesController(ICommandBus commandBus, IReadService readService)
        {
            _commandBus = commandBus;
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
            _commandBus.SendAsync(command); //Note - fire and forget with app server 

            return Request.CreateResponse(HttpStatusCode.Accepted)
                .Tap(message => message.Headers.Location = new Uri(Url.Link(RouteConfiguration.AwailableGamesRoute, new { id = gameId })));
        }

        [HttpPut]
        [Route("awailable/{id:Guid}")]
        public HttpResponseMessage Move(Guid id, Game.ReadModel.MakeMoveCommand input)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            Move move;
            if (!Enum.TryParse(input.Move, true, out move))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid move");

            var command = new Game.Domain.MakeMoveCommand(id, move, input.PlayerName);

            _commandBus.SendAsync(command);
            return Request.CreateResponse(HttpStatusCode.Accepted).Tap(
                    r => r.Headers.Location = new Uri(Url.Link(RouteConfiguration.EndedGamesRoute, new { id })));
        }


        [Route("awailable/{id:Guid}", Name = RouteConfiguration.AwailableGamesRoute)]
        public IHttpActionResult GetAwailableGame(Guid id)
        {
            var game = _readService
                .AwailableGames
                .SingleOrDefault(x => x.GameId == id);

            if (game != null)
                return Ok(game);
            return NotFound();
        }

        [Route("awailable")]
        public IEnumerable<Game.ReadModel.Game> GetAwailableGames()
        {
            return _readService
                .AwailableGames
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
