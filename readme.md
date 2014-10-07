# Scaling Lab

### Scenario

The Company that runs the popular RPS-Game-As-A-Service is having trouble to handle traffic and performance.
The service is a single solution with six endpoints. They now seek your help to scale the solution to handle the ever growing traffic.

In this lab you're going to diagnose their problem and investige options a round azure web sites scaling that could aid in solving the issues.

> The client doen't utilize and caching.

### Prequisites

- Add nuget feed - myget.org/f/treefort

###API

##### PlayerOne - Create a game with first move
> **POST** (api/games) 

	`{ playerName ="player", gameName = "testGame", move = "paper"}`

returns Accepted (202)

##### PlayerTwo - Make a move
> **PUT** (your-site/api/games/awailable/{gameId})

    { playerName = "player2", move = "rock" }

returns Accepted (202)

##### Available Games

>**GET** (api/Games/available/)

returns available games.

>**GET** api/Games/available/{ id }

returns single availible game (200/404)

##### Ended Games

>**GET** (api/Games/ended)

returns ended games. (200)


>**GET** api/Games/ended/{ id }

returns single ended game (200/404)

### Tasks
- Configure diagnostics for Azure Web Sites and add Tracing
- Load test
- Add NewRelic to further aid diagnostics
- Investigate the web sites auto scale options
- Scaling and entity caching - problems ?


### Full Game Test

 	using (var client = new HttpClient())
            {
                var createResponse = await client.PostAsJsonAsync(_baseAddress + "api/Games/", new { playerName = "Mario", gameName = "FullGame", move = "rock" });
                await Task.Delay(50);
                var awailableResponse = await client.GetAsync(createResponse.Headers.Location);
                var game = await awailableResponse.Content.ReadAsAsync<Game>();
                var moveResponse = await client.PutAsJsonAsync(_baseAddress + "api/Games/awailable/" + game.GameId, new { playerName = "Lugi", move = "paper" });
                await Task.Delay(50);
                var endenResponse = await client.GetAsync(moveResponse.Headers.Location);
                var endGame = await endenResponse.Content.ReadAsAsync<EndedGame>();

                Assert.Equal("PlayerTwoWin", endGame.Winner);     
            } 