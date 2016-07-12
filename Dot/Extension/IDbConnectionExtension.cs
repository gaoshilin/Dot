using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Dynamic;

namespace Dot.Extension
{
    public static class IDbConnectionExtension
    {
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> PROPERTIES = new ConcurrentDictionary<Type, List<PropertyInfo>>();

        public static long Insert(this IDbConnection connection, dynamic data, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var properties = GetPropertyNames(obj);
            var columns = string.Join(",", properties);
            var parameters = string.Join(",", properties.Select(p => "@" + p));
            var sql = "INSERT INTO [{0}] ({1}) VALUES ({2}) SELECT CAST(SCOPE_IDENTITY() AS BIGINT)".FormatWith(table, columns, parameters);

            return connection.ExecuteScalar<long>(sql, obj, transaction, commandTimeout);
        }

        public static Task<long> InsertAsync(this IDbConnection connection, dynamic data, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var properties = GetPropertyNames(obj);
            var columns = string.Join(",", properties);
            var parameters = string.Join(",", properties.Select(p => "@" + p));
            var sql = string.Format("INSERT INTO [{0}] ({1}) VALUES ({2}) SELECT CAST(SCOPE_IDENTITY() AS BIGINT)", table, columns, parameters);

            return connection.ExecuteScalarAsync<long>(sql, obj, transaction, commandTimeout);
        }

