using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using LNF;
using LNF.Feeds;
using LNF.Repository;
using LNF.Repository.Data;

namespace Feeds.Models
{
    public abstract class FeedModelBase
    {
        public string Format { get; set; }
        public string FileName { get; set; }

        public abstract Feed GetFeed();

        public abstract string GetFeedName();

        public IList<FeedsLog> LogItems(string path)
        {
            //example path: feeds/scheduler/reservations
            return DA.Current.Query<FeedsLog>().Where(x => x.RequestURL.Contains(path)).ToList();
        }
    }

    public static class LogGridControl
    {
        public static IHtmlString LogGrid(this HtmlHelper helper, IList<FeedsLog> items)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table class=\"grid log-grid\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th class=\"log-entry-header\">EntryDateTime</th>");
            sb.AppendLine("<th class=\"log-ipaddr-header\">IP Address</th>");
            sb.AppendLine("<th class=\"log-useragent-header\">UserAgent</th>");
            sb.AppendLine("<th class=\"log-requrl-header\">Requested RUL</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            foreach (FeedsLog item in items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>" + item.EntryDateTime.ToString() + "</td>");
                sb.AppendLine("<td>" + item.RequestIP + "</td>");
                sb.AppendLine("<td>" + item.RequestUserAgent + "</td>");
                sb.AppendLine("<td>" + new Uri(item.RequestURL).PathAndQuery + "</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            return new HtmlString(sb.ToString());
        }
    }
}