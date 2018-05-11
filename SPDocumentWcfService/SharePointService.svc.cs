using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SPDocumentWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“SharePointService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 SharePointService.svc 或 SharePointService.svc.cs，然后开始调试。
    public class SharePointService : ISharePointService
    {
        #region 列表库操作

        /// <summary>
        /// 获取指定列表库的所有列表项
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">列表库名称</param>
        /// <returns></returns>
        public SPListItems GetSPListItems(SPSetting setting, string ListName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPListItems items = docHelper.GetSPListItems(ListName);
            return items;
            //return new SPDocumentWcfService.SPListItems();
        }
        /// <summary>
        /// 获取指定列表库按条件查询的列表项
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">列表库名称</param>
        /// <param name="SearchList">查询条件（key:字段名/value:字段内容）</param>
        /// <returns></returns>
        public SPListItems GetSPListItemsBySearch(SPSetting setting, string ListName, SPListSearchs SearchList)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPListItems items = docHelper.GetSPListItems(ListName, SearchList);
            return items;
        }

        /// <summary>
        /// 获取指定列表库按条件查询和排序的列表项
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">列表库名称</param>
        /// <param name="SearchList">查询条件（key:字段名/value:字段内容）</param>
        /// <param name="OrderList">需要排序的字段组合（key:字段名/value:排序顺序:True=Asc|False=Desc）</param>
        /// <returns></returns>
        public SPListItems GetSPListItemsBySearchOrder(SPSetting setting, string ListName, SPListSearchs SearchList, Dictionary<string, SPListOrderByEnum> OrderList)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPListItems items = docHelper.GetSPListItems(ListName, SearchList, OrderList);
            return items;
        }


        /// <summary>
        /// 更新列表库指定列表项字段内容
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">列表库名称</param>
        /// <param name="iItemId">更改数据的编号</param>
        /// <param name="updateValue">需要更改的内容</param>
        public void UpdateSPListItem(SPSetting setting, string strListName, int iItemId, Dictionary<string, string> updateValue)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            docHelper.UpdateSPListItem(strListName, iItemId, updateValue);
        }

        #endregion

        #region 文档库操作

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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
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

        /// <summary>
        /// 文件夹改名操作
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">列表名称</param>
        /// <param name="strOldFolderName">文件夹原名称</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        /// <returns></returns>
        public bool UpdateFolderName(SPSetting setting, string strListName, string strOldFolderName, string strNewFolderName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.UpdateFolderName(strListName, strOldFolderName, strNewFolderName);
        }

        /// <summary>
        /// 文件夹改名操作
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">列表名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        /// <returns></returns>
        public bool UpdateFolderNameByID(SPSetting setting, string strListName, int iFolderId, string strNewFolderName)
        {

            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.UpdateFolderName(strListName, iFolderId, strNewFolderName);
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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            string strReturn = docHelper.UploadFile(strFileName, fileData, ListName, out IsUpload);
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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            int iFolderId = docHelper.UploadFile(strFileName, fileData, ListName, FolderName, out IsUpload, out strUploadMessage);
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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            string strReturn = docHelper.UploadFile(strFileName, fileData, ListName, FolderId, out IsUpload);
            return strReturn;
        }

        #endregion

        #region 文件删除相关方法

        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <param name="FileName">文件名称</param>
        /// <returns></returns>
        public bool DeleteFile(SPSetting setting, string ListName, int iFolderId, string FileName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.DeleteFile(ListName, FileName, iFolderId);
        }

        #endregion

        #region 文件获取相关方法
        /// <summary>
        /// 获取指定文件夹里面的所有文件集合(来自数据库)
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public List<SPWcfDocument> GetFolderDocuments(SPSetting setting, string ListName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPCostDocuments docs = docHelper.GetFolderDocuments(ListName, iFolderId);
            List<SPWcfDocument> wcfDocs = new List<SPWcfDocument>();
            foreach (SPCostDocument doc in docs)
            {
                SPWcfDocument wcfDoc = SPDocToWcfDoc(doc);
                wcfDocs.Add(wcfDoc);
            }
            return wcfDocs;
        }

        private static SPWcfDocument SPDocToWcfDoc(SPCostDocument doc)
        {
            return new SPDocumentWcfService.SPWcfDocument()
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
                Modified = doc.Modified,
                ModifieUser = doc.ModifieUser,
                FileLeafName = doc.FileLeafName,
                DataValues = doc.DataValues
            };
        }

        /// <summary>
        /// 获取指定文件夹里面的所有文件集合(来自接口查询)
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public List<SPWcfDocument> GetFolderDocumentsByDB(SPSetting setting, string ListName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPCostDocuments docs = docHelper.GetFolderDocuments(ListName, iFolderId);
            List<SPWcfDocument> wcfDocs = new List<SPWcfDocument>();
            foreach (SPCostDocument doc in docs)
            {
                SPWcfDocument wcfDoc = SPDocToWcfDoc(doc);
                wcfDocs.Add(wcfDoc);
            }
            return wcfDocs;
        }

        /// <summary>
        /// 获取指定文件夹里面的所有文件集合
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="strFolderName">文件夹名称</param>
        /// <returns></returns>
        public List<SPWcfDocument> GetFolderDocumentsByName(SPSetting setting, string ListName, string strFolderName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPCostDocuments docs = docHelper.GetFolderDocuments(ListName, strFolderName);
            List<SPWcfDocument> wcfDocs = new List<SPWcfDocument>();
            foreach (SPCostDocument doc in docs)
            {
                SPWcfDocument wcfDoc = SPDocToWcfDoc(doc);
                wcfDocs.Add(wcfDoc);
            }
            return wcfDocs;
        }

        /// <summary>
        /// 获取指定文档库里面的所有文件集合
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <returns></returns>
        public List<SPWcfDocument> GetFolderAllDocuments(SPSetting setting, string ListName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPCostDocuments docs = docHelper.GetFolderDocuments(ListName);
            List<SPWcfDocument> wcfDocs = new List<SPWcfDocument>();
            foreach (SPCostDocument doc in docs)
            {
                SPWcfDocument wcfDoc = SPDocToWcfDoc(doc);
                wcfDocs.Add(wcfDoc);
            }
            return wcfDocs;
        }

        /// <summary>
        /// 获取指定文件的数据流
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strWebUrl">文件完整地址</param>
        /// <param name="strListName">所在文档库</param>
        /// <returns></returns>
        public byte[] GetWebFileStream(SPSetting setting, string strWebUrl, string strListName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            byte[] fileContents = docHelper.GetWebFileStream(strWebUrl, strListName);
            return fileContents;
        }
        #endregion


        #endregion

        #region 图片库操作方法

        #region 图片库文件夹操作
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        /// <returns></returns>
        public bool UpdateImageFolderName(SPSetting setting, string strListName, int iFolderId, string strNewFolderName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.UpdateImageFolderName(strListName, iFolderId, strNewFolderName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">文档库名称</param>
        /// <param name="strFolderName">文件夹名称</param>
        /// <param name="dtDataCreated">创建时间</param>
        /// <returns></returns>
        public SPWcfFolder ImageCreateNewFolder(SPSetting setting, string strListName, string strFolderName, DateTime dtDataCreated)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            SPCostFolder folder = docHelper.ImageCreateNewFolder(strListName, strFolderName, dtDataCreated);
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

        #region 图片上传操作
        /// <summary>
        /// 上传图片到指定的文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">图片名称</param>
        /// <param name="fileData">图片内容</param>
        /// <param name="ListName">图片库名称</param>
        /// <param name="FolderName">文件夹名称</param>
        /// <param name="IsUpload">图片上传是否成功</param>
        /// <param name="strUploadMessage">图片上传的返回信息</param>
        /// <returns></returns>
        public int UploadImageFileByName(SPSetting setting, string strFileName, byte[] fileData, string ListName, string FolderName, out bool IsUpload, out string strUploadMessage)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.UploadImageFile(strFileName, fileData, ListName, FolderName, out IsUpload, out strUploadMessage);
        }
        /// <summary>
        /// 上传图片到指定的文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">图片名称</param>
        /// <param name="fileData">图片内容</param>
        /// <param name="ListName">图片库名称</param>
        /// <param name="FolderId">文件夹编号</param>
		/// <param name="IsUpload">图片上传是否成功</param>
        /// <returns></returns>
        public string UploadImageFileById(SPSetting setting, string strFileName, byte[] fileData, string ListName, int FolderId, out bool IsUpload)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.UploadImageFile(strFileName, fileData, ListName, FolderId, out IsUpload);
        }
        #endregion

        #region 图片信息获取
        /// <summary>
        /// 获取图片库里面的指定图片
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public SPImages GetImageFolderFilesById(SPSetting setting, string strListName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.GetImageFolderFiles(strListName, iFolderId);
        }
        /// <summary>
        /// 获取图片库里面的指定图片
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="strFolderFullName">文件夹名称</param>
        /// <returns></returns>
        public SPImages GetImageFolderFilesByName(SPSetting setting, string strListName, string strFolderName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.GetImageFolderFiles(strListName, strFolderName);
        }
        /// <summary>
        /// 获取图片库里面的指定图片
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">图片名称</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="iFolderId">存放文件夹编号</param>
        /// <returns></returns>
        public SPImage GetImageFolderFile(SPSetting setting, string strFileName, string strListName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.GetImageFolderFile(strFileName, strListName, iFolderId);
        }

        /// <summary>
        /// 按查询条件和排序规则查询图片库图片集合
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">图片库名称</param>
        /// <param name="SearchList">查询条件</param>
        /// <param name="OrderList">排序条件</param>
        /// <returns></returns>
        public SPImages GetImageFilesBySearchOrder(SPSetting setting, string strListName, SPListSearchs SearchList, Dictionary<string, SPListOrderByEnum> OrderList)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.GetImageFiles(strListName, SearchList, OrderList);
        }


        #endregion

        #region 删除图片文件

        /// <summary>
        /// 删除指定文件夹里面的图片文件
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">图片库名称</param>
        /// <param name="ImageFileName">图片文件名</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <returns></returns>
        public bool DeleteImageFile(SPSetting setting, string ListName, string ImageFileName, int iFolderId)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.DeleteImageFile(ListName, ImageFileName, iFolderId);
        }


        #endregion


        #endregion

        #region 列表文档库
        /// <summary>
        /// 获取指定列表或文档库的信息
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">列表库名称</param>
        /// <returns></returns>
        public SPList GetSPListInfo(SPSetting setting, string ListName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb, setting.ActionUser);
            return docHelper.GetListInfo(ListName);
        }

        #endregion

        public SPListSearch GetFileTypeTest(string value)
        {
            return new SPDocumentWcfService.SPListSearch();
        }

        public SPListFields GetTest1(SPSetting setting)
        {
            return new SPDocumentWcfService.SPListFields();
        }
    }
}
