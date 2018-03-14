namespace Dragonfly.SiteAuditorHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Dragonfly.SiteAuditorModels;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    public static class CollectionsHelper
    {
        private static UmbracoHelper umbHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);
        private static IContentService umbContentService = ApplicationContext.Current.Services.ContentService;

 

        public static List<AuditableContent> GetContentNodes()
        {
            
            var nodesList = new List<AuditableContent>();

            //var TopLevelNodes = umbHelper.ContentAtRoot();
            var topLevelNodes = umbContentService.GetRootContent().OrderBy(n=> n.SortOrder); 

            foreach (var thisNode in topLevelNodes)
            {
                nodesList.AddRange(LoopNodes(thisNode));
            }

            return nodesList;
        }

        internal static List<AuditableContent> LoopNodes(IContent ThisNode)
        {
            var nodesList = new List<AuditableContent>();

            //Add current node, then loop for children
            AuditableContent auditContent = new AuditableContent(ThisNode);
            nodesList.Add(auditContent);

            if (ThisNode.Children().Any())
            {
                foreach (var childNode in ThisNode.Children().OrderBy(n => n.SortOrder))
                {
                    nodesList.AddRange(LoopNodes(childNode));
                }
            }

            return nodesList;
        }
    }
}
