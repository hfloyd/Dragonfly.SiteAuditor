namespace Dragonfly.SiteAuditorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    /// <summary>
    /// Helpers for Auditing
    /// </summary>
    public static class AuditHelper
    {
        private static UmbracoHelper umbHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
        private static IContentService umbContentService = ApplicationContext.Current.Services.ContentService;
        private static IContentTypeService umbContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
        private static IDataTypeService  umbDataTypeService = ApplicationContext.Current.Services.DataTypeService;

        [Obsolete("Use 'string.Join()' instead")]
        public static string ConvertToSeparatedString(List<string> ListOfStrings, string Separator)
        {
            string myString = Separator;

            foreach (var listItem in ListOfStrings)
            {
                myString += Separator + listItem;
            }

            myString = myString.Replace(String.Concat(Separator, Separator), "");
            
            return myString;
        }

        /// <summary>
        /// Returns a list of all DocumentType Aliases in the site
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllContentTypesAliases()
        {
            List<string> docTypesList = new List<string>();

            foreach (var dt in umbContentTypeService.GetAllContentTypes())
            {
                docTypesList.Add(dt.Alias);
            }

            return docTypesList;
        }

        public static ContentType GetContentTypeByAlias(string DocTypeAlias)
        {
            return (ContentType)umbContentTypeService.GetContentType(DocTypeAlias);
        }

        public static DataTypeDefinition GetDataTypeDefinition(int DtDefintionId)
        {
            return (DataTypeDefinition)umbDataTypeService.GetDataTypeDefinitionById(DtDefintionId);
        }

       /// <summary>
        /// Takes an Umbraco content node and returns the full "path" to it using ancestor Node Names
        /// </summary>
        /// <param name="UmbContentNode">
        /// The Umbraco Content Node.
        /// </param>
        /// <returns>
        /// string
        /// </returns>
        public static IEnumerable<string> NodePath(IContent UmbContentNode)
        {
            var nodepathList = new List<string>();
            string pathIdsCsv = UmbContentNode.Path;
            string[] pathIdsArray = pathIdsCsv.Split(',');
            
            foreach (var sId in pathIdsArray)
            {
                if (sId != "-1")
                {
                    IContent getNode = umbContentService.GetById(Convert.ToInt32(sId));
                    string nodeName = getNode.Name;
                    nodepathList.Add(nodeName);
                }
            }

            return nodepathList;
        }

        /// <summary>
        /// Get the Alias of a template from its ID. If the Id is null or zero, "NONE" will be returned.
        /// </summary>
        /// <param name="TemplateId">
        /// The template id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTemplateAlias(int? TemplateId)
        {
            var umbFileService = ApplicationContext.Current.Services.FileService;
            string templateAlias;

            if (TemplateId == 0 | TemplateId == null)
            {
                templateAlias = "NONE";
            }
            else
            {
                var lookupTemplate = umbFileService.GetTemplate(Convert.ToInt32(TemplateId));
                templateAlias = lookupTemplate.Alias;
            }
            
            return templateAlias;
        }


        public static IEnumerable<string> GetDocTypesForProperty(string PropertyAlias)
        {
            var docTypesList =  new List<string>();

            var allDocTypes = umbContentTypeService.GetAllContentTypes();

            foreach (var docType in allDocTypes)
            {
                var matchingProps = docType.PropertyTypes.Where(n => n.Alias == PropertyAlias);
                if (matchingProps.Any())
                {
                    docTypesList.Add(docType.Alias);
                }
            }

            return docTypesList;
        }
    }
}
