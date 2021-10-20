namespace Dragonfly.SiteAuditor.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dragonfly.SiteAuditor.Models;
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

        /// <summary>
        /// Get a ContentType model for a Doctype by its alias
        /// </summary>
        /// <param name="DocTypeAlias"></param>
        /// <returns></returns>
        public static ContentType GetContentTypeByAlias(string DocTypeAlias)
        {
            return (ContentType)umbContentTypeService.GetContentType(DocTypeAlias);
        }

        /// <summary>
        /// Get a DataTypeDefinition for a Datatype by its DataTypeDefinitionId
        /// </summary>
        /// <param name="DtDefinitionId"></param>
        /// <returns></returns>
        public static DataTypeDefinition GetDataTypeDefinition(int DtDefinitionId)
        {
            return (DataTypeDefinition)umbDataTypeService.GetDataTypeDefinitionById(DtDefinitionId);
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
        
        /// <summary>
        /// Get a list of all DocTypes which contain a property of a specified Alias
        /// </summary>
        /// <param name="PropertyAlias"></param>
        /// <returns></returns>
        public static List<PropertyDoctypeInfo> GetDocTypesForProperty(string PropertyAlias)
        {
            var docTypesList =  new List<PropertyDoctypeInfo>();

            var allDocTypes = umbContentTypeService.GetAllContentTypes();

            foreach (var docType in allDocTypes)
            {
                var matchingProps = docType.PropertyTypes.Where(n => n.Alias == PropertyAlias);
                if (matchingProps.Any())
                {
                    foreach (var prop in matchingProps)
                    {
                        var x = new PropertyDoctypeInfo();
                        x.DocTypeAlias = docType.Alias;
                        
                       // x.GroupName = docType.PropertyGroups.Contains(prop)
                            docTypesList.Add(x);
                    }
                }
            }

            return docTypesList;
        }

        /// <summary>
        /// Get a NodePropertyDataTypeInfo model for a specified Node and Property Alias
        /// (Includes information about the Property, Datatype, and the node's property Value)
        /// </summary>
        /// <param name="PropertyAlias"></param>
        /// <param name="Node"></param>
        /// <returns></returns>
        public static NodePropertyDataTypeInfo GetPropertyDataTypeInfo(string PropertyAlias, IPublishedContent Node)
        {
            var umbContentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            var umbDataTypeService = ApplicationContext.Current.Services.DataTypeService;

            var dtInfo = new NodePropertyDataTypeInfo();
            dtInfo.NodeId = Node.Id;
            dtInfo.Property = Node.GetProperty(PropertyAlias);

            //Find datatype of property
            IDataTypeDefinition dataType = null;

            var docType = umbContentTypeService.GetContentType(Node.DocumentTypeId);
            var matchingProperties = docType.PropertyTypes.Where(n => n.Alias == PropertyAlias).ToList();

            if (matchingProperties.Any())
            {
                var propertyType = matchingProperties.First();
                dataType = umbDataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId);

                dtInfo.DataType = dataType;
                dtInfo.PropertyEditorAlias = dataType.PropertyEditorAlias;
                dtInfo.DatabaseType = dataType.DatabaseType.ToString();
                dtInfo.DocTypeAlias = Node.DocumentTypeAlias;
            }
            else
            {
                //Look at Compositions for prop data
                var matchingCompProperties = docType.CompositionPropertyTypes.Where(n => n.Alias == PropertyAlias).ToList();
                if (matchingCompProperties.Any())
                {
                    var propertyType = matchingCompProperties.First();
                    dataType = umbDataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId);

                    dtInfo.DataType = dataType;
                    dtInfo.PropertyEditorAlias = dataType.PropertyEditorAlias;
                    dtInfo.DatabaseType = dataType.DatabaseType.ToString();

                    if (docType.ContentTypeComposition.Any())
                    {
                        var compsList = docType.ContentTypeComposition.Where(n => n.PropertyTypeExists(PropertyAlias)).ToList();
                        if (compsList.Any())
                        {
                            dtInfo.DocTypeAlias = Node.DocumentTypeAlias;
                            dtInfo.DocTypeCompositionAlias = compsList.First().Alias;
                        }
                        else
                        {
                            dtInfo.DocTypeAlias = Node.DocumentTypeAlias;
                            dtInfo.DocTypeCompositionAlias = "Unknown Composition";
                        }
                    }
                }
                else
                {
                    dtInfo.ErrorMessage = $"No property found for alias '{PropertyAlias}' in DocType '{docType.Name}'";
                }
            }

            return dtInfo;
        }
    }
}
