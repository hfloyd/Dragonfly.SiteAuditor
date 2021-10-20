using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.SiteAuditor.Models.Dtos
{
    using Umbraco.Core.Persistence;

    //CREATE TABLE [cmsPropertyTypeGroup] (
    //[id] int IDENTITY(12,1) NOT NULL
    //, [contenttypeNodeId] int NOT NULL
    //, [text] nvarchar(255) NOT NULL
    //, [sortorder] int NOT NULL
    //, [uniqueID] uniqueidentifier DEFAULT((NEWID())) NOT NULL
    //);

    [TableName("cmsPropertyTypeGroup")]
    [PrimaryKey("id")]
    class UmbPropertyGroup
    {
        public const string TableName = "cmsPropertyTypeGroup";

        [Column("id")]
        public int Id { get; set; }

        [Column("contenttypeNodeId")]
        public int ContentTypeNodeId { get; set; }

        [Column("text")]
        public string GroupName { get; set; }

        [Column("sortorder")]
        public int SortOrder { get; set; }

        [Column("uniqueID")]
        public Guid UniqueID { get; set; }
    }
}
