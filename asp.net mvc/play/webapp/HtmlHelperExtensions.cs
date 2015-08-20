using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using webapp.Models;

namespace webapp
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString HtmlConvertToJson(this HtmlHelper htmlHelper, object model)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            return new HtmlString(JsonConvert.SerializeObject(model, settings));
        }

        public static MvcHtmlString BuildSortableLink(
            this HtmlHelper htmlHelper, string fieldName, string actionName, string sortField, QueryOptions queryOptions)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var isCurrentSortField = queryOptions.SortField == sortField;

            return new MvcHtmlString(string.Format("<a href=\"{0}\">{1} {2}</a>",
              urlHelper.Action(actionName,
              new
              {
                  SortField = sortField,
                  SortOrder = (isCurrentSortField
                      && queryOptions.SortOrder == SortOrder.Asc)
                    ? SortOrder.Desc : SortOrder.Asc
              }),
              fieldName,
              BuildSortIcon(isCurrentSortField, queryOptions)));
        }

        private static string BuildSortIcon(bool isCurrentSortField
            , QueryOptions queryOptions)
        {
            string sortIcon = "sort";

            if (isCurrentSortField)
            {
                sortIcon += "-by-alphabet";
                if (queryOptions.SortOrder == SortOrder.Desc)
                    sortIcon += "-alt";
            }

            return string.Format("<span class=\"{0} {1}{2}\"></span>",
              "glyphicon", "glyphicon-", sortIcon);
        }

        public static MvcHtmlString BuildNextPreviousLinks(
            this HtmlHelper htmlHelper,
            QueryOptions options,
            string actionName)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            return new MvcHtmlString(
                "<nav>" +
                "<ul class=\"pager\">" +
                $"<li class=\"previous {IsPreviousDisabled(options)}\">{BuildPreviousLink(urlHelper, options, actionName)}</li>" +
                $"<li class=\"next {IsNextDisabled(options)} \">{BuildNextLink(urlHelper, options, actionName)}</li>" +
                "</ul>" +
                "</nav>");
        }

        private static string BuildNextLink(UrlHelper urlHelper, QueryOptions options, string actionName)
        {
            return string.Format(
                "<a href=\"{0}\">Next <span aria-hidden=\"true\">&rarr;</span></a>",
                urlHelper.Action(actionName, new
                {
                    options.SortOrder,
                    options.SortField,
                    CurrentPage = options.CurrentPage + 1,
                    options.PageSize
                }));
        }

        private static string IsNextDisabled(QueryOptions options)
        {
            return (options.CurrentPage == options.TotalPages) ? "disabled" : string.Empty;
        }

        private static string IsPreviousDisabled(QueryOptions queryOptions)
        {
            return (queryOptions.CurrentPage == 1) ? "disabled" : string.Empty;
        }

        private static string BuildPreviousLink(UrlHelper urlHelper, QueryOptions queryOptions, string actionName)
        {
            return string.Format("<a href=\"{0}\"><span aria-hidden=\"true\">&larr;</span> Previous</a>",
                urlHelper.Action(actionName,
                    new
                    {
                        queryOptions.SortOrder,
                        queryOptions.SortField,
                        CurrentPage = queryOptions.CurrentPage - 1,
                        queryOptions.PageSize
                    }));
        }
    }
}