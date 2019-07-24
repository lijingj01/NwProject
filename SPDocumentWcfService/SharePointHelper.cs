using JohnHolliday.Caml.Net;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml;
using SP = Microsoft.SharePoint.Client;

namespace SPDocumentWcfService
{
    /// <summary>
    /// SharePoint文档处理类
    /// </summary>
    public class SharePointHelper
    {
        #region 常用属性

        #region SharePoint属性

        /// <summary>
        /// DWS接口地址
        /// </summary>
        private string DwsUrl = "_vti_bin/dws.asmx";
        /// <summary>
        /// Copy接口地址
        /// </summary>
        private string CopyUrl = "_vti_bin/copy.asmx";
        /// <summary>
        /// Image接口地址
        /// </summary>
        private string ImageUrl = "_vti_bin/imaging.asmx";
        /// <summary>
        /// List接口地址
        /// </summary>
        private string ListUrl = "_vti_bin/lists.asmx";
        /// <summary>
        /// Search接口地址
        /// </summary>
        private string SearchUrl = "_vti_bin/spsearch.asmx";

        /// <summary>
        /// 用户信息接口地址
        /// </summary>
        private string UserGroupUrl = "_vti_bin/usergroup.asmx";
        /// <summary>
        /// 各个文件的页数字段名
        /// </summary>
        internal string PageNumDisplayName = "页数";
        /// <summary>
        /// 文档库存储的文件分类字段名
        /// </summary>
        internal string DocumentTypeDisplayName = "附件分类";
        /// <summary>
        /// 通用的文件存储文档库名称
        /// </summary>
        internal string DefaultListName = "文档";
        /// <summary>
        /// 存储各种打印模板的文档库地址
        /// </summary>
        internal string TemplateDocumentName = null;
        /// <summary>
        /// 连接Web服务最大的超时时间（20分钟）
        /// </summary>
        private static int MAXTIMEOUT = 1200000;
        #endregion

        #region SharePoint相关参数配置

        /// <summary>
        /// SharePoint主站地址(新)
        /// </summary>
        public string SPSite
        {
            get;
            set;
        }

        /// <summary>
        /// SharePoint基础主站
        /// </summary>
        public string SPBaseSite
        {
            get
            {
                if (!string.IsNullOrEmpty(SPSite))
                {
                    string[] strBaseSites = SPSite.Split('/');
                    string strBaseSite = string.Format("{0}//{1}{2}", strBaseSites[0], strBaseSites[1], strBaseSites[2]);
                    return strBaseSite;
                }
                else
                {
                    return SPSite;
                }
            }
        }

        /// <summary>
        /// SharePoint站点地址(新)
        /// </summary>
        public string SPWeb
        {
            get;
            set;
        }
        /// <summary>
        /// 当前站点完整地址(新)
        /// </summary>
        public string FullWebUrl
        {
            get
            {
                return SPSite + "/" + SPWeb + "/";
            }
        }

        #endregion

        #region 公开传递参数属性
        /// <summary>
        /// 当前刚上传的文件的完整路径
        /// </summary>
        public string UpFileFullUrl
        {
            get;
            private set;
        }

        #endregion

        #region 操作账号的信息
        public string SPUserId { get; set; }
        public string SPUserPwd { get; set; }
        public string SPUserDomain { get; set; }
        /// <summary>
        /// 当前账号的验证信息(新SharePoint账号)
        /// </summary>
        public NetworkCredential SPCredential
        {
            get;
            set;
        }

        #endregion

        #region 登录帐号数据
        /// <summary>
        /// 操作用户帐号
        /// </summary>
        public string UserCode { get; set; }
        #endregion

        #region 内部特殊判断字段
        /// <summary>
        /// 更新数据后SharePoint接口返回成功的值
        /// </summary>
        private string UPDATERETURNRIGHT = "0x00000000";
        #endregion

        /// <summary>
        /// 获取SharePoint客户端对象连接对象
        /// </summary>
        /// <returns></returns>
        private SP.ClientContext CreateClientContext()
        {
            SP.ClientContext context = new SP.ClientContext(FullWebUrl);
            context.Credentials = SPCredential;
            return context;
        }

        #endregion

        #region 构造函数扩展
        //public SharePointHelper(string strUserId, string strUserPwd, string strUserDomain, string strSPSite, string strSPWeb) : this(strUserId, strUserPwd, strUserDomain, strSPSite, strSPWeb, "admin")
        //{

        //}
        public SharePointHelper(string strUserId, string strUserPwd, string strUserDomain, string strSPSite, string strSPWeb, string strUserCode)
        {
            this.SPSite = strSPSite;
            this.SPWeb = strSPWeb;
            this.SPUserId = strUserId;
            this.SPUserPwd = strUserPwd;
            this.SPUserDomain = strUserDomain;
            SPCredential = new NetworkCredential(strUserId, strUserPwd, strUserDomain);
            UserCode = strUserCode;
            //如果没有传递操作员就默认admin
            if (string.IsNullOrEmpty(UserCode))
            {
                UserCode = "admin";
            }
        }
        #endregion

        #region 文件上传
        /// <summary>
        /// 上传文件到公用文件夹里面
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库</param>
        /// <param name="IsUpload">是否上传成功</param>
        /// <returns></returns>
        public string UploadFile(string strFileName, byte[] fileData, string ListName, out bool IsUpload)
        {
            DateTime dtDataCreated = DateTime.Now;
            string strNewFileUrl = string.Empty;
            IsUpload = false;
            #region 按年月来创建文件夹层级
            string strYearFolderName = string.Format("{0:0000}", dtDataCreated.Year);
            SPCostFolder yearFolder = CreateSPFolder(ListName, strYearFolderName);
            if (yearFolder.ID > 0)
            {
                //月份文件夹
                string strMonthFolderName = string.Format("{0:0000}/{1:00}", dtDataCreated.Year, dtDataCreated.Month);
                SPCostFolder monthFolder = CreateSPFolder(ListName, strMonthFolderName);
                if (monthFolder.ID > 0)
                {
                    string strNewFullDocUrl = SPBaseSite + "/" + monthFolder.FileRef + "/" + FileEscape(strFileName);

                    Guid? fileId = null;
                    UpFileFullUrl = NewUploadFile(strFileName, fileData, strNewFullDocUrl
                                                                    , ListName, monthFolder, 0, 0, string.Empty
                                                                    , out IsUpload, out fileId);

                    strNewFileUrl = UpFileFullUrl;
                }
            }
            #endregion
            return strNewFileUrl;
        }

        #region 基础对文件夹里面文件进行上传操作

        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="IsUpload">文件上传是否成功</param>
        /// <param name="strUploadMessage">文件上传的返回信息</param>
        /// <returns></returns>
        public int UploadFile(string strFileName, byte[] fileData, string ListName, string FolderName, out bool IsUpload, out string strUploadMessage)
        {
            return UploadFile(strFileName, fileData, ListName, FolderName, 0, string.Empty, out IsUpload, out strUploadMessage);
        }
        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderId">文件夹编号</param>
		/// <param name="IsUpload">文件上传是否成功</param>
        /// <returns></returns>
        public string UploadFile(string strFileName, byte[] fileData, string ListName, int FolderId, out bool IsUpload)
        {
            return UploadFile(strFileName, fileData, ListName, FolderId, 0, string.Empty, out IsUpload);
        }

        #endregion

        #region 有文件页码和类型的上传方法

        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="iPageNum">扩展属性页数</param>
        /// <param name="strDocumentType">附件类型</param>
        /// <param name="IsUpload">文件上传是否成功</param>
        /// <param name="strUploadMessage">文件上传的返回信息</param>
        /// <returns>新的文件夹编号</returns>
        public int UploadFile(string strFileName, byte[] fileData, string ListName, string FolderName, int iPageNum, string strDocumentType, out bool IsUpload, out string strUploadMessage)
        {
            // 實例化Copy對象
            SPCopyWebService.Copy copy = new SPCopyWebService.Copy()
            {
                Url = FullWebUrl + CopyUrl,
                Credentials = SPCredential
            };
            //获取文件夹信息
            SPCostFolder folder = GetFolderInfo(ListName, FolderName);
            if (folder.ID == 0)
            {
                //文件夹未创建需要创建一个
                folder = CreateSPFolder(ListName, FolderName, DateTime.Now);
            }

            string strNewFullDocUrl = SPBaseSite + "/" + folder.FileRef + "/" + FileEscape(strFileName);
            IsUpload = false;

            Guid? fileId = null;
            UpFileFullUrl = NewUploadFile(strFileName, fileData, strNewFullDocUrl
                                                            , ListName, folder, 0, iPageNum, strDocumentType, out IsUpload, out fileId);

            strUploadMessage = UpFileFullUrl;

            return folder.ID;
        }

        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderId">文件夹编号</param>
        /// <param name="iPageNum">扩展属性页数</param>
        /// <param name="strDocumentType">附件类型</param>
		/// <param name="IsUpload">文件上传是否成功</param>
		public string UploadFile(string strFileName, byte[] fileData, string ListName, int FolderId, int iPageNum, string strDocumentType, out bool IsUpload)
        {
            // 實例化Copy對象
            SPCopyWebService.Copy copy = new SPCopyWebService.Copy()
            {
                Url = FullWebUrl + CopyUrl,
                Credentials = SPCredential
            };
            //获取文件夹信息
            SPCostFolder folder = GetFolderInfo(ListName, FolderId);

            string strNewFullDocUrl = SPBaseSite + "/" + folder.FileRef + "/" + FileEscape(strFileName);

            Guid? fileId = null;
            UpFileFullUrl = NewUploadFile(strFileName, fileData, strNewFullDocUrl
                                                            , ListName, folder, 0, iPageNum, strDocumentType
                                                            , out IsUpload, out fileId);

            return UpFileFullUrl;
        }

        #endregion

