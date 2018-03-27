using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SPDocumentWcfService.SysHelper
{
    public static class CString
    {
        #region 常规字符串操作
        // 检查字符串是否为空
        public static bool IsEmpty(string str)
        {
            if (str == null || str == "")
                return true;
            else
                return false;
        }
        //检查字符串中是否包含非法字符
        public static bool CheckValidity(string s)
        {
            string str = s;
            if (str.IndexOf("'") > 0 || str.IndexOf("&") > 0 || str.IndexOf("%") > 0 || str.IndexOf("+") > 0 || str.IndexOf("\"") > 0 || str.IndexOf("=") > 0 || str.IndexOf("!") > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 把价格精确至小数点两位
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TransformPrice(double dPrice)
        {
            double d = dPrice;
            NumberFormatInfo myNfi = new NumberFormatInfo();
            myNfi.NumberNegativePattern = 2;
            string s = d.ToString("N", myNfi);
            return s;
        }

        public static string TransToStr(float f, int iNum)
        {
            float fl = f;
            NumberFormatInfo myNfi = new NumberFormatInfo();
            myNfi.NumberNegativePattern = iNum;
            string s = f.ToString("N", myNfi);
            return s;
        }
        //截取长度,num是英文字母的总数，一个中文算两个英文
        public static string GetLetter(string str, int iNum, bool bAddDot)
        {
            string sContent = "";
            int iTmp = iNum;
            if (str == null)
                return sContent;
            else
                sContent = str;
            if (sContent.Length > 0)
            {
                if (iTmp > 0)
                {
                    if (sContent.Length * 2 > iTmp) //说明字符串的长度可能大于iNum,否则显示全部
                    {
                        char[] arrC;
                        if (sContent.Length >= iTmp) //防止因为中文的原因使ToCharArray溢出
                        {
                            arrC = str.ToCharArray(0, iTmp);
                        }
                        else
                        {
                            arrC = str.ToCharArray(0, sContent.Length);
                        }
                        int k = 0;
                        int i = 0;
                        int iLength = 0;
                        foreach (char ch in arrC)
                        {
                            iLength++;
                            k = (int)ch;
                            if (k < 0)
                            {
                                k = 65536;
                            }
                            if (k > 255)
                            {
                                //i = i + 2;
                                i++;
                            }
                            else
                            {
                                i++;
                                //iLength++;
                            }
                            if (i >= iTmp)
                                break;
                        }
                        if (bAddDot)
                            sContent = sContent.Substring(0, iLength) + "...";
                        else
                            sContent = sContent.Substring(0, iLength);
                        sContent = sContent + "";
                    }
                }
            }
            return sContent;
        }
        //根据指定字符，截取相应字符串
        public static string GetStrByLast(string sOrg, string sLast)
        {
            int iLast = sOrg.LastIndexOf(sLast);
            if (iLast > 0)
                return sOrg.Substring(iLast + 1);
            else
                return sOrg;
        }
        public static string GetPreStrByLast(string sOrg, string sLast)
        {
            int iLast = sOrg.LastIndexOf(sLast);
            if (iLast > 0)
                return sOrg.Substring(0, iLast);
            else
                return sOrg;
        }
        #endregion  常规字符串操作

        #region HTML相关操作
        public static string ClearTag(string sHtml)
        {
            if (sHtml == "")
                return "";
            string sTemp = sHtml;
            Regex re = new Regex(@"(<[^>\s]*\b(\w)+\b[^>]*>)|([\s]+)|(<>)|(&nbsp;)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return re.Replace(sHtml, "");
        }
        public static string ClearTag(string sHtml, string sRegex)
        {
            string sTemp = sHtml;
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return re.Replace(sHtml, "");
        }
        public static string ConvertToJS(string sHtml)
        {
            StringBuilder sText = new StringBuilder();
            Regex re;
            re = new Regex(@"\r\n", RegexOptions.IgnoreCase);
            string[] strArray = re.Split(sHtml);
            foreach (string strLine in strArray)
            {
                sText.Append("document.writeln(\"" + strLine.Replace("\"", "\\\"") + "\");\r\n");
            }
            return sText.ToString();
        }
        public static string ReplaceNbsp(string str)
        {
            string sContent = str;
            if (sContent.Length > 0)
            {
                sContent = sContent.Replace(" ", "");
                sContent = sContent.Replace("&nbsp;", "");
                sContent = "&nbsp;&nbsp;&nbsp;&nbsp;" + sContent;
            }
            return sContent;
        }
        public static string StringToHtml(string str)
        {
            string sContent = str;
            if (sContent.Length > 0)
            {
                char csCr = (char)13;
                sContent = sContent.Replace(csCr.ToString(), "<br>");
                sContent = sContent.Replace("\n", "");
                sContent = sContent.Replace(" ", "&nbsp;");
                sContent = sContent.Replace("　", "&nbsp;&nbsp;");
                sContent = sContent.Replace("\"", "＂");
            }
            return sContent;
        }

        //截取长度并转换为HTML
        public static string AcquireAssignString(string str, int num)
        {
            string sContent = str;
            sContent = GetLetter(sContent, num, false);
            sContent = StringToHtml(sContent);
            return sContent;
        }

        //此方法与AcquireAssignString的功能已经一样，为了不报错，故保留此方法
        public static string TranslateToHtmlString(string str, int num)
        {
            string sContent = str;
            sContent = GetLetter(sContent, num, false);
            sContent = StringToHtml(sContent);
            return sContent;
        }

        public static string AddBlankAtForefront(string str)
        {
            string sContent = str;
            return sContent;
        }

        /// <summary>
        /// 方法名称：ToBreakWord
        /// 内容摘要：在长字符串中加入换行符，使其在浏览器中能自动换行
        /// </summary>
        /// <param name="strContent">要显示的字符串</param>
        /// <param name="length">每行显示的长度</param>
        /// <returns>转换后的内容</returns>
        public static string ToHTMLBreakWord(string strContent, int length)
        {
            //如果为空，则返回空字符串
            if (strContent == null)
            {
                return String.Empty;
            }
            //如果长度不够，则直接返回
            if (strContent.Length <= length)
            {
                return strContent;
            }
            string strTemp = String.Empty;
            //如果足够长，则在其中加入空格
            while (strContent.Length > length)
            {
                strTemp += strContent.Substring(0, length) + "<br />";
                strContent = strContent.Substring(length, strContent.Length - length);
            }
            strTemp += " " + strContent;
            return strTemp;
        }
        #endregion HTML相关操作

        #region 其他字符串操作

        /// <summary>
        /// 格式化为版本号字符串
        /// </summary>
        /// <param name="sVersion"></param>
        /// <returns></returns>
        public static string SetVersionFormat(string sVersion)
        {
            if (sVersion == null || sVersion == "") return "";
            int n = 0, k = 0;

            string stmVersion = "";
            while (n < 4 && k > -1)
            {
                k = sVersion.IndexOf(".", k + 1);
                n++;
            }
            if (k > 0)
            {
                stmVersion = sVersion.Substring(0, k);
            }
            else
            {
                stmVersion = sVersion;
            }

            return stmVersion;
        }
        /// <summary>
        /// 格式化字符串为 SQL 语句字段
        /// </summary>
        /// <param name="fldList"></param>
        /// <returns></returns>
        public static string GetSQLFildList(string fldList)
        {
            if (fldList == null)
                return "*";
            if (fldList.Trim() == "")
                return "*";
            if (fldList.Trim() == "*")
                return "*";
            //先去掉空格，[]符号
            string strTemp = fldList;
            strTemp = strTemp.Replace(" ", "");
            strTemp = strTemp.Replace("[", "").Replace("]", "");
            //为防止使用保留字，给所有字段加上[]
            strTemp = "[" + strTemp + "]";
            strTemp = strTemp.Replace('，', ',');
            strTemp = strTemp.Replace(",", "],[");
            return strTemp;
        }

        /// <summary>
        /// 格式化SQL语句里面的特殊字符
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public static string GetSQLSpecialString(string strSQL)
        {
            if (string.IsNullOrEmpty(strSQL))
                return strSQL;

            string strTemp = string.Empty;
            strTemp = strSQL;
            strTemp = strTemp.Replace("^^", "[[][^][^]]");
            strTemp = strTemp.Replace("[]", "[[]]");
            strTemp = strTemp.Replace("_", "[_]");
            strTemp = strTemp.Replace("%", "[%]");
            strTemp = strTemp.Replace("[", "[[]");
            strTemp = strTemp.Replace("^", "[[][^]]");

            return strTemp;
        }

        #endregion 其他字符串操作

        #region 判断字符串能否转换成数字的方法
        /// <summary>
        /// 判断字符串能否转换成数字
        /// </summary>
        /// <param name="strObj">字符</param>
        /// <returns></returns>
        public static bool IsNumeric(string strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                objNumberPattern.IsMatch(strNumber);
        }
        #endregion

        #region Xml字符的处理
        /// <summary>
        /// 将字符串里面的特殊字符转换成Xml所识别的字符
        /// </summary>
        /// <param name="strText">需要转换的字符</param>
        /// <returns></returns>
        public static string GetXmlString(string strText)
        {
            //strText = strText.Replace("<", "&lt;");
            //strText = strText.Replace(">", "&gt;");
            //strText = strText.Replace("&", "&amp;");
            //strText = strText.Replace("'", "&apos;");
            //strText = strText.Replace("\"", "&quot;");
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