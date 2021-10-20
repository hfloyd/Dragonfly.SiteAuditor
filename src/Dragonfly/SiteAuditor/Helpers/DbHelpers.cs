namespace Dragonfly.SiteAuditor.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Dragonfly.SiteAuditor.Models.Dtos;
    using Umbraco.Core;
    using Umbraco.Core.Persistence;

    //static partial class Constants
    //{
    //    //public static class DbProviderNames
    //    //{
    //    //    public const string SqlServer = "System.Data.SqlClient";
    //    //    public const string SqlCe = "System.Data.SqlServerCe.4.0";
    //    //}
    //}

    internal static class DbHelper
    {

        public static Dictionary<string, string> GetDataTypeConfig(int DataTypeNodeId)
        {
            var configDict = new Dictionary<string, string>();

            var db = ApplicationContext.Current.DatabaseContext.Database;
            //var sql = $"SELECT [alias],[value] FROM [cmsDataTypePreValues] WHERE [datatypeNodeId]={DataTypeNodeId};";
            var sqlWhere = $"[datatypeNodeId]={DataTypeNodeId}";
            var data = db.Fetch<UmbDataTypePrevalue>(new Sql().Select("*").From(UmbDataTypePrevalue.TableName)
                .Where(sqlWhere));

            foreach (var pv in data)
            {
                configDict.Add(pv.Alias, pv.Value);
            }

            return configDict;
        }

        //public static string GetPropertyGroupName(int DocTypeId, string PropertyAlias)
        //{
        //    var db = ApplicationContext.Current.DatabaseContext.Database;
            
        //    var sqlWhere = $"[contenttypeNodeId]={DocTypeId}";
        //    var dataGroups = db.Fetch<UmbPropertyGroup>(new Sql().Select("*").From(UmbPropertyGroup.TableName)
        //        .Where(sqlWhere));


        //}

        public static List<UmbPropertyGroup> GetAllPropertiesRaw()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            //var sqlWhere = $"[contenttypeNodeId]={DocTypeId}";
            var data = db.Fetch<UmbPropertyGroup>(new Sql().Select("*").From(UmbPropertyGroup.TableName));

            return data;
        }



        #region Unnecessary DB access low-level code
        //internal enum DbProviderName
        //{
        //    SqlCe,
        //    SqlServer
        //}

        //public static string GetDbProviderNameString(DbProviderName Enum)
        //{
        //    switch (Enum)
        //    {
        //        case DbProviderName.SqlCe:
        //            return Constants.DbProviderNames.SqlCe;
        //            break;

        //        case DbProviderName.SqlServer:
        //            return Constants.DbProviderNames.SqlServer;
        //            break;
        //        default:
        //            return "";
        //    }
        //}

        //public static string GetConnectionString()
        //{
        //    return ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ToString();
        //}

        //public static DbConnection GetSqlConnection()
        //{
        //    var conString = GetConnectionString();
        //    var providerName = DetectProviderNameFromConnectionString(conString);
        //    var factory = DbProviderFactories.GetFactory(GetDbProviderNameString(providerName));
        //    var connection = factory.CreateConnection();
        //    connection.ConnectionString = conString;

        //    return connection;
        //}

        //public static DbProviderName DetectProviderNameFromConnectionString(string connectionString)
        //{
        //    var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
        //    var allKeys = builder.Keys.Cast<string>();

        //    if (allKeys.InvariantContains("Data Source")
        //        //this dictionary is case insensitive
        //        && builder["Data source"].ToString().InvariantContains(".sdf"))
        //    {
        //        return DbProviderName.SqlCe;
        //    }

        //    return DbProviderName.SqlServer;
        //}

        #endregion
    }
}