        #region 文件上传内部操作
        /// <summary>
        /// 文件上传时对文件名进行特别验证和转换，将特殊字符进行过滤
        /// </summary>
        /// <param name="strFileName">需要转换的文件名称</param>
        /// <returns></returns>
        private string FileEscape(string strFileName)
        {
            // & " ? < > # {} % ~ / \
            //得到文件名后缀
            string strFileIco = strFileName.Substring(strFileName.IndexOf('.') + 1);

            string s = strFileName.Replace("&", "").Replace("\"", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("#", "").Replace("{", "").Replace("}", "").Replace("%", "")
                  .Replace("~", "").Replace("/", "").Replace("\\", "").Replace("（", "").Replace("）", "").Replace("：", "").Replace("..", ".").Replace("•", ".").Replace(" ", "").Replace("+","＋");
            if (s.Length > 100)
                return s.Substring(0, 100) + "." + strFileName;
            else
                return s;
        }

        private string UploadFile(string strFileName, byte[] fileData, string strFullDocUrl
                , string strListName, SPCostFolder folder, int iUserTaskId
                , int iPageNum, string strDocumentType
                , out bool IsUpload
                , out Guid? fileId
                )
        {
            // 實例化Copy對象
            SPCopyWebService.Copy copy = new SPCopyWebService.Copy()
            {
                Url = FullWebUrl + CopyUrl,
                Credentials = SPCredential
            };

            string strNewFileName = FileEscape(strFileName);

            // 文件存放的路徑
            string destinationUrl = strFullDocUrl;
            string strSourceUrl = "http://void(0)";
            string[] destinationUrls = { destinationUrl };

            SPCopyWebService.FieldInformation info1 = new SPCopyWebService.FieldInformation
            {
                DisplayName = strNewFileName,
                InternalName = strNewFileName,
                Type = SPCopyWebService.FieldType.File,
                Value = strNewFileName
            };
            SPCopyWebService.FieldInformation[] info = { info1 };
            var copyResult = new SPCopyWebService.CopyResult();
            SPCopyWebService.CopyResult[] copyResults = { copyResult };
            copy.Timeout = MAXTIMEOUT;
            // 調用自帶的寫入方法
            copy.CopyIntoItems(strSourceUrl, destinationUrls, info, fileData, out copyResults);
            fileId = null;
            //Success:复制操作成功为指定的目标位置时，将使用此值。(成功)
            //DestinationInvalid：此值用于指示出错时的目标位置不是同一个域作为源目标或目标位置点到无效目标服务器的文件夹位置。
            //DestinationMWS:此值用于指示未能复制文件，因为目标位置是在会议工作区网站。
            //SourceInvalid:此值用于指示出错时复制操作的源位置未引用的源位置中的现有文件。
            //DestinationCheckedOut:此值用于指示出错时的目标位置上的文件已签出，并且不能重写。
            //InvalidUrl:此值用于指示出错时 IRI 的目标位置的格式不正确。
            //Unknown:此值用于指示错误的所有其他错误条件给定的目标位置。（失败）

            if (copyResults[0].ErrorCode == SPCopyWebService.CopyErrorCode.Success)
            {
                //文件上传成功的操作

                IsUpload = true;
                #region 将文件上传信息记录下来

                #region 查询获取刚上传的文件信息
                SPCostDocument docItem = GetCostDocument(strNewFileName, strListName, folder);
                fileId = docItem.UniqueId;
                #endregion
                int iFolderId = 0;
                string strFolderName = string.Empty;
                if (folder != null)
                {
                    iFolderId = folder.ID;
                    strFolderName = folder.FileLeafRef;
                }

                #region 保存到数据库
                AddUpFileLog(strListName, iFolderId, strFolderName, strNewFileName, docItem.UniqueId, strFullDocUrl, iUserTaskId, iPageNum, strDocumentType, fileData);
                #endregion

                #endregion

                return strFullDocUrl;
            }
            else
            {
                IsUpload = false;
                return copyResults[0].ErrorMessage;
            }
        }

        private string NewUploadFile(string strFileName, byte[] fileData, string strFullDocUrl
                , string strListName, SPCostFolder folder, int iUserTaskId
                , int iPageNum, string strDocumentType
                , out bool IsUpload
                , out Guid? fileId)
        {
            fileId = null;
            //先将文件流转换成Stream
            try
            {
                string contentType = "文档";
                Stream stream = SysHelper.FileHelper.BytesToStream(fileData);
                SP.ClientContext clientContext = CreateClientContext();
                SP.Web web = clientContext.Web;
                SP.List list = web.Lists.GetByTitle(strListName);
                bool bTarFileExist = true;

                string strNewFileName = FileEscape(strFileName);

                string strDocumentUrl = "/" + folder.FileRef + "/" + strNewFileName;
                #region 判断文件是否存在

                Microsoft.SharePoint.Client.File targetFile = web.GetFileByServerRelativeUrl(strDocumentUrl);
                targetFile.RefreshLoad();
                clientContext.Load(targetFile);
                try
                {
                    clientContext.ExecuteQuery();
                }
                catch
                {
                    bTarFileExist = false;
                }

                #endregion

                #region 存在的文件是否需要签出
                if (bTarFileExist)
                {
                    // If the target document is checked out by another user, execute UndoCheckOut.
                    if (targetFile.CheckOutType != SP.CheckOutType.None)
                    {
                        targetFile.UndoCheckOut();
                    }

                    // Check out the target document before uploading.
                    targetFile.CheckOut();
                    clientContext.ExecuteQuery();
                }
                #endregion
                //上传文件
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, strDocumentUrl, stream, true);

                #region 获取新上传的文件 更新类型

                Microsoft.SharePoint.Client.File newFile = web.GetFileByServerRelativeUrl(strDocumentUrl);

                newFile.RefreshLoad();
                clientContext.Load(newFile);
                clientContext.ExecuteQuery();

                // Get target file ContentType.
                SP.ContentType newFileContentType = null;
                //if (!defaultContentTypes.Contains(contentType))
                //{
                SP.ContentTypeCollection listContentTypes = list.ContentTypes;
                clientContext.Load(listContentTypes, types => types.Include(type => type.Id, type => type.Name, type => type.Parent));
                var result = clientContext.LoadQuery(listContentTypes.Where(c => c.Name == contentType));
                clientContext.ExecuteQuery();
                newFileContentType = result.FirstOrDefault();

                // Set new file ContentType with the correct value.
                clientContext.Load(newFile.ListItemAllFields);
                if (newFileContentType != null)
                {
                    newFile.ListItemAllFields["ContentTypeId"] = newFileContentType.Id.ToString();
                    newFile.ListItemAllFields.Update();
                }
                //}

                // Check in the docuemnt with a draft version.取消签入操作
                //newFile.CheckIn(string.Empty, SP.CheckinType.MinorCheckIn);
                // Excute the document upload.
                clientContext.ExecuteQuery();

                #endregion
                if (newFile.Length > 0)
                {
                    fileId = newFile.UniqueId;

                    #region 将文件上传信息记录下来

                    int iFolderId = 0;
                    string strFolderName = string.Empty;
                    if (folder != null)
                    {
                        iFolderId = folder.ID;
                        strFolderName = folder.FileLeafRef;
                    }

                    #region 保存到数据库
                    AddUpFileLog(strListName, iFolderId, strFolderName, strNewFileName, newFile.UniqueId, strFullDocUrl, iUserTaskId, iPageNum, strDocumentType, fileData);
                    #endregion

                    #endregion

                    IsUpload = true;
                    return strFullDocUrl;
                }
                else
                {
                    IsUpload = false;
                    return "文档上传失败！";
                }
               
            }
            catch (Exception ex)
            {
                IsUpload = false;
                return ex.Message;
            }
        }

        /// <summary>
        /// 上传文件时将对应的信息保存起来方便后续处理
        /// </summary>
        /// <param name="strListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号（无文件夹就是0）</param>
        /// <param name="strFolderName">文件夹名称（无文件夹就为空）</param>
        /// <param name="strFileLeafRef">文件名称</param>
        /// <param name="FileUniqueId">文件编号</param>
        /// <param name="strFileWebFullRef">文件完整地址</param>
        /// <param name="iUserTaskId">当前操作的流程步骤编号（特殊流程附件属性）</param>
        /// <param name="iPageNum">页面数量</param>
        /// <param name="strDocumentType">文档类型</param>
        /// <param name="fileData">文件内容</param>
        private void AddUpFileLog(string strListName, int iFolderId, string strFolderName, string strFileLeafRef, Guid FileUniqueId
            , string strFileWebFullRef, int iUserTaskId
            , int iPageNum, string strDocumentType
            , byte[] fileData
            )
        {
            try
            {
                Data.FileLogDataClassesDataContext dataContext = new Data.FileLogDataClassesDataContext();
                Data.Files data = new Data.Files()
                {
                    ListName = strListName,
                    FolderId = iFolderId,
                    FolderName = strFolderName,
                    FileLeafRef = strFileLeafRef,
                    UniqueId = FileUniqueId,
                    FileWebFullRef = strFileWebFullRef,
                    Created = DateTime.Now,
                    CreateUser = UserCode,
                    Modified = DateTime.Now,
                    ModifieUser = UserCode,
                    UserTaskId = iUserTaskId,
                    IsDel = false,
                    PageNum = iPageNum,
                    DocumentType = strDocumentType
                };

                dataContext.Files.InsertOnSubmit(data);
                dataContext.SubmitChanges();
            }
            catch (Exception ex)
            {
                string strTitle = "保存文件【" + strFileWebFullRef + "】上传记录时出现错误";
                string strBody = ex.Message + "<br>";
                strBody += ex.TargetSite;
                strBody += ex.StackTrace;

                EMailHelper.SendMail("lijingj", strTitle, strBody);
            }
        }

