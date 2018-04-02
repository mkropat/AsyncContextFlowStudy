using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using Nito.AsyncEx;

namespace AsyncContextFlowStudy.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class AsyncTestController : Controller
    {
        public async Task<ActionResult> FullAsync()
        {
            var correlationId = Guid.NewGuid();
            var expectedUser = Thread.CurrentPrincipal.Identity?.Name;

            TestLog.Instance.Write(correlationId, $"Started FullAsync Test");
            WaitForNextSecond();

            var i = 0;
            var end = DateTime.UtcNow.AddMinutes(1);
            while (DateTime.UtcNow < end)
            {
                var message = $"Running {i + 1}";
                if (expectedUser != Thread.CurrentPrincipal.Identity?.Name)
                    message += " *********MISMATCH************";
                TestLog.Instance.Write(correlationId, message);

                await Task.Delay(TimeSpan.FromSeconds(RandomGaussian(1, 0.2)));

                i++;
            }
            TestLog.Instance.Write(correlationId, $"Finished");

            return Content("<script>window.close()</script>");
        }

        public ActionResult AsyncContextRun()
        {
            var correlationId = Guid.NewGuid();
            var expectedUser = Thread.CurrentPrincipal.Identity?.Name;

            TestLog.Instance.Write(correlationId, $"Started AsyncContext.Run Test");
            WaitForNextSecond();

            AsyncContext.Run(async () =>
            {
                var i = 0;
                var end = DateTime.UtcNow.AddMinutes(1);
                while (DateTime.UtcNow < end)
                {
                    var message = $"Running {i + 1}";
                    if (expectedUser != Thread.CurrentPrincipal.Identity?.Name)
                        message += " *********MISMATCH************";
                    TestLog.Instance.Write(correlationId, message);

                    await Task.Delay(TimeSpan.FromSeconds(RandomGaussian(1, 0.2)))
                        .ConfigureAwait(false); // force thread hopping (default is to post back to main thread)

                    i++;
                }
                TestLog.Instance.Write(correlationId, $"Finished");
            });

            return Content("<script>window.close()</script>");
        }

        static void WaitForNextSecond()
        {
            var secondLater = DateTime.UtcNow.AddSeconds(1);
            var nextSecond = new DateTime(secondLater.Ticks - (secondLater.Ticks % TimeSpan.TicksPerSecond), secondLater.Kind);
            while (DateTime.UtcNow < nextSecond)
                Thread.Sleep(10);
        }

        static double RandomGaussian(double mean = 0, double stdDev = 1)
        {
            var rand = new Random(); //reuse this if you are generating many
            var u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            var u2 = 1.0 - rand.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }
    }
}