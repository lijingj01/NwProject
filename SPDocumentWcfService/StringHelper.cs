using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SPDocumentWcfService
{
    public static class StringHelper
    {
        #region Xml字符的处理
        /// <summary>
        /// 将字符串里面的特殊字符转换成Xml所识别的字符
        /// </summary>
        /// <param name="strText">需要转换的字符</param>
        /// <returns></returns>
        public static string GetXmlString(string strText)
        {
            strText = Regex.Replace(strText, "<", "&lt;");
            strText = Regex.Replace(strText, ">", "&gt;");
            strText = Regex.Replace(strText, "&", "&amp;");
            strText = Regex.Replace(strText, "'", "&apos;");
            strText = Regex.Replace(strText, "\"", "&quot;");
            return strText;
        }

        /// <summary>
        /// 将Xml字符串数据里面的特殊字符转换成原有的标示
        /// </summary>
        /// <param name="strXml">需要转换的字符</param>
        /// <returns></returns>
        public static string GetXmlToString(string strXml)
        {
            strXml = Regex.Replace(strXml, "&lt;", "<");
            strXml = Regex.Replace(strXml, "&gt;", ">");
            strXml = Regex.Replace(strXml, "&amp;", "&");
            strXml = Regex.Replace(strXml, "&apos;", "'");
            strXml = Regex.Replace(strXml, "&quot;", "\"");
            return strXml;
        }

        /// <summary>
        /// 将字符串里面的特殊字符转换成Xml所识别的字符
        /// </summary>
        /// <param name="strText">需要转换的字符</param>
        /// <returns></returns>
        public static string GetXmlInnerString(string strText)
        {
            strText = Regex.Replace(strText, "&", "&amp;");
            strText = Regex.Replace(strText, "'", "&apos;");
            strText = Regex.Replace(strText, "\"", "&quot;");
            return strText;
        }
        #endregion
    }
}