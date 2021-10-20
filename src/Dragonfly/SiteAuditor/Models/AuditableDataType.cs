using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.SiteAuditor.Models
{
    using Umbraco.Core.Models;

    public class AuditableDataType
    {
        public string Name { get; set; }
        public string EditorAlias { get; set; }
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public IEnumerable<KeyValuePair<PropertyType, string>> UsedOnProperties { get; set; }
        public string ConfigurationJson { get; set; }
        public List<string> FolderPath { get; set; }

        /// <summary>
        /// Default string used for NodePathAsText
        /// ' » ' unless explicitly changed
        /// </summary>
        public string DefaultDelimiter
        {
            get { return _defaultDelimiter; }
            internal set { _defaultDelimiter = value; }
        }
        private string _defaultDelimiter = " » ";

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

   
    }
}