        public static int Update(this IDbConnection connection, dynamic data, dynamic condition, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var conditionObj = condition as object;

            var updatePropertyInfos = obj.GetProperties();
            var wherePropertyInfos = conditionObj.GetProperties();

            var updateProperties = updatePropertyInfos.Select(p => p.Name);
            var whereProperties = wherePropertyInfos.Select(p => p.Name);

            var updateFields = string.Join(",", updateProperties.Select(p => p + " = @" + p));
            var whereFields = string.Empty;

            if (whereProperties.Any())
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => p + " = @w_" + p));
            }

            var sql = string.Format("UPDATE [{0}] SET {1}{2}", table, updateFields, whereFields);

            var parameters = new DynamicParameters(data);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);

            return connection.Execute(sql, parameters, transaction, commandTimeout);
        }

        public static Task<int> UpdateAsync(this IDbConnection connection, dynamic data, dynamic condition, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var obj = data as object;
            var conditionObj = condition as object;

            var updatePropertyInfos = obj.GetProperties();
            var wherePropertyInfos = conditionObj.GetProperties();

            var updateProperties = updatePropertyInfos.Select(p => p.Name);
            var whereProperties = wherePropertyInfos.Select(p => p.Name);

            var updateFields = string.Join(",", updateProperties.Select(p => p + " = @" + p));
            var whereFields = string.Empty;

            if (whereProperties.Any())
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => p + " = @w_" + p));
            }

            var sql = string.Format("UPDATE [{0}] SET {1}{2}", table, updateFields, whereFields);

            var parameters = new DynamicParameters(data);
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            wherePropertyInfos.ForEach(p => expandoObject.Add("w_" + p.Name, p.GetValue(conditionObj, null)));
            parameters.AddDynamicParams(expandoObject);

            return connection.ExecuteAsync(sql, parameters, transaction, commandTimeout);
        }

        public static int Delete(this IDbConnection connection, dynamic condition, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;
            var whereFields = string.Empty;
            var whereProperties = GetPropertyNames(conditionObj);
            if (whereProperties.Count > 0)
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => p + " = @" + p));
            }

            var sql = string.Format("DELETE FROM [{0}]{1}", table, whereFields);

            return connection.Execute(sql, conditionObj, transaction, commandTimeout);
        }

        public static Task<int> DeleteAsync(this IDbConnection connection, dynamic condition, string table, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;
            var whereFields = string.Empty;
            var whereProperties = GetPropertyNames(conditionObj);
            if (whereProperties.Count > 0)
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => p + " = @" + p));
            }

            var sql = string.Format("DELETE FROM [{0}]{1}", table, whereFields);

            return connection.ExecuteAsync(sql, conditionObj, transaction, commandTimeout);
        }

        public static int GetCount(this IDbConnection connection, object condition, string table, bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryList<int>(connection, condition, table, "COUNT(*)", isOr, transaction, commandTimeout).Single();
        }

        public static Task<int> GetCountAsync(this IDbConnection connection, object condition, string table, bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryListAsync<int>(connection, condition, table, "COUNT(*)", isOr, transaction, commandTimeout).ContinueWith<int>(t => t.Result.Single());
        }

        public static IEnumerable<dynamic> QueryList(this IDbConnection connection, dynamic condition, string table, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryList<dynamic>(connection, condition, table, columns, isOr, transaction, commandTimeout);
        }

        public static Task<IEnumerable<dynamic>> QueryListAsync(this IDbConnection connection, dynamic condition, string table, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryListAsync<dynamic>(connection, condition, table, columns, isOr, transaction, commandTimeout);
        }

        public static IEnumerable<T> QueryList<T>(this IDbConnection connection, object condition, string table, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return connection.Query<T>(BuildQuerySQL(condition, table, columns, isOr), condition, transaction, true, commandTimeout);
        }

        public static Task<IEnumerable<T>> QueryListAsync<T>(this IDbConnection connection, object condition, string table, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return connection.QueryAsync<T>(BuildQuerySQL(condition, table, columns, isOr), condition, transaction, commandTimeout);
        }

        public static IEnumerable<dynamic> QueryPaged(this IDbConnection connection, dynamic condition, string table, string orderBy, int pageIndex, int pageSize, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryPaged<dynamic>(connection, condition, table, orderBy, pageIndex, pageSize, columns, isOr, transaction, commandTimeout);
        }

        public static Task<IEnumerable<dynamic>> QueryPagedAsync(this IDbConnection connection, dynamic condition, string table, string orderBy, int pageIndex, int pageSize, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return QueryPagedAsync<dynamic>(connection, condition, table, orderBy, pageIndex, pageSize, columns, isOr, transaction, commandTimeout);
        }

        public static IEnumerable<T> QueryPaged<T>(this IDbConnection connection, dynamic condition, string table, string orderBy, int pageIndex, int pageSize, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;
            var whereFields = string.Empty;
            var properties = GetPropertyNames(conditionObj);
            if (properties.Count > 0)
            {
                var separator = isOr ? " OR " : " AND ";
                whereFields = " WHERE " + string.Join(separator, properties.Select(p => p + " = @" + p));
            }
            var sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber, {0} FROM {2}{3}) AS Total WHERE RowNumber >= {4} AND RowNumber <= {5}", columns, orderBy, table, whereFields, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            return connection.Query<T>(sql, conditionObj, transaction, true, commandTimeout);
        }

        public static Task<IEnumerable<T>> QueryPagedAsync<T>(this IDbConnection connection, dynamic condition, string table, string orderBy, int pageIndex, int pageSize, string columns = "*", bool isOr = false, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var conditionObj = condition as object;
            var whereFields = string.Empty;
            var properties = GetPropertyNames(conditionObj);
            if (properties.Count > 0)
            {
                var separator = isOr ? " OR " : " AND ";
                whereFields = " WHERE " + string.Join(separator, properties.Select(p => p + " = @" + p));
            }
            var sql = string.Format("SELECT {0} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber, {0} FROM {2}{3}) AS Total WHERE RowNumber >= {4} AND RowNumber <= {5}", columns, orderBy, table, whereFields, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            return connection.QueryAsync<T>(sql, conditionObj, transaction, commandTimeout);
        }

        public static IEnumerable<TParent> QueryList<TParent, TChild, TParentKey>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            dynamic data = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null)
        {
            var param = data as object;
            var cache = new Dictionary<TParentKey, TParent>();
            Func<TParent, TChild, TParent> parentSelector = (parent, child) =>
            {
                var parentKey = parentKeySelector(parent);
                if (!cache.ContainsKey(parentKey))
                    cache.Add(parentKey, parent);
                else
                    parent = cache[parentKey];

                var children = childSelector(parent);
                if (!children.Contains(child))
                    children.Add(child);

                return parent;
            };

            connection.Query<TParent, TChild, TParent>(sql, parentSelector, param, transaction, true, splitOn, commandTimeout);

            return cache.Values;
        }

        public static IEnumerable<TParent> QueryList<TParent, TParentKey, TChild, TChildKey, TChildChild>(
            this IDbConnection connection,
            string sql,
            Func<TParent, TParentKey> parentKeySelector,
            Func<TParent, IList<TChild>> childSelector,
            Func<TChild, TChildKey> childKeySelector,
            Func<TChild, IList<TChildChild>> childChildSelector,
            dynamic param = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null)
        {
            Dictionary<TParentKey, TParent> parentCache = new Dictionary<TParentKey, TParent>();
            Dictionary<TChildKey, TChild> childCache = new Dictionary<TChildKey, TChild>();

            connection.Query<TParent, TChild, TChildChild, TParent>(
                sql,
                (parent, child, childChild) =>
                {
                    if (!parentCache.ContainsKey(parentKeySelector(parent)))
                        parentCache.Add(parentKeySelector(parent), parent);

                    TParent cachedParent = parentCache[parentKeySelector(parent)];

                    IList<TChild> children = childSelector(cachedParent);
                    TChildKey childKey = childKeySelector(child);
                    if (!childCache.ContainsKey(childKey))
                    {
                        children.Add(child);
                        childCache.Add(childKey, child);
                    }

                    IList<TChildChild> childChildren = childChildSelector(childCache[childKey]);
                    childChildren.Add(childChild);

                    return cachedParent;
                },
                param as object, transaction, true, splitOn, commandTimeout);

            return parentCache.Values;
        }

        #region Util methods

        private static string BuildQuerySQL(dynamic condition, string table, string selectPart = "*", bool isOr = false)
        {
            var conditionObj = condition as object;
            var properties = GetPropertyNames(conditionObj);
            if (properties.Count == 0)
                return string.Format("SELECT {1} FROM [{0}]", table, selectPart);

            var separator = isOr ? " OR " : " AND ";
            var wherePart = string.Join(separator, properties.Select(p => p + " = @" + p));

            var sql = string.Format("SELECT {2} FROM [{0}] WHERE {1}", table, wherePart, selectPart);
            return sql;
        }

        private static List<string> GetPropertyNames(object obj)
        {
            if (obj == null)
                return new List<string>();

            if (obj is DynamicParameters)
                return (obj as DynamicParameters).ParameterNames.ToList();

            return obj.GetProperties().Select(x => x.Name).ToList();
        }

        #endregion
    }
}