
namespace Dragonfly.SiteAuditor.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dragonfly.SiteAuditor.Models;
    using Umbraco.Core.Models;

    public static class Extensions
    {
        #region IContent
        public static bool HasPropertyValue(this IContent Content, string PropertyAlias)
        {
            var hasProp = Content.HasProperty(PropertyAlias);

            if (!hasProp)
            {
                return false;
            }

            var valObject = Content.GetValue(PropertyAlias);
            if (valObject == null)
            {
                return false;
            }

            var valString = Content.GetValue<string>(PropertyAlias);
            if (valString == "")
            {
                return false;
            }

            return true;
        }
        
        public static string NodePathAsCustomText(this IContent Content, string Separator = " » ")
        {
            var paths= AuditHelper.NodePath(Content);
            var nodePath = string.Join(Separator, paths);
            return nodePath;
        }

        //public static IEnumerable<AuditableContent> ToAuditableContent(this IEnumerable<IContent> Content)
        //{
        //    var list = new List<AuditableContent>();
        //    if (Content != null)
        //    {
        //        foreach (var item in Content)
        //        {
        //          var x = new AuditableContent();
        //                list.Add(x);
        //            }
        //        }
        //    }

        //    return list;
        //}


        #endregion
    }
}
