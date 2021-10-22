namespace Dragonfly.SiteAuditor.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    public class AuditableTemplate
    {
        #region Private vars

        private string _defaultDelimiter = " » ";

        #endregion

        #region Public Props
        public string Name { get; set; }
        public string Alias { get; set; }
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public GuidUdi Udi { get; set; }
        public List<string> FolderPath { get; set; }
        public bool IsMaster { get; set; }
        public string HasMaster { get; set; }
        public int CodeLength { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string OriginalPath { get; set; }
        public int? UsedOnContent { get; set; }
        public IEnumerable<IContentType> IsAllowedOn { get; set; }
        public IEnumerable<IContentType> DefaultTemplateFor { get; set; }
        

        /// <summary>
        /// Default string used for PathAsText
        /// ' » ' unless explicitly changed
        /// </summary>
        public string DefaultDelimiter
        {
            get { return _defaultDelimiter; }
            internal set { _defaultDelimiter = value; }
        }
        
        /// <summary>
        /// Full path in a single delimited string using object's default delimiter
        /// </summary>
        public string PathAsText
        {
            get
            {
                var path = string.Join(this.DefaultDelimiter, this.FolderPath);
                return path;
            }
        }

        public string TemplateCode { get; set; }
        public string ViewLayout { get; set; }

        #endregion

        public AuditableTemplate()
        {
            //For conversion see: 
            //SiteAuditorService.ConvertITemplateToAuditableTemplate()
        }
    }
}
