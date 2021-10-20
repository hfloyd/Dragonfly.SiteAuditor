namespace Dragonfly.SiteAuditor.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Dragonfly.SiteAuditor.Helpers;
    using Newtonsoft.Json;
    using Umbraco.Core.Models;

    [DataContract]
    public class AuditableProperty
    {
        private List<PropertyDoctypeInfo> _docTypes = new List<PropertyDoctypeInfo>();

        #region Public Props
        [DataMember]
        public PropertyType UmbPropertyType { get; internal set; }

        [DataMember]
        public string InDocType { get; internal set; }

        [DataMember]
        public string InDocTypeGroup { get; internal set; }

        [DataMember]
        public List<PropertyDoctypeInfo> AllDocTypes
        {
            get { return _docTypes; }
            internal set { _docTypes = value; }
        }

        [DataMember]
        public IDataTypeDefinition DataType { get; internal set; }

        [DataMember]
        public Dictionary<string, string> DataTypeConfig { get; internal set; }

        [DataMember]
        public bool IsNestedContent { get; internal set; }

        [DataMember]
        public IEnumerable<NestedContentContentTypesConfigItem> NestedContentDocTypesConfig { get; internal set; }

        public IDictionary<string, object> DataTypeConfigDictionary { get; set; }
        public string InComposition { get; set; }
        public object GroupName { get; set; }

        #endregion

        #region Constructor

        public AuditableProperty()
        {
            
        }

        /// <summary>
        /// Meta data about a Property for Auditing purposes.
        /// </summary>
        /// <param name="UmbPropertyType"></param>
        public AuditableProperty(PropertyType UmbPropertyType)
        {
            this.UmbPropertyType = UmbPropertyType;
            // this.DocTypes = new List<string>();
            this.DataType = AuditHelper.GetDataTypeDefinition(this.UmbPropertyType.DataTypeDefinitionId);
            this.DataTypeConfig = DbHelper.GetDataTypeConfig(this.DataType.Id);

            _docTypes = AuditHelper.GetDocTypesForProperty(UmbPropertyType.Alias);

            //this.InDocType = 

            if (this.DataType.PropertyEditorAlias.Contains("NestedContent"))
            {
                this.IsNestedContent = true;
                var contentJson = this.DataTypeConfig["contentTypes"];

                var types = JsonConvert
                    .DeserializeObject<IEnumerable<NestedContentContentTypesConfigItem>>(contentJson);
                this.NestedContentDocTypesConfig = types;
            }
        }

        #endregion

        #region Methods

        public List<PropertyDoctypeInfo> GetAllDocTypes()
        {
            if (_docTypes.Any())
            {
                return _docTypes;
            }
            else
            {
                return AuditHelper.GetDocTypesForProperty(UmbPropertyType.Alias);
            }
        }
        #endregion

    }

    public class SiteAuditableProperties
    {
        public string PropsForDoctype { get; internal set; }
        public IEnumerable<AuditableProperty> AllProperties { get; internal set; }

        public SiteAuditableProperties()
        {
            this.PropsForDoctype = "[All]";
            List<AuditableProperty> propertiesList = new List<AuditableProperty>();

            var allDocTypes = AuditHelper.GetAllContentTypesAliases();

            foreach (var docTypeAlias in allDocTypes)
            {
                ContentType ct = AuditHelper.GetContentTypeByAlias(docTypeAlias);

                foreach (var prop in ct.PropertyTypes)
                {
                    //test for the same property already in list
                    if (propertiesList.Exists(i => i.UmbPropertyType.Alias == prop.Alias & i.UmbPropertyType.Name == prop.Name & i.UmbPropertyType.DataTypeDefinitionId == prop.DataTypeDefinitionId))
                    {
                        //Add current DocType to existing property
                        var info = new PropertyDoctypeInfo();
                        info.DocTypeAlias = docTypeAlias;
                        info.GroupName = "";
                        propertiesList.Find(i => i.UmbPropertyType.Alias == prop.Alias).AllDocTypes.Add(info);
                    }
                    else
                    {
                        //Add new property
                        AuditableProperty auditProp = new AuditableProperty(prop);

                        var info = new PropertyDoctypeInfo();
                        info.DocTypeAlias = docTypeAlias;
                        info.GroupName = "";

                        auditProp.AllDocTypes.Add(info);
                        propertiesList.Add(auditProp);
                    }
                }
            }

            this.AllProperties = propertiesList;
        }

        public SiteAuditableProperties(string DocTypeAlias)
        {
            this.PropsForDoctype = DocTypeAlias;
            List<AuditableProperty> propertiesList = new List<AuditableProperty>();

            ContentType ct = AuditHelper.GetContentTypeByAlias(DocTypeAlias);

            foreach (var prop in ct.PropertyTypes)
            {
                //Add property
                AuditableProperty auditProp = new AuditableProperty(prop);

                var info = new PropertyDoctypeInfo();
                info.DocTypeAlias = DocTypeAlias;
                info.GroupName = "";

                auditProp.AllDocTypes.Add(info);
                propertiesList.Add(auditProp);
            }

            this.AllProperties = propertiesList;
        }
    }

    public class PropertyDoctypeInfo
    {
        public string DocTypeAlias { get; set; }

        public string GroupName { get; set; }
        public int Id { get; set; }
    }
}
