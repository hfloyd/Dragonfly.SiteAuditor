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
    /// Helper for getting collections of various site information for auditing
    /// </summary>
    public static class CollectionsHelper
    {
        private static readonly UmbracoHelper umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
        private static readonly IContentService umbContentService = ApplicationContext.Current.Services.ContentService;
        private static readonly IContentTypeService umbDocTypeService = ApplicationContext.Current.Services.ContentTypeService;

        #region All Nodes (IPublishedContent)

        /// <summary>
        /// Gets all site nodes as IPublishedContent
        /// </summary>
        /// <param name="IncludeUnpublished">Should unpublished nodes be included? (They will be returned as 'virtual' IPublishedContent models)</param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> GetAllNodes(bool IncludeUnpublished = false)
        {
            var nodesList = new List<IPublishedContent>();

            if (IncludeUnpublished)
            {
                //Get nodes as IContent
                var topLevelNodes = umbContentService.GetRootContent().OrderBy(n => n.SortOrder);

                foreach (var thisNode in topLevelNodes)
                {
                    nodesList.AddRange(LoopNodes(thisNode));
                }
            }
            else
            {
                //Get nodes as IPublishedContent
                var topLevelNodes = umbracoHelper.TypedContentAtRoot().OrderBy(n => n.SortOrder);

                foreach (var thisNode in topLevelNodes)
                {
                    nodesList.AddRange(LoopNodes(thisNode));
                }
            }

            return nodesList;
        }

        internal static IEnumerable<IPublishedContent> LoopNodes(IPublishedContent ThisNode)
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

        internal static IEnumerable<IPublishedContent> LoopNodes(IContent ThisNode)
        {
            var nodesList = new List<IPublishedContent>();

            //Add current node, then loop for children
            try
            {
                nodesList.Add(ThisNode.ToPublishedContent());

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

        #region DocTypes

        /// <summary>
        /// Gets list of all DocTypes on site as IContentType models
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IContentType> GetAllDocTypes()
        {
            var list = new List<IContentType>();

            var doctypes = umbDocTypeService.GetAllContentTypes();

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
        /// Gets list of all DocTypes on site as AuditableDoctype models
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AuditableDocType> GetAuditableDocTypes()
        {
            var list = new List<AuditableDocType>();

            var doctypes = umbDocTypeService.GetAllContentTypes();

            foreach (var type in doctypes)
            {
                if (type != null)
                {
                    list.Add(new AuditableDocType(type));
                }
            }

            return list;
        }

        #endregion


        #region AuditableContent

        /// <summary>
        /// Gets all site nodes as AuditableContent models
        /// </summary>
        /// <returns></returns>
        //public static List<AuditableContent> GetContentNodes()
        //{
        //    var nodesList = new List<AuditableContent>();

        //    //var TopLevelNodes = umbHelper.ContentAtRoot();
        //    var topLevelNodes = umbContentService.GetRootContent().OrderBy(n => n.SortOrder);

        //    foreach (var thisNode in topLevelNodes)
        //    {
        //        nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
        //    }

        //    return nodesList;
        //}


        /// <summary>
        /// Gets all descendant nodes from provided Root Node Id as AuditableContent models
        /// </summary>
        /// <param name="RootNodeId">Integer Id of Root Node</param>
        /// <returns></returns>
        //public static List<AuditableContent> GetContentNodes(int RootNodeId)
        //{
        //    var nodesList = new List<AuditableContent>();

        //    //var TopLevelNodes = umbHelper.ContentAtRoot();
        //    var topLevelNodes = umbContentService.GetByIds(RootNodeId.AsEnumerableOfOne()).OrderBy(n => n.SortOrder);

        //    foreach (var thisNode in topLevelNodes)
        //    {
        //        nodesList.AddRange(LoopForAuditableContentNodes(thisNode));
        //    }

        //    return nodesList;
        //}


        /// <summary>
        /// Gets all descendant nodes from provided Root Node Id as AuditableContent models
        /// </summary>
        /// <param name="RootNodeUdi">Udi of Root Node</param>
        /// <returns></returns>
        //public static List<AuditableContent> GetContentNodes(Udi RootNodeUdi)
        //{
        //    var nodesList = new List<AuditableContent>();
        //    var replaceStr = $"umb://{RootNodeUdi.EntityType}/";
        //    var guidStr = RootNodeUdi.ToString().Replace(replaceStr, "");
        //    var guid = new Guid(guidStr);
        //    var node = umbContentService.GetById(guid);
        //    if (node != null)
        //    {
        //        //var TopLevelNodes = umbHelper.ContentAtRoot();
        //        //var topLevelNodes = umbContentService.GetById(node.Id)//.OrderBy(n => n.SortOrder);

        //        //foreach (var thisNode in topLevelNodes)
        //        //{
        //        nodesList.AddRange(LoopForAuditableContentNodes(node));
        //        // }
        //    }

        //    return nodesList;
        //}

        //internal static List<AuditableContent> LoopForAuditableContentNodes(IContent ThisNode)
        //{
        //    var nodesList = new List<AuditableContent>();

        //    //Add current node, then loop for children
        //    AuditableContent auditContent = new AuditableContent(ThisNode);
        //    nodesList.Add(auditContent);

        //    if (ThisNode.Children().Any())
        //    {
        //        foreach (var childNode in ThisNode.Children().OrderBy(n => n.SortOrder))
        //        {
        //            nodesList.AddRange(LoopForAuditableContentNodes(childNode));
        //        }
        //    }

        //    return nodesList;
        //}

        #endregion
    }
}
