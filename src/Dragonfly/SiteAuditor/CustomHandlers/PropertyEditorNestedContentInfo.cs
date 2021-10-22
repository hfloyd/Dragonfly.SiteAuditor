namespace Dragonfly.SiteAuditor.CustomHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web.PropertyEditors;


    //TODO: reconfigure as proper Interface, etc.
    internal static class PropertyEditorNestedContentInfo
    {
        internal static IEnumerable<string> GetRelatedDocumentTypes(IDataTypeDefinition DataType, ServiceContext Services)
        {
            if (DataType.PropertyEditorAlias == "Umbraco.NestedContent")
            {
                var preValues = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(DataType.Id).PreValuesAsDictionary;
                return GetRelatedDocumentTypes(preValues);
            }

            return new List<string>();
        }

        internal static IEnumerable<string> GetRelatedDocumentTypes(IDictionary<string, PreValue> PreValues)
        {
            var doctypes = new List<string>();

            try
            {
                var value = PreValues["contentTypes"].Value;

                if (value.StartsWith("["))
                {
                    //Array
                    var configs = JsonConvert.DeserializeObject<JArray>(value);
                    foreach (var token in configs)
                    {
                        var config = ((JObject)token);
                        var contentType = GetContentTypeAliasFromItem(config);
                        doctypes.Add(contentType);
                    }
                }
                else
                {
                    //Single
                    var config = ((JObject)value);
                    var contentType = GetContentTypeAliasFromItem(config);
                    doctypes.Add(contentType);
                }
            }
            catch (Exception e)
            {

            }

            return doctypes;
        }



        internal static string GetContentTypeAliasFromItem(JObject item)
        {
            var key = "ncContentTypeAlias";
            var contentTypeAliasProperty = item[key];
            if (contentTypeAliasProperty != null)
            {
                return contentTypeAliasProperty.ToObject<string>();
            }

            key = "ncAlias";
            contentTypeAliasProperty = item[key];
            if (contentTypeAliasProperty != null)
            {
                return contentTypeAliasProperty.ToObject<string>();
            }

            return null;
        }
    }
}
