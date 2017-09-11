using Feeds.Models;
using LNF;
using LNF.Feeds;
using System;
using System.Text;
using System.Web.Mvc;

namespace Feeds.Controllers
{
    public class SchedulerController : Controller
    {
        [Route("scheduler")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("scheduler/reservations")]
        public ActionResult Reservations(SchedulerReservationsModel model)
        {
            return View(model);
        }

        [HttpPost, Route("scheduler/reservations")]
        public ActionResult ReservationsGetFeed(SchedulerReservationsModel model)
        {
            if (string.IsNullOrEmpty(model.FileName))
            {
                if (model.UserName == "all" && model.ResourceID != "all")
                    model.FileName = "tool-reservations";
                else if (model.UserName != "all" && model.ResourceID == "all")
                    model.FileName = "user-reservations";
                else if (model.UserName != "all" && model.ResourceID != "all")
                    model.FileName = "user-tool-reservations";
                else
                    model.FileName = "all-reservations";
            }

            return RedirectToAction("ReservationsFeed", new { model.Format, model.UserName, model.ResourceID, model.FileName });
        }

        [HttpGet, Route("scheduler/reservations/{Format}/{UserName}/{ResourceID}/{FileName}")]
        public ActionResult ReservationsFeed(SchedulerReservationsModel model)
        {
            if (!string.IsNullOrEmpty(model.Format))
            {
                Feed f = model.GetFeed();
                f.WriteLogEntry(Providers.Context.Current);
                string content = f.Render(model.GetFeedName());
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                return File(buffer, f.ContentType, f.GetFileNameWithExtension(model.FileName));
            }
            else
                return RedirectToAction("Reservations");
        }
    }
}