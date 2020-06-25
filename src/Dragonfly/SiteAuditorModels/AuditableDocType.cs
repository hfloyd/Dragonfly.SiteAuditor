namespace Dragonfly.SiteAuditorModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.Serialization;
    using Umbraco.Core;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using Umbraco.Core.Models;

    
    public class AuditableDocType
    {
        #region Private vars
        private UmbracoHelper umbHelper = new UmbracoHelper(UmbracoContext.Current);
        private IContentTypeService umbDocTypeService = ApplicationContext.Current.Services.ContentTypeService;

        #endregion

        #region Public Props

        public IContentType ContentType { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }
        public Guid GUID { get; set; }
        public string DefaultTemplateName { get; set; }

        //TODO: Add Info about compositions/parents/folders: IsComposition, HasCompositions, etc.

        #endregion

        #region Constructor

        /// <inheritdoc />
        public AuditableDocType(IContentType ContentType)
        {
            this.ContentType = ContentType;
            this.Name = ContentType.Name;
            this.Alias = ContentType.Alias;
            this.GUID = ContentType.Key;

            if (ContentType.DefaultTemplate != null)
            {
                this.DefaultTemplateName = ContentType.DefaultTemplate.Name;
            }
            else
            {
                this.DefaultTemplateName = "NONE";
            }

            // var x = ContentType.AllowedTemplates
        }

        public AuditableDocType()
        {
            
        }

        #endregion
    }
}
