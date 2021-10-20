using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.SiteAuditor.Models.Dtos
{
    using Umbraco.Core.Persistence;

    //CREATE TABLE [cmsPropertyType] (
    //  [id] int IDENTITY (50,1) NOT NULL
    //, [dataTypeId] int NOT NULL
    //, [contentTypeId] int NOT NULL
    //, [propertyTypeGroupId] int NULL
    //, [Alias] nvarchar(255) NOT NULL
    //, [Name] nvarchar(255) NULL
    //, [sortOrder] int DEFAULT (('0')) NOT NULL
    //, [mandatory] bit DEFAULT (('0')) NOT NULL
    //, [validationRegExp] nvarchar(255) NULL
    //, [Description] nvarchar(2000) NULL
    //, [UniqueID] uniqueidentifier DEFAULT ((NEWID())) NOT NULL
    //);

    [TableName("cmsPropertyType")]
    [PrimaryKey("id")]
    class UmbPropertyType
    {
        public const string TableName = "cmsPropertyType";

        [Column("id")]
        public int Id { get; set; }

        [Column("dataTypeId")]
        public int DataTypeId { get; set; }

        [Column("contentTypeId")]
        public int ContentTypeId { get; set; }

        [Column("propertyTypeGroupId")]
        public int PropertyTypeGroupId { get; set; }

        [Column("Alias")]
        public string Alias { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("sortorder")]
        public int SortOrder { get; set; }

        [Column("validationRegExp")]
        public string ValidationRegExp { get; set; }

        [Column("Description")]
        public string Description { get; set; }


        [Column("uniqueID")]
        public Guid UniqueID { get; set; }
    }
}
