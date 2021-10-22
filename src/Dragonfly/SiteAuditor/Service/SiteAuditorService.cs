namespace Dragonfly.SiteAuditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Mvc;
    using System.Web.WebPages;
    using Dragonfly.SiteAuditor.CustomHandlers;
    using Dragonfly.SiteAuditor.Helpers;
    using Dragonfly.SiteAuditor.Models;
    using Newtonsoft.Json;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Core.Services;
    using umbraco.interfaces;
    using Umbraco.Web;
    using Umbraco.Web.PropertyEditors;
    using Umbraco.Web.Security;

    public class SiteAuditorService
    {
        private UmbracoHelper _umbracoHelper;
        // private readonly ILogger _logger;
        //private readonly AppCaches _appCaches;
        private readonly UmbracoContext _umbracoContext;
        private readonly ServiceContext _services;

        /// <summary>
        /// Default string used for NodePathAsText
        /// ' » ' unless explicitly changed
        /// </summary>
        public string DefaultDelimiter
        {
            get { return _defaultDelimiter; }
            internal set { _defaultDelimiter = value; }
        }
        private string _defaultDelimiter = " » ";

        private IEnumerable<AuditableContent> _AllAuditableContent = new List<AuditableContent>();
        private Dictionary<int, IEnumerable<AuditableContent>> _RangeAuditableContent = new Dictionary<int, IEnumerable<AuditableContent>>();
        private IEnumerable<IContentTypeComposition> _AllContentTypeComps = new List<IContentType>();
        private IEnumerable<IPublishedContent> _AllPublishedNodes = new List<IPublishedContent>();
        private IEnumerable<IContent> _AllContent = new List<IContent>();
        private IEnumerable<AuditableDataType> _AllAuditableDataTypes = new List<AuditableDataType>();
        private Dictionary<PropertyType, string> _AllPropsWithDocTypes = new Dictionary<PropertyType, string>();
        private Dictionary<string, IEnumerable<AuditableTemplate>> _AllAuditableTemplates = new Dictionary<string, IEnumerable<AuditableTemplate>>();

        #region ctor
        public SiteAuditorService()
        {
            //Services
            this._umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            this._services = ApplicationContext.Current.Services;
            //this._logger = _umbracoHelper;
            //this._appCaches = Current.AppCaches;
            this._umbracoContext = UmbracoContext.Current;
        }

        public SiteAuditorService(UmbracoHelper UmbHelper, UmbracoContext UmbContext, ServiceContext Services)
        {
            //Services
            this._umbracoHelper = UmbHelper;
            this._services = Services;
            //   this._logger = Logger;
            //this._appCaches = Current.AppCaches;
            this._umbracoContext = UmbContext;
        }

        #endregion

        #region All Nodes (IPublishedContent)

        /// <summary>
        /// Gets all published site nodes as IPublishedContent
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetPublishedNodes()
        {
            if (!_AllPublishedNodes.Any())
            {
                var nodesList = new List<IPublishedContent>();

                var topLevelNodes = _umbracoHelper.TypedContentAtRoot().OrderBy(n => n.SortOrder);

                foreach (var thisNode in topLevelNodes)
                {
                    nodesList.AddRange(LoopNodes(thisNode));
                }

                _AllPublishedNodes = nodesList;
            }

            return _AllPublishedNodes.ToList();
        }

        private IEnumerable<IPublishedContent> LoopNodes(IPublishedContent ThisNode)
        {
            var nodesList = new List<IPublishedContent>();

            //Add current node, then loop for children
            try
            {
                nodesList.Add(ThisNode);

                if (ThisNode.Children().Any())
                {
                    foreach (var childNode in ThisNode.Children().OrderBy(n => n.SortOrder))
                    {
                        nodesList.AddRange(LoopNodes(childNode));
                    }
                }
            }
            catch (Exception e)
            {
                //skip
            }

            return nodesList;
        }


        #endregion

        #region All Nodes (IContent)

        public List<IContent> GetAllContent()
        {
            if (!_AllContent.Any())
            {
                var nodesList = new List<IContent>();

                var topLevelContentNodes = _services.ContentService.GetRootContent().OrderBy(n => n.SortOrder);

                foreach (var thisNode in topLevelContentNodes)
                {
                    nodesList.AddRange(LoopForContentNodes(thisNode));
                }

                _AllContent = nodesList;
            }

            return _AllContent.ToList();
        }

        internal List<IContent> LoopForContentNodes(IContent ThisNode)
        {
            var nodesList = new List<IContent>();

            //Add current node, then loop for children
            nodesList.Add(ThisNode);

            //figure out num of children
            long countChildren;
            var test = _services.ContentService.GetPagedChildren(ThisNode.Id, 0, 1, out countChildren);
            if (countChildren > 0)
            {
                long countTest;
                var allChildren = _services.ContentService.GetPagedChildren(ThisNode.Id, 0, Convert.ToInt32(countChildren), out countTest);
                foreach (var childNode in allChildren.OrderBy(n => n.SortOrder))
                {
                    nodesList.AddRange(LoopForContentNodes(childNode));
                }
            }

            return nodesList;
        }
        #endregion

        #region AuditableContent

        /// <summary>
        /// Gets all site nodes as AuditableContent models
        /// </summary>
        /// <returns></returns>
        public List<AuditableContent> GetAllAuditableContent()
        {
            if (!_AllAuditableContent.Any())
            {
                var nodesList = new List<AuditableContent>();

                var topLevelContentNodes = _services.ContentService.GetRootContent().OrderBy(n => n.SortOrder);

                foreach (var thisNode in topLevelContentNodes)
                {
                    nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
                }

                _AllAuditableContent = nodesList;
            }

            return _AllAuditableContent.ToList();
        }

        /// <summary>
        /// Gets all descendant nodes from provided Root Node Id as AuditableContent models
        /// </summary>
        /// <param name="RootNodeId">Integer Id of Root Node</param>
        /// <returns></returns>
        public List<AuditableContent> GetAuditableContent(int RootNodeId)
        {
            if (_RangeAuditableContent.Any())
            {
                if (_RangeAuditableContent.ContainsKey(RootNodeId))
                {
                    return _RangeAuditableContent[RootNodeId].ToList();
                }
            }

            //else calculate new range and add to global list
            var nodesList = new List<AuditableContent>();

            var topLevelNodes = _services.ContentService.GetByIds(RootNodeId.AsEnumerableOfOne()).OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
            }

            _RangeAuditableContent.Add(RootNodeId, nodesList);

            return nodesList;
        }

        /// <summary>
        /// Gets all descendant nodes from provided Root Node Id as AuditableContent models
        /// </summary>
        /// <param name="RootNodeUdi">Udi of Root Node</param>
        /// <returns></returns>
        //public List<AuditableContent> GetContentNodes(Udi RootNodeUdi)
        //{
        //    var nodesList = new List<AuditableContent>();

        //    //var TopLevelNodes = umbHelper.ContentAtRoot();
        //    var node = _services.ContentService.GetById(RootNodeUdi)
        //    var topLevelNodes = _services.ContentService.GetByIds(RootNodeUdi.AsEnumerableOfOne()).OrderBy(n => n.SortOrder);

        //    foreach (var thisNode in topLevelNodes)
        //    {
        //        nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
        //    }

        //    return nodesList;
        //}

        public List<AuditableContent> GetAuditableContent(string DocTypeAlias)
        {
            var allContent = GetAllAuditableContent();

            var filteredContent = allContent.Where(n => n.DocTypeAlias == DocTypeAlias);

            return filteredContent.ToList();
        }

        public List<AuditableContent> GetContentWithProperty(string PropertyAlias)
        {
            var allContent = GetAllContent();
            var contentWithProp = allContent.Where(n => n.HasProperty(PropertyAlias)).ToList();

            return ConvertIContentToAuditableContent(contentWithProp);
        }

        internal List<AuditableContent> LoopForAuditableContentNodes(IContent ThisNode)
        {
            var nodesList = new List<AuditableContent>();

            //Add current node, then loop for children
            AuditableContent auditContent = ConvertIContentToAuditableContent(ThisNode);
            nodesList.Add(auditContent);

            //figure out num of children
            long countChildren;
            var test = _services.ContentService.GetPagedChildren(ThisNode.Id, 0, 1, out countChildren);
            if (countChildren > 0)
            {
                long countTest;
                var allChildren = _services.ContentService.GetPagedChildren(ThisNode.Id, 0, Convert.ToInt32(countChildren), out countTest);
                foreach (var childNode in allChildren.OrderBy(n => n.SortOrder))
                {
                    nodesList.AddRange(LoopForAuditableContentNodes(childNode));
                }
            }

            return nodesList;
        }

        private AuditableContent ConvertIContentToAuditableContent(IContent ThisIContent)
        {
            var ac = new AuditableContent();
            ac.UmbContentNode = ThisIContent;
            ac.IsPublished = ac.UmbContentNode.Published;
            ac.DocTypeAlias = ThisIContent.ContentType.Alias;

            if (ThisIContent.Template != null)
            {
                var template = _services.FileService.GetTemplate((int)ThisIContent.Template.Id);
                ac.TemplateAlias = template.Alias;
            }
            else
            {
                ac.TemplateAlias = "NONE";
            }

            if (ac.UmbContentNode.Published)
            {
                try
                {
                    var iPub = _umbracoHelper.Content(ThisIContent.Id);
                    ac.UmbPublishedNode = iPub;
                }
                catch (Exception e)
                {
                    //Get preview - unpublished
                    var iPub = _umbracoContext.ContentCache.GetById(true, ThisIContent.Id);
                    ac.UmbPublishedNode = iPub;
                }
            }

            ac.RelativeNiceUrl = ac.UmbPublishedNode != null ? ac.UmbPublishedNode.Url : "UNPUBLISHED";
            ac.FullNiceUrl = ac.UmbPublishedNode != null ? ac.UmbPublishedNode.UrlAbsolute() : "UNPUBLISHED";
            ac.NodePath = AuditHelper.NodePath(ThisIContent);
            ac.NodePathAsText = string.Join(this.DefaultDelimiter, ac.NodePath);

            return ac;
        }

        private List<AuditableContent> ConvertIContentToAuditableContent(List<IContent> ContentList)
        {
            var nodesList = new List<AuditableContent>();
            foreach (var content in ContentList)
            {
                nodesList.Add(ConvertIContentToAuditableContent(content));
            }
            return nodesList;
        }

        #endregion

        #region DocTypes

        /// <summary>
        /// Gets list of all DocTypes on site as IContentType models
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IContentType> GetAllDocTypes()
        {
            var list = new List<IContentType>();

            var doctypes = _services.ContentTypeService.GetAllContentTypes();

            foreach (var type in doctypes)
            {
                if (type != null)
                {
                    list.Add(type);
                }
            }

            return list;
        }

        /// <summary>
        /// Get all Doctypes which are used as Compositions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IContentTypeComposition> GetAllCompositions()
        {
            if (!_AllContentTypeComps.Any())
            {
                var doctypes = _services.ContentTypeService.GetAllContentTypes();
                var comps = doctypes.SelectMany(n => n.ContentTypeComposition);

                _AllContentTypeComps = comps.DistinctBy(n => n.Id);

                return _AllContentTypeComps;
            }

            return _AllContentTypeComps;
        }


        /// <summary>
        /// Gets list of all DocTypes on site as AuditableDoctype models
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuditableDocType> GetAuditableDocTypes()
        {
            var list = new List<AuditableDocType>();

            var doctypes = _services.ContentTypeService.GetAllContentTypes();

            foreach (var type in doctypes)
            {
                if (type != null)
                {
                    list.Add(ConvertIContentTypeToAuditableDocType(type));
                }
            }

            return list;
        }

        private AuditableDocType ConvertIContentTypeToAuditableDocType(IContentType ContentType)
        {
            var adt = new AuditableDocType();
            adt.ContentType = ContentType;
            adt.Name = ContentType.Name;
            adt.Alias = ContentType.Alias;
            adt.Guid = ContentType.Key;
            adt.Id = ContentType.Id;

            if (ContentType.DefaultTemplate != null)
            {
                adt.DefaultTemplateName = ContentType.DefaultTemplate.Name;
            }
            else
            {
                adt.DefaultTemplateName = "NONE";
            }

            var templates = new Dictionary<int, string>();
            foreach (var template in ContentType.AllowedTemplates)
            {
                templates.Add(template.Id, template.Alias);
            }
            adt.AllowedTemplates = templates;

            var hasComps = new Dictionary<int, string>();
            foreach (var comp in ContentType.ContentTypeComposition)
            {
                hasComps.Add(comp.Id, comp.Alias);
            }
            adt.CompositionsUsed = hasComps;

            var allCompsIds = GetAllCompositions().Select(n => n.Id);

            adt.IsComposition = allCompsIds.Contains(ContentType.Id);
            adt.FolderPath = GetFolderContainerPath(ContentType);

            var allContent = GetAllContent();
            var matchingContent = allContent.Where(n => n.ContentType.Id == ContentType.Id);
            adt.HasContentNodes = matchingContent.Any();
            adt.QtyContentNodes = matchingContent.Count();

            var allDataTypes = GetAllAuditableDataTypes();
            var elementTypes = allDataTypes.Where(n => n.UsesDocTypes.Contains(ContentType.Alias));
            adt.UsedInDataTypes = elementTypes;
            adt.IsElement = elementTypes.Any();

            return adt;
        }

        private List<string> GetFolderContainerPath(IContentType ContentType)
        {
            var folders = new List<string>();
            var ids = ContentType.Path.Split(',');

            try
            {
                //The final one is the DataType, so exclude it
                foreach (var sId in ids.Take(ids.Length - 1))
                {
                    if (sId != "-1")
                    {
                        var container = _services.ContentTypeService.GetContentTypeContainer(Convert.ToInt32(sId));
                        folders.Add(container.Name);
                    }
                }
            }
            catch (Exception e)
            {
                folders.Add("~ERROR~");
                var msg = $"Error in 'GetFolderContainerPath()' for ContentType {ContentType.Id} - '{ContentType.Name}'";
                LogHelper.Error(typeof(SiteAuditorService), msg, e);
            }

            return folders;
        }

        #endregion

        #region AuditableProperties

        public SiteAuditableProperties AllProperties()
        {
            var allProps = new SiteAuditableProperties();
            allProps.PropsForDoctype = "[All]";
            List<AuditableProperty> propertiesList = new List<AuditableProperty>();

            var allDocTypes = _services.ContentTypeService.GetAllContentTypes();

            foreach (var docType in allDocTypes)
            {
                //var ct = _services.ContentTypeService.Get(docTypeAlias);

                foreach (var prop in docType.PropertyTypes)
                {
                    //test for the same property already in list
                    if (propertiesList.Exists(i => i.UmbPropertyType.Alias == prop.Alias & i.UmbPropertyType.Name == prop.Name & i.UmbPropertyType.DataTypeId == prop.DataTypeId))
                    {
                        //Add current DocType to existing property
                        var info = new PropertyDoctypeInfo();
                        info.Id = docType.Id;
                        info.DocTypeAlias = docType.Alias;
                        info.GroupName = "";
                        propertiesList.Find(i => i.UmbPropertyType.Alias == prop.Alias).AllDocTypes.Add(info);
                    }
                    else
                    {
                        //Add new property
                        AuditableProperty auditProp = PropertyTypeToAuditableProperty(prop);

                        var info = new PropertyDoctypeInfo();
                        info.DocTypeAlias = docType.Alias;
                        info.GroupName = "";

                        auditProp.AllDocTypes.Add(info);
                        propertiesList.Add(auditProp);
                    }
                }
            }

            allProps.AllProperties = propertiesList;
            return allProps;
        }

        /// <summary>
        /// Get a ContentType model for a Doctype by its alias
        /// </summary>
        /// <param name="DocTypeAlias"></param>
        /// <returns></returns>
        public IContentType GetContentTypeByAlias(string DocTypeAlias)
        {
            return _services.ContentTypeService.GetContentType(DocTypeAlias);
        }

        public SiteAuditableProperties AllPropertiesForDocType(string DocTypeAlias)
        {
            var allProps = new SiteAuditableProperties();
            allProps.PropsForDoctype = DocTypeAlias;
            List<AuditableProperty> propertiesList = new List<AuditableProperty>();

            var ct = _services.ContentTypeService.GetContentType(DocTypeAlias);
            var propsDone = new List<int>();

            //First, compositions
            foreach (var comp in ct.ContentTypeComposition)
            {
                foreach (var group in comp.CompositionPropertyGroups)
                {
                    foreach (var prop in group.PropertyTypes)
                    {
                        AuditableProperty auditProp = PropertyTypeToAuditableProperty(prop);

                        auditProp.InComposition = comp.Name;
                        auditProp.GroupName = group.Name;

                        propertiesList.Add(auditProp);
                        propsDone.Add(prop.Id);
                    }
                }
            }

            //Next, non-comp properties
            foreach (var group in ct.CompositionPropertyGroups)
            {
                foreach (var prop in group.PropertyTypes)
                {
                    //check if already added...
                    if (!propsDone.Contains(prop.Id))
                    {
                        AuditableProperty auditProp = PropertyTypeToAuditableProperty(prop);
                        auditProp.GroupName = group.Name;
                        auditProp.InComposition = "~NONE";
                        propertiesList.Add(auditProp);
                    }
                }

            }

            allProps.AllProperties = propertiesList;
            return allProps;
        }

        /// <summary>
        /// Meta data about a Property for Auditing purposes.
        /// </summary>
        /// <param name="UmbPropertyType"></param>
        private AuditableProperty PropertyTypeToAuditableProperty(PropertyType UmbPropertyType)
        {
            var ap = new AuditableProperty();
            ap.UmbPropertyType = UmbPropertyType;

            ap.DataType = _services.DataTypeService.GetDataTypeDefinitionById(UmbPropertyType.DataTypeId);

            //ap.DataTypeConfigType = ap.DataType.AdditionalData.GetType();
            try
            {
                var configDict = ap.DataType.AdditionalData;
                ap.DataTypeConfigDictionary = configDict;
            }
            catch (Exception e)
            {
                //ignore
                ap.DataTypeConfigDictionary = new Dictionary<string, object>();
            }

            //var  docTypes = AuditHelper.GetDocTypesForProperty(UmbPropertyType.Alias);
            // this.DocTypes = new List<string>();

            if (ap.DataType.PropertyEditorAlias.Contains("NestedContent"))
            {
                ap.IsNestedContent = true;
                var config = new NestedContentConfig(ap.DataType.AdditionalData);
                //var contentJson = ["contentTypes"];

                //var types = JsonConvert
                //    .DeserializeObject<IEnumerable<NestedContentContentTypesConfigItem>>(contentJson);
                ap.NestedContentDocTypesConfig = config.ContentTypes;
            }

            return ap;
        }

        /// <summary>
        /// Get a list of all DocTypes which contain a property of a specified Alias
        /// </summary>
        /// <param name="PropertyAlias"></param>
        /// <returns></returns>
        public List<PropertyDoctypeInfo> GetDocTypesForProperty(string PropertyAlias)
        {
            var docTypesList = new List<PropertyDoctypeInfo>();

            var allDocTypes = _services.ContentTypeService.GetAllContentTypes();

            foreach (var docType in allDocTypes)
            {
                var matchingProps = docType.CompositionPropertyTypes.Where(n => n.Alias == PropertyAlias).ToList();
                if (matchingProps.Any())
                {
                    foreach (var prop in matchingProps)
                    {
                        var x = new PropertyDoctypeInfo();
                        x.DocTypeAlias = docType.Alias;

                        var matchingGroups = docType.PropertyGroups.Where(n => n.PropertyTypes.Contains(prop.Alias)).ToList();
                        if (matchingGroups.Any())
                        {
                            x.GroupName = matchingGroups.First().Name;
                        }

                        docTypesList.Add(x);
                    }
                }
            }

            return docTypesList;
        }

        private Dictionary<PropertyType, string> PropsWithDocTypes()
        {
            if (!_AllPropsWithDocTypes.Any())
            {
                var properties = new Dictionary<PropertyType, string>();
                var docTypes = _services.ContentTypeService.GetAllContentTypes();
                foreach (var doc in docTypes)
                {
                    foreach (var prop in doc.PropertyTypes)
                    {
                        properties.Add(prop, doc.Alias);
                    }
                }

                _AllPropsWithDocTypes = properties;
            }

            return _AllPropsWithDocTypes;
        }

        #endregion

        #region AuditableDataTypes

        public IEnumerable<AuditableDataType> GetAllAuditableDataTypes()
        {
            if (!_AllAuditableDataTypes.Any())
            {
                var list = new List<AuditableDataType>();
                var datatypes = _services.DataTypeService.GetAllDataTypeDefinitions();

                foreach (var dt in datatypes)
                {
                    list.Add(ConvertIDataTypeDefinitionToAuditableDataType(dt));
                }

                _AllAuditableDataTypes = list;
            }

            return _AllAuditableDataTypes.ToList();
        }

        private AuditableDataType ConvertIDataTypeDefinitionToAuditableDataType(IDataTypeDefinition DataType)
        {
            var properties = PropsWithDocTypes();

            var adt = new AuditableDataType();
            adt.Name = DataType.Name;
            adt.EditorAlias = DataType.PropertyEditorAlias;
            adt.Guid = DataType.Key;
            adt.Id = DataType.Id;
            adt.FolderPath = GetFolderContainerPath(DataType);

            var matchingProps = properties.Where(p => p.Key.DataTypeDefinitionId == DataType.Id);
            adt.UsedOnProperties = matchingProps;

            var preValues = _services.DataTypeService.GetPreValuesCollectionByDataTypeId(DataType.Id).PreValuesAsDictionary;
            adt.ConfigurationDictionary = preValues;
            adt.ConfigurationJson = JsonConvert.SerializeObject(preValues);

            adt.UsesDocTypes = new List<string>();
            if (DataType.PropertyEditorAlias == "Umbraco.NestedContent")
            {
                adt.UsesDocTypes = PropertyEditorNestedContentInfo.GetRelatedDocumentTypes(preValues);
            }

            return adt;
        }


        private List<string> GetFolderContainerPath(IDataTypeDefinition DataType)
        {
            var folders = new List<string>();
            var ids = DataType.Path.Split(',');

            try
            {
                //The final one is the DataType, so exclude it
                foreach (var sId in ids.Take(ids.Length - 1))
                {
                    if (sId != "-1")
                    {
                        var container = _services.DataTypeService.GetContainer(Convert.ToInt32(sId));
                        folders.Add(container.Name);
                    }
                }
            }
            catch (Exception e)
            {
                folders.Add("~ERROR~");
                var msg = $"Error in 'GetFolderContainerPath()' for DataType {DataType.Id} - '{DataType.Name}'";
                LogHelper.Error(typeof(SiteAuditorService), msg, e);
            }

            return folders;
        }

        #endregion

        #region Special Queries

        public IEnumerable<IGrouping<string, string>> TemplatesUsedOnContent()
        {
            var allContent = this.GetAllAuditableContent();
            var allContentTemplates = allContent.Select(n => n.TemplateAlias);
            return allContentTemplates.GroupBy(n => n);
        }

        public IEnumerable<string> TemplatesNotUsedOnContent()
        {
            var allTemplateAliases = _services.FileService.GetTemplates().Select(n => n.Alias).ToList();

            var allContent = this.GetAllAuditableContent();

            var contentTemplatesInUse = allContent.Select(n => n.TemplateAlias).Distinct().ToList();

            var templatesWithoutContent = allTemplateAliases.Except(contentTemplatesInUse);

            return templatesWithoutContent.OrderBy(n => n).ToList();

        }

        #endregion

        #region AuditableTemplates
        public IEnumerable<AuditableTemplate> GetAuditableTemplates(bool IncludeContentNodesCount)
        {
            var key = IncludeContentNodesCount ? "IncludesContent" : "DoesNotIncludeContent";
            if (_AllAuditableTemplates.Any())
            {
                if (_AllAuditableTemplates.ContainsKey(key))
                {
                    return _AllAuditableTemplates[key].ToList();
                }
            }

            //else fill list
            var list = new List<AuditableTemplate>();
            var templates = _services.FileService.GetTemplates();

            foreach (var temp in templates)
            {
                list.Add(ConvertITemplateToAuditableTemplate(temp, IncludeContentNodesCount));
            }

            _AllAuditableTemplates.Add(key, list);

            return list;
        }

        private AuditableTemplate ConvertITemplateToAuditableTemplate(ITemplate Template, bool IncludeContentNodesCount)
        {
            var docTypes = GetAllDocTypes().ToList();

            var at = new AuditableTemplate();
            at.Name = Template.Name;
            at.Alias = Template.Alias;
            at.Guid = Template.Key;
            at.Id = Template.Id;
            at.Udi = Template.GetUdi();
            at.FolderPath = GetFolderContainerPath(Template);
            at.TemplateCode = Template.Content;
            at.CodeLength = Template.Content.Length;
            at.CreateDate = Template.CreateDate;
            at.UpdateDate = Template.UpdateDate;
            at.OriginalPath = Template.OriginalPath;

            var layout = GetLayoutFromViewFile(Template);
            at.ViewLayout = layout;

            at.IsMaster = Template.IsMasterTemplate;
            at.HasMaster = Template.MasterTemplateAlias;

            if (IncludeContentNodesCount)
            {
                var content = GetAllAuditableContent().ToList();

                var matchingContent = content.Where(p => p.TemplateAlias == Template.Alias);
                at.UsedOnContent = matchingContent.Count();
            }
            else
            {
                at.UsedOnContent = null;
            }

            var doctypesAllowed = docTypes.Where(n => n.AllowedTemplates.Contains(Template)).ToList();
            if (doctypesAllowed.Any())
            {
                at.IsAllowedOn = doctypesAllowed;
            }
            else
            {
                at.IsAllowedOn = new List<IContentType>();
            }

            var doctypeDefault = docTypes.Where(n => n.DefaultTemplate != null && n.DefaultTemplate.Id == Template.Id).ToList();
            if (doctypeDefault.Any())
            {
                at.DefaultTemplateFor = doctypeDefault;
            }
            else
            {
                at.DefaultTemplateFor = new List<IContentType>();
            }

            return at;
        }

        private List<string> GetFolderContainerPath(ITemplate Template)
        {
            var folders = new List<string>();
            var ids = Template.Path.Split(',');

            try
            {
                //The final one is the current item, so exclude it
                foreach (var sId in ids.Take(ids.Length - 1))
                {
                    if (sId != "-1")
                    {
                        var container = _services.FileService.GetTemplate(Convert.ToInt32(sId));
                        folders.Add(container.Name);
                    }
                }
            }
            catch (Exception e)
            {
                folders.Add("~ERROR~");
                var msg = $"Error in 'GetFolderContainerPath()' for Template {Template.Id} - '{Template.Name}'";
                LogHelper.Error(typeof(SiteAuditorService), msg, e);
            }

            return folders;
        }

        private string GetTemplateAlias(IContent Content)
        {
            string templateAlias = "NONE";
            if (Content.Template != null)
            {
                var template = _services.FileService.GetTemplate((int)Content.Template.Id);
                templateAlias = template.Alias;
            }

            return templateAlias;
        }

        private string GetLayoutFromViewFile(ITemplate Template)
        {
            var fileContent = Template.Content;

            var matchColl = Regex.Matches(fileContent, @"Layout\s*=\s*""*(\w*)");
            if (matchColl.Count > 0)
            {
                var matches = matchColl.Cast<Match>().ToList();
                var fullMatch = matches.Select(x => x.Value).ToList();
                var layoutOnly = matches.Select(x => x.Groups[1].Value).ToList();

                var result= layoutOnly.First();
                if (result.Trim() == "null")
                {
                    return "[NULL]";
                }
                else
                {
                    return result.Trim();
                }
            }

            return "[UNKNOWN]";

            //var viewPath = Template.VirtualPath;
            //var httpContext = new HttpContextWrapper(new HttpContext(new HttpRequest("", "http://dummyurl", ""), new HttpResponse(new StreamWriter(new MemoryStream()))));
            //var page = Activator.CreateInstance(BuildManager.GetCompiledType(viewPath)) as WebViewPage;

            //page.Context = httpContext;
            //page.PushContext(new WebPageContext(), new StreamWriter(new MemoryStream()));
            //page.Execute();

            //var layoutFileFullName = page.Layout;

            //return string.IsNullOrEmpty(layoutFileFullName)? "NULL" :  layoutFileFullName;

        }
        #endregion


    }
}
