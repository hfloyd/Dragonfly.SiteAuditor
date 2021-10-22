namespace Dragonfly.SiteAuditor.Models
{
    using System;
    using System.Collections.Generic;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    public class AuditableDocType
    {
        #region Private vars
        //private UmbracoHelper umbHelper = new UmbracoHelper(UmbracoContext.Current);
        //private IContentTypeService umbDocTypeService = ApplicationContext.Current.Services.ContentTypeService;
        private string _defaultDelimiter = " » ";
        #endregion

        #region Public Props

        public IContentType ContentType { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public IEnumerable<string> FolderPath { get; internal set; }
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public string DefaultTemplateName { get; set; }
        public Dictionary<int, string> AllowedTemplates { get; set; }
        public bool HasContentNodes { get; set; }
        public int QtyContentNodes { get; set; }
        public bool IsElement { get; set; }
        public bool IsComposition { get; set; }
        public Dictionary<int, string> CompositionsUsed { get; set; }

        /// <summary>
        /// Default string used for NodePathAsText
        /// ' » ' unless explicitly changed
        /// </summary>
        public string DefaultDelimiter
        {
            get { return _defaultDelimiter; }
            internal set { _defaultDelimiter = value; }
        }

        /// <summary>
        /// Full path to node in a single delimited string using object's default delimiter
        /// </summary>
        public string PathAsText
        {
            get
            {
                var path = string.Join(this.DefaultDelimiter, this.FolderPath);
                return path;
            }
        }

        public IEnumerable<AuditableDataType> UsedInDataTypes { get; set; }

        #endregion

        #region Constructor

        public AuditableDocType()
        {
            //For conversion see: 
            //SiteAuditorService.ConvertIContentTypeToAuditableDocType()
        }

        /// <inheritdoc />
        //public AuditableDocType(IContentType ContentType)
        //{
        //    this.ContentType = ContentType;
        //    this.Name = ContentType.Name;
        //    this.Alias = ContentType.Alias;
        //    this.Guid = ContentType.Key;

        //    if (ContentType.DefaultTemplate != null)
        //    {
        //        this.DefaultTemplateName = ContentType.DefaultTemplate.Name;
        //    }
        //    else
        //    {
        //        this.DefaultTemplateName = "NONE";
        //    }

        //    // var x = ContentType.AllowedTemplates
        //}

        #endregion
    }
}
