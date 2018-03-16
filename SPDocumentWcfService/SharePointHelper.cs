using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml;

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
                    string strNewFullDocUrl = SPBaseSite + "/" + monthFolder.FileRef + "/" + strFileName;

                    Guid? fileId = null;
                    UpFileFullUrl = UploadFile(strFileName, fileData, strNewFullDocUrl
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

            string strNewFullDocUrl = SPBaseSite + "/" + folder.FileRef + "/" + strFileName;
            IsUpload = false;

            Guid? fileId = null;
            UpFileFullUrl = UploadFile(strFileName, fileData, strNewFullDocUrl
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

            string strNewFullDocUrl = SPBaseSite + "/" + folder.FileRef + "/" + strFileName;

            Guid? fileId = null;
            UpFileFullUrl = UploadFile(strFileName, fileData, strNewFullDocUrl
                                                            , ListName, folder, 0, iPageNum, strDocumentType
                                                            , out IsUpload, out fileId);

            return UpFileFullUrl;
        }

        #endregion

        #region 文件上传内部操作

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


            // 文件存放的路徑
            string destinationUrl = strFullDocUrl;
            string strSourceUrl = "http://void(0)";
            string[] destinationUrls = { destinationUrl };

            SPCopyWebService.FieldInformation info1 = new SPCopyWebService.FieldInformation
            {
                DisplayName = strFileName,
                InternalName = strFileName,
                Type = SPCopyWebService.FieldType.File,
                Value = strFileName
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
                SPCostDocument docItem = GetCostDocument(strFileName, strListName, folder);
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
                AddUpFileLog(strListName, iFolderId, strFolderName, strFileName, docItem.UniqueId, strFullDocUrl, iUserTaskId, iPageNum, strDocumentType, fileData);
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
                SPCostList list = GetListInfo(ListName);
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
                SPCostList list = GetListInfo(ListName);
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
                        try
                        {
                            Data.Folders dbfolder = new Data.Folders()
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
                            Data.FileLogDataClassesDataContext fileDBContext = new Data.FileLogDataClassesDataContext();
                            fileDBContext.Folders.InsertOnSubmit(dbfolder);
                            fileDBContext.SubmitChanges();
                        }
                        catch (Exception ex) { }
                        #endregion
                    }
                    else
                    {
                        throw new Exception("文件夹创建失败：" + resultCreate);
                    }
                }

                return folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                SPCostList list = GetListInfo(strListName);

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
            SPCostList list = GetListInfo(strListName);

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
            SPCostList list = GetListInfo(ListName);
            //查询文件夹
            SPCostFolder folder = GetFolderInfo(ListName, list.RootFolder, FolderName);
            folder.ListUrl = list.ListUrl;
            return folder;
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

            SPCostFolder folder = new SPCostFolder(ndListItems, strSmallFolderName);
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
                return GetFolderInfo(ListName, strParentUrl, strFolderName);
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

                SPCostFolder spfolder = new SPCostFolder(ndListItems);
                SPCostList list = GetListInfo(ListName);
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

            XmlNode ndListItems = listHelper.GetListItems(ListName, null, ndQuery,
                null, null, ndQueryOptions, null);

            SPCostFolders folders = new SPCostFolders(ndListItems);

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

            SPCostFolder folder = new SPCostFolder(ndListItems);
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

                SPCostList list = GetListInfo(strListName);
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

                SPCostList listItem = GetListInfo(ListName);

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

                SPCostList listItem = GetListInfo(ListName);

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
        public SPCostList GetListInfo(string ListName)
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
                SPCostList list = new SPCostList(node);
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

        #region 图片信息获取

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
            SPCostList list = GetListInfo(strListName);

            StringBuilder strBatch = new StringBuilder();
            strBatch.AppendFormat("<Method ID='{0}' Cmd='Update'>", iItemId);
            strBatch.AppendFormat("<Field Name='ID'>{0}</Field>", iItemId);
            #region 更新字段
            foreach (KeyValuePair<string, string> kv in updateValue)
            {
                SPCostListField field = list.Fields.GetField(kv.Key);
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

                SPCostList list = GetListInfo(ListName);

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
        public SPListItems GetSPListItems(string ListName, Dictionary<string, string> SearchList)
        {
            try
            {

                SPCostList list = GetListInfo(ListName);

                #region 组合查询条件
                StringBuilder strSerachXml = new StringBuilder();
                strSerachXml.Append("<Where><And>");
                foreach (KeyValuePair<string, string> kv in SearchList)
                {
                    SPCostListField field = list.Fields.GetField(kv.Key);
                    string strFiledName = field.Name;
                    strSerachXml.AppendFormat("<Eq><FieldRef Name='{0}'/><Value Type='{2}'>{1}</Value></Eq>", strFiledName, kv.Value, field.Type);
                }
                strSerachXml.Append("</And></Where>");
                #endregion

                return GetSPListItems(ListName, list, strSerachXml.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SPListItems GetSPListItems(string ListName, SPCostList list, string strSerachXml)
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
    }

    #region SharePoint实体对象

    #region 基础对象

    /// <summary>
    /// SharePoint实体对象基础类
    /// </summary>
    [DataContract(IsReference = true)]
    public class SPBaseClass
    {
        #region SharePoint相关参数配置
        /// <summary>
        /// SharePoint主站地址
        /// </summary>
        [DataMember]
        public string SPSite
        {
            get;
            set;
        }
        /// <summary>
        /// SharePoint站点地址
        /// </summary>
        [DataMember]
        public string SPWeb
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
        /// 当前站点完整地址
        /// </summary>
        [DataMember]
        public string FullWebUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(_fullWebUrl))
                {
                    return _fullWebUrl;
                }
                else
                {
                    return SPSite + "/" + SPWeb + "/";
                }
            }
            set
            {
                _fullWebUrl = value;
            }
        }
        private string _fullWebUrl;

        /// <summary>
        /// 文件本地存储的根目录
        /// </summary>
        [DataMember]
        public string FileLocalRoot
        {
            get;
            set;
        }
        #endregion

        public SPBaseClass()
        {
            //SPSite = System.Configuration.ConfigurationManager.AppSettings["SPSite"];
            //SPWeb = System.Configuration.ConfigurationManager.AppSettings["SPWeb"];

            //FileLocalRoot = System.Configuration.ConfigurationManager.AppSettings["FileCreateTempDir"];
            //FullWebUrl = SPSite + "/" + SPWeb + "/";
        }
    }
    #endregion

    #region 文档/列表库


    /// <summary>
    /// 对应的SharePoint列表对象
    /// </summary>
    [DataContract]
    public class SPCostList : SPBaseClass
    {
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DocTemplateUrl
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DefaultViewUrl
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MobileDefaultViewUrl
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid ID
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Title
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ImageUrl
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid Name
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int BaseType
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid FeatureId
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime Created
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime Modified
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Version
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RootFolder
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string WebFullUrl
        {
            get;
            internal set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid WebId
        {
            get;
            internal set;
        }


        /// <summary>
        /// 站点地址
        /// </summary>
        [DataMember]
        public string SPSiteUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 完整地址
        /// </summary>
        [DataMember]
        public string ListFullUrl
        {
            get
            {
                return string.Format("{0}{1}", SPSiteUrl, RootFolder);
            }
        }
        #endregion

        #region 构造函数

        public SPCostList()
        {

        }

        internal SPCostList(XmlNode ListNode)
            : this()
        {
            //获取列表库的结构
            DocTemplateUrl = ListNode.Attributes["DocTemplateUrl"].Value;
            DefaultViewUrl = ListNode.Attributes["DefaultViewUrl"].Value;
            MobileDefaultViewUrl = ListNode.Attributes["MobileDefaultViewUrl"].Value;
            ID = new Guid(ListNode.Attributes["ID"].Value);
            Title = ListNode.Attributes["Title"].Value;
            Description = ListNode.Attributes["Description"].Value;
            ImageUrl = ListNode.Attributes["ImageUrl"].Value;
            Name = new Guid(ListNode.Attributes["Name"].Value);
            BaseType = Convert.ToInt32(ListNode.Attributes["BaseType"].Value);
            FeatureId = new Guid(ListNode.Attributes["FeatureId"].Value);
            Version = ListNode.Attributes["Version"].Value;
            RootFolder = ListNode.Attributes["RootFolder"].Value;
            WebFullUrl = ListNode.Attributes["WebFullUrl"].Value;
            WebId = new Guid(ListNode.Attributes["WebId"].Value);

            Fields = new SPCostListFields();
            foreach (XmlNode node in ListNode.FirstChild.ChildNodes)
            {
                SPCostListField field = new SPCostListField(node);
                Fields.Add(field);
            }
        }

        #endregion

        #region 集合属性
        /// <summary>
        /// 字段集合
        /// </summary>
        [DataMember]
        public SPCostListFields Fields
        {
            get;
            set;
        }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 列表的完整路径
        /// </summary>
        [DataMember]
        public string ListUrl
        {
            get
            {
                return RootFolder.Substring(RootFolder.LastIndexOf("/") + 1);
            }
        }

        #endregion
    }

    /// <summary>
    /// 对应的SharePoint列表对象的字段对象
    /// </summary>
    [DataContract]
    public class SPCostListField : SPBaseClass
    {
        #region 属性
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public Guid ID
        {
            get;
            set;
        }
        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember]
        public string DisplayName
        {
            get;
            set;
        }
        /// <summary>
        /// 内部名称
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 字段类型
        /// </summary>
        [DataMember]
        public string Type
        {
            get;
            set;
        }
        /// <summary>
        /// 对应的数据库字段
        /// </summary>
        [DataMember]
        public string ColName
        {
            get;
            set;
        }
        #endregion

        #region 构造函数

        public SPCostListField()
        {

        }
        /// <summary>
        /// 根据Xml创建字段数据
        /// </summary>
        /// <param name="node"></param>
        internal SPCostListField(XmlNode node)
            : this()
        {
            ID = new Guid(node.Attributes["ID"].Value);
            DisplayName = node.Attributes["DisplayName"].Value;
            Name = node.Attributes["Name"].Value;
            Type = node.Attributes["Type"].Value;
            //ColName = node.Attributes["ColName"].Value;
        }

        #endregion
    }

    /// <summary>
    /// 对应的SharePoint列表对象的字段对象集合
    /// </summary>
    [DataContract]
    public class SPCostListFields : List<SPCostListField>
    {
        /// <summary>
        /// 通过显示名称来获取对于的字段数据
        /// </summary>
        /// <param name="strDisplayName">显示名称</param>
        /// <returns></returns>
        public SPCostListField GetField(string strDisplayName)
        {
            foreach (SPCostListField field in this)
            {
                if (field.DisplayName == strDisplayName)
                {
                    return field;
                }
            }
            return null;
        }
    }

    #endregion

    #region 文档

    /// <summary>
    /// 对应的SharePoint文档库里面的文件信息
    /// </summary>
    [DataContract]
    public class SPCostDocument : SPBaseClass
    {
        #region 属性
        /// <summary>
        /// 文件夹编号
        /// </summary>
        [DataMember]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 文件唯一编号
        /// </summary>
        [DataMember]
        public Guid UniqueId
        {
            get;
            set;
        }
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        public string FileLeafRef
        {
            get;
            set;
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        [DataMember]
        public string FileRef
        {
            get;
            set;
        }
        /// <summary>
        /// 文件使用的图标
        /// </summary>
        [DataMember]
        public string DocIcon
        {
            get;
            set;
        }
        /// <summary>
        /// 文件的最后修改时间
        /// </summary>
        [DataMember]
        public DateTime Modified
        {
            get;
            set;
        }
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        public string FileName
        {
            get
            {
                return FileLeafRef;
            }
        }
        /// <summary>
        /// 文件存放地址
        /// </summary>
        [DataMember]
        public string FileUrl
        {
            get
            {
                return FileFullRef;
            }
        }
        #endregion

        #region 内部属性

        /// <summary>
        /// 文件夹地址关键字
        /// </summary>
        private string FolderNameKey = "ows_FileLeafRef";
        /// <summary>
        /// 文件夹地址关键字
        /// </summary>
        private string FolderUrlKey = "ows_FileRef";
        /// <summary>
        /// 编号
        /// </summary>
        private string FolderID = "ows_ID";
        /// <summary>
        /// 唯一编号
        /// </summary>
        private string FolderUniqueId = "ows_UniqueId";
        /// <summary>
        /// 图标
        /// </summary>
        private string FolderDocIcon = "ows_DocIcon";
        /// <summary>
        /// 修改时间
        /// </summary>
        private string FolderModified = "ows_Modified";
        #endregion

        #region 构造函数

        public SPCostDocument()
        {
            _fileLocalUrl = string.Empty;
            UserIsCreate = 0;
            this.FileIsMy = false;
            DataValues = new SPDocumentWcfService.SPListItemDataValues();
        }


        #region 命名空间内方法

        internal SPCostDocument(XmlNode ndListItems, SPCostList list, SPCostFolder folder)
            : this()
        {
            this.SPList = list;
            this.SPFolder = folder;
            this.SPSite = folder.SPSite;
            this.SPWeb = folder.SPWeb;
            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);
            foreach (XmlNode node in nodes)
            {
                CreateDoc(node);
            }
        }

        /// <summary>
        /// 根据分析好的数据产生对应的实体数据
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pageField">特殊的页数字段标志</param>
        internal void XmlLoad(XmlNode node, SPCostListField pageField)
        {
            string strPageFieldName = "ows_" + pageField.Name;
            int iPageNum = 0;
            if (node.Attributes[strPageFieldName] != null)
            {
                string strPageNum = node.Attributes[strPageFieldName].Value;
                //因为数字太长将截取数据
                strPageNum = strPageNum.Remove(strPageNum.IndexOf("."));
                iPageNum = Convert.ToInt32(strPageNum);
            }
            XmlCreateInfo(node, iPageNum);
        }

        /// <summary>
        /// 根据分析好的数据产生对应的实体数据
        /// </summary>
        /// <param name="node"></param>
        /// <param name="pageField">特殊的页数字段标志</param>
        /// <param name="typeField">特殊的附件分类标志</param>
        internal void XmlLoad(XmlNode node, SPCostListField pageField, SPCostListField typeField)
        {
            string strPageFieldName = "ows_" + pageField.Name;
            string strPageNum = node.Attributes[strPageFieldName].Value;
            //因为数字太长将截取数据
            strPageNum = strPageNum.Remove(strPageNum.IndexOf("."));
            int iPageNum = Convert.ToInt32(strPageNum);

            string strTypeFieldName = "ows_" + typeField.Name;
            string strTypeName = node.Attributes[strTypeFieldName].Value;

            XmlCreateInfo(node, iPageNum, strTypeName);
        }

        /// <summary>
        /// 根据分析好的数据产生对应的实体数据
        /// </summary>
        /// <param name="node"></param>
        internal void XmlLoad(XmlNode node)
        {
            int iPageNum = 1;
            XmlCreateInfo(node, iPageNum);

        }

        #endregion

        #region 内部方法

        private void CreateDoc(XmlNode node)
        {
            string strFolderName = node.Attributes[FolderNameKey].Value;
            string strFolderUrl = node.Attributes[FolderUrlKey].Value;
            string strID = node.Attributes[FolderID].Value;
            string strUID = node.Attributes[FolderUniqueId].Value;
            string strIcon = node.Attributes[FolderDocIcon].Value;
            string strModified = node.Attributes[FolderModified].Value;

            ID = Convert.ToInt32(strID);
            UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
            FileLeafRef = strFolderName.Substring(strFolderName.IndexOf("#") + 1);
            FileRef = strFolderUrl.Substring(strFolderUrl.IndexOf("#") + 1);
            DocIcon = strIcon;
            Modified = Convert.ToDateTime(strModified);
            FileFullRef = SPBaseSite + "/" + FileRef;
            FileWebFullRef = SPBaseSite + "/" + FileRef;

            #region 其它属性加入

            foreach (SPCostListField field in SPList.Fields)
            {
                string strFieldName = "ows_" + field.Name;
                if (node.Attributes[strFieldName] != null)
                {
                    string value = node.Attributes[strFieldName].Value;

                    SPListItemDataValue dv = new SPListItemDataValue();
                    dv.DataName = field.DisplayName;
                    dv.DataType = field.Type;
                    dv.DataValue = value;
                    DataValues.Add(dv);
                }
            }

            #endregion
        }

        private void XmlCreateInfo(XmlNode node, int iPageNum)
        {
            CreateDoc(node);
            PageNum = iPageNum;
        }

        private void XmlCreateInfo(XmlNode node, int iPageNum, string strTypeName)
        {
            CreateDoc(node);
            PageNum = iPageNum;
            DocumentType = strTypeName;
        }

        #endregion

        /// <summary>
        /// 判断附件是否存放在明源服务器里面
        /// </summary>
        private bool FileIsMy
        {
            get;
            set;
        }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 删除文件的地址信息
        /// </summary>
        [DataMember]
        public string DelFileFullRef
        {
            get
            {
                string strFileFullRef = SPBaseSite + "/" + FileRef;
                return strFileFullRef;
            }
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        [DataMember]
        public string FileFullRef
        {
            set;
            get;
            //get
            //{
            //    string strFileFullRef = SPBaseSite + "/" + FileRef;

            //    //使用新方法打开地址 Uri.EscapeDataString()
            //    //strFileFullRef = @"\ShowOutFile.ashx?f=" + HttpUtility.UrlEncode(strFileFullRef);
            //    //strFileFullRef = @"\ShowOutFile.ashx?f=" + Uri.EscapeDataString(strFileFullRef);

            //    return strFileFullRef;
            //}
        }
        /// <summary>
        /// 完整路径(在线打开)
        /// </summary>
        [DataMember]
        public string FileWebFullRef
        {
            set;
            get;
            //get
            //{
            //    //?web=1

            //    string strWebFullRef = string.Empty;
            //    if (!FileIsMy)
            //    {
            //        strWebFullRef = SPSite + "/" + FileRef + "";
            //        //strWebFullRef = @"\ShowOutFile.ashx?f=" + Uri.EscapeDataString(strWebFullRef);
            //    }
            //    else
            //    {
            //        strWebFullRef = _fileLocalUrl;
            //    }

            //    //HttpServerUtility pageServer = 
            //    //完整地址使用新的打开地址
            //    //strWebFullRef = @"\ShowOutFile.ashx?f=" + HttpUtility.UrlEncode(strWebFullRef);


            //    return strWebFullRef;
            //}
        }
        /// <summary>
        /// 扩展的页数
        /// </summary>
        [DataMember]
        public int PageNum
        {
            get;
            internal set;
        }
        /// <summary>
        /// 文档类型
        /// </summary>
        [DataMember]
        public string DocumentType
        {
            get;
            internal set;
        }
        /// <summary>
        /// 上传操作用户
        /// </summary>
        [DataMember]
        public string CreateUser
        {
            get;
            set;
        }
        /// <summary>
        /// 上传时间
        /// </summary>
        [DataMember]
        public DateTime Created
        {
            get;
            set;
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        [DataMember]
        public string ModifieUser
        {
            get;
            set;
        }

        /// <summary>
        /// 文档库信息
        /// </summary>
        internal SPCostList SPList
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹信息
        /// </summary>
        internal SPCostFolder SPFolder
        {
            get;
            set;
        }
        /// <summary>
        /// 文件的本地存储地址
        /// </summary>
        private string _fileLocalUrl;

        /// <summary>
        /// 用户是否是创建者（文件上传人）
        /// </summary>
        public int UserIsCreate
        {
            get;
            set;
        }

        /// <summary>
        /// 文件名称（没有文件类型后缀）
        /// </summary>
        [DataMember]
        public string FileLeafName
        {
            get
            {
                string strFileName = FileLeafRef.Substring(0, FileLeafRef.IndexOf('.'));
                return strFileName;
            }
        }
        #endregion

        #region 文档扩展属性集合

        /// <summary>
        /// 列表数据集合
        /// </summary>
        public SPListItemDataValues DataValues
        {
            get; set;
        }

        #endregion
    }
    [DataContract]
    /// <summary>
    /// 对应的SharePoint文档库里面的文件集合
    /// </summary>
    public class SPCostDocuments : List<SPCostDocument>
    {

        #region 内部属性
        /// <summary>
        /// 各个文件的页数字段名
        /// </summary>
        private string PageNumDisplayName = "页数";

        /// <summary>
        /// 文档库存储的文件分类字段名
        /// </summary>
        internal string DocumentTypeDisplayName = "附件分类";

        /// <summary>
        /// 文档库信息
        /// </summary>
        internal SPCostList SPList
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹信息
        /// </summary>
        internal SPCostFolder SPFolder
        {
            get;
            set;
        }

        #endregion

        #region 构造函数

        public SPCostDocuments()
        {

        }

        internal SPCostDocuments(XmlNode ndListItems)
            : this()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            foreach (XmlNode node in nodes)
            {
                SPCostDocument doc = new SPCostDocument();
                doc.XmlLoad(node);
                this.Add(doc);
            }
        }

        internal SPCostDocuments(XmlNode ndListItems, SPCostList listItem)
            : this()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            //取得页数的内部编码
            SPCostListField pageField = listItem.Fields.GetField(PageNumDisplayName);

            //判断有没有附件类型字段
            SPCostListField typeField = listItem.Fields.GetField(DocumentTypeDisplayName);
            this.SPList = listItem;
            this.SPFolder = null;
            #region 读取列表数据

            foreach (XmlNode node in nodes)
            {
                SPCostDocument doc = new SPCostDocument();
                doc.SPList = listItem;
                doc.SPFolder = null;
                doc.SPSite = listItem.SPSite;
                doc.SPWeb = listItem.SPWeb;
                if (typeField != null)
                {
                    doc.XmlLoad(node, pageField, typeField);
                }
                else if (pageField != null)
                {
                    doc.XmlLoad(node, pageField);
                }
                else
                {
                    doc.XmlLoad(node);
                }

                this.Add(doc);
            }

            #endregion
        }


        internal SPCostDocuments(XmlNode ndListItems, SPCostList listItem, SPCostFolder folder)
            : this()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            //取得页数的内部编码
            SPCostListField pageField = listItem.Fields.GetField(PageNumDisplayName);

            //判断有没有附件类型字段
            SPCostListField typeField = listItem.Fields.GetField(DocumentTypeDisplayName);
            this.SPList = listItem;
            this.SPFolder = folder;
            #region 读取列表数据

            foreach (XmlNode node in nodes)
            {
                SPCostDocument doc = new SPCostDocument();
                doc.SPList = listItem;
                doc.SPFolder = folder;
                doc.SPSite = folder.SPSite;
                doc.SPWeb = folder.SPWeb;
                if (typeField != null)
                {
                    doc.XmlLoad(node, pageField, typeField);
                }
                else if (pageField != null)
                {
                    doc.XmlLoad(node, pageField);
                }
                else
                {
                    doc.XmlLoad(node);
                }

                this.Add(doc);
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// 将集合数据用Json方式展示
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式  'HH':'mm':'ss"     
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd";

            string strJson = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, timeConverter);

            return strJson;
        }


    }

    #endregion

    #region 文件夹
    [DataContract(IsReference = true)]
    /// <summary>
    /// 对应的SharePoint文档库里面的文件夹信息
    /// </summary>
    public class SPCostFolder : SPBaseClass
    {
        #region 属性
        [DataMember]
        /// <summary>
        /// 文件夹编号
        /// </summary>
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹唯一编号
        /// </summary>
        [DataMember]
        public Guid UniqueId
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹名称
        /// </summary>
        [DataMember]
        public string FileLeafRef
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹路径
        /// </summary>
        [DataMember]
        public string FileRef
        {
            get;
            set;
        }
        /// <summary>
        /// 上级目录地址
        /// </summary>
        [DataMember]
        public string ParentUrl
        {
            get
            {
                return FileRef.Substring(0, FileRef.LastIndexOf("/"));
            }
        }
        #endregion

        #region 构造函数

        public SPCostFolder()
        {

        }

        internal SPCostFolder(XmlNode ndListItems)
        {
            //文件夹名称关键字
            string FolderNameKey = "ows_FileLeafRef";
            //文件夹地址关键字
            string FolderUrlKey = "ows_FileRef";
            //编号
            string FolderID = "ows_ID";
            //唯一编号
            string FolderUniqueId = "ows_UniqueId";

            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);
            foreach (XmlNode node in nodes)
            {
                string strFolderName = node.Attributes[FolderNameKey].Value;
                string strFolderUrl = node.Attributes[FolderUrlKey].Value;
                string strID = node.Attributes[FolderID].Value;
                string strUID = node.Attributes[FolderUniqueId].Value;

                ID = Convert.ToInt32(strID);
                UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
                FileLeafRef = strFolderName.Substring(strFolderName.IndexOf("#") + 1);
                FileRef = strFolderUrl.Substring(strFolderUrl.IndexOf("#") + 1);
            }
        }

        /// <summary>
        /// 根据文件夹集合来得到指定文件夹
        /// </summary>
        /// <param name="ndListItems"></param>
        /// <param name="strFolderName"></param>
        internal SPCostFolder(XmlNode ndListItems, string FolderName)
        {
            //文件夹名称关键字
            string FolderNameKey = "ows_FileLeafRef";
            //文件夹地址关键字
            string FolderUrlKey = "ows_FileRef";
            //编号
            string FolderID = "ows_ID";
            //唯一编号
            string FolderUniqueId = "ows_UniqueId";

            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);
            foreach (XmlNode node in nodes)
            {
                string strFolderName = node.Attributes[FolderNameKey].Value;
                FileLeafRef = strFolderName.Substring(strFolderName.IndexOf("#") + 1);
                if (FolderName == FileLeafRef)
                {
                    string strFolderUrl = node.Attributes[FolderUrlKey].Value;
                    string strID = node.Attributes[FolderID].Value;
                    string strUID = node.Attributes[FolderUniqueId].Value;

                    ID = Convert.ToInt32(strID);
                    UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
                    FileRef = strFolderUrl.Substring(strFolderUrl.IndexOf("#") + 1);
                    break;
                }
            }
        }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 文档库完整地址
        /// </summary>
        [DataMember]
        public string FileFullRef
        {
            get
            {
                return SPBaseSite + "/" + FileRef;
            }
        }
        /// <summary>
        /// 对应的文档库名称
        /// </summary>
        [DataMember]
        public string ListName
        {
            get;
            set;
        }
        /// <summary>
        /// 对应的库Url
        /// </summary>
        public string ListUrl { get; set; }
        #endregion
    }

    [DataContract]
    public class SPCostFolders : List<SPCostFolder>
    {
        public SPCostFolders()
        {

        }

        internal SPCostFolders(XmlNode ndListItems)
            : this()
        {

            //文件夹名称关键字
            string FolderNameKey = "ows_FileLeafRef";
            //文件夹地址关键字
            string FolderUrlKey = "ows_FileRef";
            //编号
            string FolderID = "ows_ID";
            //唯一编号
            string FolderUniqueId = "ows_UniqueId";

            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);
            foreach (XmlNode node in nodes)
            {

                string strFolderName = node.Attributes[FolderNameKey].Value;
                string strFolderUrl = node.Attributes[FolderUrlKey].Value;
                string strID = node.Attributes[FolderID].Value;
                string strUID = node.Attributes[FolderUniqueId].Value;

                SPCostFolder folder = new SPCostFolder()
                {
                    ID = Convert.ToInt32(strID),
                    UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1)),
                    FileLeafRef = strFolderName.Substring(strFolderName.IndexOf("#") + 1),
                    FileRef = strFolderUrl.Substring(strFolderUrl.IndexOf("#") + 1),
                };
                this.Add(folder);
            }
        }
    }

    #endregion

    #region 列表内容
    [DataContract]
    public class SPListItem
    {
        #region 属性
        /// <summary>
        /// 编号
        /// </summary>
        [DataMember]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 唯一编号
        /// </summary>
        [DataMember]
        public Guid UniqueId
        {
            get;
            set;
        }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        public DateTime Modified
        {
            get;
            set;
        }
        /// <summary>
        /// 列表数据集合
        /// </summary>
        [DataMember]
        public SPListItemDataValues DataValues
        {
            get; set;
        }

        /// <summary>
        /// 列表库信息
        /// </summary>
        internal SPCostList SPList
        {
            get;
            set;
        }

        #endregion

        #region 内部属性
        /// <summary>
        /// 编号
        /// </summary>
        private string ItemID = "ows_ID";
        /// <summary>
        /// 唯一编号
        /// </summary>
        private string ItemUniqueId = "ows_UniqueId";
        /// <summary>
        /// 修改时间
        /// </summary>
        private string ItemModified = "ows_Created";
        #endregion

        #region 构造函数
        public SPListItem()
        {
            DataValues = new SPListItemDataValues();
        }
        #endregion

        #region 内部数据构造
        internal void XmlLoad(XmlNode node, SPCostList listItem)
        {
            this.SPList = listItem;
            CreateDoc(node);
        }

        private void CreateDoc(XmlNode node)
        {

            string strID = node.Attributes[ItemID].Value;
            string strUID = node.Attributes[ItemUniqueId].Value;
            string strModified = node.Attributes[ItemModified].Value;

            ID = Convert.ToInt32(strID);
            UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
            Modified = Convert.ToDateTime(strModified);

            foreach (SPCostListField field in SPList.Fields)
            {
                string strFieldName = "ows_" + field.Name;
                if (node.Attributes[strFieldName] != null)
                {
                    string value = node.Attributes[strFieldName].Value;

                    SPListItemDataValue dv = new SPListItemDataValue();
                    dv.DataName = field.DisplayName;
                    dv.DataType = field.Type;
                    dv.DataValue = value;
                    DataValues.Add(dv);
                }
            }
        }

        #endregion
    }

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPListItem))]
    public class SPListItems : List<SPListItem>
    {
        //private List<SPListItem> items;
        //public SPListItems()
        //{
        //    Items = new List<SPListItem>();
        //}

        //#region 扩展专用方法
        //public int Count()
        //{
        //    return items.Count();
        //}

        //public void Add(SPListItem item)
        //{
        //    items.Add(item);
        //}

        //public void Clear()
        //{
        //    items.Clear();
        //}

        //public SPListItem this[int index]
        //{
        //    get
        //    {
        //        return items[index];
        //    }
        //}
        //#endregion
    }

    [DataContract]
    public class SPListItemDataValue
    {

        #region 属性
        /// <summary>
        /// 内部名称
        /// </summary>
        [DataMember]
        public string DataName
        {
            get;
            set;
        }
        /// <summary>
        /// 字段类型
        /// </summary>
        [DataMember]
        public string DataType
        {
            get;
            set;
        }
        /// <summary>
        /// 对应的数据
        /// </summary>
        [DataMember]
        public string DataValue
        {
            get { return _dataValue; }
            set
            {
                _dataValue = value;
                #region 设置用户集合

                if (DataType.Contains("User"))
                {
                    string[] strSplit1 = DataValue.Split(new string[] { ";#" }, StringSplitOptions.None);
                    if (strSplit1.Length > 0)
                    {
                        foreach (string strSp1 in strSplit1)
                        {
                            int iTemp = 0;
                            if (!int.TryParse(strSp1, out iTemp))
                            {
                                string[] strSplit2 = strSp1.Split(new string[] { ",#" }, StringSplitOptions.None);
                                foreach (string strSp2 in strSplit2)
                                {
                                    if (!string.IsNullOrEmpty(strSp2))
                                    {
                                        string[] strSplit3 = strSp2.Split('|');
                                        if (strSplit3.Length > 1)
                                        {
                                            DomainUserList.Add(strSplit3[1]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }
        }
        private string _dataValue;

        /// <summary>
        /// 如果字段是User类型，可以通过这个属性得到所有用户的域帐号信息
        /// </summary>
        [DataMember]
        public List<string> DomainUserList
        {
            get; set;
        }
        /*
        /// <summary>
        /// 如果字段是User类型，可以通过这个属性得到用户的域帐号信息
        /// </summary>
        [DataMember]
        public string DomainUserName
        {
            get
            {
                if (DataType.Contains("User"))
                {
                    string[] strSplit1 = DataValue.Split(new string[] { ";#" }, StringSplitOptions.None);
                    if (strSplit1.Length > 0)
                    {
                        string[] strSPlit2 = strSplit1[1].Split(new string[] { ",#" }, StringSplitOptions.None);
                        if (strSPlit2.Length > 1)
                        {
                            return strSPlit2[1];
                        }
                        else
                        {
                            return DataValue;
                        }
                    }
                    else
                    {
                        return DataValue;
                    }
                }
                else
                {
                    return DataValue;
                }
            }
            set
            {
                _domainUserName = value;
            }
        }
        private string _domainUserName;
        */
        public SPListItemDataValue()
        {
            DomainUserList = new List<string>();
        }

        //public int SPUserId
        //{

        //}
        #endregion
    }
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPListItemDataValue))]
    public class SPListItemDataValues : List<SPListItemDataValue>
    {
        //private List<SPListItemDataValue> items;
        //public SPListItemDataValues()
        //{
        //    items = new List<SPDocumentWcfService.SPListItemDataValue>();
        //}


        //public void Add(SPListItemDataValue item)
        //{
        //    items.Add(item);
        //}

        //public void Clear()
        //{
        //    items.Clear();
        //}

        //public SPListItemDataValue this[int index]
        //{
        //    get
        //    {
        //        return items[index];
        //    }
        //}
        /*
        /// <summary>
        /// 通过数据名称来得到对应的列表数据
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        [DataMember]
        public SPListItemDataValue this[string strName]
        {
            get
            {
                foreach (SPListItemDataValue item in this)
                {
                    if (item.DataName == strName)
                    {
                        return item;
                    }
                }
                return null;
            }
            set
            {
                foreach (SPListItemDataValue item in this)
                {
                    if (item.DataName == strName)
                    {
                        item.DataValue = value.DataValue;
                    }
                }
            }
        }
        */
    }

    #endregion

    #region 图片对象
    /// <summary>
    /// 对应的SharePoint图片库里面的图片信息
    /// </summary>
    [DataContract(IsReference = true)]
    public class SPImage
    {
        #region 属性
        /// <summary>
        /// 图片编号
        /// </summary>
        [DataMember]
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 图片唯一编号
        /// </summary>
        [DataMember]
        public Guid UniqueId
        {
            get;
            set;
        }
        /// <summary>
        /// 图片名称
        /// </summary>
        [DataMember]
        public string FileLeafRef
        {
            get;
            set;
        }
        /// <summary>
        /// 图片相对路径
        /// </summary>
        [DataMember]
        public string FileRef
        {
            get;
            set;
        }
        /// <summary>
        /// 图片使用的图标
        /// </summary>
        [DataMember]
        public string DocIcon
        {
            get;
            set;
        }
        /// <summary>
        /// 图片的创建时间
        /// </summary>
        [DataMember]
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// 文件的最后修改时间
        /// </summary>
        [DataMember]
        public DateTime Modified
        {
            get;
            set;
        }

        /// <summary>
        /// 图片的完整路径
        /// </summary>
        [DataMember]
        public string EncodedAbsUrl
        {
            get; set;
        }
        #endregion

        #region 内部属性

        /// <summary>
        /// 图片名称内部关键字
        /// </summary>
        private string ImageNameKey = "ows_FileLeafRef";
        /// <summary>
        /// 图片相对地址关键字
        /// </summary>
        private string ImageUrlKey = "ows_FileRef";
        /// <summary>
        /// 编号
        /// </summary>
        private string ImageIDKey = "ows_ID";
        /// <summary>
        /// 唯一编号
        /// </summary>
        private string ImageUniqueIdKey = "ows_UniqueId";
        /// <summary>
        /// 图标
        /// </summary>
        private string ImageDocIconKey = "ows_DocIcon";
        /// <summary>
        /// 添加时间
        /// </summary>
        private string ImageCreatedKey = "ows_Created";
        /// <summary>
        /// 修改时间
        /// </summary>
        private string ImageModifiedKey = "ows_Modified";
        /// <summary>
        /// 图片完整路径关键字
        /// </summary>
        private string EncodedAbsUrlKey = "ows_EncodedAbsUrl";
        #endregion

        #region 构造方法
        public SPImage()
        {

        }

        public SPImage(XmlNode node) : this()
        {
            string strImageName = node.Attributes[ImageNameKey].Value;
            string strImageUrl = node.Attributes[ImageUrlKey].Value;
            string strID = node.Attributes[ImageIDKey].Value;
            string strUID = node.Attributes[ImageUniqueIdKey].Value;
            string strIcon = node.Attributes[ImageDocIconKey].Value;
            string strCreated = node.Attributes[ImageCreatedKey].Value;
            string strModified = node.Attributes[ImageModifiedKey].Value;
            string strAbsUrl = node.Attributes[EncodedAbsUrlKey].Value;

            ID = Convert.ToInt32(strID);
            UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
            FileLeafRef = strImageName.Substring(strImageName.IndexOf("#") + 1);
            FileRef = strImageUrl.Substring(strImageUrl.IndexOf("#") + 1);
            DocIcon = strIcon;
            Created = Convert.ToDateTime(strCreated);
            Modified = Convert.ToDateTime(strModified);
            EncodedAbsUrl = strAbsUrl.Substring(strAbsUrl.IndexOf("#") + 1);
        }
        #endregion
    }

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPImage))]
    public class SPImages : List<SPImage>
    {

        /// <summary>
        /// 将类型集合数据用Json方式展示
        /// </summary>
        /// <returns></returns>
        public string ToJsonString()
        {
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式  'HH':'mm':'ss"     
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd";

            string strJson = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, timeConverter);

            return strJson;
        }


    }
    #endregion

    #endregion

    #region SharePoint相关接口枚举
    /// <summary>
    /// SharePoint接口返回相关数据类型
    /// </summary>
    public enum SPRights
    {
        /// <summary>
        /// 值： 0x00000000。
        /// 在 Web 站点上具有任何权限。通过用户界面不可用。
        /// 组： 不适用。
        /// </summary>
        EmptyMask = 0x00000000,
        /// <summary>
        /// 值： 0x00000001。
        /// 查看列表、 文档库中的文档中的项目并查看 Web 讨论评论。
        /// 组： 读者、 讨论参与者、 网站设计者、 管理员。
        /// </summary>
        ViewListItems = 0x00000001,
        /// <summary>
        /// 值： 0x00000002。
        /// 将项目添加到列表、 将文档添加到文档库，和添加 Web 讨论评论。
        /// 组: 参与者、 网站设计者、 管理员。
        /// </summary>
        AddListItems = 0x00000002,
        /// <summary>
        /// 值: 0x00000004。
        /// 编辑列表中的项目、 编辑文档库中的文档、 编辑文档中的 Web 讨论评论和自定义文档库中的 Web 部件页。
        /// 组: 参与者、 网站设计者、 管理员。
        /// </summary>
        EditListItems = 0x00000004,
        /// <summary>
        /// 值： 0x00000008。
        /// 从一个列表、 文档从文档库中，并在文档中的 Web 讨论评论删除项目。
        /// 组: 参与者，WebDesigner，管理员。
        /// </summary>
        DeleteListItems = 0x00000008,
        /// <summary>
        /// 值： 0x00000100。
        /// 签入文档而不保存当前更改。
        /// 组: WebDesigner，管理员。
        /// </summary>
        CancelCheckout = 0x00000100,
        /// <summary>
        /// 值： 0x00000200。
        /// 创建、 更改和删除列表的个人视图。
        /// 组: 参与者，WebDesigner，管理员。
        /// </summary>
        ManagePersonalViews = 0x00000200,
        /// <summary>
        /// 值： 0x00000800。
        /// 创建和删除列表、 添加或删除列在列表中，并添加或删除列表的公共视图。
        /// 组: WebDesigner，管理员。
        /// </summary>
        ManageLists = 0x00000800,
        /// <summary>
        /// 值： 0x00010000。
        /// 允许用户打开 Web 站点、 列表或文件夹。
        /// 组: 客人、 读卡器、 参与者、 WebDesigner、 管理员。
        /// </summary>
        OpenWeb = 0x00010000,
        /// <summary>
        /// 值： 0x00020000。
        /// 查看 Web 站点中的页面。
        /// 组: 阅读器，参与者，WebDesigner，管理员。
        /// </summary>
        ViewPages = 0x00020000,
        /// <summary>
        /// 值： 0x00040000。
        /// 添加、 更改或删除 HTML 页或 Web 部件页和编辑使用 Windows SharePoint Services–compatible 编辑器的 Web 网站。
        /// 组: WebDesigner，管理员。
        /// </summary>
        AddAndCustomizePages = 0x00040000,
        /// <summary>
        /// 值： 0x00080000。
        /// 将主题或边框应用于整个 Web 站点。
        /// 组: 网站设计者、 管理员。
        /// </summary>
        ApplyThemeAndBorder = 0x00080000,
        /// <summary>
        /// 值： 0x01000000。
        /// 创建一组可以在网站集内的任何位置使用的用户。
        /// 组： 管理员。
        /// </summary>
        ApplyStyleSheets = 0x00100000,
        /// <summary>
        /// 值： 0x00200000。
        /// 查看有关网站的使用率报告。
        /// 组： 管理员。
        /// </summary>
        ViewUsageData = 0x00200000,
        /// <summary>
        /// 值： 0x00400000。
        /// 创建 Web 站点使用自助式网站创建。
        /// 组： 读者、 讨论参与者、 网站设计者、 管理员。
        /// </summary>
        CreateSSCSite = 0x00400000,
        /// <summary>
        /// 值： 0x00800000。
        /// 创建子网站，例如工作组网站、 会议工作区网站和文档工作区网站。
        /// 组： 管理员。
        /// </summary>
        ManageSubwebs = 0x00800000,
        /// <summary>
        /// 值： 0x01000000。
        /// 创建一组可以在网站集内的任何位置使用的用户。
        /// 组： 管理员。
        /// </summary>
        CreatePersonalGroups = 0x01000000,
        /// <summary>
        /// 值： 0x02000000。
        /// 创建、 更改和删除网站用户组，包括将用户添加到网站用户组和 specifyi 的权限分配给网站用户组。
        /// 组： 管理员。
        /// </summary>
        ManageRoles = 0x02000000,
        /// <summary>
        /// 值： 0x04000000。
        /// 通过使用 Microsoft Office SharePoint Designer 2007 和 Web DAV 接口枚举的 Web 站点中文件和文件夹。
        /// 组: 参与者，WebDesigner，管理员。
        /// </summary>
        BrowseDirectories = 0x04000000,
        /// <summary>
        /// 值： 0x08000000。
        /// 查看 web 站点的用户的信息。来宾、 读卡器、 参与者，Web 设计器中，管理员。
        /// </summary>
        BrowseUserInfo = 0x08000000,
        /// <summary>
        /// 值： 0x10000000。
        /// 添加或删除 Web 部件上的个人 Web 部件。
        /// 组: 参与者，WebDesigner，管理员。
        /// </summary>
        AddDelPrivateWebParts = 0x10000000,
        /// <summary>
        /// 值： 0x20000000。
        /// 更新 Web 部件以显示个性化的信息。
        /// 组: 参与者，WebDesigner，管理员。
        /// </summary>
        UpdatePersonalWebParts = 0x20000000,
        /// <summary>
        /// 值： 0x40000000。
        /// 管理站点，包括为该站点执行所有管理任务和管理内容和权限的能力。
        /// 组： 管理员。
        /// </summary>
        ManageWeb = 0x40000000,
        /// <summary>
        /// 值：-1。
        /// 在 Web 站点上具有所有权限。通过用户界面不可用。
        /// 组： 不适用。
        /// </summary>
        FullMask = -1

    }
    #endregion
}