using System.Linq;
using System.Web.Mvc;

namespace AsyncContextFlowStudy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Entries = TestLog.Instance.GetEntries()
                .OrderBy(x => x.Timestamp)
                .ToArray();

            return View();
        }

        [HttpPost]
        public ActionResult Reset()
        {
            TestLog.Instance.Reset();
            return RedirectToAction("Index");
        }
    }
}