        /// <summary>
        /// 删除文件时候将上传记录标注为已删除
        /// </summary>
        /// <param name="strListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号（无文件夹就是0）</param>
        /// <param name="FileUniqueId">文件编号</param>
        /// <param name="strDelFileFullRef">完整路径</param>
        private void DelUpFileLog(string strListName, int iFolderId, Guid FileUniqueId, string strDelFileFullRef)
        {
            try
            {
                Data.FileLogDataClassesDataContext dataContext = new Data.FileLogDataClassesDataContext();
                //查询记录
                Data.Files file = dataContext.Files.FirstOrDefault<Data.Files>(c => c.ListName == strListName & c.FolderId == iFolderId & c.UniqueId == FileUniqueId);
                if (file != null)
                {
                    file.IsDel = true;
                    file.Modified = DateTime.Now;
                    file.ModifieUser = UserCode;
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string strTitle = "删除文件【" + strDelFileFullRef + "】上传记录时出现错误";
                string strBody = ex.Message + "<br>";
                strBody += ex.TargetSite;
                strBody += ex.StackTrace;

                EMailHelper.SendMail("lijingj", strTitle, strBody);
            }
        }

        /// <summary>
        /// 删除文件时候将上传记录标注为已删除
        /// </summary>
        /// <param name="strListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号（无文件夹就是0）</param>
        /// <param name="strFileName">文件名</param>
        private void DelUpFileLog(string strListName, int iFolderId, string strFileName)
        {
            try
            {
                Data.FileLogDataClassesDataContext dataContext = new Data.FileLogDataClassesDataContext();
                //查询记录
                Data.Files file = dataContext.Files.FirstOrDefault<Data.Files>(c => c.ListName == strListName & c.FolderId == iFolderId & c.FileLeafRef == strFileName);
                if (file != null)
                {
                    file.IsDel = true;
                    file.Modified = DateTime.Now;
                    file.ModifieUser = UserCode;
                    dataContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                string strTitle = string.Format("删除文件【文档库:{0} 文件夹编号:{1} 文件名:{2}】上传记录时出现错误", strListName, iFolderId, strFileName);
                string strBody = ex.Message + "<br>";
                strBody += ex.TargetSite;
                strBody += ex.StackTrace;

                EMailHelper.SendMail("lijingj", strTitle, strBody);
            }
        }


        #endregion

        #endregion

        #region 文件删除操作

        /// <summary>
        /// 删除指定文档库的指定文件
        /// </summary>
        /// <param name="ListName">文档库名</param>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public bool DeleteFile(string ListName, string FileName)
        {
            try
            {
                bool IsDelFile = false;

                #region 先获取文件信息
                XmlDocument xmlFindDoc = new System.Xml.XmlDocument();

                XmlNode ndQuery = xmlFindDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlFindDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlFindDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");
                ndQueryOptions.InnerXml =
                    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                    "<DateInUtc>TRUE</DateInUtc>";

                ndViewFields.InnerXml = "<FieldRef Name='ID' />";
                ndQuery.InnerXml = "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='Text'>" + FileName + "</Value></Eq></Where>";

                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                        null, null, ndQueryOptions, null);
                SPList list = GetListInfo(ListName);
                //document.SPList = list;
                SPCostDocument document = new SPCostDocument(ndListItems, list, null);
                #endregion

                #region 选择删除文件

                StringBuilder strBatch = new StringBuilder();
                strBatch.AppendFormat("<Method ID='{0}' Cmd='Delete'>", document.ID);
                strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", document.ID);
                strBatch.AppendFormat("<Field Name='FileRef'>{0}</Field>", document.DelFileFullRef);
                strBatch.Append("</Method>");

                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                System.Xml.XmlElement elBatch = xmlDoc.CreateElement("Batch");

                elBatch.InnerXml = strBatch.ToString();

                XmlNode ndReturn = listHelper.UpdateListItems(ListName, elBatch);//第一个参数是列表名
                IsDelFile = true;
                #endregion

                #region 将上传记录标注为已删除
                DelUpFileLog(ListName, 0, document.UniqueId, document.DelFileFullRef);
                #endregion

                return IsDelFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除指定文档库的指定文件
        /// </summary>
        /// <param name="ListName">文档库名</param>
        /// <param name="FileName">文件名</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public bool DeleteFile(string ListName, string FileName, int iFolderId)
        {
            try
            {
                bool IsDelFile = false;
                SPCostDocument document;
                if (iFolderId > 0)
                {
                    SPCostFolder folder = GetFolderInfo(ListName, iFolderId);
                    document = GetCostDocument(FileName, ListName, folder);
                }
                else
                {
                    document = GetCostDocument(FileName, ListName);
                }

                #region 选择删除文件

                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };

                StringBuilder strBatch = new StringBuilder();
                strBatch.AppendFormat("<Method ID='{0}' Cmd='Delete'>", document.ID);
                strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", document.ID);
                strBatch.AppendFormat("<Field Name='FileRef'>{0}</Field>", document.DelFileFullRef);
                strBatch.Append("</Method>");

                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                System.Xml.XmlElement elBatch = xmlDoc.CreateElement("Batch");

                elBatch.InnerXml = strBatch.ToString();

                XmlNode ndReturn = listHelper.UpdateListItems(ListName, elBatch);//第一个参数是列表名
                IsDelFile = true;
                #endregion

                #region 将上传记录标注为已删除
                DelUpFileLog(ListName, iFolderId, document.UniqueId, document.DelFileFullRef);
                #endregion

                return IsDelFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 文件夹操作方法

        #region 创建文件夹

        /// <summary>
        /// 为特定的文档库创建对应的文件夹(按年月来组织文件夹层级)
        /// </summary>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="dtDataCreated">业务数据时间</param>
        /// <returns></returns>
        public SPCostFolder CreateSPFolder(string ListName, string FolderName, DateTime dtDataCreated)
        {
            SPCostFolder newFolder = new SPCostFolder();
            #region 先按年月来创建文件夹层级
            string strYearFolderName = string.Format("{0:0000}", dtDataCreated.Year);
            SPCostFolder yearFolder = CreateSPFolder(ListName, strYearFolderName);
            if (yearFolder.ID > 0)
            {
                //月份文件夹
                string strMonthFolderName = string.Format("{0:0000}/{1:00}", dtDataCreated.Year, dtDataCreated.Month);
                SPCostFolder monthFolder = CreateSPFolder(ListName, strMonthFolderName);
                if (monthFolder.ID > 0)
                {
                    string strNewFolderName = string.Format("{0:0000}/{1:00}/{2}", dtDataCreated.Year, dtDataCreated.Month, FolderName);
                    newFolder = CreateSPFolder(ListName, strNewFolderName);
                }
            }
            #endregion
            return newFolder;
        }

        /// <summary>
        /// 为特定的文档库创建对应的文件夹(针对新SharePoint)
        /// </summary>
        /// <param name="DocumentName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <returns></returns>
        public SPCostFolder CreateSPFolder(string ListName, string FolderName)
        {
            try
            {
                //int iFolderId = 0;
                //先获取列表信息
                SPList list = GetListInfo(ListName);
                //先判断文件夹是否已经创建，已经创建的文件夹不需要再次创建
                SPCostFolder folder = GetFolderInfo(ListName, list.RootFolder, FolderName);
                if (folder.ID == 0)
                {
                    SPDwsWebService.Dws myDws = new SPDwsWebService.Dws();
                    myDws.Credentials = SPCredential;
                    myDws.Url = FullWebUrl + DwsUrl;

                    string strDocUrl = list.ListUrl + "/" + FolderName;
                    //创建文件夹
                    var resultCreate = myDws.CreateFolder(strDocUrl);

                    if (resultCreate.Contains("<Result/>"))
                    {
                        folder = GetFolderInfo(ListName, list.ListUrl, FolderName);
                        //iFolderId = folder.ID;

                        #region 文件夹记录起来
                        SaveFolderLog(ListName, folder);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("文件夹创建失败：" + resultCreate);
                    }
                }
                else
                {
                    //特殊情况判断是否已经记录，没有记录的需要补充记录数据
                    #region 文件夹记录起来
                    SaveFolderLog(ListName, folder);
                    #endregion
                }

                return folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveFolderLog(string ListName, SPCostFolder folder)
        {
            try
            {
                Data.FileLogDataClassesDataContext fileDBContext = new Data.FileLogDataClassesDataContext();
                //避免重复记录
                Data.Folders dbfolder = fileDBContext.Folders.FirstOrDefault<Data.Folders>(c => c.SPWeb == this.SPWeb & c.ListName == ListName & c.FolderId == folder.ID);
                if (dbfolder == null)
                {
                    dbfolder = new Data.Folders()
                    {
                        SPSite = SPSite,
                        SPWeb = SPWeb,
                        ListName = ListName,
                        FolderId = folder.ID,
                        FolderName = folder.FileLeafRef,
                        FolderUniqueId = folder.UniqueId,
                        FileLeafRef = folder.FileLeafRef,
                        FileRef = folder.FileRef,
                        ParentUrl = folder.ParentUrl,
                        Created = DateTime.Now,
                        CreateUser = UserCode
                    };

                    fileDBContext.Folders.InsertOnSubmit(dbfolder);
                    fileDBContext.SubmitChanges();
                }
            }
            catch (Exception ex) { }
        }

        #endregion

        #region 文件夹改名


        /// <summary>
        /// 修改文件夹名称
        /// </summary>
        /// <param name="strListName">列表名称</param>
        /// <param name="strOldFolderName">文件夹原名称</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        public bool UpdateFolderName(string strListName, string strOldFolderName, string strNewFolderName)
        {

            bool isUpdate = false;
            //原文件夹名称和改名的文件夹名称相同就没有必要进行修改
            if (strOldFolderName == strNewFolderName)
            {
                isUpdate = true;
            }
            else
            {
                //获取文件夹的编号
                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                SPList list = GetListInfo(strListName);

                SPCostFolder folder = GetFolderInfo(strListName, list.ListUrl, strOldFolderName);

                StringBuilder strBatch = new StringBuilder();
                strBatch.AppendFormat("<Method ID='{0}' Cmd='Update'>", folder.ID);
                strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", folder.ID);
                strBatch.AppendFormat("<Field Name='owshiddenversion'>{0}</Field>", 1);
                strBatch.AppendFormat("<Field Name='FileRef'>{0}</Field>", folder.FileFullRef);
                strBatch.AppendFormat("<Field Name='FSObjType'>{0}</Field>", 1);
                strBatch.AppendFormat("<Field Name='BaseName'>{0}</Field>", strNewFolderName);
                strBatch.Append("</Method>");

                XmlDocument xmlUpdateDoc = new System.Xml.XmlDocument();

                System.Xml.XmlElement elBatch = xmlUpdateDoc.CreateElement("Batch");

                elBatch.InnerXml = strBatch.ToString();

                XmlNode ndReturn = listHelper.UpdateListItems(strListName, elBatch);

                #region 更新文件夹名称成功后需要同步更新数据
                if (ndReturn.InnerText == UPDATERETURNRIGHT)
                {
                    isUpdate = true;
                    Data.FileLogDataClassesDataContext fileDBContext = new Data.FileLogDataClassesDataContext();
                    Data.Folders dFolder = fileDBContext.Folders.FirstOrDefault<Data.Folders>(c => c.ListName == strListName & c.FolderId == folder.ID & c.FolderUniqueId == folder.UniqueId);
                    if (dFolder != null)
                    {
                        dFolder.FolderName = strNewFolderName;
                        dFolder.FileLeafRef = strNewFolderName;
                        dFolder.FileRef = dFolder.ParentUrl + "/" + strNewFolderName;

                        #region 文件夹更名后需要同步更新已经上传过的文件信息
                        var files = (from c in fileDBContext.Files
                                     where c.FolderId == dFolder.FolderId & c.ListName == dFolder.ListName
                                     select c
                                     ).ToList();
                        foreach(Data.Files file in files)
                        {
                            string strFileOldFolderName = file.FolderName;
                            file.FolderName = dFolder.FolderName;
                            //文件完整地址需要更新
                            string strOldFile = file.FileWebFullRef;
                            file.FileWebFullRef = strOldFile.Replace(strFileOldFolderName, dFolder.FolderName);
                        }
                        #endregion

                        fileDBContext.SubmitChanges();
                    }
                }
                #endregion
            }
            return isUpdate;
        }

        /// <summary>
        /// 修改文件夹名称
        /// </summary>
        /// <param name="strListName">列表名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        public bool UpdateFolderName(string strListName, int iFolderId, string strNewFolderName)
        {
            bool isUpdate = false;
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            SPList list = GetListInfo(strListName);

            SPCostFolder folder = GetFolderInfo(strListName, iFolderId);

            //原文件夹名称和改名的文件夹名称相同就没有必要进行修改
            if (folder.FileLeafRef != strNewFolderName)
            {

                StringBuilder strBatch = new StringBuilder();
                strBatch.AppendFormat("<Method ID='{0}' Cmd='Update'>", folder.ID);
                strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", folder.ID);
                strBatch.AppendFormat("<Field Name='owshiddenversion'>{0}</Field>", 1);
                strBatch.AppendFormat("<Field Name='FileRef'>{0}</Field>", folder.FileFullRef);
                strBatch.AppendFormat("<Field Name='FSObjType'>{0}</Field>", 1);
                strBatch.AppendFormat("<Field Name='BaseName'>{0}</Field>", strNewFolderName);
                strBatch.Append("</Method>");

                XmlDocument xmlUpdateDoc = new System.Xml.XmlDocument();

                System.Xml.XmlElement elBatch = xmlUpdateDoc.CreateElement("Batch");

                elBatch.InnerXml = strBatch.ToString();

                XmlNode ndReturn = listHelper.UpdateListItems(strListName, elBatch);

                #region 更新文件夹名称成功后需要同步更新数据
                if (ndReturn.InnerText == UPDATERETURNRIGHT)
                {
                    isUpdate = true;
                    Data.FileLogDataClassesDataContext fileDBContext = new Data.FileLogDataClassesDataContext();
                    Data.Folders dFolder = fileDBContext.Folders.FirstOrDefault<Data.Folders>(c => c.ListName == strListName & c.FolderId == folder.ID & c.FolderUniqueId == folder.UniqueId);
                    if (dFolder != null)
                    {
                        dFolder.FolderName = strNewFolderName;
                        dFolder.FileLeafRef = strNewFolderName;
                        dFolder.FileRef = dFolder.ParentUrl + "/" + strNewFolderName;

                        #region 文件夹更名后需要同步更新已经上传过的文件信息
                        var files = (from c in fileDBContext.Files
                                     where c.FolderId == dFolder.FolderId & c.ListName == dFolder.ListName
                                     select c
                                     ).ToList();
                        foreach (Data.Files file in files)
                        {
                            string strFileOldFolderName = file.FolderName;
                            file.FolderName = dFolder.FolderName;
                            //文件完整地址需要更新
                            string strOldFile = file.FileWebFullRef;
                            file.FileWebFullRef = strOldFile.Replace(strFileOldFolderName, dFolder.FolderName);
                        }
                        #endregion

                        fileDBContext.SubmitChanges();
                    }
                }
                #endregion
            }
            else
            {
                isUpdate = true;
            }
            return isUpdate;
        }

        #endregion

        #region 查询文件夹

        /// <summary>
        /// 获取指定的文件夹
        /// </summary>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <returns></returns>
        public SPCostFolder GetFolderInfo(string ListName, string FolderName)
        {
            //先获取列表信息
            SPList list = GetListInfo(ListName);
            //查询文件夹
            SPCostFolder folder = GetFolderInfo(ListName, list.RootFolder, FolderName);
            folder.ListUrl = list.ListUrl;
            return folder;
        }

        /// <summary>
        /// 通过数据查询得到指定的文件夹
        /// </summary>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <returns></returns>
        public SPCostFolder GetFolderInfoByDB(string ListName, string FolderName)
        {
            Data.FileLogDataClassesDataContext dataContext = new Data.FileLogDataClassesDataContext();
            string strSite = this.SPSite + "/";
            //c.SPSite == strSite & 
            Data.Folders folder = dataContext.Folders.SingleOrDefault<Data.Folders>(c => c.SPWeb == this.SPWeb & c.ListName == ListName & c.FolderName == FolderName);
            if (folder != null)
            {
                //SPCostList list = GetListInfo(ListName);
                string strFolderName = folder.FileLeafRef;
                string strParentUrl = "/" + folder.ParentUrl;
                SPCostFolder spfolder = GetFolderInfo(ListName, strParentUrl, strFolderName);
                SPList list = GetListInfo(ListName);
                spfolder.ListUrl = list.ListUrl;
                return spfolder;
            }
            else
            {
                #region 旧数据处理方法

                //先获取列表信息
                SPList list = GetListInfo(ListName);
                //查询文件夹
                SPCostFolder spfolder = GetFolderInfo(ListName, list.RootFolder, FolderName);
                spfolder.ListUrl = list.ListUrl;
                return spfolder;

                #endregion
            }
        }

        /// <summary>
        /// 获取指定的文件夹
        /// </summary>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderListUrl">列表链接</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <returns></returns>
        private SPCostFolder GetFolderInfo(string ListName, string FolderListUrl, string FolderName)
        {
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            //查询限制
            //ndQueryOptions.InnerXml =
            //    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
            //    "<DateInUtc>TRUE</DateInUtc>";
            string[] strSplitFolder = FolderName.Split('/');
            string strParentFolderName = FolderListUrl;
            if (strParentFolderName.Last() != '/')
            {
                strParentFolderName += "/";
            }
            if (strSplitFolder.Length > 1)
            {
                for (int i = 0; i < strSplitFolder.Length - 1; i++)
                {
                    strParentFolderName += strSplitFolder[i] + @"/";
                }
            }
            string strSmallFolderName = strSplitFolder[strSplitFolder.Length - 1];

            ndQueryOptions.InnerXml = "<QueryOptions><Folder>" + strParentFolderName + "</Folder></QueryOptions>";
            //<QueryOptions><Folder>/teams/G11N-IT/Lists/FAQ/vCenter</Folder></QueryOptions>"
            //<ViewAttributes Scope='Recursive'/>
            // ListName + @"/"+ @"/" + ListName + @"/" + ListName +
            //string strSearchName = ListName + @"/" + FolderName;
            //strSearchName =  "07";
            //ndQueryOptions.InnerXml = "<Folder>" + strSearchName + "</Folder>";
            //查询字段
            //ndViewFields.InnerXml = "<FieldRef Name='ID' />";
            //查询条件 + strParentFolderName + "/" 

            ndQuery.InnerXml = "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='Text'>" + strSmallFolderName + "</Value></Eq></Where>";
            //ndQuery.InnerXml = "<Query><OrderBy><FieldRef Name='Title'/></OrderBy></Query>";
            //ndQuery.InnerXml = "<mylistitemrequest><Query><Where><Eq><FieldRef Name=\"FSObjType\"><Value Type=\"Lookup\">0</Value></Eq></Where></Query><ViewFields><FieldRef Name=\"EncodedAbsUrl\"/><FieldRef Name=\"ID\"><FieldRef Name=\"Title\" /></ViewFields><QueryOptions><Folder>" + strSearchName + "</Folder></QueryOptions></mylistitemrequest>";

            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                null, null, ndQueryOptions, null);

            SPCostFolder folder = new SPCostFolder();
            folder.CreateFolder(ndListItems, strSmallFolderName);
            folder.ListName = ListName;
            folder.SPSite = SPSite;
            folder.SPWeb = SPWeb;
            //if (folder.ID == 0)
            //{
            //    //没有文件夹就需要创建
            //    folder = CreateSPFolder(ListName, FolderName, DateTime.Now);
            //}

            return folder;
        }
        /// <summary>
        /// 获取指定的文件夹
        /// </summary>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderId">文件夹编号</param>
        /// <returns></returns>
        public SPCostFolder GetFolderInfo(string ListName, int FolderId)
        {
            //新文件夹层级造成二级三级的文件夹无法通过编号查询得到，需要用文件夹路径来查询
            Data.FileLogDataClassesDataContext dataContext = new Data.FileLogDataClassesDataContext();
            string strSite = this.SPSite + "/";
            //c.SPSite == strSite & 
            Data.Folders folder = dataContext.Folders.SingleOrDefault<Data.Folders>(c => c.SPWeb == this.SPWeb & c.ListName == ListName & c.FolderId == FolderId);
            if (folder != null)
            {
                //SPCostList list = GetListInfo(ListName);
                string strFolderName = folder.FileLeafRef;
                string strParentUrl = "/" + folder.ParentUrl;
                SPCostFolder spfolder = GetFolderInfo(ListName, strParentUrl, strFolderName);
                SPList list = GetListInfo(ListName);
                spfolder.ListUrl = list.ListUrl;
                return spfolder;
            }
            else
            {
                #region 旧处理方法

                //获取文件夹的编号
                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlDocument xmlDoc = new System.Xml.XmlDocument();
                XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                //查询限制
                ndQueryOptions.InnerXml =
                    "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                    "<DateInUtc>TRUE</DateInUtc>";
                //查询字段
                //ndViewFields.InnerXml = "<FieldRef Name='ID' />";
                //查询条件
                ndQuery.InnerXml = "<Where><Eq><FieldRef Name='ID'/><Value Type='Number'>" + FolderId.ToString() + "</Value></Eq></Where>";
                XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                    null, null, ndQueryOptions, null);

                //listHelper.get

                SPCostFolder spfolder = new SPCostFolder();
                spfolder.CreateFolder(ndListItems);
                SPList list = GetListInfo(ListName);
                spfolder.ListUrl = list.ListUrl;
                spfolder.ListName = ListName;

                return spfolder;
                #endregion
            }


        }

        public SPCostFolders GetListFolders(string ListName)
        {
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            //查询限制
            ndQueryOptions.InnerXml =
                "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                "<DateInUtc>TRUE</DateInUtc>";
            //查询字段
            //ndViewFields.InnerXml = "<FieldRef Name='ID' />";
            //查询条件

            StringBuilder strQueryOptionsXml = new StringBuilder();
            strQueryOptionsXml.Append("<QueryOptions>");
            strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
            strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
            strQueryOptionsXml.Append("<Folder></Folder>");
            strQueryOptionsXml.Append("</QueryOptions>");

            ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

            ndQuery.InnerXml = "<Where><Eq><FieldRef Name='ContentType'/><Value Type='Text'>文件夹</Value></Eq></Where>";

            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                null, null, ndQueryOptions, null);

            SPCostFolders folders = new SPCostFolders();
            folders.CreateFolders(ndListItems);
            return folders;
        }

        public SPCostFolders GetListFolderChilds(string ListName, string FolderListUrl)
        {
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            string strParentFolderName = FolderListUrl;
            if (strParentFolderName.Last() != '/')
            {
                strParentFolderName += @"/";
            }

            StringBuilder strQueryOptionsXml = new StringBuilder();
            strQueryOptionsXml.Append("<QueryOptions>");
            strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
            strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
            strQueryOptionsXml.Append("<Folder>" + strParentFolderName + "</Folder>");
            strQueryOptionsXml.Append("</QueryOptions>");

            ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

            ndQuery.InnerXml = "<Where><Eq><FieldRef Name='ContentType'/><Value Type='Text'>文件夹</Value></Eq></Where>";

            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                null, null, ndQueryOptions, null);

            SPCostFolders folders = new SPCostFolders();
            folders.CreateFolders(ndListItems);
            return folders;
        }

        public SPCostFolder SearchFolderInfo(string ListName, string FolderName)
        {
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            //查询限制
            ndQueryOptions.InnerXml =
                "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                "<DateInUtc>TRUE</DateInUtc>";
            //查询字段
            //ndViewFields.InnerXml = "<FieldRef Name='ID' />";
            //查询条件
            ndQuery.InnerXml = "<Where><Eq><FieldRef Name='NameOrTitle'/><Value Type='String'>" + FolderName + "</Value></Eq></Where>";
            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                null, null, ndQueryOptions, null);

            SPCostFolder folder = new SPCostFolder();
            folder.CreateFolder(ndListItems);
            folder.ListName = ListName;
            if (folder.ID == 0)
            {
                return null;
            }
            else
            {
                return folder;
            }
        }

        /// <summary>
        /// 获取文档库的所有文件夹，包括层级
        /// </summary>
        /// <param name="ListName"></param>
        /// <returns></returns>
        public SPCostFolders GetListFullFolders(string ListName)
        {
            try
            {
                SPList list = GetListInfo(ListName);
                string strListUrl = list.ListUrl;
                //得到根目录
                SPCostFolders folders = GetListFolders(ListName);
                foreach(SPCostFolder folder in folders)
                {
                    SPCostFolders childFoldes = LoadFolderChilds(ListName, strListUrl, folder);
                    folder.Childs = childFoldes;
                }

                return folders;
            }
            catch
            {
                throw;
            }
        }

        private SPCostFolders LoadFolderChilds(string ListName, string strListUrl, SPCostFolder folder)
        {
            string[] strParentUrls = folder.FileRef.Split('/');
            string strParentFolder = string.Empty;
            int iStartIndex = -1;
            for (int i = 0; i < strParentUrls.Length; i++)
            {
                if (strParentUrls[i] == strListUrl)
                {
                    iStartIndex = i;
                }
                if (iStartIndex >= 0)
                {
                    strParentFolder += strParentUrls[i] + "/";
                }
            }
            SPCostFolders childFoldes = GetListFolderChilds(ListName, strParentFolder);
            //递归加载子目录
            foreach(SPCostFolder child in childFoldes)
            {
                SPCostFolders childs = LoadFolderChilds(ListName, strListUrl, child);
                child.Childs = childs;
            }
            return childFoldes;
        }
        #endregion

        #endregion

        #region 获取文件列表

        /// <summary>
        /// 获取服务器文件的流数据（文件流）
        /// </summary>
        /// <param name="strWebUrl">服务器文件路径</param>
        /// <param name="strListName">文档库名称</param>
        /// <returns></returns>
        public byte[] GetWebFileStream(string strWebUrl, string strListName)
        {
            try
            {
                string strFileName = strWebUrl.Substring(strWebUrl.LastIndexOf("/") + 1);
                //实例化文档对象
                // 實例化Copy對象
                SPCopyWebService.Copy copy = new SPCopyWebService.Copy()
                {
                    Url = FullWebUrl + CopyUrl,
                    Credentials = SPCredential
                };

                //将文件的流数据提取
                byte[] fileContents = { };
                SPCopyWebService.FieldInformation[] fields = { };
                copy.GetItem(strWebUrl, out fields, out fileContents);

                return fileContents;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定的文件信息
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="strListName">文档库名称</param>
        /// <returns></returns>
        private SPCostDocument GetCostDocument(string strFileName, string strListName)
        {
            return GetCostDocument(strFileName, strListName, null);
        }

        /// <summary>
        /// 获取指定的文件信息
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="strListName">文档库名称</param>
        /// <param name="folder">文件夹对象</param>
        /// <returns></returns>
        private SPCostDocument GetCostDocument(string strFileName, string strListName, SPCostFolder folder)
        {
            string strSerachXml = string.Empty;
            string strQueryOptions = string.Empty;
            try
            {
                XmlDocument xmlFindDoc = new System.Xml.XmlDocument();

                string strFileN = StringHelper.GetXmlString(strFileName);

                XmlNode ndQuery = xmlFindDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlFindDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlFindDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                strQueryOptions = "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                    "<DateInUtc>TRUE</DateInUtc>";
                if (folder != null)
                {
                    strQueryOptions += "<Folder>" + folder.FileFullRef + "</Folder>";
                }

                ndQueryOptions.InnerXml = strQueryOptions;

                ndViewFields.InnerXml = "<FieldRef Name='ID' />";
                strSerachXml = "<Where><Eq><FieldRef Name='FileLeafRef'/><Value Type='Text'>" + strFileN + "</Value></Eq></Where>";
                ndQuery.InnerXml = strSerachXml;

                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlNode ndListItems = listHelper.GetListItems(strListName, null, ndQuery,
                        null, null, ndQueryOptions, null);

                SPList list = GetListInfo(strListName);
                SPCostDocument document = new SPCostDocument(ndListItems, list, folder);

                return document;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "|ndQuery:" + strSerachXml + "|ndQueryOptions:" + strQueryOptions + "|ListName:" + strListName, ex);
            }
        }

        /// <summary>
        /// 通过列表和文件夹编号获取对应的文件列表
        /// </summary>
        /// <param name="ListName">列表名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public SPCostDocuments GetFolderDocuments(string ListName, int iFolderId)
        {
            try
            {

                //先获取文件夹信息
                SPCostFolder folder = GetFolderInfo(ListName, iFolderId);

                //获取文件夹的编号
                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                //查询文件列表

                ndViewFields.InnerXml = "<FieldRef Name='ID' />";

                StringBuilder strQueryOptionsXml = new StringBuilder();
                strQueryOptionsXml.Append("<QueryOptions>");
                strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
                strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
                strQueryOptionsXml.Append("<Folder>" + folder.FileFullRef + "</Folder>");
                strQueryOptionsXml.Append("</QueryOptions>");


                ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

                ndQuery.InnerXml = "";
                //查询对应的文件
                XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

                SPList listItem = GetListInfo(ListName);

                SPCostDocuments items = new SPCostDocuments(ndListItems, listItem, folder);

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 通过列表和文件夹名称获取对应的文件列表
        /// </summary>
        /// <param name="ListName">列表名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <returns></returns>
        public SPCostDocuments GetFolderDocuments(string ListName, string FolderName)
        {
            try
            {

                //先获取文件夹信息
                SPCostFolder folder = GetFolderInfoByDB(ListName, FolderName);

                //获取文件夹的编号
                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                //查询文件列表

                ndViewFields.InnerXml = "<FieldRef Name='ID' />";

                StringBuilder strQueryOptionsXml = new StringBuilder();
                strQueryOptionsXml.Append("<QueryOptions>");
                strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
                strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
                strQueryOptionsXml.Append("<Folder>" + folder.FileFullRef + "</Folder>");
                strQueryOptionsXml.Append("</QueryOptions>");


                ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

                ndQuery.InnerXml = "";
                //查询对应的文件
                XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

                SPList listItem = GetListInfo(ListName);

                SPCostDocuments items = new SPCostDocuments(ndListItems, listItem, folder);

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过列表和文件夹名称获取对应的文件列表
        /// </summary>
        /// <param name="ListName">列表名称</param>
        /// <param name="FolderListUrl">父级地址</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <returns></returns>
        public SPCostDocuments GetFolderDocuments(string ListName, string FolderListUrl, string FolderName)
        {
            try
            {

                //先获取文件夹信息
                SPCostFolder folder = GetFolderInfo(ListName, FolderListUrl, FolderName);

                //获取文件夹的编号
                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                //查询文件列表

                ndViewFields.InnerXml = "<FieldRef Name='ID' />";

                StringBuilder strQueryOptionsXml = new StringBuilder();
                strQueryOptionsXml.Append("<QueryOptions>");
                strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
                strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
                strQueryOptionsXml.Append("<Folder>" + folder.FileFullRef + "</Folder>");
                strQueryOptionsXml.Append("</QueryOptions>");


                ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

                ndQuery.InnerXml = "";
                //查询对应的文件
                XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

                SPList listItem = GetListInfo(ListName);

                SPCostDocuments items = new SPCostDocuments(ndListItems, listItem, folder);

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过列表和文件夹编号获取对应的文件列表
        /// </summary>
        /// <param name="ListName">列表名称</param>
        /// <returns></returns>
        public SPCostDocuments GetFolderDocuments(string ListName)
        {
            try
            {

                //获取文件夹的编号
                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlDocument xmlDoc = new System.Xml.XmlDocument();

                XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
                XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
                XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

                //查询文件列表

                ndViewFields.InnerXml = "<FieldRef Name='ID' />";

                StringBuilder strQueryOptionsXml = new StringBuilder();
                strQueryOptionsXml.Append("<QueryOptions>");
                strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
                strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
                strQueryOptionsXml.Append("<Folder></Folder>");
                strQueryOptionsXml.Append("</QueryOptions>");


                ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

                ndQuery.InnerXml = "";
                //查询对应的文件
                XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

                SPList listItem = GetListInfo(ListName);

                SPCostDocuments items = new SPCostDocuments(ndListItems, listItem);

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取文档库信息

        /// <summary>
        /// 通过文档库名称获取对应的文档库信息(新)
        /// </summary>
        /// <param name="ListName">文档库名称</param>
        /// <returns></returns>
        public SPList GetListInfo(string ListName)
        {
            try
            {


                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };
                XmlNode node = listHelper.GetList(ListName);
                //获取列表库的结构
                SPList list = new SPList(node);
                list.SPSite = SPSite;
                list.SPWeb = SPWeb;

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 图片库相关操作

        #region 图片库文件夹操作

        /// <summary>
        /// 更新图片库里面特定文件夹的名称
        /// </summary>
        /// <param name="strListName">图片库名称</param>
        /// <param name="listHelper">接口对象</param>
        /// <param name="strNewFolderTitle">图片文件夹新名称</param>
        /// <param name="strFolderFullTitle">图片文件夹原名称</param>
        /// <param name="strNewFolderFullTitle">图片文件夹新名称的完整路径</param>
        /// <returns></returns>
        private int UpdateImageFolderName(string strListName, SPListWebService.Lists listHelper
                                , string strNewFolderTitle
                                , string strFolderFullTitle
                                , string strNewFolderFullTitle
                                 )
        {
            SPCostFolder newFolder = GetFolderInfo(strListName, strFolderFullTitle);
            if (newFolder != null)
            {
                StringBuilder strBatch = new StringBuilder();
                strBatch.AppendFormat("<Method ID='{0}' Cmd='Update'>", newFolder.ID);
                strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", newFolder.ID);
                strBatch.AppendFormat("<Field Name='owshiddenversion'>{0}</Field>", 1);
                strBatch.AppendFormat("<Field Name='FileRef'>{0}</Field>", newFolder.FileFullRef);
                strBatch.AppendFormat("<Field Name='FSObjType'>{0}</Field>", 1);
                strBatch.AppendFormat("<Field Name='BaseName'>{0}</Field>", strNewFolderTitle);
                strBatch.Append("</Method>");

                XmlDocument xmlUpdateDoc = new System.Xml.XmlDocument();

                System.Xml.XmlElement elBatch = xmlUpdateDoc.CreateElement("Batch");

                elBatch.InnerXml = strBatch.ToString();

                XmlNode ndReturn = listHelper.UpdateListItems(strListName, elBatch);

                #region 文件夹记录起来
                newFolder = GetFolderInfo(strListName, strNewFolderFullTitle);
                try
                {
                    Data.FileLogDataClassesDataContext fileDBContext = new Data.FileLogDataClassesDataContext();
                    //判断是否已经存在
                    Data.Folders dbfolder = fileDBContext.Folders.SingleOrDefault<Data.Folders>(c => c.ListName == strListName & c.FolderId == newFolder.ID & c.FolderUniqueId == newFolder.UniqueId);
                    if (dbfolder == null)
                    {
                        dbfolder = new Data.Folders()
                        {
                            SPSite = SPSite,
                            SPWeb = SPWeb,
                            ListName = strListName,
                            FolderId = newFolder.ID,
                            FolderName = newFolder.FileLeafRef,
                            FolderUniqueId = newFolder.UniqueId,
                            FileLeafRef = newFolder.FileLeafRef,
                            FileRef = newFolder.FileRef,
                            ParentUrl = newFolder.ParentUrl,
                            Created = DateTime.Now,
                            CreateUser = UserCode
                        };

                        fileDBContext.Folders.InsertOnSubmit(dbfolder);

                    } else
                    {
                        dbfolder.FolderName = newFolder.FileLeafRef;
                        dbfolder.FileLeafRef = newFolder.FileLeafRef;
                        dbfolder.FileRef = newFolder.FileRef;
                    }
                    fileDBContext.SubmitChanges();
                }
                catch (Exception ex) { }
                #endregion

                return newFolder.ID;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 更新图片库里面的文件夹名称
        /// </summary>
        /// <param name="strListName">图片库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        /// <returns></returns>
        public bool UpdateImageFolderName(string strListName, int iFolderId, string strNewFolderName)
        {
            try
            {
                bool isUpdate = false;

                //通过数据库来提取得到对应的文件夹数据
                SPCostFolder folder = GetFolderInfo(strListName, iFolderId);
                if (folder != null)
                {

                    SPListWebService.Lists listHelper = new SPListWebService.Lists()
                    {
                        Url = FullWebUrl + ListUrl,
                        Credentials = SPCredential
                    };

                    StringBuilder strBatch = new StringBuilder();
                    strBatch.AppendFormat("<Method ID='{0}' Cmd='Update'>", folder.ID);
                    strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", folder.ID);
                    strBatch.AppendFormat("<Field Name='owshiddenversion'>{0}</Field>", 1);
                    strBatch.AppendFormat("<Field Name='FileRef'>{0}</Field>", folder.FileFullRef);
                    strBatch.AppendFormat("<Field Name='FSObjType'>{0}</Field>", 1);
                    strBatch.AppendFormat("<Field Name='BaseName'>{0}</Field>", strNewFolderName);
                    strBatch.Append("</Method>");

                    XmlDocument xmlUpdateDoc = new System.Xml.XmlDocument();

                    System.Xml.XmlElement elBatch = xmlUpdateDoc.CreateElement("Batch");

                    elBatch.InnerXml = strBatch.ToString();

                    XmlNode ndReturn = listHelper.UpdateListItems(strListName, elBatch);

                    #region 更新文件夹名称成功后需要同步更新数据
                    if (ndReturn.InnerText == UPDATERETURNRIGHT)
                    {
                        isUpdate = true;
                        Data.FileLogDataClassesDataContext fileDBContext = new Data.FileLogDataClassesDataContext();
                        Data.Folders dFolder = fileDBContext.Folders.FirstOrDefault<Data.Folders>(c => c.ListName == strListName & c.FolderId == folder.ID & c.FolderUniqueId == folder.UniqueId);
                        if (dFolder != null)
                        {
                            dFolder.FolderName = strNewFolderName;
                            dFolder.FileLeafRef = strNewFolderName;
                            dFolder.FileRef = dFolder.ParentUrl + "/" + strNewFolderName;

                            #region 文件夹更名后需要同步更新已经上传过的文件信息
                            var files = (from c in fileDBContext.Files
                                         where c.FolderId == dFolder.FolderId & c.ListName == dFolder.ListName
                                         select c
                                         ).ToList();
                            foreach (Data.Files file in files)
                            {
                                string strFileOldFolderName = file.FolderName;
                                file.FolderName = dFolder.FolderName;
                                //文件完整地址需要更新
                                string strOldFile = file.FileWebFullRef;
                                file.FileWebFullRef = strOldFile.Replace(strFileOldFolderName, dFolder.FolderName);
                            }
                            #endregion

                            fileDBContext.SubmitChanges();
                        }
                    }
                    #endregion
                }
                return isUpdate;
            } catch
            {
                throw;
            }
        }

        /// <summary>
        /// 图片库创建文件夹
        /// </summary>
        /// <param name="strListName">文档库名称</param>
        /// <param name="strFolderName">文件夹名称</param>
        /// <param name="dtDataCreated">创建时间</param>
        /// <returns></returns>
        public SPCostFolder ImageCreateNewFolder(string strListName, string strFolderName, DateTime dtDataCreated)
        {
            try
            {
                int iNewFolderId = 0;
                // 實例化图片库对象
                SPImageWebService.Imaging imageHelper = new SPImageWebService.Imaging()
                {
                    Url = FullWebUrl + ImageUrl,
                    Credentials = SPCredential
                };

                SPListWebService.Lists listHelper = new SPListWebService.Lists()
                {
                    Url = FullWebUrl + ListUrl,
                    Credentials = SPCredential
                };

                #region 年文件夹

                string strYearFolderName = string.Format("{0:0000}", dtDataCreated.Year);
                SPCostFolder yearFolder = GetFolderInfo(strListName, strYearFolderName);
                if (yearFolder.ID == 0)
                {
                    XmlNode newNode = imageHelper.CreateNewFolder(strListName, "");
                    string strTempFolderName = newNode.Attributes["title"].InnerText;

                    #region 文件夹改名
                    int iYearFolderId = UpdateImageFolderName(strListName, listHelper, strYearFolderName, strTempFolderName, strYearFolderName);
                    #endregion
                }

                #endregion

                #region 月文件夹
                string strMonthFolderName = string.Format("{0:0000}/{1:00}", dtDataCreated.Year, dtDataCreated.Month);
                SPCostFolder monthFolder = GetFolderInfo(strListName, strMonthFolderName);
                if (monthFolder.ID == 0)
                {
                    XmlNode newChildNode = imageHelper.CreateNewFolder(strListName, strYearFolderName);
                    string strNewChildFolderTitle = strYearFolderName + "/" + newChildNode.Attributes["title"].InnerText;

                    #region 文件夹改名
                    string strNewMonthFolder = string.Format("{0:00}", dtDataCreated.Month);
                    int iMonthFolderId = UpdateImageFolderName(strListName, listHelper
                                                    , strNewMonthFolder, strNewChildFolderTitle, strMonthFolderName);
                    #endregion
                }
                #endregion


                #region 实际文件夹
                string strNewFolderName = string.Format("{0:0000}/{1:00}/{2}", dtDataCreated.Year, dtDataCreated.Month, strFolderName);
                SPCostFolder newFolder = GetFolderInfo(strListName, strNewFolderName);
                if (newFolder.ID == 0)
                {
                    XmlNode newChildNode = imageHelper.CreateNewFolder(strListName, strMonthFolderName);
                    string strNewChildFolderTitle = strMonthFolderName + "/" + newChildNode.Attributes["title"].InnerText;

                    #region 文件夹改名
                    iNewFolderId = UpdateImageFolderName(strListName, listHelper
                                                    , strFolderName, strNewChildFolderTitle, strNewFolderName);

                    #endregion
                    newFolder = GetFolderInfo(strListName, iNewFolderId);
                }
                #endregion

                return newFolder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 图片文件上传操作

        /// <summary>
        /// 上传图片到指定的文件夹里面
        /// </summary>
        /// <param name="strFileName">图片名称</param>
        /// <param name="fileData">图片内容</param>
        /// <param name="ListName">图片库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="IsUpload">图片上传是否成功</param>
        /// <param name="strUploadMessage">图片上传的返回信息</param>
        /// <returns></returns>
        public int UploadImageFile(string strFileName, byte[] fileData, string ListName, string FolderName, out bool IsUpload, out string strUploadMessage)
        {
            //获取文件夹信息
            SPCostFolder folder = GetFolderInfo(ListName, FolderName);
            if (folder.ID == 0)
            {
                //文件夹未创建需要创建一个
                folder = ImageCreateNewFolder(ListName, FolderName, DateTime.Now);
            }
            //string strNewFullDocUrl = SPBaseSite + "/" + folder.FileRef + "/" + strFileName;
            #region 拆分字段

            string[] strFolderUrlSplit = folder.FileRef.Split('/');
            string strStartIndexKey = folder.ListUrl;
            int iStartIndex = strFolderUrlSplit.ToList().IndexOf(strStartIndexKey) + 1;
            string strFolderName = string.Empty;
            for (int i = iStartIndex; i < strFolderUrlSplit.Length; i++)
            {
                strFolderName += strFolderUrlSplit[i] + "/";
            }
            strFolderName = strFolderName.Remove(strFolderName.LastIndexOf("/"));

            #endregion

            IsUpload = false;

            Guid? fileId = null;
            UpFileFullUrl = UploadImageFile(strFileName, fileData, strFolderName
                                                           , ListName, folder, out IsUpload, out fileId);

            strUploadMessage = UpFileFullUrl;

            return folder.ID;
        }
        /// <summary>
        /// 上传图片到指定的文件夹里面
        /// </summary>
        /// <param name="strFileName">图片名称</param>
        /// <param name="fileData">图片内容</param>
        /// <param name="ListName">图片库名称</param>
        /// <param name="FolderId">文件夹编号</param>
		/// <param name="IsUpload">图片上传是否成功</param>
        /// <returns></returns>
        public string UploadImageFile(string strFileName, byte[] fileData, string ListName, int FolderId, out bool IsUpload)
        {
            //获取文件夹信息
            SPCostFolder folder = GetFolderInfo(ListName, FolderId);

            #region 拆分字段

            string[] strFolderUrlSplit = folder.FileRef.Split('/');
            string strStartIndexKey = folder.ListUrl;
            int iStartIndex = strFolderUrlSplit.ToList().IndexOf(strStartIndexKey) + 1;
            string strFolderName = string.Empty;
            for (int i = iStartIndex; i < strFolderUrlSplit.Length; i++)
            {
                strFolderName += strFolderUrlSplit[i] + "/";
            }
            strFolderName = strFolderName.Remove(strFolderName.LastIndexOf("/"));

            #endregion

            IsUpload = false;

            Guid? fileId = null;
            UpFileFullUrl = UploadImageFile(strFileName, fileData, strFolderName
                                                           , ListName, folder, out IsUpload, out fileId);
            return UpFileFullUrl;
        }

        #region 图片上传内部操作

        private string UploadImageFile(string strFileName, byte[] fileData
                , string strFolderRef
                , string strListName
                , SPCostFolder folder
                , out bool IsUpload
                , out Guid? fileId
                )
        {
            // 實例化图片库对象
            SPImageWebService.Imaging imageHelper = new SPImageWebService.Imaging()
            {
                Url = FullWebUrl + ImageUrl,
                Credentials = SPCredential
            };


            #region 上传附件
            XmlDocument resdoc = new XmlDocument();
            XmlNode resnode = resdoc.CreateNode(XmlNodeType.Element, "Result", "");
            resnode = imageHelper.Upload(strListName, strFolderRef, fileData, strFileName, true);

            #endregion

            string strFullDocUrl = SPBaseSite + "/" + folder.FileRef + "/" + strFileName;
            fileId = null;
            if (resnode.Name == "Upload")
            {
                //文件上传成功的操作

                IsUpload = true;
                #region 将文件上传信息记录下来

                #region 查询获取刚上传的文件信息
                SPImage docItem = GetImageFolderFile(strFileName, strListName, folder);
                fileId = docItem.UniqueId;
                #endregion
                int iFolderId = 0;
                string strFolderName = string.Empty;
                if (folder != null)
                {
                    iFolderId = folder.ID;
                    strFolderName = folder.FileLeafRef;
                }

                #region 保存到数据库
                AddUpFileLog(strListName, iFolderId, strFolderName, strFileName, docItem.UniqueId, strFullDocUrl, 0, 0, string.Empty, fileData);
                #endregion

                #endregion

                return strFullDocUrl;
            }
            else
            {
                IsUpload = false;
                return resnode.InnerText;
            }
        }

        #endregion

        #endregion

        #region 图片文件删除操作

        /// <summary>
        /// 删除指定文件夹里面的图片文件
        /// </summary>
        /// <param name="ListName">图片库名称</param>
        /// <param name="ImageFileName">图片文件名</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public bool DeleteImageFile(string ListName, string ImageFileName, int iFolderId)
        {
            try
            {
                bool IsDelFile = false;
                // 實例化图片库对象
                SPImageWebService.Imaging imageHelper = new SPImageWebService.Imaging()
                {
                    Url = FullWebUrl + ImageUrl,
                    Credentials = SPCredential
                };
                SPCostFolder newFolder = GetFolderInfo(ListName, iFolderId);
                if (newFolder != null)
                {
                    //string strFolderUrl = "2018/03/F001";
                    #region 拆分字段

                    string[] strFolderUrlSplit = newFolder.FileRef.Split('/');
                    string strStartIndexKey = newFolder.ListUrl;
                    int iStartIndex = strFolderUrlSplit.ToList().IndexOf(strStartIndexKey) + 1;
                    string strFolderFullName = string.Empty;
                    for (int i = iStartIndex; i < strFolderUrlSplit.Length; i++)
                    {
                        strFolderFullName += strFolderUrlSplit[i] + "/";
                    }
                    strFolderFullName = strFolderFullName.Remove(strFolderFullName.LastIndexOf("/"));

                    #endregion

                    XmlNode result = imageHelper.Delete(ListName, strFolderFullName, new string[] { ImageFileName });
                    if(result.Name == "results")
                    {
                        #region 将上传记录标注为已删除
                        DelUpFileLog(ListName, iFolderId, ImageFileName);
                        #endregion
                        IsDelFile = true;
                    }
                }

               
                return IsDelFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region 图片信息获取

        public SPImage GetImageFile(string ListName, string strFileName)
        {
            SPList list = GetListInfo(ListName);

            StringBuilder strSerachXml = new StringBuilder();
            //图片库图片默认名称
            string strTitleName = "名称";
            #region 组合查询条件  
            strSerachXml.Append("<Where>");
            SPListField field = list.Fields.GetField(strTitleName);
            string strFiledName = field.Name;
            strSerachXml.AppendFormat("<Eq><FieldRef Name='{0}'/><Value Type='{2}'>{1}</Value></Eq>", strFiledName, strFileName, field.Type);
            strSerachXml.Append("</Where>");
            #endregion

            //获取文件夹的编号
            // 實例化图片库对象
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };

            XmlDocument xmlDoc = new System.Xml.XmlDocument();

            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            //查询文件列表

            ndViewFields.InnerXml = "<FieldRef Name='ID' />";

            StringBuilder strQueryOptionsXml = new StringBuilder();
            strQueryOptionsXml.Append("<QueryOptions>");
            strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
            strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
            strQueryOptionsXml.Append("<ExpandUserField>True</ExpandUserField>");
            strQueryOptionsXml.Append("</QueryOptions>");

            ndViewFields.InnerXml = "<FieldRef Name='ID' />";


            ndQuery.InnerXml = strSerachXml.ToString();
            ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

            //ndQuery.InnerXml = "";
            //查询对应的文件
            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

            SPImages items = new SPImages();

            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            foreach (XmlNode node in nodes)
            {
                SPImage item = new SPImage(node);
                items.Add(item);
            }
            if (items.Count() > 0)
            {
                return items[0];
            }
            else
            {
                return new SPImage();
            }
        }

        /// <summary>
        /// 按条件获取图片库的图片集合
        /// </summary>
        /// <param name="ListName">图片库名称</param>
        /// <param name="SearchList">查询条件</param>
        /// <param name="OrderList">排序字段（字段名/排序顺序:True=Asc|False=Desc）</param>
        /// <returns></returns>
        public SPImages GetImageFiles(string ListName, SPListSearchs SearchList, Dictionary<string, SPListOrderByEnum> OrderList)
        {
            SPList list = GetListInfo(ListName);

            StringBuilder strSerachXml = new StringBuilder();

            #region 组合查询条件

            List<string> eqList = new List<string>();

            int iIndex = 0;
            foreach (SPListSearch ls in SearchList)
            {
                iIndex++;
                SPListField field = list.Fields.GetField(ls.SearchFieldName);
                string strFiledName = field.Name;
                
                string strFieldCMAL = CAML.FieldRef(strFiledName);
                string strValueCAML = CAML.Value(field.Type, ls.SearchFieldValue);

                #region 判断逻辑

                string strSearchInfo = string.Empty;
                switch (ls.SearchType)
                {
                    case SPListSearchTypeEnum.Eq:
                        strSearchInfo = CAML.Eq(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.Neq:
                        strSearchInfo = CAML.Neq(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.Lt:
                        strSearchInfo = CAML.Lt(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.Leq:
                        strSearchInfo = CAML.Leq(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.Gt:
                        strSearchInfo = CAML.Gt(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.Geq:
                        strSearchInfo = CAML.Geq(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.IsNull:
                        strSearchInfo = CAML.IsNull(strFieldCMAL);
                        break;
                    case SPListSearchTypeEnum.IsNotNull:
                        strSearchInfo = CAML.IsNotNull(strFieldCMAL);
                        break;
                    case SPListSearchTypeEnum.Contains:
                        strSearchInfo = CAML.Contains(strFieldCMAL, strValueCAML);
                        break;
                    case SPListSearchTypeEnum.BeginsWith:
                        strSearchInfo = CAML.BeginsWith(strFieldCMAL, strValueCAML);
                        break;
                    default: break;
                }

                eqList.Add(strSearchInfo);

                #endregion
            }

            #region 按条件数量组合                
            if (eqList.Count > 2)
            {
                #region 超过2个条件需组合And查询

                string strCAML = string.Empty;
                while (eqList.Count >= 3)
                {
                    strCAML = CAML.And(eqList[0], eqList[1]);
                    string strT1 = eqList[0];
                    string strT2 = eqList[1];
                    string strT3 = eqList[2];
                    eqList.Remove(strT1);
                    eqList.Remove(strT2);

                    strCAML = CAML.And(strCAML, strT3);
                    eqList.Remove(strT3);
                }

                if (eqList.Count == 2)
                {
                    strCAML = CAML.And(strCAML, eqList[0]);
                    strCAML = CAML.And(strCAML, eqList[1]);
                }
                else if (eqList.Count == 1)
                {
                    strCAML = CAML.And(strCAML, eqList[0]);
                }

                #endregion
                strSerachXml.Append(CAML.Where(strCAML));
            }
            else if (eqList.Count == 2)
            {
                #region 2个条件以内
                strSerachXml.Append(CAML.Where(
                                                CAML.And(eqList[0], eqList[1])
                                      )
                                    );
                #endregion
            }
            else
            {
                #region 单个条件
                strSerachXml.Append(CAML.Where(eqList[0]));
                #endregion
            }

            #endregion

            #endregion

            #region 组合排序条件
            if (OrderList.Count > 0)
            {
                List<string> strOrderBys = new List<string>();
                foreach (KeyValuePair<string, SPListOrderByEnum> kv in OrderList)
                {
                    SPListField field = list.Fields.GetField(kv.Key);
                    string strFiledName = field.Name;
                    strOrderBys.Add(CAML.FieldRef(strFiledName, kv.Value == SPListOrderByEnum.Desc ? CAML.SortType.Descending : CAML.SortType.Ascending));
                }
                strSerachXml.Append(CAML.OrderBy(strOrderBys.ToArray()));
            }
            #endregion

            #region 旧处理方法

            /*
            #region 组合查询条件 
            if (SearchList.Count > 0)
            {
                strSerachXml.Append("<Where>");
                if (SearchList.Count > 1)
                {
                    strSerachXml.Append("<And>");
                }
                foreach (KeyValuePair<string, string> kv in SearchList)
                {
                    SPCostListField field = list.Fields.GetField(kv.Key);
                    string strFiledName = field.Name;
                    strSerachXml.AppendFormat("<Eq><FieldRef Name='{0}'/><Value Type='{2}'>{1}</Value></Eq>", strFiledName, kv.Value, field.Type);
                }
                if (SearchList.Count > 1)
                {
                    strSerachXml.Append("</And>");
                }
                strSerachXml.Append("</Where>");
            }
            #endregion

            #region 组合排序条件
            if (OrderList.Count() > 0)
            {
                strSerachXml.Append("<OrderBy>");
                foreach (KeyValuePair<string, bool> kv in OrderList)
                {
                    SPCostListField field = list.Fields.GetField(kv.Key);
                    string strFiledName = field.Name;
                    strSerachXml.AppendFormat("<FieldRef Name='{0}' Ascending='{1}'/>", strFiledName, (kv.Value).ToString().ToUpper());
                }
                strSerachXml.Append("</OrderBy>");
            }
            #endregion
            */

            #endregion

            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            XmlDocument xmlDoc = new System.Xml.XmlDocument();

            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            //查询文件列表

            ndViewFields.InnerXml = "<FieldRef Name='ID' />";

            StringBuilder strQueryOptionsXml = new StringBuilder();
            strQueryOptionsXml.Append("<QueryOptions>");
            strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
            strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
            strQueryOptionsXml.Append("<ExpandUserField>True</ExpandUserField>");
            strQueryOptionsXml.Append("</QueryOptions>");

            ndViewFields.InnerXml = "<FieldRef Name='ID' />";


            ndQuery.InnerXml = strSerachXml.ToString();
            ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

            //ndQuery.InnerXml = "";
            //查询对应的文件
            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

            SPImages items = new SPImages();

            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            foreach (XmlNode node in nodes)
            {
                SPImage item = new SPImage(node, list);
                items.Add(item);
            }
            return items;
        }


        /// <summary>
        /// 获取图片库里面的指定图片
        /// </summary>
        /// <param name="strFileName">图片名称</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="iFolderId">存放文件夹编号</param>
        /// <returns></returns>
        public SPImage GetImageFolderFile(string strFileName, string strListName, int iFolderId)
        {
            SPCostFolder folder = GetFolderInfo(strListName, iFolderId);
            return GetImageFolderFile(strFileName, strListName, folder);
        }

        /// <summary>
        /// 获取图片库里面的指定图片
        /// </summary>
        /// <param name="strFileName">图片名称</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="folder">文件夹对象</param>
        /// <returns></returns>
        public SPImage GetImageFolderFile(string strFileName, string strListName, SPCostFolder folder)
        {
            try
            {
                #region 拆分字段

                string[] strFolderUrlSplit = folder.FileRef.Split('/');
                string strStartIndexKey = folder.ListUrl;
                int iStartIndex = strFolderUrlSplit.ToList().IndexOf(strStartIndexKey) + 1;
                string strFolderFullName = string.Empty;
                for (int i = iStartIndex; i < strFolderUrlSplit.Length; i++)
                {
                    strFolderFullName += strFolderUrlSplit[i] + "/";
                }
                strFolderFullName = strFolderFullName.Remove(strFolderFullName.LastIndexOf("/"));

                #endregion

                SPImages items = GetImageFolderAllFiles(strListName, strFolderFullName);

                SPImage image = new SPDocumentWcfService.SPImage();
                foreach (SPImage item in items)
                {
                    if (item.FileLeafRef == strFileName)
                    {
                        image = item;
                        break;
                    }
                }
                return image;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 获取图片库里面指定文件夹所有图片集合
        /// </summary>
        /// <param name="strListName">图片库名称</param>
        /// <param name="strFolderFullName">文件夹名称</param>
        /// <returns></returns>
        public SPImages GetImageFolderFiles(string strListName, string strFolderName)
        {
            try
            {
                SPCostFolder folder = GetFolderInfo(strListName, strFolderName);

                if (folder.ID > 0)
                {
                    #region 拆分字段

                    string[] strFolderUrlSplit = folder.FileRef.Split('/');
                    string strStartIndexKey = folder.ListUrl;
                    int iStartIndex = strFolderUrlSplit.ToList().IndexOf(strStartIndexKey) + 1;
                    string strFolderFullName = string.Empty;
                    for (int i = iStartIndex; i < strFolderUrlSplit.Length; i++)
                    {
                        strFolderFullName += strFolderUrlSplit[i] + "/";
                    }
                    strFolderFullName = strFolderFullName.Remove(strFolderFullName.LastIndexOf("/"));

                    #endregion

                    return GetImageFolderAllFiles(strListName, strFolderFullName);
                }
                else
                {
                    return new SPImages();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取图片库里面指定文件夹所有图片集合
        /// </summary>
        /// <param name="strListName">图片库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public SPImages GetImageFolderFiles(string strListName, int iFolderId)
        {
            try
            {
                SPCostFolder folder = GetFolderInfo(strListName, iFolderId);
                if (folder.ID > 0)
                {
                    #region 拆分字段

                    string[] strFolderUrlSplit = folder.FileRef.Split('/');
                    string strStartIndexKey = folder.ListUrl;
                    int iStartIndex = strFolderUrlSplit.ToList().IndexOf(strStartIndexKey) + 1;
                    string strFolderFullName = string.Empty;
                    for (int i = iStartIndex; i < strFolderUrlSplit.Length; i++)
                    {
                        strFolderFullName += strFolderUrlSplit[i] + "/";
                    }
                    strFolderFullName = strFolderFullName.Remove(strFolderFullName.LastIndexOf("/"));

                    #endregion

                    return GetImageFolderAllFiles(strListName, strFolderFullName);
                }
                else
                {
                    return new SPImages();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取图片库里面指定文件夹所有图片集合内部方法
        /// </summary>
        /// <param name="strListName">图片库名称</param>
        /// <param name="strFolderFullName">文件夹完整路径</param>
        /// <returns></returns>
        private SPImages GetImageFolderAllFiles(string strListName, string strFolderFullName)
        {
            try
            {
                SPImages items = new SPImages();
                // 實例化图片库对象
                SPImageWebService.Imaging imageHelper = new SPImageWebService.Imaging()
                {
                    Url = FullWebUrl + ImageUrl,
                    Credentials = SPCredential
                };
                XmlNode node = imageHelper.GetListItems(strListName, strFolderFullName);
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.LocalName == "row")
                    {
                        SPImage item = new SPImage(childNode);
                        items.Add(item);
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region 列表库相关操作

        #region 更新操作
        /// <summary>
        /// 更新列表库指定列表项字段内容
        /// </summary>
        /// <param name="strListName">列表库名称</param>
        /// <param name="iItemId">更改数据的编号</param>
        /// <param name="updateValue">需要更改的内容</param>
        public void UpdateSPListItem(string strListName, int iItemId, Dictionary<string, string> updateValue)
        {
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            SPList list = GetListInfo(strListName);

            StringBuilder strBatch = new StringBuilder();
            strBatch.AppendFormat("<Method ID='{0}' Cmd='Update'>", iItemId);
            strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", iItemId);
            #region 更新字段
            foreach (KeyValuePair<string, string> kv in updateValue)
            {
                SPListField field = list.Fields.GetField(kv.Key);
                string strFiledName = field.Name;
                strBatch.AppendFormat("<Field Name='{0}'>{1}</Field>", strFiledName, kv.Value);
            }
            #endregion

            strBatch.Append("</Method>");

            XmlDocument xmlUpdateDoc = new System.Xml.XmlDocument();

            System.Xml.XmlElement elBatch = xmlUpdateDoc.CreateElement("Batch");

            elBatch.InnerXml = strBatch.ToString();

            XmlNode ndReturn = listHelper.UpdateListItems(strListName, elBatch);
        }
        #endregion

        #region 查询操作

        /// <summary>
        /// 获取列表库所有的列表项内容
        /// </summary>
        /// <param name="ListName">列表库名称</param>
        /// <returns></returns>
        public SPListItems GetSPListItems(string ListName)
        {
            try
            {

                SPList list = GetListInfo(ListName);

                return GetSPListItems(ListName, list, string.Empty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取列表库里面的指定内容的列表项内容
        /// </summary>
        /// <param name="ListName">列表库名称</param>
        /// <param name="SearchList">查询的字段组合</param>
        /// <returns></returns>
        public SPListItems GetSPListItems(string ListName, SPListSearchs SearchList)
        {
            try
            {

                return GetSPListItems(ListName, SearchList, new Dictionary<string, SPListOrderByEnum>());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取列表库里面的指定内容的列表项内容
        /// </summary>
        /// <param name="ListName">列表库名称</param>
        /// <param name="SearchList">查询的字段组合</param>
        /// <param name="OrderList">需要排序的字段组合（字段名/排序顺序:True=Asc|False=Desc）</param>
        /// <returns></returns>
        public SPListItems GetSPListItems(string ListName, SPListSearchs SearchList,Dictionary<string, SPListOrderByEnum> OrderList)
        {
            try
            {

                SPList list = GetListInfo(ListName);

                StringBuilder strSerachXml = new StringBuilder();
                #region 组合查询条件
                //strSerachXml.Append("<Where>");
                //if (SearchList.Count > 1)
                //{
                //    strSerachXml.Append("<And>");
                //}
                //foreach (KeyValuePair<string, string> kv in SearchList)
                //{
                //    SPCostListField field = list.Fields.GetField(kv.Key);
                //    string strFiledName = field.Name;
                //    strSerachXml.AppendFormat("<Eq><FieldRef Name='{0}'/><Value Type='{2}'>{1}</Value></Eq>", strFiledName, kv.Value, field.Type);
                //}
                //if (SearchList.Count > 1)
                //{
                //    strSerachXml.Append("</And>");
                //}
                //strSerachXml.Append("</Where>");


                List<string> eqList = new List<string>();

                int iIndex = 0;
                foreach (SPListSearch ls in SearchList)
                {
                    iIndex++;
                    SPListField field = list.Fields.GetField(ls.SearchFieldName);
                    string strFiledName = field.Name;

                    string strFieldCMAL = CAML.FieldRef(strFiledName);
                    string strValueCAML = CAML.Value(field.Type, ls.SearchFieldValue);

                    #region 判断逻辑

                    string strSearchInfo = string.Empty;
                    switch (ls.SearchType)
                    {
                        case SPListSearchTypeEnum.Eq:
                            strSearchInfo = CAML.Eq(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.Neq:
                            strSearchInfo = CAML.Neq(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.Lt:
                            strSearchInfo = CAML.Lt(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.Leq:
                            strSearchInfo = CAML.Leq(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.Gt:
                            strSearchInfo = CAML.Gt(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.Geq:
                            strSearchInfo = CAML.Geq(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.IsNull:
                            strSearchInfo = CAML.IsNull(strFieldCMAL);
                            break;
                        case SPListSearchTypeEnum.IsNotNull:
                            strSearchInfo = CAML.IsNotNull(strFieldCMAL);
                            break;
                        case SPListSearchTypeEnum.Contains:
                            strSearchInfo = CAML.Contains(strFieldCMAL, strValueCAML);
                            break;
                        case SPListSearchTypeEnum.BeginsWith:
                            strSearchInfo = CAML.BeginsWith(strFieldCMAL, strValueCAML);
                            break;
                        default:break;
                    }

                    eqList.Add(strSearchInfo);

                    #endregion
                }

                #region 按条件数量组合                
                if (eqList.Count > 2)
                {
                    #region 超过2个条件需组合And查询

                    string strCAML = string.Empty;
                    while (eqList.Count >= 3)
                    {
                        strCAML = CAML.And(eqList[0], eqList[1]);
                        string strT1 = eqList[0];
                        string strT2 = eqList[1];
                        string strT3 = eqList[2];
                        eqList.Remove(strT1);
                        eqList.Remove(strT2);

                        strCAML = CAML.And(strCAML, strT3);
                        eqList.Remove(strT3);
                    }

                    if (eqList.Count == 2)
                    {
                        strCAML = CAML.And(strCAML, eqList[0]);
                        strCAML = CAML.And(strCAML, eqList[1]);
                    }
                    else if (eqList.Count == 1)
                    {
                        strCAML = CAML.And(strCAML, eqList[0]);
                    }

                    #endregion
                    strSerachXml.Append(CAML.Where(strCAML));
                }
                else if (eqList.Count == 2)
                {
                    #region 2个条件以内
                    strSerachXml.Append(CAML.Where(
                                                    CAML.And(eqList[0], eqList[1])
                                          )
                                        );
                    #endregion
                }
                else
                {
                    #region 单个条件
                    strSerachXml.Append(CAML.Where(eqList[0]));
                    #endregion
                }

                #endregion

                #endregion

                #region 组合排序条件
                if (OrderList.Count > 0)
                {
                    List<string> strOrderBys = new List<string>();
                    //strSerachXml.Append("<OrderBy>");
                    foreach (KeyValuePair<string, SPListOrderByEnum> kv in OrderList)
                    {
                        SPListField field = list.Fields.GetField(kv.Key);
                        string strFiledName = field.Name;
                        //strSerachXml.AppendFormat("<FieldRef Name='{0}' Ascending='{1}'/>", strFiledName, kv.Value.ToString().ToUpper());
                        strOrderBys.Add(CAML.FieldRef(strFiledName, kv.Value == SPListOrderByEnum.Desc ? CAML.SortType.Descending : CAML.SortType.Ascending));
                    }
                    //strSerachXml.Append("</OrderBy>");
                    strSerachXml.Append(CAML.OrderBy(strOrderBys.ToArray()));
                }
                #endregion

                return GetSPListItems(ListName, list, strSerachXml.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SPListItems GetSPListItems(string ListName, SPList list, string strSerachXml)
        {
            //获取文件夹的编号
            SPListWebService.Lists listHelper = new SPListWebService.Lists()
            {
                Url = FullWebUrl + ListUrl,
                Credentials = SPCredential
            };
            XmlDocument xmlDoc = new System.Xml.XmlDocument();

            XmlNode ndQuery = xmlDoc.CreateNode(XmlNodeType.Element, "Query", "");
            XmlNode ndViewFields = xmlDoc.CreateNode(XmlNodeType.Element, "ViewFields", "");
            XmlNode ndQueryOptions = xmlDoc.CreateNode(XmlNodeType.Element, "QueryOptions", "");

            //查询文件列表

            ndViewFields.InnerXml = "<FieldRef Name='ID' />";
            
            StringBuilder strQueryOptionsXml = new StringBuilder();
            strQueryOptionsXml.Append("<QueryOptions>");
            strQueryOptionsXml.Append("<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>");
            strQueryOptionsXml.Append("<DateInUtc>TRUE</DateInUtc>");
            strQueryOptionsXml.Append("<ExpandUserField>True</ExpandUserField>");
            strQueryOptionsXml.Append("</QueryOptions>");

            ndViewFields.InnerXml = "<FieldRef Name='ID' />";


            ndQuery.InnerXml = strSerachXml;
            ndQueryOptions.InnerXml = strQueryOptionsXml.ToString();

            //ndQuery.InnerXml = "";
            //查询对应的文件
            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery, null, null, ndQueryOptions, null);

            SPListItems items = new SPListItems();

            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            foreach (XmlNode node in nodes)
            {
                SPListItem item = new SPListItem();
                item.XmlLoad(node, list);

                items.Add(item);
            }

            return items;
        }

        #endregion

        #endregion

        #region 用户信息操作
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userLoginName"></param>
        /// <returns></returns>
        public SPUser GetUserInfo(string userLoginName)
        {
            try
            {
                SPUserGourpWebService.UserGroup userHelper = new SPUserGourpWebService.UserGroup()
                {
                    Url = FullWebUrl + UserGroupUrl,
                    Credentials = SPCredential
                };
                userLoginName = "i:0#.w|" + userLoginName;
                XmlNode ndReturn = userHelper.GetUserInfo(userLoginName);
                SPUser user = new SPUser(ndReturn);
                return user;
            }
            catch (Exception ex)
            {
                return new SPUser();
            }
        }
        #endregion
    }

}