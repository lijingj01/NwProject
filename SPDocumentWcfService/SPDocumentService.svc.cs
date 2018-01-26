using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SPDocumentWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class SPDocumentService : ISPDocumentService
    {

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        #region 文件夹处理方法

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">文档库</param>
        /// <param name="strFolderName">文件夹名称</param>
        /// <param name="dtCreated">日期</param>
        /// <returns></returns>
        public SPWcfFolder CreateSPFolder(SPSetting setting, string strListName, string strFolderName, DateTime dtCreated)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            SPCostFolder folder = docHelper.CreateSPFolder(strListName, strFolderName, dtCreated);
            SPWcfFolder wFolder = new SPDocumentWcfService.SPWcfFolder()
            {
                ID = folder.ID,
                FileRef = folder.FileRef,
                FileLeafRef = folder.FileLeafRef,
                FileFullRef = folder.FileFullRef,
                ListName = folder.ListName,
                ParentUrl = folder.ParentUrl,
                UniqueId = folder.UniqueId
            };
            return wFolder;
        }

        #endregion

        #region 文件上传相关方法

        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderId">文件夹编号</param>
        /// <param name="iPageNum">扩展属性页数</param>
        /// <param name="strDocumentType">附件类型</param>
        /// <param name="IsUpload">文件上传是否成功</param>
        /// <returns>文件的完整地址</returns>
        public string UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, int FolderId, int iPageNum, string strDocumentType, out bool IsUpload)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            string strReturn = docHelper.UploadFile(strFileName, fileData, ListName, FolderId, iPageNum, strDocumentType, out IsUpload);
            return strReturn;
        }

        /// <summary>
        /// 上传文件到指定的文件夹里面（新建文件夹）
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="iPageNum">扩展属性页数</param>
        /// <param name="strDocumentType">附件类型</param>
        /// <param name="IsUpload">文件上传是否成功</param>
        /// <param name="strUploadMessage">文件上传的返回信息</param>
        /// <returns>新文件夹编号</returns>
        public int UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, string FolderName, int iPageNum, string strDocumentType, out bool IsUpload, out string strUploadMessage)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            int iFolderId = docHelper.UploadFile(strFileName, fileData, ListName, FolderName, iPageNum, strDocumentType, out IsUpload, out strUploadMessage);
            return iFolderId;
        }

        /// <summary>
        /// 上传文件到公用文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库</param>
        /// <param name="IsUpload">是否上传成功</param>
        /// <returns></returns>
        public string UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, out bool IsUpload)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            string strReturn = docHelper.UploadFile(strFileName, fileData, ListName,  out IsUpload);
            return strReturn;
        }

        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="IsUpload">文件上传是否成功</param>
        /// <param name="strUploadMessage">文件上传的返回信息</param>
        /// <returns></returns>
        public int UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, string FolderName, out bool IsUpload, out string strUploadMessage)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            int iFolderId = docHelper.UploadFile(strFileName, fileData, ListName, FolderName,  out IsUpload, out strUploadMessage);
            return iFolderId;
        }
        /// <summary>
        /// 上传文件到指定的文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="FolderId">文件夹编号</param>
		/// <param name="IsUpload">文件上传是否成功</param>
        /// <returns></returns>
        public string UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, int FolderId, out bool IsUpload)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            string strReturn = docHelper.UploadFile(strFileName, fileData, ListName, FolderId, out IsUpload);
            return strReturn;
        }

        public string UploadFileTest( FileUploadMessage upfile)
        {
            bool IsUpload = false;
            SPSetting setting = upfile.Setting;
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            //long fileLength = upfile.fileData.Length;
            byte[] fileContents = upfile.fileData; ;

            string strReturn = docHelper.UploadFile(upfile.FileName, fileContents, upfile.ListName, out IsUpload);
            return strReturn;
        }

        #endregion

        #region 文件获取相关方法
        /// <summary>
        /// 获取指定文件夹里面的所有文件集合
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public List<SPWcfDocument> GetFolderDocuments(SPSetting setting, string ListName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            SPCostDocuments docs = docHelper.GetFolderDocuments(ListName, iFolderId);
            List<SPWcfDocument> wcfDocs = new List<SPWcfDocument>();
            foreach(SPCostDocument doc in docs)
            {
                SPWcfDocument wcfDoc = new SPDocumentWcfService.SPWcfDocument()
                {
                    ID = doc.ID,
                    UniqueId = doc.UniqueId,
                    FileLeafRef = doc.FileLeafRef,
                    FileRef = doc.FileRef,
                    DocIcon = doc.DocIcon,
                    DelFileFullRef = doc.DelFileFullRef,
                    FileFullRef = doc.FileFullRef,
                    FileWebFullRef = doc.FileWebFullRef,
                    PageNum = doc.PageNum,
                    DocumentType = doc.DocumentType,
                    CreateUser = doc.CreateUser,
                    Created = doc.Created,
                    ModifieUser = doc.ModifieUser,
                    FileLeafName = doc.FileLeafName
                };
                wcfDocs.Add(wcfDoc);
            }
            return wcfDocs;
        }

        public SPWcfDocument GetFolderDocumentTest(SPSetting setting, string ListName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            SPCostDocuments docs = docHelper.GetFolderDocuments(ListName, iFolderId);
            List<SPWcfDocument> wcfDocs = new List<SPWcfDocument>();
            foreach (SPCostDocument doc in docs)
            {
                SPWcfDocument wcfDoc = new SPDocumentWcfService.SPWcfDocument()
                {
                    ID = doc.ID,
                    UniqueId = doc.UniqueId,
                    FileLeafRef = doc.FileLeafRef,
                    FileRef = doc.FileRef,
                    DocIcon = doc.DocIcon,
                    DelFileFullRef = doc.DelFileFullRef,
                    FileFullRef = doc.FileFullRef,
                    FileWebFullRef = doc.FileWebFullRef,
                    PageNum = doc.PageNum,
                    DocumentType = doc.DocumentType,
                    CreateUser = doc.CreateUser,
                    Created = doc.Created,
                    ModifieUser = doc.ModifieUser,
                    FileLeafName = doc.FileLeafName,
                    FileUrl = doc.FileFullRef,
                    FileName = doc.FileLeafRef
                };
                wcfDocs.Add(wcfDoc);
            }
            return wcfDocs[0];
        }


        #endregion

        
    }
}
