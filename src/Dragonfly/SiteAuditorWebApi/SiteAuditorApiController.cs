namespace Dragonfly.SiteAuditorWebApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web.Http;
    using Dragonfly.NetModels;
    using Dragonfly.SiteAuditorHelpers;
    using Dragonfly.SiteAuditorModels;
    using Newtonsoft.Json;
    using Umbraco.Web.WebApi;
    using Umbraco.Web.WebApi.Filters;

    // /Umbraco/Api/SiteAuditorApi <-- UmbracoApiController
    // /Umbraco/backoffice/Api/SiteAuditorApi <-- UmbracoAuthorizedApiController [IsBackOffice]


    /// <inheritdoc />
    //[PluginController("SiteAuditor")]
    // [UmbracoUserTimeoutFilter]
    [IsBackOffice]

    public class SiteAuditorApiController : UmbracoAuthorizedApiController
    {

        /// /Umbraco/backoffice/Api/SiteAuditorApi/Test
        [System.Web.Http.AcceptVerbs("GET")]
        public bool Test()
        {
            return true;
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/TestCSV
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage TestCsv()
        {
            var returnSB = new StringBuilder();

            var tableData = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                tableData.AppendFormat(
                    "\"{0}\",{1},\"{2}\",{3}{4}",
                    "Name " + i,
                    i,
                    string.Format("Some text about item #{0} for demo.", i),
                    DateTime.Now,
                    Environment.NewLine);
            }
            returnSB.Append(tableData);

            return StringBuilderToFile(returnSB, "Test.csv");
        }

        /// /Umbraco/Api/PublicApi/Help
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage Help()
        {
            var returnSB = new StringBuilder();

            returnSB.AppendLine("<h1>Site Auditor</h1>");

            returnSB.AppendLine("<h2>Information about Content</h2>");

            returnSB.AppendLine("<h3>All Content Nodes</h3>");
            returnSB.AppendLine("<p>These will take a long time to run for large sites. Please be patient.</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsXml\">GetAllContentAsXml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsJson\">GetAllContentAsJson</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsHtmlTable\">GetAllContentAsHtmlTable</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsCsv\">GetAllContentAsCsv</a> [no parameters]</li>");
            returnSB.AppendLine("</ul>");

            returnSB.AppendLine("<h2>Information about Document Type Properties</h2>");

            returnSB.AppendLine("<h3>All Properties</h3>");
            //returnSB.AppendLine("<p>Note</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsXml\">GetAllPropertiesAsXml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsJson\">GetAllPropertiesAsJson</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsHtml\">GetAllPropertiesAsHtml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsCsv\">GetAllPropertiesAsCsv</a> [no parameters]</li>");
            returnSB.AppendLine("</ul>");

            //returnSB.AppendLine("<h3>All Content Nodes</h3>");
            //returnSB.AppendLine("<p>Note</p>");
            //returnSB.AppendLine("<ul>");
            //returnSB.AppendLine("<li><a target=\"_blank\" href=\"\"></a></li>");
            //returnSB.AppendLine("</ul>");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        #region Content Nodes

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsXml
        [System.Web.Http.AcceptVerbs("GET")]
        public List<AuditableContent> GetAllContentAsXml()
        {
            var allNodes = CollectionsHelper.GetContentNodes();

            return allNodes;
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllContentAsJson()
        {
            var returnSB = new StringBuilder();

            var allNodes = CollectionsHelper.GetContentNodes();

            string json = JsonConvert.SerializeObject(allNodes);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsHtmlTable
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllContentAsHtmlTable()
        {
            var returnSB = new StringBuilder();

            var allNodes = CollectionsHelper.GetContentNodes();

            var tableStart = @"
                <table>
                    <tr>
                        <th>Node Name</th>
                        <th>NodeID</th>
                        <th>Node Path</th>
                        <th>DocType</th>
                        <th>ParentID</th>
                        <th>Full URL</th>
                        <th>Level</th>
                        <th>Sort Order</th>
                        <th>Template Name</th>
                        <th>Create Date</th>
                        <th>Update Date</th>
                    </tr>";

            var tableEnd = @"</table>";

            var tableData = new StringBuilder();

            foreach (var auditNode in allNodes)
            {
                tableData.AppendLine("<tr>");

                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.Name));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.Id));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.NodePathAsText));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.ContentType.Alias));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.ParentId));
                tableData.AppendLine(string.Format("<td><a href=\"{0}\" target=\"_blank\">{0}</a></td>", auditNode.FullNiceUrl));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.Level));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.SortOrder));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.TemplateAlias));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.CreateDate));
                tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.UpdateDate));

                tableData.AppendLine("</tr>");
            }

            returnSB.AppendLine(tableStart);
            returnSB.Append(tableData);
            returnSB.AppendLine(tableEnd);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsCsv
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllContentAsCsv()
        {
            var returnSB = new StringBuilder();

            var allNodes = CollectionsHelper.GetContentNodes();

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"Node Name\",\"NodeID\",\"Node Path\",\"DocType\",\"ParentID\",\"Full URL\",\"Level\",\"Sort Order\",\"Template Name\",\"Create Date\",\"Update Date\"");

            foreach (var auditNode in allNodes)
            {
                tableData.AppendFormat(
                    "\"{0}\",{1},\"{2}\",\"{3}\",{4},\"{5}\",{6},{7},\"{8}\",{9},{10}{11}",
                    auditNode.UmbContentNode.Name,
                    auditNode.UmbContentNode.Id,
                    auditNode.NodePathAsCustomText(" > "),
                    auditNode.UmbContentNode.ContentType.Alias,
                    auditNode.UmbContentNode.ParentId,
                    auditNode.FullNiceUrl,
                    auditNode.UmbContentNode.Level,
                    auditNode.UmbContentNode.SortOrder,
                    auditNode.TemplateAlias,
                    auditNode.UmbContentNode.CreateDate,
                    auditNode.UmbContentNode.UpdateDate,
                    Environment.NewLine);
            }
            returnSB.Append(tableData);

            return StringBuilderToFile(returnSB, "AllNodes.csv");
        }

        #endregion

        #region Properties Info

        // /Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsXml
        [System.Web.Http.AcceptVerbs("GET")]
        public IEnumerable<AuditableProperty> GetAllPropertiesAsXml()
        {
            var siteProps = new SiteAuditableProperties();
            var propertiesList = siteProps.AllProperties;
            return propertiesList;
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllPropertiesAsJson()
        {
            var returnSB = new StringBuilder();

            var siteProps = new SiteAuditableProperties();
            var propertiesList = siteProps.AllProperties;
            string json = JsonConvert.SerializeObject(propertiesList);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        // /Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsHtml
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllPropertiesAsHtml()
        {
            var returnSB = new StringBuilder();

            var siteProps = new SiteAuditableProperties();
            var propertiesList = siteProps.AllProperties;

            var tableStart = @"
                <table>
                    <tr>
                        <th>Property Name</th>
                        <th>Property Alias</th>
                        <th>DataType Name</th>
                        <th>DataType Property Editor</th>
                        <th>DataType Database Type</th>
                        <th>DocumentTypes Used In</th>
                        <th>Qty of DocumentTypes</th>
                    </tr>";

            var tableEnd = @"</table>";

            var tableData = new StringBuilder();

            foreach (var prop in propertiesList)
            {
                tableData.AppendLine("<tr>");

                tableData.AppendLine(string.Format("<td>{0}</td>", prop.UmbPropertyType.Name));
                tableData.AppendLine(string.Format("<td>{0}</td>", prop.UmbPropertyType.Alias));
                tableData.AppendLine(string.Format("<td>{0}</td>", prop.DataType.Name));
                tableData.AppendLine(string.Format("<td>{0}</td>", prop.DataType.PropertyEditorAlias));
                tableData.AppendLine(string.Format("<td>{0}</td>", prop.DataType.DatabaseType));
                tableData.AppendLine(string.Format("<td>{0}</td>", string.Join(", ", prop.DocTypes)));
                tableData.AppendLine(string.Format("<td>{0}</td>", prop.DocTypes.Count()));

                tableData.AppendLine("</tr>");
            }

            returnSB.AppendLine(tableStart);
            returnSB.Append(tableData);
            returnSB.AppendLine(tableEnd);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }


        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsCsv
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllPropertiesAsCsv()
        {
            var returnSB = new StringBuilder();

            var siteProps = new SiteAuditableProperties();
            var propertiesList = siteProps.AllProperties;

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"Property Name\",\"Property Alias\",\"DataType Name\",\"DataType Property Editor\",\"DataType Database Type\",\"DocumentTypes Used In\",\"Qty of DocumentTypes\"" );

            foreach (var prop in propertiesList)
            {
                tableData.AppendFormat(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",{6}{7}",
                    prop.UmbPropertyType.Name,
                    prop.UmbPropertyType.Alias,
                    prop.DataType.Name,
                    prop.DataType.PropertyEditorAlias,
                    prop.DataType.DatabaseType,
                    string.Join(", ", prop.DocTypes),
                    prop.DocTypes.Count(),
                    Environment.NewLine);
            }

            returnSB.Append(tableData);

            return StringBuilderToFile(returnSB, "AllProperties.csv");
        }

        //public IHttpActionResult GetProperty(string PropertyAlias)
        //{
        //    var AllPropertiesList = GetAllProperties();
        //    var property = AllPropertiesList.FirstOrDefault((p) => p.Alias == PropertyAlias);
        //    if (property == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(property);
        //}

        #endregion

        #region DataType Info

        /// /Umbraco/backoffice/Api/SiteAuditorApi/ListAllDataTypes?ForExport=false
        [HttpGet]
        public StatusMessage ListAllDataTypes(bool ForExport)
        {
            var returnMsg = new StatusMessage();
            var msgSB = new StringBuilder();

            var datatypeService = this.Services.DataTypeService;

            var datatypes = datatypeService.GetAllDataTypeDefinitions();

            if (ForExport)
            {
                msgSB.AppendLine(" ");
                msgSB.AppendLine(string.Format("{0}|{1}|{2}", "Name", "Property Editor Alias", "GUID"));
            }
            else
            {
                msgSB.AppendLine(string.Format("{0} [{1}] {2}", "Name", "Property Editor Alias", "GUID"));
            }


            foreach (var dt in datatypes)
            {
                if (ForExport)
                {
                    msgSB.AppendLine(string.Format("{0}|{1}|{2}", dt.Name, dt.PropertyEditorAlias, dt.Key));
                }
                else
                {
                    msgSB.AppendLine(string.Format("{0} [{1}] {2}", dt.Name, dt.PropertyEditorAlias, dt.Key));
                }
            }

            returnMsg.Message = msgSB.ToString();
            returnMsg.Success = true;
            return returnMsg;
        }


        #endregion

        #region DocTypes Info

        /// /Umbraco/backoffice/Api/SiteAuditorApi/ListAllDocTypes?ForExport=false
        [HttpGet]
        public StatusMessage ListAllDocTypes(bool ForExport)
        {
            var returnMsg = new StatusMessage();
            var msgSB = new StringBuilder();

            var doctypeService = this.Services.ContentTypeService;

            var doctypes = doctypeService.GetAllContentTypes();

            if (ForExport)
            {
                msgSB.AppendLine(" ");
                msgSB.AppendLine(string.Format("{0}|{1}|{2}", "Name", "Alias", "GUID"));
            }
            else
            {
                msgSB.AppendLine(string.Format("{0} [{1}] {2}", "Name", "Alias", "GUID"));
            }


            foreach (var dt in doctypes)
            {
                if (ForExport)
                {
                    msgSB.AppendLine(string.Format("{0}|{1}|{2}", dt.Name, dt.Alias, dt.Key));
                }
                else
                {
                    msgSB.AppendLine(string.Format("{0} [{1}] {2}", dt.Name, dt.Alias, dt.Key));
                }
            }

            returnMsg.Message = msgSB.ToString();
            returnMsg.Success = true;
            return returnMsg;
        }


        #endregion

        #region Shared Functions

        private static HttpResponseMessage StringBuilderToFile(StringBuilder StringData, string OutputFileName = "Export.csv", string MediaType = "text/csv")
        {
            //TODO: Need to figure out why » is returning as Â (likely an issue with unicode in general...?)

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(StringData.ToString());
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
            result.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment") { FileName = OutputFileName };
            return result;
        }
        #endregion

        #region Examples/Templates

        /// /Umbraco/backoffice/Api/SiteAuditorApi/ExampleReturnJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage ExampleReturnJson()
        {
            var returnSB = new StringBuilder();

            var testData = new StatusMessage(true, "This is a test object so you can see JSON!");
            string json = JsonConvert.SerializeObject(testData);

            returnSB.AppendLine(json);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        // /Umbraco/backoffice/Api/SiteAuditorApi/ExampleReturnHtml
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage ExampleReturnHtml()
        {
            var returnSB = new StringBuilder();

            returnSB.AppendLine("<h1>Hello! This is HTML</h1>");
            returnSB.AppendLine("<p>Use this type of return when you want to exclude &lt;XML&gt;&lt;/XML&gt; tags from your output and don\'t want it to be encoded automatically.</p>");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/ExampleReturnCsv
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage ExampleReturnCsv()
        {
            var returnSB = new StringBuilder();
            var tableData = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                tableData.AppendFormat(
                    "\"{0}\",{1},\"{2}\",{3}{4}",
                    "Name " + i,
                    i,
                    string.Format("Some text about item #{0} for demo.", i),
                    DateTime.Now,
                    Environment.NewLine);
            }
            returnSB.Append(tableData);

            return StringBuilderToFile(returnSB, "Example.csv");
        }

        #endregion
    }
}
