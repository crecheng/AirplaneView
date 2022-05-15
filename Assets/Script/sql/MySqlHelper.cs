/*using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace Script
{
    public static class MySqlHelper
    {
        public static string Server = "127.0.0.1";

        public static string UserId = "root";

        public static string Password = "123456";

        public static string Database = "airportdata";
        

        public static MySqlConnection Connection = null;

        public static void Close()
        {
            if(!(Connection != null && Connection.State == ConnectionState.Connecting))
                return;
            try
            {
                Connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static async Task<bool> ConnectionSql()
        {
            if (Connection != null && Connection.State == ConnectionState.Connecting)
                return true;
            string constructorString =
                $"server={Server};user={UserId};password={Password};Database={Database};port=3306;charset=utf8";
            Connection = new MySqlConnection(constructorString);

            await Task.Run(() =>
            {
                try
                {
                    Connection.Open();
                }
                catch (Exception e)
                {
                    Connection = null;
                    Console.WriteLine(e);
                }
            });

            return Connection != null;
        }

        public static async Task<bool>  CheckConnection()
        {
            if(Connection != null && Connection.State == ConnectionState.Open)
            {
                return true;
            }
            
            if(Connection != null && Connection.State != ConnectionState.Open)
            {
                Close();
                Connection = null;
            }
            
            if (Connection == null)
            {
                await ConnectionSql();
                if (Connection == null)
                {
                    return false;
                }
                else
                    return true;
            }
            return false;
        }

        public static async Task<int> Insert<T>(IList<T> dataList,string tableName=null) where T : IMysqlData
        {
            var check = await CheckConnection();
            if (!check)
                return -1;
            if (dataList == null || dataList.Count <= 0)
                return -1;

            int count = 0;
            var mycmd = new MySqlCommand("", Connection);
            await Task.Run(() =>
            {
                tableName ??= typeof(T).Name;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("INSERT INTO ");
                stringBuilder.Append(tableName.ToLower());
                stringBuilder.Append("(").Append(dataList[0].GetTableHead()).Append(")")
                    .Append(" VALUES ");
                
                int i = 0;
                
                StringBuilder value = new StringBuilder();
                foreach (var data in dataList)
                {
                    value.Append("(").Append(data.GetInsertString()).Append("),");
                    i++;
                    if (i >= 100)
                    {
                        value.Remove(value.Length - 1, 1);
                        value.Insert(0, stringBuilder.ToString()).Append(";");
                        mycmd.CommandText = value.ToString();
                        count += mycmd.ExecuteNonQuery();
                        value.Clear();
                        i = 0;
                    }
                }

                if (i != 0)
                {
                    value.Remove(value.Length - 1, 1);
                    value.Insert(0, stringBuilder.ToString()).Append(";");
                    mycmd.CommandText = value.ToString();
                    count += mycmd.ExecuteNonQuery();
                    /*count += InsertCmd(value.ToString());#1#
                }
            });
            
            mycmd.Dispose();
            return count;
        }

        public static async Task<bool> Insert<T>(T data, string tableName = null) where T : IMysqlData
        {
            var check = await CheckConnection();
            if (!check)
                return false;
            if (data == null )
                return false;

            tableName ??= typeof(T).Name;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("INSERT INTO ");
            stringBuilder.Append(tableName.ToLower());
            stringBuilder.Append("(").Append(data.GetTableHead()).Append(")")
                .Append(" VALUES ").Append("(").Append(data.GetInsertString()).Append(");");;

            return await InsertCmd(stringBuilder.ToString())>0;
        }

        public static async Task<int> InsertCmd(string sqlCmd)
        {
            var check = await CheckConnection();
            if (!check)
                return -1;
            MySqlCommand myCmd = new MySqlCommand(sqlCmd, Connection);
            int i= await myCmd.ExecuteNonQueryAsync();
            myCmd.Dispose();
            return i;
        }

        public static async Task<List<T>> Select<T>(string cmd,int limit=500,int offset=0, string tableName = null) where T : IMysqlData,new()
        {
            var check = await CheckConnection();
            if (!check)
                return null;

            List<T> result = new List<T>();
            await Task.Run(() =>
            {
                tableName ??= typeof(T).Name.ToLower();
                T tmp = new T();
                string commitCmd =
                    $"select {tmp.GetTableHead(true)} from {tableName} where {cmd} limit {limit} offset {offset}";
                MySqlCommand mycmd = new MySqlCommand(commitCmd, Connection);
                var type = typeof(T);
                Dictionary<string, FieldInfo> fieldInfos = new Dictionary<string, FieldInfo>();
                var fields = type.GetFields();
                foreach (var field in fields)
                {
                    fieldInfos.Add(field.Name.ToLower(), field);
                }

                MySqlDataReader sqlResult = null;
                try
                {
                    sqlResult = mycmd.ExecuteReader();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                if (sqlResult == null)
                    return;
                while (sqlResult.Read())
                {
                    T t = new T();
                    for (int i = 0; i < sqlResult.FieldCount; i++)
                    {
                        var name = sqlResult.GetName(i);
                        if (fieldInfos.ContainsKey(name))
                        {
                            fieldInfos[name].SetValue(t, sqlResult.GetValue(i));
                        }
                    }

                    result.Add(t);
                }
            });
            // from airlinedata where arrive like '%HAK%' limit 100 offset 100;
            return result;
        }

        

    }
}*/