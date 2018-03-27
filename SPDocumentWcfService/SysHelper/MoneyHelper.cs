using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SPDocumentWcfService.SysHelper
{
    // 该类重载的 ToString() 方法返回的是大写金额字符串
    public class Money
    {

        /// <summary>
        /// “元”，可以改为“圆”、“卢布”之类
        /// </summary>
        public string Yuan = "元";
        /// <summary>
        /// “角”，可以改为“拾”
        /// </summary>
        public string Jiao = "角";
        /// <summary>
        /// “分”，可以改为“美分”之类
        /// </summary>
        public string Fen = "分";
        static string Digit = "零壹贰叁肆伍陆柒捌玖";      // 大写数字
        bool isAllZero = true;                        // 片段内是否全零
        bool isPreZero = true;                        // 低一位数字是否是零
        bool Overflow = false;                       // 溢出标志
        long money100;                                   // 金额*100，即以“分”为单位的金额
        long value;                                      // money100的绝对值
                                                         //StringBuilder sb;         // 大写金额字符串，逆序
                                                         /// <summary>
                                                         ///  只读属性: "零元"
                                                         /// </summary>
        public string ZeroString
        {
            get
            {
                return Digit[0] + Yuan;
            }
        }

        public Money()
            : this(decimal.Zero)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="money"></param>
        public Money(decimal money)
        {
            try
            {
                money100 = (long)(money * 100m);
            }
            catch
            {
                Overflow = true;
            }
            if (money100 == long.MinValue) Overflow = true;
        }
        /// <summary>
        /// 重载 ToString() 方法，返回大写金额字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Overflow) return "金额超出范围";
            if (money100 == 0) return ZeroString;
            string[] Unit = { Yuan, "万", "亿", "万", "亿亿" };
            value = System.Math.Abs(money100);
            ParseSection(true, sb);
            for (int i = 0; i < Unit.Length && value > 0; i++)
            {
                if (isPreZero && !isAllZero) sb.Append(Digit[0]);
                if (i == 4 && sb.ToString().EndsWith(Unit[2]))
                    sb.Remove(sb.Length - Unit[2].Length, Unit[2].Length);
                sb.Append(Unit[i]);
                ParseSection(false, sb);
                if ((i % 2) == 1 && isAllZero)
                    sb.Remove(sb.Length - Unit[i].Length, Unit[i].Length);
            }
            if (money100 < 0) sb.Append("负");
            return Reverse(sb);
        }
        /// <summary>
        /// 解析“片段”: “角分(2位)”或“万以内的一段(4位)”
        /// </summary>
        /// <param name="isJiaoFen"></param>
        private void ParseSection(bool isJiaoFen, StringBuilder sb)
        {
            string[] Unit = isJiaoFen ?
              new string[] { Fen, Jiao } :
              new string[] { "", "拾", "佰", "仟" };
            isAllZero = true;
            for (int i = 0; i < Unit.Length && value > 0; i++)
            {
                int d = (int)(value % 10);
                if (d != 0)
                {
                    if (isPreZero && !isAllZero) sb.Append(Digit[0]);
                    sb.AppendFormat("{0}{1}", Unit[i], Digit[d]);
                    isAllZero = false;
                }
                isPreZero = (d == 0);
                value /= 10;
            }
        }
        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <returns></returns>
        private string Reverse(StringBuilder sb)
        {
            StringBuilder sbReversed = new StringBuilder();
            for (int i = sb.Length - 1; i >= 0; i--)
                sbReversed.Append(sb[i]);
            return sbReversed.ToString();
        }
    }
}