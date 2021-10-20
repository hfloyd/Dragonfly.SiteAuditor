namespace Dragonfly.SiteAuditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        private IEnumerable<IContent> _AllContent = new List<IContent>();

        private IEnumerable<IContentTypeComposition> _AllContentTypeComps = new List<IContentType>();

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
        /// Gets all site nodes as IPublishedContent
        /// </summary>
        /// <param name="IncludeUnpublished">Should unpublished nodes be included? (They will be returned as 'virtual' IPublishedContent models)</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetAllNodes()
        {
            var nodesList = new List<IPublishedContent>();

            //if (IncludeUnpublished)
            //{
            //    //Get nodes as IContent
            //    var topLevelNodes = _services.ContentService.GetRootContent().OrderBy(n => n.SortOrder);

            //    foreach (var thisNode in topLevelNodes)
            //    {
            //        nodesList.AddRange(LoopNodes(thisNode));
            //    }
            //}
            //else
            //{
            //Get nodes as IPublishedContent
            var topLevelNodes = _umbracoHelper.TypedContentAtRoot().OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopNodes(thisNode));
            }
            // }

            return nodesList;
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

        //private IEnumerable<IPublishedContent> LoopNodes(IContent ThisNode)
        //{
        //    var nodesList = new List<IPublishedContent>();

        //    //Add current node, then loop for children
        //    try
        //    {
        //        nodesList.Add(ThisNode.ToPublishedContent());

        //        if (ThisNode.Children().Any())
        //        {
        //            foreach (var childNode in ThisNode.Children().OrderBy(n => n.SortOrder))
        //            {
        //                nodesList.AddRange(LoopNodes(childNode));
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //skip
        //    }

        //    return nodesList;
        //}

        #endregion

        #region Content 

        public List<IContent> GetAllContent()
        {
            var nodesList = new List<IContent>();

            var topLevelContentNodes = _services.ContentService.GetRootContent().OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelContentNodes)
            {
                nodesList.AddRange(LoopForContentNodes(thisNode));
            }

            _AllContent = nodesList;
            return nodesList;
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
        public List<AuditableContent> GetContentNodes()
        {
            if (_AllAuditableContent.Any())
            {
                return _AllAuditableContent.ToList();
            }

            var nodesList = new List<AuditableContent>();

            var topLevelContentNodes = _services.ContentService.GetRootContent().OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelContentNodes)
            {
                nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
            }

            _AllAuditableContent = nodesList;
            return nodesList;
        }

        /// <summary>
        /// Gets all descendant nodes from provided Root Node Id as AuditableContent models
        /// </summary>
        /// <param name="RootNodeId">Integer Id of Root Node</param>
        /// <returns></returns>
        public List<AuditableContent> GetContentNodes(int RootNodeId)
        {
            var nodesList = new List<AuditableContent>();

            var topLevelNodes = _services.ContentService.GetByIds(RootNodeId.AsEnumerableOfOne()).OrderBy(n => n.SortOrder);

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
            }

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

        public List<AuditableContent> GetContentNodes(string DocTypeAlias)
        {
            var nodesList = new List<AuditableContent>();

            IEnumerable<IContent> allContent;
            if (_AllContent.Any())
            {
                allContent = _AllContent.ToList();
            }
            else
            {
                allContent = GetAllContent().ToList();
            }

            var filteredContent = allContent.Where(n => n.ContentType.Alias == DocTypeAlias);

            foreach (var content in filteredContent)
            {
                nodesList.Add(ConvertIContentToAuditableContent(content));
            }

            return nodesList;
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
                    //ac.RelativeNiceUrl = iPub.Url(mode: UrlMode.Relative);
                    //ac.FullNiceUrl = iPub.Url(mode: UrlMode.Absolute);
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

        private AuditableContent ConvertIPubContentToAuditableContent(IPublishedContent PubContentNode)
        {
            var ac = new AuditableContent();
            ac.UmbContentNode = _services.ContentService.GetById(PubContentNode.Id);
            ac.UmbPublishedNode = PubContentNode;
            ac.NodePath = AuditHelper.NodePath(ac.UmbContentNode);
            ac.TemplateAlias = GetTemplateAlias(ac.UmbContentNode);
            //this.DocTypes = new List<string>();

            return ac;
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

        public IEnumerable<IContentTypeComposition> GetAllCompositions()
        {
            if (_AllContentTypeComps.Any())
            {
                return _AllContentTypeComps;
            }
            else
            {
                var doctypes = _services.ContentTypeService.GetAllContentTypes();
                var comps = doctypes.SelectMany(n => n.ContentTypeComposition);

                _AllContentTypeComps = comps.DistinctBy(n => n.Id);

                return _AllContentTypeComps;
            }

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
            //adt.IsElement = ContentType.;

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

            //adt.HasContentNodes = _services.ContentTypeService. HasContentNodes(ContentType.Id);



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



        ///// <summary>
        ///// Gets list of all DocTypes on site as AuditableDoctype models
        ///// </summary>
        ///// <returns></returns>
        //public static IEnumerable<AuditableDocType> GetAuditableDocTypes()
        //{
        //    var list = new List<AuditableDocType>();

        //    var doctypes = umbDocTypeService.GetAllContentTypes();

        //    foreach (var type in doctypes)
        //    {
        //        if (type != null)
        //        {
        //            list.Add(new AuditableDocType(type));
        //        }
        //    }

        //    return list;
        //}

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
            var properties = new Dictionary<PropertyType, string>();
            var docTypes = _services.ContentTypeService.GetAllContentTypes();
            foreach (var doc in docTypes)
            {
                foreach (var prop in doc.PropertyTypes)
                {
                    properties.Add(prop, doc.Alias);
                }
            }

            return properties;
        }

        #endregion

        #region AuditableDataTypes

        public IEnumerable<AuditableDataType> AllDataTypes()
        {
            var list = new List<AuditableDataType>();
            var datatypes = _services.DataTypeService.GetAllDataTypeDefinitions();

            var properties = PropsWithDocTypes();


            foreach (var dt in datatypes)
            {
                var adt = new AuditableDataType();
                adt.Name = dt.Name;
                adt.EditorAlias = dt.PropertyEditorAlias;
                adt.Guid = dt.Key;
                adt.Id = dt.Id;
                adt.FolderPath = GetFolderContainerPath(dt);
                adt.ConfigurationJson = JsonConvert.SerializeObject(dt.AdditionalData);

                var matchingProps = properties.Where(p => p.Key.DataTypeId == dt.Key);
                adt.UsedOnProperties = matchingProps;

                list.Add(adt);
            }

            return list;
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
            var allContent = this.GetContentNodes();
            var allContentTemplates = allContent.Select(n => n.TemplateAlias);
            return allContentTemplates.GroupBy(n => n);
        }

        public IEnumerable<string> TemplatesNotUsedOnContent()
        {
            var allTemplateAliases = _services.FileService.GetTemplates().Select(n => n.Alias).ToList();

            var allContent = this.GetContentNodes();

            var contentTemplatesInUse = allContent.Select(n => n.TemplateAlias).Distinct().ToList();

            var templatesWithoutContent = allTemplateAliases.Except(contentTemplatesInUse);

            return templatesWithoutContent.OrderBy(n => n).ToList();

        }

        #endregion

        #region AuditableTemplates
        public IEnumerable<AuditableTemplate> GetAuditableTemplates()
        {
            var list = new List<AuditableTemplate>();
            var templates = _services.FileService.GetTemplates();

            var content = GetContentNodes().ToList();
            var docTypes = GetAllDocTypes().ToList();

            foreach (var temp in templates)
            {
                var at = new AuditableTemplate();
                at.Name = temp.Name;
                at.Alias = temp.Alias;
                at.Guid = temp.Key;
                at.Id = temp.Id;
                at.Udi = temp.GetUdi();
                at.FolderPath = GetFolderContainerPath(temp);
                at.IsMaster = temp.IsMasterTemplate;
                at.HasMaster = temp.MasterTemplateAlias;
                at.CodeLength = temp.Content.Length;
                at.CreateDate = temp.CreateDate;
                at.UpdateDate = temp.UpdateDate;
                at.OriginalPath = temp.OriginalPath;
                //at.XXX = temp.;
                //at.XXX = temp.UpdateDate;
                //at.ConfigurationJson = JsonConvert.SerializeObject(temp.Configuration);

                var matchingContent = content.Where(p => p.TemplateAlias == temp.Alias);
                at.UsedOnContent = matchingContent.Count();

                var doctypesAllowed = docTypes.Where(n => n.AllowedTemplates.Contains(temp));
                if (doctypesAllowed.Any())
                {
                    at.IsAllowedOn = doctypesAllowed;
                }
                else
                {
                    at.IsAllowedOn = new List<IContentType>();
                }

                var doctypeDefault = docTypes.Where(n => n.DefaultTemplate != null && n.DefaultTemplate.Id == temp.Id);
                if (doctypeDefault.Any())
                {
                    at.DefaultTemplateFor = doctypeDefault;
                }
                else
                {
                    at.DefaultTemplateFor = new List<IContentType>();
                }

                list.Add(at);
            }

            return list;
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
        #endregion


    }
}
