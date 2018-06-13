using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Data;

namespace EkiDataBase
{
    public class DataBaseConnection
    {
        static OleDbConnection _conn;
        static string _dbpath;
        static bool _isEncrypted;
        static bool _isNewFormat;
        public DataBaseConnection(string path = "", bool isNewFormat = true, bool isEncrypted = true)
        {
            _isEncrypted = isEncrypted;
            _isNewFormat = isNewFormat;
            if (path != "")
            {
                _dbpath = path;
            }
            else
            {
                _dbpath = Environment.CurrentDirectory + "\\database\\rs.edb";
            }
            if (!File.Exists(_dbpath))
            {
                CreateDataBaseFile(_isNewFormat);
            }
        }
        public void Open(string password = "")
        {
            if(_isEncrypted)
            {
                Decrypt();
            }
            _conn = new OleDbConnection();
            string providerString = "";
            if(_isNewFormat)
            {
                providerString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
            }
            else
            {
                providerString = "Provider=Microsoft.ACE.OLEDB.4.0;Data Source=";
            }
            _conn.ConnectionString = providerString + _dbpath + ";Persist Security Info=True;";
            if (password != "") { _conn.ConnectionString += "Jet Oledb:DataBase Password=" + password + ";"; }
            _conn.Open();
        }
        public void Close()
        {
            _conn.Close();
            if(_isEncrypted)
            {
                Encrypt();
            }
        }
        void CreateDataBaseFile(bool isNewFormat)
        {

        }
        //读取记录，返回DataTable类型
        public DataTable Select(string sql)
        {
            OleDbDataAdapter da = new OleDbDataAdapter(sql, _conn);
            DataSet ds = new DataSet();
            da.Fill(ds, "Details");
            return ds.Tables["Details"];
        }
        //读取记录，返回OleDbDataReader类型
        public OleDbDataReader Read(string sql)
        {
            return new OleDbCommand(sql, _conn).ExecuteReader();
        }
        //运行SQL语句，不返回结果
        public void Run(string sql)
        {
            OleDbCommand comm = new OleDbCommand(sql, _conn);
            comm.ExecuteNonQuery();
        }
        //加密、解密数据库
        public void Encrypt(string dbpath = "")
        {
            if(_isNewFormat)
            {
                WriteToDataBase(new byte[] { 0, 2, 0, 1, 83, 117, 97, 111, 100, 98, 114, 101, 32, 66, 67, 70 }, dbpath);
            }
            else
            {
                WriteToDataBase(new byte[] { 0, 2, 0, 1, 83, 117, 97, 111, 100, 98, 114, 101, 32, 75, 101, 117 }, dbpath);
            }
        }
        void Decrypt()
        {
            if (_isNewFormat)
            {
                WriteToDataBase(new byte[] { 0, 1, 0, 0, 83, 116, 97, 110, 100, 97, 114, 100, 32, 65, 67, 69 });
            }
            else
            {
                WriteToDataBase(new byte[] { 0, 1, 0, 0, 83, 116, 97, 110, 100, 97, 114, 100, 32, 74, 101, 116 });
            }
        }
        //加密、解密具体操作
        void WriteToDataBase(byte[] b, string dbpath = "")
        {
            FileStream fs;
            if (dbpath == "")
            {
                fs = new FileStream(_dbpath, FileMode.Open);
            }
            else
            {
                fs = new FileStream(dbpath, FileMode.Open);
            }
            byte[] dbbytes = new byte[fs.Length];
            fs.Read(dbbytes, 0, (int)fs.Length);
            Array.Copy(b, dbbytes, 16);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(dbbytes, 0, (int)fs.Length);
            fs.Flush();
            fs.Close();
        }
        //返回所使用的数据库类型
        public string BasedOn()
        {
            return "Access";
        }
    }
}
