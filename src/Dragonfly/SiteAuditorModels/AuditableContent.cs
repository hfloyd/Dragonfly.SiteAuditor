namespace Dragonfly.SiteAuditorModels
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Dragonfly.SiteAuditorHelpers;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    /// <summary>
    /// Meta data about a Content Node for Auditing purposes.
    /// </summary>
    [DataContract]
    public class AuditableContent
    {
        #region Private vars
        private UmbracoHelper umbHelper = new UmbracoHelper(UmbracoContext.Current);
        private IContentService umbContentService = ApplicationContext.Current.Services.ContentService;

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

        /// <summary>
        /// Full path to node in a single delimited string using object's default delimiter
        /// </summary>
        public string NodePathAsText
        {
            get
            {

                var nodePath = string.Join(this.DefaultDelimiter, this.NodePath);
                return nodePath;
            }
        }

        /// <summary>
        /// Alias of the Template assigned tot his Content Node. Returns "NONE" if there is no template.
        /// </summary>
        public string TemplateAlias
        {
            get
            {
                string templateAlias = "NONE";
                if (this.UmbContentNode.Template != null)
                {
                    templateAlias = this.UmbContentNode.Template.Alias;
                }

                return templateAlias;
            }
        }

        /// <summary>
        /// Url with domain name. Returns "UNPUBLISHED" if there is no public url.
        /// </summary>
        public string FullNiceUrl
        {
            get
            {
                string NiceUrl;

                if (this.UmbContentNode.Published & this.UmbPublishedNode != null)
                {
                    NiceUrl = this.UmbPublishedNode.UrlWithDomain();
                }
                else
                {
                    NiceUrl = "UNPUBLISHED";
                }

                return NiceUrl;
            }
        }

        /// <summary>
        /// Path-only Url. Returns "UNPUBLISHED" if there is no public url.
        /// </summary>
        public string RelativeNiceUrl
        {
            get
            {
                string NiceUrl;

                if (this.UmbContentNode.Published)
                {

                    NiceUrl = this.UmbPublishedNode.Url;
                }
                else
                {
                    NiceUrl = "UNPUBLISHED";
                }

                return NiceUrl;
            }
        }

        #endregion

        #region Constructor
        public AuditableContent(IContent UmbContentNode)
        {
            this.UmbContentNode = UmbContentNode;
            this.UmbPublishedNode = umbHelper.TypedContent(UmbContentNode.Id);
            this.NodePath = AuditHelper.NodePath(this.UmbContentNode);
            //this.DocTypes = new List<string>();
        }

        public AuditableContent(IPublishedContent umbContentNode)
        {
            this.UmbContentNode = umbContentService.GetById(umbContentNode.Id);
            this.UmbPublishedNode = umbContentNode;
            this.NodePath = AuditHelper.NodePath(this.UmbContentNode);
            //this.DocTypes = new List<string>();
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
