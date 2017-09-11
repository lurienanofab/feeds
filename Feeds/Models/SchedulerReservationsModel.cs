using LNF.Feeds;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Feeds.Models
{
    public class SchedulerReservationsModel : FeedModelBase
    {
        private List<SelectListItem> _UserOptions;
        private List<SelectListItem> _ToolOptions;

        public string UserName { get; set; }
        public string ResourceID { get; set; }

        public List<SelectListItem> UserOptions
        {
            get
            {
                if (_UserOptions == null)
                {
                    _UserOptions = new List<SelectListItem>();
                    IList<ClientInfo> query = ClientInfo.SelectByPriv(ClientPrivilege.LabUser)
                        .Where(x => x.ClientActive && x.ClientOrgActive && x.EmailRank == 1)
                        .OrderBy(x => x.DisplayName).ToList();

                    SelectListItem sli;
                    sli = new SelectListItem();
                    sli.Value = "all";
                    sli.Text = "-- All Users --";
                    _UserOptions.Add(sli);

                    foreach (ClientInfo item in query)
                    {
                        sli = new SelectListItem();
                        sli.Value = item.UserName;
                        sli.Text = item.DisplayName;
                        _UserOptions.Add(sli);
                    }
                }

                return _UserOptions;
            }
        }

        public List<SelectListItem> ToolOptions
        {
            get
            {
                if (_ToolOptions == null)
                {
                    _ToolOptions = new List<SelectListItem>();
                    IList<Resource> query = DA.Current.Query<Resource>().OrderBy(x => x.ResourceName).ToList();

                    SelectListItem sli;
                    sli = new SelectListItem();
                    sli.Value = "all";
                    sli.Text = "-- All Tools --";
                    _ToolOptions.Add(sli);

                    foreach (Resource item in query)
                    {
                        sli = new SelectListItem();
                        sli.Value = item.ResourceID.ToString();
                        sli.Text = item.ResourceName;
                        _ToolOptions.Add(sli);
                    }
                }

                return _ToolOptions;
            }
        }

        public override string GetFeedName()
        {
            return "Reservations|" + UserName + "|" + ResourceID;
        }

        public override Feed GetFeed()
        {
            Feed result = FeedGenerator.Scheduler.Reservations.CreateFeed(FeedFormats.Calendar, UserName, ResourceID);
            return result;
        }
    }
}