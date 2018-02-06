using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SPDocumentWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ISharePointService”。
    [ServiceContract]
    public interface ISharePointService
    {

        #region 文件操作相关接口
        /// <summary>
        /// 创建指定文件夹
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">文档库名称</param>
        /// <param name="strFolderName">文件夹名称</param>
        /// <param name="dtCreated">时间</param>
        /// <returns></returns>
        [OperationContract]
        SPWcfFolder CreateSPFolder(SPSetting setting, string strListName, string strFolderName, DateTime dtCreated);

        /// <summary>
        /// 文件夹改名操作
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strListName">列表名称</param>
        /// <param name="strOldFolderName">文件夹原名称</param>
        /// <param name="strNewFolderName">文件夹新名称</param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateFolderName(SPSetting setting, string strListName, string strOldFolderName, string strNewFolderName);

        #region 文件上传相关方法


        /// <summary>
        /// 上传文件到公用文件夹里面
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="fileData">文件内容</param>
        /// <param name="ListName">文档库</param>
        /// <param name="IsUpload">是否上传成功</param>
        /// <returns></returns>
        [OperationContract(Name = "UploadFileSmall")]
        string UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, out bool IsUpload);
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
        [OperationContract(Name = "UploadFileSmallInt")]
        int UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, string FolderName, out bool IsUpload, out string strUploadMessage);
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
        [OperationContract(Name = "UploadFileSmallString")]
        string UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, int FolderId, out bool IsUpload);

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
        [OperationContract(Name = "UploadFileFolderInt")]
        int UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, string FolderName, int iPageNum, string strDocumentType, out bool IsUpload, out string strUploadMessage);

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
        [OperationContract(Name = "UploadFileFolderString")]
        string UploadFile(SPSetting setting, string strFileName, byte[] fileData, string ListName, int FolderId, int iPageNum, string strDocumentType, out bool IsUpload);

        #endregion

        #region 文件获取相关方法
        [OperationContract]
        [ServiceKnownType(typeof(SPWcfDocument))]
        List<SPWcfDocument> GetFolderDocuments(SPSetting setting, string ListName, int iFolderId);
        #endregion

        #region 文件删除的方法
        /// <summary>
        /// 删除指定的文件
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">文档库名称</param>
        /// <param name="iFolderId">文件夹编号</param>
        /// <param name="FileName">文件名称</param>
        /// <returns></returns>
        [OperationContract]
        bool DeleteFile(SPSetting setting, string ListName, int iFolderId, string FileName);
        #endregion

        #endregion

        #region 列表库相关操作

        [OperationContract]
        [ServiceKnownType(typeof(SPListItems))]
        SPListItems GetSPListItems(SPSetting setting, string ListName);

        [OperationContract]
        [ServiceKnownType(typeof(SPListItems))]
        SPListItems GetSPListItemsBySearch(SPSetting setting, string ListName, Dictionary<string, string> SearchList);

        [OperationContract]
        void UpdateSPListItem(SPSetting setting, string strListName, int iItemId, Dictionary<string, string> updateValue);
        #endregion
    }
}
