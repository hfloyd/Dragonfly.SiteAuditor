namespace Dragonfly.SiteAuditorModels
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Dragonfly.SiteAuditorHelpers;
    using Umbraco.Core.Models;

    [DataContract]
    public class AuditableProperty
    {
        private List<string> _docTypes;

        #region Public Props
        [DataMember]
        public PropertyType UmbPropertyType { get; internal set; }

        [DataMember]
        public List<string> DocTypes
        {
            get { return _docTypes; }
            internal set { _docTypes = value; }
        }

        [DataMember]
        public DataTypeDefinition DataType { get; internal set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Meta data about a Property for Auditing purposes.
        /// </summary>
        /// <param name="UmbPropertyType"></param>
        public AuditableProperty(PropertyType UmbPropertyType)
        {
            this.UmbPropertyType = UmbPropertyType;
            this.DocTypes = new List<string>();
            this.DataType = AuditHelper.GetDataTypeDefinition(this.UmbPropertyType.DataTypeDefinitionId);  
        }

        #endregion

        #region Methods

        public IEnumerable<string> GetDocTypes()
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
        public IEnumerable<AuditableProperty> AllProperties { get; internal set; }

        public SiteAuditableProperties()
        {
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
                        propertiesList.Find(i => i.UmbPropertyType.Alias == prop.Alias).DocTypes.Add(docTypeAlias);
                    }
                    else
                    {
                        //Add new property
                        AuditableProperty auditProp = new AuditableProperty(prop);
                        auditProp.DocTypes.Add(docTypeAlias);
                        propertiesList.Add(auditProp);
                    }
                }
            }

            this.AllProperties= propertiesList;
        }

        public SiteAuditableProperties(string DocTypeAlias)
        {
            List<AuditableProperty> propertiesList = new List<AuditableProperty>();

            ContentType ct = AuditHelper.GetContentTypeByAlias(DocTypeAlias);

            foreach (var prop in ct.PropertyTypes)
            {
                //Add property
                AuditableProperty auditProp = new AuditableProperty(prop);
                auditProp.DocTypes.Add(DocTypeAlias);
                propertiesList.Add(auditProp);
            }

            this.AllProperties = propertiesList;
        }
    }
}
