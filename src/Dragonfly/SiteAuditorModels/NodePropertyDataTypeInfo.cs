namespace Dragonfly.SiteAuditorModels
{
    using Umbraco.Core.Models;

    public class NodePropertyDataTypeInfo
    {
        public int NodeId { get; set; }
        public string DocTypeAlias { get; set; }
        public string DocTypeCompositionAlias { get; set; }
        public string PropertyEditorAlias { get; set; }
        public string DatabaseType { get; set; }
        public string ErrorMessage { get; set; }
        public IDataTypeDefinition DataType { get; set; }
        public IPublishedProperty Property { get; set; }

        public bool HasError
        {
            get
            {
                if (ErrorMessage != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool UsesComposition
        {
            get
            {
                if (DocTypeCompositionAlias != null && DocTypeCompositionAlias != "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
