using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper.Contrib.Extensions;
using System;
using casa_benjamin.Helpers;
using casa_benjamin.Modules.Shared.Values;

namespace casa_benjamin.Modules.Shared.Repositories
{
    public class GenericRepository: IGenericRepository
    {
        public long Insert<T>(T entity) where T : class
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Insert(entity);
            }
        }

        public bool Delete<T>(T entity) where T : class
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Delete(entity);
            }
        }

        public bool Update<T>(T entity) where T : class
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Update(entity);
            }
        }

        public List<T> Get<T>(string query) where T : class
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<T>(query).ToList();
            }
        }

        public List<T> GetAll<T>() where T : class
        {      
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.GetAll<T>().ToList();
            }
        }

        public T GetOne<T>(string query) where T : class
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<T>(query).FirstOrDefault();
            }
        }

        public List<T> GetStruct<T>(string query) where T : struct
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<T>(query).ToList();
            }
        }

        public object ExecuteScalar(string query, object param = null)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                var result = con.ExecuteScalar(query,param);
                return result;
            }
        }

        public PagedTableResponse<T> GetTable<T>(PagedTableRequest req, string customSelect = null) where T : class
        {
            string select = customSelect ?? $"select * from {req.table}";
            string q = string.Empty;
         
            if (req.from.HasValue)
            {
                if (req.to.HasValue)
                {
                    q += $" where DATE({req.dateField}) between '{req.from.Value.ToMySqlDateString()}' and '{req.to.Value.ToMySqlDateString()}'";
                }
                else
                {
                    q += $" where DATE({req.dateField}) = '{req.from.Value.ToMySqlDateString()}'";
                }

                if (!string.IsNullOrEmpty(req.predicate))
                {
                    q += $" and {req.predicate}";
                }

                if (!string.IsNullOrEmpty(req.search))
                {
                    q += $" and {req.sortBy} like '%{req.search}%'";
                }
            }
            else if (!string.IsNullOrEmpty(req.predicate))
            {
                q += $" where {req.predicate}";
            }
            else if (!string.IsNullOrEmpty(req.search))
            {
                q += $" where {req.sortBy} like '%{req.search}%'";
            }
                    

            if (!string.IsNullOrEmpty(req.groupBy))
            {
                q += $" group by {req.groupBy}";
            }

            if (!string.IsNullOrEmpty(req.sortBy))
            {
                string dir = req.sortDesc ? "desc" : "asc";
                q += $" order by {req.sortBy} {dir}";
            }

            string limit = (req.start == 0 && req.length == 0) ? "": $" limit {req.start},{req.length}";
            string countQuery = $"select count(*) from ({select + q}) tbl";
            
            var result = new PagedTableResponse<T>();
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                result.data = con.Query<T>(select + q + limit).ToList();
                result.recordsTotal = Convert.ToInt32(con.ExecuteScalar(countQuery));
                result.recordsFiltered = result.recordsTotal;
            }

            return result;
        }    
    }

    public interface IGenericRepository
    {
        long Insert<T>(T entity) where T : class;
        bool Delete<T>(T entity) where T : class;
        bool Update<T>(T entity) where T : class;
        List<T> Get<T>(string query) where T : class;  
        List<T> GetAll<T>() where T : class;  
        T GetOne<T>(string query) where T : class;  
        List<T> GetStruct<T>(string query) where T : struct;
        object ExecuteScalar(string query, object param = null);
        PagedTableResponse<T> GetTable<T>(PagedTableRequest req, string customSelect = null) where T : class;
    }
}