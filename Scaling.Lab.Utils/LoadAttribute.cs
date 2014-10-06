using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Scaling.Lab.Utils
{
    public class LoadAttribute : ActionFilterAttribute
    {
        private readonly int _delayMilliseconds;
        private readonly int _cpuValue;

        public LoadAttribute(int delayMilliseconds = 10, int cpuValue = 10)
        {
            _delayMilliseconds = delayMilliseconds;
            _cpuValue = cpuValue;
        }

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.Request.Method == HttpMethod.Get)
            {
                var t1 = Task.Delay(_delayMilliseconds, cancellationToken);
                var t2 = Task.Run(() => CpuUtils.Slow(_cpuValue), cancellationToken);
                await Task.WhenAll(t1, t2);
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}