using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SPDocumentWcfService.SysHelper
{
    public class FileHelper
    {
        #region Filed
        private string fileName;
        StreamReader ts;
        StreamWriter ws;
        private bool opened, writeOpened;
        #endregion

        #region  构造函数
        //-----------
        public FileHelper()
        {
            init();
        }
        //-----------
        private void init()
        {
            opened = false;
            writeOpened = false;
        }
        //-----------
        public FileHelper(string file_name)
        {
            fileName = file_name;
            init();
        }
        #endregion

        #region 一般文件操作
        //-----------
        public bool OpenForRead(string file_name)
        {
            fileName = file_name;
            try
            {
                ts = new StreamReader(fileName);
                opened = true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            return true;
        }
        //-----------
        public bool OpenForRead()
        {
            return OpenForRead(fileName);
        }
        //-----------
        public string ReadLine()
        {
            return ts.ReadLine();
        }
        //-----------
        public void WriteLine(string s)
        {
            ws.WriteLine(s);
        }
        //-----------
        public void close()
        {
            if (opened)
                ts.Close();
            if (writeOpened)
                ws.Close();
        }
        //-----------
        public bool OpenForWrite()
        {
            return OpenForWrite(fileName);
        }
        //-----------
        public bool OpenForWrite(string file_name)
        {
            try
            {
                ws = new StreamWriter(file_name);
                fileName = file_name;
                writeOpened = true;
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }
        #endregion

        #region 文件夹处理方法

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="strDir">目录路径</param>
        public static void CreateDir(string strDir)
        {
            //先判断目录是否存在，不存在就创建
            if (System.IO.Directory.Exists(strDir) == false)
            {
                System.IO.Directory.CreateDirectory(strDir);
            }
        }

        #endregion

        #region 文件读操作
        public static string Read(string sFile)
        {
            return Read(sFile, "gb2312");
        }
        public static string Read(string sFile, string sCoding)
        {
            Encoding code = Encoding.GetEncoding(sCoding);
            // 读取文件 
            StreamReader sr = null;
            string strContent = "";
            try
            {
                sr = new StreamReader(sFile, code);
                strContent = sr.ReadToEnd(); // 读取文件 
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return strContent;
        }
        #endregion 文件读操作

        #region 文件写操作
        public static void Write(string sPath, string sContent)
        {
            Write(sPath, sContent, "gb2312");
        }
        public static void Write(string sPath, string sContent, string sCoding)
        {
            // 写文件 
            StreamWriter sw = null;
            Encoding code = Encoding.GetEncoding(sCoding);
            try
            {

                FileInfo f = new FileInfo(sPath);

                if (f.Exists)
                {
                    FileAttributes oldAttributes = f.Attributes;
                    f.Attributes = FileAttributes.Normal;
                    f.Delete();

                    sw = new StreamWriter(sPath, false, code);
                    sw.Write(sContent);
                    sw.Flush();
                    sw.Close();
                    f = new FileInfo(sPath);
                    f.Attributes = oldAttributes;
                }
                else
                {
                    sw = new StreamWriter(sPath, false, code);
                    sw.Write(sContent);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(sPath + Ex.Message);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
        #endregion 文件写操作

        #region 删除文件
        public static void Delete(string sPath)
        {
            try
            {
                FileInfo f = new FileInfo(sPath);
                if (f.Exists)
                {
                    f.Attributes = FileAttributes.Normal;
                    f.Delete();
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        #endregion 删降文件

        #region
        public static void Create(string sPath)
        {
            FileStream fs = null;
            try
            {
                fs = File.Create(sPath);
                fs.Close();
            }
            catch
            { }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        #endregion
    }
}