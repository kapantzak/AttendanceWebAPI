using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AttendanceWebApi.Helpers
{
    
    public struct AdoResultScalar
    {
        public string Result { get; set; }
        public bool ErrorFound { get; set; }
        public string ErrorMsg { get; set; }
    }

    public struct AdoResultDataTable
    {
        public DataTable DataTable { get; set; }
        public bool ErrorFound { get; set; }
        public string ErrorMsg { get; set; }
    }

    public static class AdoHelper
    {

        /// <summary>
        /// Execute scalar query
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static AdoResultScalar ExecuteScalar(SqlCommand cmd, string connString)
        {
            var res = new AdoResultScalar()
            {
                ErrorFound = false
            };
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    cmd.Connection = conn;
                    conn.Open();
                    res.Result = cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    res.ErrorFound = true;
                    res.ErrorMsg = ex.Message;
                }
            }
            return res;
        }

        public static AdoResultDataTable GetDataTable(SqlCommand cmd, string connString)
        {
            var res = new AdoResultDataTable()
            {
                ErrorFound = false
            };
            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    cmd.Connection = conn;
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        res.DataTable = dt;
                    }
                }
                catch (SqlException ex)
                {
                    res.ErrorFound = true;
                    res.ErrorMsg = ex.Message;
                }
                finally
                {
                    cmd.Connection.Close();
                }
            }
            return res;
        }

        /// <summary>
        /// Get next id for specific table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connString"></param>
        /// <param name="idColumnName"></param>
        /// <returns></returns>
        public static int GetNextId(string tableName, string connString, string idColumnName = "ID")
        {
            string query = @"DECLARE @TableName nvarchar(50), @ColName nvarchar(50);
                            SET @TableName = @Set_TableName;
                            SET @ColName = @Set_ColName;
                            EXEC('SELECT MAX(' + @ColName + ') + 1 AS NextID FROM ' + @TableName);";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@Set_TableName", tableName);
            cmd.Parameters.AddWithValue("@Set_ColName", idColumnName);
            var sdt = AdoHelper.ExecuteScalar(cmd, connString);
            int id;
            if (!sdt.ErrorFound && int.TryParse(sdt.Result, out id))
            {
                return id;
            }
            return 0;
        }

    }
}
