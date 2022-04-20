namespace Dragonfly.SiteAuditor.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Dragonfly.NetModels;
    using Dragonfly.SiteAuditor.Helpers;
    using Dragonfly.SiteAuditor.Models;
    using Dragonfly.Umbraco7Helpers;
    using Newtonsoft.Json;
    using Umbraco.Core;
    using Umbraco.Web.WebApi;

    // /Umbraco/Api/SiteAuditorApi <-- UmbracoApiController
    // /Umbraco/backoffice/Api/SiteAuditorApi <-- UmbracoAuthorizedApiController [IsBackOffice]

    //[PluginController("SiteAuditor")]
    // [UmbracoUserTimeoutFilter]
    [IsBackOffice]

    public class SiteAuditorApiController : UmbracoAuthorizedApiController
    {
        /// /Umbraco/backoffice/Api/SiteAuditorApi/Help
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage Help()
        {
            var returnSB = new StringBuilder();

            returnSB.AppendLine("<h1>Site Auditor</h1>");

            returnSB.AppendLine("<h2>Content</h2>");

            returnSB.AppendLine("<h3>All Content Nodes</h3>");
            returnSB.AppendLine("<p>These will take a long time to run for large sites. Please be patient.</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsXml\">Get All Content As Xml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsJson\">Get All Content As Json</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsHtml\">Get All Content As HtmlTable</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsCsv\">Get All Content As Csv</a> [no parameters]</li>");
            returnSB.AppendLine("</ul>");

            returnSB.AppendLine("<h3>Content Nodes with Property Data</h3>");
            //returnSB.AppendLine("<p>Note</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetContentWithValues\">Get Content With Values</a></li>");
            returnSB.AppendLine("</ul>");


            returnSB.AppendLine("<h2>Document Types</h2>");

            returnSB.AppendLine("<h3>All DocTypes</h3>");
            //returnSB.AppendLine("<p>Note</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsXml\">Get All Doctypes As Xml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsJson\">Get All Doctypes As Json</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsHtml\">Get All Doctypes As Html</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsCsv\">Get All Doctypes As Csv</a> [no parameters]</li>");
            returnSB.AppendLine("</ul>");

            returnSB.AppendLine("<h2>Document Type Properties</h2>");

            returnSB.AppendLine("<h3>All Properties</h3>");
            //returnSB.AppendLine("<p>Note</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsXml\">Get All Properties As Xml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsJson\">Get All Properties As Json</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsHtml\">Get All Properties As Html</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsCsv\">Get All Properties As Csv</a> [no parameters]</li>");
            returnSB.AppendLine("</ul>");

            returnSB.AppendLine("<h2>Data Types</h2>");

            returnSB.AppendLine("<h3>All DataTypes</h3>");
            //returnSB.AppendLine("<p>Note</p>");
            returnSB.AppendLine("<ul>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsXml\">Get All DataTypes As Xml</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsJson\">Get All DataTypes As Json</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsHtml\">Get All DataTypes As Html</a> [no parameters]</li>");
            returnSB.AppendLine("<li><a target=\"_blank\" href=\"/Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsCsv\">Get All DataTypes As Csv</a> [no parameters]</li>");
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

        private SiteAuditorService GetSiteAuditorService()
        {
            return new SiteAuditorService(Umbraco, UmbracoContext, Services);
        }

        #region Content Nodes

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsXml
        [System.Web.Http.AcceptVerbs("GET")]
        public List<AuditableContent> GetAllContentAsXml()
        {
            var saService = GetSiteAuditorService();
            var allNodes = saService.GetAllAuditableContent();

            return allNodes;
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllContentAsJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllContentAsJson()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var allNodes = saService.GetAllAuditableContent();

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
            //TODO: Use Partial View
            var returnSB = new StringBuilder();
            var saService = GetSiteAuditorService();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllContentAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            var allNodes = saService.GetAllAuditableContent();

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = allNodes;
            //viewData.Add("SpecialMessage", specialMessage);

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }

            // var tableStart = @"
            //      <table>
            //          <tr>
            //               <th>Node Name</th>
            //               <th>NodeID</th>
            //               <th>Node Path</th>
            //               <th>DocType</th>
            //               <th>ParentID</th>
            //               <th>Full URL</th>
            //               <th>Level</th>
            //                <th>Sort Order</th>
            //                <th>Template Name</th>
            //                 <th>Create Date</th>
            //                 <th>Update Date</th>
            //             </tr>";

            //     var tableEnd = @"</table>";

            //     var tableData = new StringBuilder();

            //     foreach (var auditNode in allNodes)
            //    {
            //         tableData.AppendLine("<tr>");

            //        tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.Name));
            //         tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.Id));
            //          tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.NodePathAsText));
            //         tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.ContentType.Alias));
            //           tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.ParentId));
            //           tableData.AppendLine(string.Format("<td><a href=\"{0}\" target=\"_blank\">{0}</a></td>", auditNode.FullNiceUrl));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.Level));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.SortOrder));
            //             tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.TemplateAlias));
            //              tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.CreateDate));
            //             tableData.AppendLine(string.Format("<td>{0}</td>", auditNode.UmbContentNode.UpdateDate));

            //             tableData.AppendLine("</tr>");
            //        }

            //        returnSB.AppendLine(tableStart);
            //       returnSB.Append(tableData);
            //       returnSB.AppendLine(tableEnd);

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
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var allNodes = saService.GetAllAuditableContent();

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"Node Name\"" +
                ",\"NodeID\"" +
                ",\"Node Path\"" +
                ",\"DocType\"" +
                ",\"ParentID\"" +
                ",\"Full URL\"" +
                ",\"Level\"" +
                ",\"Sort Order\"" +
                ",\"Template Name\"" +
                ",\"Udi\"" +
                ",\"Create Date\"" +
                ",\"Update Date\"");

            foreach (var auditNode in allNodes)
            {
  var nodeLine = $"\"{auditNode.UmbContentNode.Name}\"" +
                               $",{auditNode.UmbContentNode.Id}" +
                               $",\"{auditNode.NodePathAsCustomText(" > ")}\"" +
                               $",\"{auditNode.UmbContentNode.ContentType.Alias}\"" +
                               $",{auditNode.UmbContentNode.ParentId}" +
                               $",\"{auditNode.FullNiceUrl}\"" +
                               $",{auditNode.UmbContentNode.Level}" +
                               $",{auditNode.UmbContentNode.SortOrder}" +
                               $",\"{auditNode.TemplateAlias}\"" +
                               $",\"{auditNode.UmbContentNode.GetUdi()}\"" +
                               $",{auditNode.UmbContentNode.CreateDate}" +
                               $",{auditNode.UmbContentNode.UpdateDate}" +
                               $"{Environment.NewLine}";
  
 tableData.Append(nodeLine);

             // tableData.AppendFormat(
        //            "\"{0}\",{1},\"{2}\",\"{3}\",{4},\"{5}\",{6},{7},\"{8}\",{9},{10}{11}",
       //             auditNode.UmbContentNode.Name,
       //             auditNode.UmbContentNode.Id,
       //             auditNode.NodePathAsCustomText(" > "),
       //             auditNode.UmbContentNode.ContentType.Alias,
        //            auditNode.UmbContentNode.ParentId,
       //             auditNode.FullNiceUrl,
      //              auditNode.UmbContentNode.Level,
      //              auditNode.UmbContentNode.SortOrder,
       //             auditNode.TemplateAlias,
       //             auditNode.UmbContentNode.CreateDate,
       //             auditNode.UmbContentNode.UpdateDate,
       //             Environment.NewLine);
       


     }
            returnSB.Append(tableData);

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "AllNodes.csv");
        }

             /// /Umbraco/backoffice/Api/SiteAuditorApi/GetContentForDoctypeHtml?DocTypeAlias=X
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetContentForDoctypeHtml(string DocTypeAlias)
        {
            var returnSB = new StringBuilder();
            var saService = GetSiteAuditorService();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllContentAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            var allNodes = saService.GetAuditableContent(DocTypeAlias);

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = allNodes;
            //viewData.Add("SpecialMessage", specialMessage);

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetContentForDoctypeHtml
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetContentForDoctypeHtml()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            //BUILD HTML
            var allSiteDocTypes = saService.GetAllDocTypes();
            var allAliases = allSiteDocTypes.Select(n => n.Alias).OrderBy(n => n).ToList();

            //BUILD HTML
            returnSB.AppendLine($"<h1>Get Content for a Selected Document Type</h1>");
            returnSB.AppendLine($"<h3>Available Document Types</h3>");
            //returnSB.AppendLine("<p>Note: Choosing the 'All' option will take significantly longer to load than the 'Published' option because we need to bypass the cache and query the database directly.</p>");

            returnSB.AppendLine("<ul>");

            foreach (var alias in allAliases)
            {
                var url = $"/Umbraco/backoffice/Api/SiteAuditorApi/GetContentForDoctypeHtml?DocTypeAlias={alias}";

                returnSB.AppendLine($"<li>{alias} <a target=\"_blank\" href=\"{url}\">View</a></li>");
            }

            returnSB.AppendLine("</ul>");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        #endregion

        #region GetContentWithValues
        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetContentWithValues?PropertyAlias=xxx&IncludeUnpublished=false
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetContentWithValues(string PropertyAlias, bool IncludeUnpublished = false)
        {
            var returnSB = new StringBuilder();
            var fancyFormat = true;

            var pvPath = "~/App_Plugins/Dragonfly.SiteAuditor/Views/ContentWithValuesTable.cshtml";

            //FIND NODES TO DISPLAY
            var allNodes = CollectionsHelper.GetAllNodes(IncludeUnpublished).ToList();
            var nodesWithValue = allNodes.Where(n => n.HasPropertyWithValue(PropertyAlias)).ToList();

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = nodesWithValue;
            viewData.Add("PropertyAlias", PropertyAlias);
            viewData.Add("IncludeUnpublished", IncludeUnpublished);

            //RENDER
            var controllerContext = this.ControllerContext;
            var displayHtml = ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
            returnSB.AppendLine(displayHtml);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetContentWithValues
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetContentWithValues()
        {
            var returnSB = new StringBuilder();

            //BUILD HTML
            var allSiteDocTypes = CollectionsHelper.GetAllDocTypes();

            var allProps = allSiteDocTypes.SelectMany(n => n.PropertyTypes);

            var allPropsAliases = allProps.Select(n => n.Alias).Distinct();

            //BUILD HTML
            returnSB.AppendLine($"<h1>Get Content with Values</h1>");
            returnSB.AppendLine($"<h3>Available Properties</h3>");
            returnSB.AppendLine("<p>Note: Choosing the 'All' option will take significantly longer to load than the 'Published' option because we need to bypass the cache and query the database directly.</p>");

            returnSB.AppendLine("<ul>");

            foreach (var propAlias in allPropsAliases.OrderBy(n => n))
            {
                var url1 =
                    $"/Umbraco/backoffice/Api/SiteAuditorApi/GetContentWithValues?PropertyAlias={propAlias}&IncludeUnpublished=false";
                var url2 =
                    $"/Umbraco/backoffice/Api/SiteAuditorApi/GetContentWithValues?PropertyAlias={propAlias}&IncludeUnpublished=true";

                returnSB.AppendLine($"<li>{propAlias} <a target=\"_blank\" href=\"{url1}\">Published</a> | <a target=\"_blank\" href=\"{url2}\">All</a></li>");
            }

            returnSB.AppendLine("</ul>");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
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

        // /Umbraco/backoffice/Api/SiteAuditorApi/GetAllPropertiesAsHtmlTable
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllPropertiesAsHtmlTable()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllPropertiesAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            var siteProps = new SiteAuditableProperties();
            var propertiesList = siteProps.AllProperties;

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = propertiesList;
            //viewData.Add("SpecialMessage", specialMessage);

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }



            //  var tableStart = @"
            //              <table>
            //                 <tr>
            //                     <th>Property Name</th>
            //                     <th>Property Alias</th>
            //                      <th>DataType Name</th>
            //                     <th>DataType Property Editor</th>
            //                      <th>DataType Database Type</th>
            //                      <th>DocumentTypes Used In</th>
            //                     <th>Qty of DocumentTypes</th>
            //                  </tr>";

            //          var tableEnd = @"</table>";

            //          var tableData = new StringBuilder();

            //       foreach (var prop in propertiesList)
            //       {
            //          tableData.AppendLine("<tr>");
            //
            //            tableData.AppendLine(string.Format("<td>{0}</td>", prop.UmbPropertyType.Name));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", prop.UmbPropertyType.Alias));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", prop.DataType.Name));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", prop.DataType.PropertyEditorAlias));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", prop.DataType.DatabaseType));
            //            tableData.AppendLine(string.Format("<td>{0}</td>", string.Join(", ", prop.AllDocTypes.Select(n => n.DocTypeAlias))));
            //           tableData.AppendLine(string.Format("<td>{0}</td>", prop.AllDocTypes.Count()));
            //
            //            tableData.AppendLine("</tr>");
            //        }

            //      returnSB.AppendLine(tableStart);
            //       returnSB.Append(tableData);
            //       returnSB.AppendLine(tableEnd);

            //RETURN AS HTML
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
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var siteProps = new SiteAuditableProperties();
            var propertiesList = siteProps.AllProperties;

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"Property Name\",\"Property Alias\",\"DataType Name\",\"DataType Property Editor\",\"DataType Database Type\",\"DocumentTypes Used In\",\"Qty of DocumentTypes\"");

            foreach (var prop in propertiesList)
            {
                tableData.AppendFormat(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",{6}{7}",
                    prop.UmbPropertyType.Name,
                    prop.UmbPropertyType.Alias,
                    prop.DataType.Name,
                    prop.DataType.PropertyEditorAlias,
                    prop.DataType.DatabaseType,
                    string.Join(", ", prop.AllDocTypes.Select(n => n.DocTypeAlias)),
                    prop.AllDocTypes.Count(),
                    Environment.NewLine);
            }

            returnSB.Append(tableData);

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "AllProperties.csv");
        }

        // /Umbraco/backoffice/Api/SiteAuditorApi/GetPropertiesForDoctypeHtml?DocTypeAlias=xxx
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetPropertiesForDoctypeHtml(string DocTypeAlias)
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllPropertiesAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            var siteProps = saService.AllPropertiesForDocType(DocTypeAlias);
            var propertiesList = siteProps.AllProperties;

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = propertiesList;
            viewData.Add("Title", $"Properties for Document Type '{DocTypeAlias}'");

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };

        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetPropertiesForDoctypeHtml
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetPropertiesForDoctypeHtml()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            //BUILD HTML
            var allSiteDocTypes = saService.GetAllDocTypes();
            var allAliases = allSiteDocTypes.Select(n => n.Alias).OrderBy(n => n).ToList();

            //BUILD HTML
            returnSB.AppendLine($"<h1>Get Properties for a Selected Document Type</h1>");
            returnSB.AppendLine($"<h3>Available Document Types</h3>");
            //returnSB.AppendLine("<p>Note: Choosing the 'All' option will take significantly longer to load than the 'Published' option because we need to bypass the cache and query the database directly.</p>");

            returnSB.AppendLine("<ul>");

            foreach (var alias in allAliases)
            {
                var url = $"/Umbraco/backoffice/Api/SiteAuditorApi/GetPropertiesForDoctypeHtml?DocTypeAlias={alias}";

                returnSB.AppendLine($"<li>{alias} <a target=\"_blank\" href=\"{url}\">View</a></li>");
            }

            returnSB.AppendLine("</ul>");

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
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

        // /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsXml
        [System.Web.Http.AcceptVerbs("GET")]
        public IEnumerable<AuditableDataType> GetAllDataTypesAsXml()
        {
            var saService = GetSiteAuditorService();
            var dataTypes = saService.GetAllAuditableDataTypes();

            return dataTypes;
        }


        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllDataTypesAsJson()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var dataTypes = saService.GetAllAuditableDataTypes();
            string json = JsonConvert.SerializeObject(dataTypes);

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

        // /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsHtmlTable
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllDataTypesAsHtmlTable()
        {
            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllDataTypesAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            //var returnStatusMsg = new StatusMessage(true); //assume success
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var dataTypes = saService.GetAllAuditableDataTypes();

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = dataTypes;
            //viewData.Add("SpecialMessage", specialMessage);

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDataTypesAsCsv
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllDataTypesAsCsv()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var dataTypes = saService.GetAllAuditableDataTypes();

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"DataType Name\",\"Property Editor Alias\",\"Id\",\"Guid Key\",\"Used On Properties\",\"Qty of Properties\"");

            foreach (var dt in dataTypes)
            {
                tableData.AppendFormat(
                    "\"{0}\",\"{1}\",{2},\"{3}\",\"{4}\",{5}{6}",
                    dt.Name,
                    dt.EditorAlias,
                    dt.Id,
                    dt.Guid.ToString(),
                    string.Join(" | ", dt.UsedOnProperties.Select(n => $"{n.Value} [{n.Key.Alias}]")),
                    dt.UsedOnProperties.Count(),
                    Environment.NewLine);
            }

            returnSB.Append(tableData);

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "AllDataTypes.csv");
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/ListAllDataTypes?ForExport=false
        [System.Web.Http.AcceptVerbs("GET")]
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

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsXml
        [System.Web.Http.AcceptVerbs("GET")]
        public IEnumerable<AuditableDocType> GetAllDocTypesAsXml()
        {
            var saService = GetSiteAuditorService();
            var allDts = saService.GetAuditableDocTypes();

            return allDts;
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsJson
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllDocTypesAsJson()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var allDts = saService.GetAuditableDocTypes();

            string json = JsonConvert.SerializeObject(allDts);

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

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsHtml
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllDocTypesAsHtmlTable()
        {
            var returnSB = new StringBuilder();
            var saService = GetSiteAuditorService();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllDocTypesAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";
            var allDts = saService.GetAuditableDocTypes();

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = allDts;
            //viewData.Add("SpecialMessage", specialMessage);

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }



            //    var tableStart = @"
            //         <table>
            //             <tr>
            //                 <th>Doctype Name</th>
            //                 <th>Alias</th>
            //                  <th>Default Template</th>
            //                  <th>GUID</th>
            //                   <th>Create Date</th>
            //                  <th>Update Date</th>
            //               </tr>";

            //      var tableEnd = @"</table>";

            //      var tableData = new StringBuilder();

            //      foreach (var item in allDts)
            //       {
            //          tableData.AppendLine("<tr>");

            //           tableData.AppendLine(string.Format("<td>{0}</td>", item.Name));
            //           tableData.AppendLine(string.Format("<td>{0}</td>", item.Alias));
            //           tableData.AppendLine(string.Format("<td>{0}</td>", item.DefaultTemplateName));
            //           tableData.AppendLine(string.Format("<td>{0}</td>", item.GUID));
            //          tableData.AppendLine(string.Format("<td>{0}</td>", item.ContentType.CreateDate));
            //           tableData.AppendLine(string.Format("<td>{0}</td>", item.ContentType.UpdateDate));

            //           tableData.AppendLine("</tr>");
            //       }

            //      returnSB.AppendLine(tableStart);
            //       returnSB.Append(tableData);
            //       returnSB.AppendLine(tableEnd);

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllDocTypesAsCsv
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllDocTypesAsCsv()
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var allDts = saService.GetAuditableDocTypes();

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"Doctype Name\",\"Alias\",\"Default Template\",\"GUID\",\"Create Date\",\"Update Date\"");

            foreach (var item in allDts)
            {
                tableData.AppendFormat(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",{4},{5}{6}",
                    item.Name,
                    item.Alias,
                    item.DefaultTemplateName,
                    item.Guid,
                    item.ContentType.CreateDate,
                    item.ContentType.UpdateDate,
                    Environment.NewLine);
            }
            returnSB.Append(tableData);

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "AllDoctypes.csv");
        }

        #endregion

        #region Templates Info

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllTemplatesAsXml?IncludeContentNodesCount=false
        [System.Web.Http.AcceptVerbs("GET")]
        public IEnumerable<AuditableTemplate> GetAllTemplatesAsXml(bool IncludeContentNodesCount = false)
        {
            var saService = GetSiteAuditorService();
            var allTemps = saService.GetAuditableTemplates(IncludeContentNodesCount);

            return allTemps;
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllTemplatesAsJson?IncludeContentNodesCount=false
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllTemplatesAsJson(bool IncludeContentNodesCount = false)
        {
            var saService = GetSiteAuditorService();

            var allTemps = saService.GetAuditableTemplates(IncludeContentNodesCount);

            string json = JsonConvert.SerializeObject(allTemps);

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllTemplatesAsHtmlTable?IncludeContentNodesCount=false
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllTemplatesAsHtmlTable(bool IncludeContentNodesCount = false)
        {
            var returnSB = new StringBuilder();
            var saService = GetSiteAuditorService();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/AllTemplatesAsHtmlTable.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            var allTemps = saService.GetAuditableTemplates(IncludeContentNodesCount);

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = allTemps;
            viewData.Add("IncludeContentNodesCount", IncludeContentNodesCount);

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }

        /// /Umbraco/backoffice/Api/SiteAuditorApi/GetAllTemplatesAsCsv?IncludeContentNodesCount=false
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage GetAllTemplatesAsCsv(bool IncludeContentNodesCount = false)
        {
            var saService = GetSiteAuditorService();
            var returnSB = new StringBuilder();

            var allTemplates = saService.GetAuditableTemplates(IncludeContentNodesCount);

            var tableData = new StringBuilder();

            tableData.AppendLine(
                "\"Doctype Name\",\"Alias\",\"Is a Default Template For Doctypes\",\"GUID\",\"Create Date\",\"Update Date\"");

            foreach (var item in allTemplates)
            {
                tableData.AppendFormat(
                    "\"{0}\",\"{1}\",\"{2}\",\"{3}\",{4},{5}{6}",
                    item.Name,
                    item.Alias,
                    item.DefaultTemplateFor.Any(),
                    item.Guid,
                    item.CreateDate,
                    item.UpdateDate,
                    Environment.NewLine);
            }
            returnSB.Append(tableData);

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "AllDoctypes.csv");
        }

        #endregion

        #region Special Queries
        /// /Umbraco/backoffice/Api/SiteAuditorApi/TemplateUsageReport
        [System.Web.Http.AcceptVerbs("GET")]
        public HttpResponseMessage TemplateUsageReport()
        {
            var returnSB = new StringBuilder();
            var saService = GetSiteAuditorService();

            var pvPath = "/App_Plugins/Dragonfly.SiteAuditor/Views/TemplateUsageReport.cshtml"; // _TesterConfig.GetAppPluginsPath() + "Views/Start.cshtml";

            var sm = new StatusMessage();

            //VIEW DATA 
            var viewData = new ViewDataDictionary();
            viewData.Model = sm;
            viewData.Add("TemplatesUsedOnContent", saService.TemplatesUsedOnContent());
            viewData.Add("TemplatesNotUsedOnContent", saService.TemplatesNotUsedOnContent());

            //RENDER
            try
            {
                var controllerContext = this.ControllerContext;
                var displayHtml =
                    ApiControllerHtmlHelper.GetPartialViewHtml(controllerContext, pvPath, viewData, HttpContext.Current);
                returnSB.AppendLine(displayHtml);
            }
            catch (System.ArgumentNullException eNull)
            {
                if (eNull.Message.Contains("Parameter name: view"))
                {
                    throw new ArgumentNullException($"The View file '{pvPath}' is missing and cannot be rendered.", eNull);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            { throw; }

            //RETURN AS HTML
            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    returnSB.ToString(),
                    Encoding.UTF8,
                    "text/html"
                )
            };
        }
        #endregion

        #region Tests & Examples

        /// /Umbraco/backoffice/Api/AuthorizedApi/Test
        [System.Web.Http.AcceptVerbs("GET")]
        public bool Test()
        {
            //LogHelper.Info<PublicApiController>("Test STARTED/ENDED");
            return true;
        }

        /// /Umbraco/backoffice/Api/AuthorizedApi/ExampleReturnHtml
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

        /// /Umbraco/backoffice/Api/AuthorizedApi/ExampleReturnJson
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

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "Example.csv");
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

            return Dragonfly.NetHelpers.Http.StringBuilderToFile(returnSB, "Test.csv");
        }

        #endregion
    }
}
