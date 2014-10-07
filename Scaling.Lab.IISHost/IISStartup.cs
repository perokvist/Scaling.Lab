using Microsoft.Owin;
using Owin;
using Scaling.Lab.IISHost;

[assembly: OwinStartup(typeof(IISStartup))]

namespace Scaling.Lab.IISHost
{
    public class IISStartup
    {
        public void Configuration(IAppBuilder app)
        {
            //http://www.asp.net/aspnet/overview/owin-and-katana/owin-middleware-in-the-iis-integrated-pipeline
            //app.UseStageMarker(PipelineStage.Authenticate);
            new RPS.Api.Startup().Configuration(app);
        }
    }
}
