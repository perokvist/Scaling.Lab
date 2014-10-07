using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RPS.Game.ReadModel;

namespace Scaling.Lab.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dummy RPS Client");
            Play(new Uri("http://scaling-lab.azurewebsites.net/"), Console.WriteLine);
            Play(new Uri("http://scaling-lab.azurewebsites.net/"), Console.WriteLine);
            Play(new Uri("http://scaling-lab.azurewebsites.net/"), Console.WriteLine);

            Console.ReadLine();
        }

        static async void Play(Uri baseAddress, Action<string> logger)
        {
            using (var client = new HttpClient())
            {
                logger("Creating Game");
                var createResponse = await client.PostAsJsonAsync(baseAddress + "api/Games/", new { playerName = "Mario", gameName = "FullGame", move = "rock" });
                logger(string.Format("Game Created @ {0}", createResponse.Headers.Location));
                await Task.Delay(50);
                var availableResponse = await client.GetAsync(createResponse.Headers.Location);
                var game = await availableResponse.Content.ReadAsAsync<Game>();
                logger(string.Format("Making move on Game : {0}", game.GameId));
                var moveResponse = await client.PutAsJsonAsync(baseAddress + "api/Games/available/" + game.GameId, new { playerName = "Lugi", move = "paper" });
                logger(string.Format("Move made @ {0}", moveResponse.Headers.Location));
                for (int i = 1; i < 5; i++)
                {
                    await Task.Delay(50);
                    var endenResponse = await client.GetAsync(moveResponse.Headers.Location);
                    if (endenResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        var endGame = await endenResponse.Content.ReadAsAsync<EndedGame>();
                        logger(string.Format("Winner : {0}", endGame.Winner));
                        return;
                    }
                    logger(string.Format("No ended game found after {0} milliseconds", i * 50));
                }
                
               
               
            }
        }
    }
}
