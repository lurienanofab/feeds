using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Specialized;

namespace Feeds.Controllers
{
    public class AjaxController : Controller
    {
        [Route("ajax/log")]
        public ActionResult GetLogItems()
        {
            var dtp = new DataTablesParams(HttpContext.Request.Form);

            IQueryable<FeedsLog> query = DA.Current.Query<FeedsLog>();
            IQueryable<FeedsLog> filtered = null;

            if (!string.IsNullOrEmpty(dtp.Search.Value))
            {
                filtered = query.Where(x => x.RequestIP.Contains(dtp.Search.Value) || x.RequestURL.Contains(dtp.Search.Value) || x.RequestUserAgent.Contains(dtp.Search.Value));
            }
            else
            {
                filtered = query;
            }

            if (dtp.Order.Count() > 0)
            {
                foreach (var ord in dtp.Order)
                {
                    var col = dtp.Columns.ElementAt(ord.Column);
                    if (col.Orderable)
                    {
                        if (col.Name == "DateTime")
                        {
                            if (ord.Dir == "asc")
                                filtered = filtered.OrderBy(x => x.EntryDateTime);
                            else
                                filtered = filtered.OrderByDescending(x => x.EntryDateTime);

                        }
                    }
                }
            }

            var data = filtered.Skip(dtp.Start).Take(dtp.Length).ToList();

            var draw = dtp.Draw;
            var recordsTotal = query.Count();
            var recordsFiltered = filtered.Count();

            return Json(new { draw, recordsTotal, recordsFiltered, data });
        }


    }

    public class DataTablesParams
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public SearchParam Search { get; set; }
        public IEnumerable<OrderParam> Order { get; set; }
        public IEnumerable<ColumnParam> Columns { get; set; }

        public DataTablesParams(NameValueCollection nvc)
        {
            Draw = int.Parse(nvc["draw"]);
            Start = int.Parse(nvc["start"]);
            Length = int.Parse(nvc["length"]);
            Search = new SearchParam(nvc["search[value]"], nvc["search[regex]"]);
            Order = GetOrder(nvc);
            Columns = GetColumns(nvc);
        }

        private IEnumerable<ColumnParam> GetColumns(NameValueCollection nvc)
        {
            var result = new List<ColumnParam>();

            int index = 0;
            while (nvc.AllKeys.Any(x => x.StartsWith(string.Format("columns[{0}]", index))))
            {
                result.Add(new ColumnParam()
                {
                    Data = nvc[string.Format("columns[{0}][data]", index)],
                    Name = nvc[string.Format("columns[{0}][name]", index)],
                    Searchable = bool.Parse(nvc[string.Format("columns[{0}][searchable]", index)]),
                    Orderable = bool.Parse(nvc[string.Format("columns[{0}][orderable]", index)]),
                    Search = new SearchParam(nvc[string.Format("columns[{0}][search][value]", index)], nvc[string.Format("columns[{0}][search][regex]", index)])
                });

                index++;
            }

            return result;
        }

        private IEnumerable<OrderParam> GetOrder(NameValueCollection nvc)
        {
            var result = new List<OrderParam>();

            int index = 0;
            while (nvc.AllKeys.Any(x => x.StartsWith(string.Format("order[{0}]", index))))
            {
                result.Add(new OrderParam()
                {
                    Column = int.Parse(nvc[string.Format("order[{0}][column]", index)]),
                    Dir = nvc[string.Format("order[{0}][dir]", index)]
                });

                index++;
            }

            return result;
        }
    }

    public class SearchParam
    {
        public string Value { get; set; }
        public string Regex { get; set; }

        public SearchParam(string value, string regex)
        {
            Value = value;
            Regex = regex;
        }
    }

    public class ColumnParam
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public SearchParam Search { get; set; }
    }

    public class OrderParam
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }
}