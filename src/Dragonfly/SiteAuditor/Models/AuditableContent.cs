namespace Dragonfly.SiteAuditor.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Umbraco.Core.Models;

    /// <summary>
    /// Meta data about a Content Node for Auditing purposes.
    /// </summary>
    [DataContract]
    public class AuditableContent
    {
        #region Private vars
        private string _defaultDelimiter = " » ";
        #endregion

        #region Public Props
        /// <summary>
        /// Gets or sets the content node as an IContent
        /// </summary>
        public IContent UmbContentNode { get; internal set; }

        /// <summary>
        /// Gets or sets the content node and an IPublishedContent
        /// </summary>
        public IPublishedContent UmbPublishedNode { get; internal set; }

        /// <summary>
        /// Full path to node in a single delimited string using object's default delimiter
        /// </summary>
        public string NodePathAsText { get; internal set; }
        
        /// <summary>
        /// Alias of the Template assigned tot his Content Node. Returns "NONE" if there is no template.
        /// </summary>
        public string TemplateAlias { get; internal set; }

        public bool IsPublished { get; internal set; }

        public string DocTypeAlias { get; internal set; }

        /// <summary>
        /// Path-only Url. Returns "UNPUBLISHED" if there is no public url.
        /// </summary>
        public string RelativeNiceUrl { get; internal set; }

        /// <summary>
        /// Url with domain name. Returns "UNPUBLISHED" if there is no public url.
        /// </summary>
        public string FullNiceUrl { get; internal set; }

        /// <summary>
        /// The node path.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public IEnumerable<string> NodePath { get; internal set; }

        /// <summary>
        /// Default string used for NodePathAsText
        /// ' » ' unless explicitly changed
        /// </summary>
        public string DefaultDelimiter
        {
            get { return _defaultDelimiter; }
            internal set { _defaultDelimiter = value; }
        }

        #endregion

        #region Constructor

        public AuditableContent()
        {
            //For conversion see: 
            //SiteAuditorService.ConvertIContentToAuditableContent()
        }

        #endregion

        #region Methods

        public string NodePathAsCustomText(string Separator = " » ")
        {
            var nodePath = string.Join(Separator, this.NodePath);
            return nodePath;
        }

        #endregion

    }
}
