namespace Dragonfly.SiteAuditor.Models.Dtos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Umbraco.Core.Persistence;
    
    //CREATE TABLE[cmsDataTypePreValues] (
    //[id] int IDENTITY(10,1) NOT NULL
    //, [datatypeNodeId] int NOT NULL
    //, [value] ntext NULL
    //, [sortorder] int NOT NULL
    //, [alias] nvarchar(50) NULL
    //);

    [TableName("cmsDataTypePreValues")]
    [PrimaryKey("id")]
    class UmbDataTypePrevalue
    {
        public const string TableName = "cmsDataTypePreValues";

        [Column("id")]
        public int Id { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("sortorder")]
        public int SortOrder { get; set; }

        [Column("alias")]
        public string Alias { get; set; }
    }
}
