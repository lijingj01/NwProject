using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace SPDocumentWcfService
{
    /// <summary>
    /// 发送邮件的相关操作方法类
    /// </summary>
    public static class EMailHelper
    {
        #region 属性

        /// <summary>
        /// 邮件默认发送人
        /// </summary>
        private static string MailFromUser = System.Configuration.ConfigurationManager.AppSettings["MailFromUser"];
        /// <summary>
        /// 邮件默认发送人密码
        /// </summary>
        private static string MailFromUserPwd = System.Configuration.ConfigurationManager.AppSettings["MailFromUserPwd"];
        /// <summary>
        /// 邮件的后缀
        /// </summary>
        private static string MailUrl = System.Configuration.ConfigurationManager.AppSettings["MailUrl"];
        /// <summary>
        /// 邮件的服务器地址
        /// </summary>
        private static string MailServer = System.Configuration.ConfigurationManager.AppSettings["MailServer"];

        /// <summary>
        /// 系统管理员账号
        /// </summary>
        private static string SystemAdmin = System.Configuration.ConfigurationManager.AppSettings["SystemAdmin"];
        /// <summary>
        /// 当前系统是否是测试系统
        /// </summary>
        private static bool SystemIsTest = System.Configuration.ConfigurationManager.AppSettings["IsTestVersion"] == "1";
        #endregion

        #region 邮件发送方法

        /// <summary>
        /// 发送邮件给相应的用户
        /// </summary>
        /// <param name="strFrom">发送人</param>
        /// <param name="strTo">收件人</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件内容</param>
        public static void FromSendMail(string strFrom, string strTo, string strTitle, string strBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(strFrom) & !string.IsNullOrEmpty(strTo))
                {
                    string strFromMail = strFrom + MailUrl;
                    string strToUser = strTo + MailUrl;
                    #region 测试系统发邮件都默认给管理员
                    string[] strSystemAdmin = SystemAdmin.Split(';');
                    if (SystemIsTest)
                    {
                        strToUser = strSystemAdmin[0] + MailUrl;
                        strTitle = "[测试]" + strTitle;
                    }
                    #endregion

                    MailMessage mail = new MailMessage(strFromMail, strToUser, strTitle, strBody);
                    #region 增加邮件密送给管理员操作
                    foreach (string strAdmin in strSystemAdmin)
                    {
                        mail.Bcc.Add(new MailAddress(strAdmin + MailUrl));
                    }
                    #endregion
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(MailServer);
                    smtp.Credentials = new System.Net.NetworkCredential(strFrom, MailFromUserPwd);
                    //string userState = "邮件发送完成！";
                    //smtp.SendAsync(mail, userState);
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 发送邮件给相应的用户
        /// </summary>
        /// <param name="strToList">收件人集合</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件内容</param>
        public static void SendMailList(List<string> strToList, string strTitle, string strBody)
        {
            try
            {
                if (strToList.Count > 0)
                {
                    string strFromMail = MailFromUser + MailUrl;
                    //string strToUser = strTo + MailUrl;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(strFromMail);
                    mail.Subject = strTitle;
                    mail.Body = strBody;
                    mail.IsBodyHtml = true;
                    string[] strSystemAdmin = SystemAdmin.Split(';');
                    foreach (string strTo in strToList)
                    {
                        if (!string.IsNullOrEmpty(strTo))
                        {
                            #region 测试系统发邮件都默认给管理员
                            if (SystemIsTest)
                            {
                                mail.To.Add(strSystemAdmin[0] + MailUrl);
                                strTitle = "[测试]" + strTitle;
                            }
                            else
                            {
                                mail.To.Add(strTo + MailUrl);
                            }
                            #endregion
                        }
                    }
                    #region 增加邮件密送给管理员操作
                    foreach (string strAdmin in strSystemAdmin)
                    {
                        mail.Bcc.Add(new MailAddress(strAdmin + MailUrl));
                    }
                    #endregion

                    SmtpClient smtp = new SmtpClient(MailServer);

                    smtp.Credentials = new System.Net.NetworkCredential(MailFromUser, MailFromUserPwd);
                    //string userState = "邮件发送完成！";
                    //smtp.SendAsync(mail, userState);
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 管理员发送邮件给相应的用户
        /// </summary>
        /// <param name="strTo">收件的用户名</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件内容</param>
        public static void SendMail(string strTo, string strTitle, string strBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(strTo))
                {
                    string strFromMail = MailFromUser + MailUrl;
                    string strToUser = strTo + MailUrl;

                    #region 测试系统发邮件都默认给管理员
                    string[] strSystemAdmin = SystemAdmin.Split(';');
                    if (SystemIsTest)
                    {
                        strToUser = strSystemAdmin[0] + MailUrl;
                        strTitle = "[测试]" + strTitle;
                    }
                    #endregion

                    MailMessage mail = new MailMessage(strFromMail, strToUser, strTitle, strBody);
                    #region 增加邮件密送给管理员操作
                    foreach (string strAdmin in strSystemAdmin)
                    {
                        mail.Bcc.Add(new MailAddress(strAdmin + MailUrl));
                    }
                    #endregion

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(MailServer);
                    smtp.Credentials = new System.Net.NetworkCredential(MailFromUser, MailFromUserPwd);
                    //string userState = "邮件发送完成！";
                    //smtp.SendAsync(mail, userState);
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 将错误异常数据发送给管理员
        /// </summary>
        /// <param name="strTo">管理员账号</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="ex">错误内容</param>
        public static void SendExceptionToAdmin(string strTo, string strTitle, Exception ex)
        {
            try
            {
                if (!string.IsNullOrEmpty(strTo))
                {
                    string strFromMail = MailFromUser + MailUrl;
                    string strToUser = strTo + MailUrl;

                    StringBuilder strBody = new StringBuilder();
                    strBody.AppendFormat("Message:{0} <br />", ex.Message);
                    strBody.AppendFormat("Source:{0} <br />", ex.Source);
                    strBody.AppendFormat("StackTrace:{0} <br />", ex.StackTrace);
                    strBody.AppendFormat("TargetSite:{0} <br />", ex.TargetSite.Name);

                    MailMessage mail = new MailMessage(strFromMail, strToUser, strTitle, strBody.ToString());

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(MailServer);
                    smtp.Credentials = new System.Net.NetworkCredential(MailFromUser, MailFromUserPwd);
                    smtp.Send(mail);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 将错误异常数据发送给管理员
        /// </summary>
        /// <param name="strTo">管理员账号</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件内容</param>
        /// <param name="ex">错误内容</param>
        public static void SendExceptionToAdmin(string strTo, string strTitle, string strBody, Exception ex)
        {
            try
            {
                if (!string.IsNullOrEmpty(strTo))
                {
                    string strFromMail = MailFromUser + MailUrl;
                    string strToUser = strTo + MailUrl;

                    StringBuilder strEBody = new StringBuilder();
                    strEBody.AppendLine(strBody);
                    strEBody.AppendFormat("<br />Message:{0} <br />", ex.Message);
                    strEBody.AppendFormat("Source:{0} <br />", ex.Source);
                    strEBody.AppendFormat("StackTrace:{0} <br />", ex.StackTrace);
                    strEBody.AppendFormat("TargetSite:{0} <br />", ex.TargetSite.Name);

                    MailMessage mail = new MailMessage(strFromMail, strToUser, strTitle, strEBody.ToString());

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(MailServer);
                    smtp.Credentials = new System.Net.NetworkCredential(MailFromUser, MailFromUserPwd);
                    smtp.Send(mail);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 将错误异常数据发送给管理员
        /// </summary>
        /// <param name="strTo">管理员账号</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件内容</param>
        public static void SendExceptionToAdmin(string strTo, string strTitle, string strBody)
        {
            try
            {
                if (!string.IsNullOrEmpty(strTo))
                {
                    string strFromMail = MailFromUser + MailUrl;
                    string strToUser = strTo + MailUrl;

                    MailMessage mail = new MailMessage(strFromMail, strToUser, strTitle, strBody);

                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient(MailServer);
                    smtp.Credentials = new System.Net.NetworkCredential(MailFromUser, MailFromUserPwd);
                    smtp.Send(mail);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 发送邮件给相应的用户
        /// </summary>
        /// <param name="strToList">收件人集合</param>
        /// <param name="strCcList">抄送人集合</param>
        /// <param name="strTitle">邮件标题</param>
        /// <param name="strBody">邮件内容</param>
        /// <param name="attList">附件列表</param>
        public static void SendMailList(List<string> strToList, List<string> strCcList, string strTitle, string strBody, List<Attachment> attList)
        {
            try
            {
                if (strToList.Count > 0)
                {
                    string strFromMail = MailFromUser + MailUrl;
                    //string strToUser = strTo + MailUrl;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(strFromMail);
                    mail.Subject = strTitle;
                    mail.Body = strBody;
                    mail.IsBodyHtml = true;
                    string[] strSystemAdmin = SystemAdmin.Split(';');

                    //附件列表
                    foreach (Attachment att in attList)
                    {
                        mail.Attachments.Add(att);
                    }

                    foreach (string strTo in strToList)
                    {
                        #region 测试系统发邮件都默认给管理员
                        if (SystemIsTest)
                        {
                            mail.To.Add(strSystemAdmin[0] + MailUrl);
                            strTitle = "[测试]" + strTitle;
                        }
                        else
                        {
                            mail.To.Add(strTo + MailUrl);
                        }
                        #endregion

                    }
                    //抄送用户
                    foreach (string strCc in strCcList)
                    {
                        mail.CC.Add(strCc + MailUrl);
                    }
                    #region 增加邮件密送给管理员操作
                    foreach (string strAdmin in strSystemAdmin)
                    {
                        mail.Bcc.Add(new MailAddress(strAdmin + MailUrl));
                    }
                    #endregion

                    SmtpClient smtp = new SmtpClient(MailServer);

                    smtp.Credentials = new System.Net.NetworkCredential(MailFromUser, MailFromUserPwd);
                    //string userState = "邮件发送完成！";
                    //smtp.SendAsync(mail, userState);
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}