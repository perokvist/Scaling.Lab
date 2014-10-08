# Scaling Lab

### Scenario

The Company that runs the popular RPS-Game-As-A-Service is having trouble to handle traffic and performance.
The service is a single solution with six endpoints. They now seek your help to scale the solution to handle the ever growing traffic.

In this lab you're going to diagnose their problem and investige options around azure web sites scaling that could aid in solving the issues.

> Note: The client doesn't utilize and caching, for the purpose of the lab.

### Prequisites

- Add nuget feed - https://myget.org/f/treefort

###API

##### PlayerOne - Create a game with first move
> **POST** (api/games) 

	`{ playerName ="player", gameName = "testGame", move = "paper"}`

returns Accepted (202) with location header.

##### PlayerTwo - Make a move
> **PUT** (api/games/available/{gameId})

    { playerName = "player2", move = "rock" }

returns Accepted (202) with location header.

##### Available Games

>**GET** (api/Games/available/)

returns available games.

>**GET** api/Games/available/{ id }

returns single available game (200/404)

##### Ended Games

>**GET** (api/Games/ended)

returns ended games. (200)


>**GET** api/Games/ended/{ id }

returns single ended game (200/404)


### Instructions

##### Part 1 - Exploring

1. Publish the solution to an Azure web site.
2. Try out site manager on yoursite.scm.azurewebsites.net
3. Enable Trace logging in the app Startup
4. Find one or more ways to use tracing. (see resources)
5. Add some metric to observe. [documentation](http://azure.microsoft.com/en-us/documentation/articles/web-sites-monitor/)
6. Follow the guide to configure an alert on your metric. Use the Dummy client to get the alert to trigger.
7. Add NewRelic to your site, usig site extensions or [documentation](https://docs.newrelic.com/docs/agents/net-agent/azure-installation/azure-websites). 
8. Use the Dummy client to produce some metric to explore in NewRelic.
9. Add loader.io to your web site and follow instruction to verify your site.
10. Create a simple loader.io test. Check results and NewRelic metrics.

##### Part 2 - Scaling

In this part you should determine a scaling strategy. Configure your site for scaling and explore the strategy through load tests.
In the end of the lab session each team should present their findings and lessons learned.

We'll dicuss this part in the lab introduction.

##### Part 3 - Clean up

Remove all your sites and add-on after presentations.

### Resources
- [Streaming Diagnostics Trace Logging from the Azure Command Line (plus Glimpse!)](http://www.hanselman.com/blog/StreamingDiagnosticsTraceLoggingFromTheAzureCommandLinePlusGlimpse.aspx)
- [Azure Website Logging - Tips and Tools](http://blog.amitapple.com/post/2014/06/azure-website-logging/#.VDPaLfmSyPY)
- [Scaling a standard Azure website to 380k queries per minute of 163M records with loader.io](http://www.troyhunt.com/2014/07/scaling-standard-azure-website-to-380k.html)

### Code Utils

In the code, their are two utils for simulating time and CPU usage. Tweak these as you please to inprove your test/simulation.

Time and CPU on all GET, attribute;

            config.Filters.Add(new LoadAttribute(100, 20));

Util for Prime Number calculation (CPU);

            CpuUtils.Slow(1500);



