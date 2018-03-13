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

    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    #region 自定义类型
    /// <summary>
    /// SharePoint站点配置类
    /// </summary>
    [DataContract]
    public class SPSetting
    {
        #region 基础数据
        private string _spsite = string.Empty;
        private string _spweb = string.Empty;
        private string _spuserid = string.Empty;
        private string _spuserpwd = string.Empty;
        private string _spuserdomain = string.Empty;
        private string _actionuser = string.Empty;
        #endregion

        #region 公开可序列化数据
        /// <summary>
        /// SharePoint主站点
        /// </summary>
        [DataMember]
        public string SPSite
        {
            get { return _spsite; }
            set { _spsite = value; }
        }
        /// <summary>
        /// SharePoint网站
        /// </summary>
        [DataMember]
        public string SPWeb
        {
            get { return _spweb; }
            set { _spweb = value; }
        }
        /// <summary>
        /// 登录用户帐号
        /// </summary>
        [DataMember]
        public string SPUserId
        {
            get { return _spuserid; }
            set { _spuserid = value; }
        }
        /// <summary>
        /// 登录用户密码
        /// </summary>
        [DataMember]
        public string SPUserPwd
        {
            get { return _spuserpwd; }
            set { _spuserpwd = value; }
        }
        /// <summary>
        /// 登录用户域
        /// </summary>
        [DataMember]
        public string SPUserDomain
        {
            get { return _spuserdomain; }
            set { _spuserdomain = value; }
        }

        /// <summary>
        /// 当前操作用户帐号
        /// </summary>
        [DataMember]
        public string ActionUser
        {
            get { return _actionuser; }
            set { _actionuser = value; }
        }
        #endregion

    }

    /// <summary>
    /// 用来传输的文件夹类
    /// </summary>
    [DataContract]
    public class SPWcfFolder
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
            set; get;
        }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 文档库完整地址
        /// </summary>
        [DataMember]
        public string FileFullRef
        {
            set; get;
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
        #endregion
    }

    [DataContract]
    [KnownType(typeof(SPWcfDocument))]
    public class SPWcfDocument
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
            get;
            set;
        }
        /// <summary>
        /// 文件存放地址
        /// </summary>
        [DataMember]
        public string FileUrl
        {
            get; set;
        }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 删除文件的地址信息
        /// </summary>
        [DataMember]
        public string DelFileFullRef
        {
            get; set;
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        [DataMember]
        public string FileFullRef
        {
            get; set;
        }
        /// <summary>
        /// 完整路径(在线打开)
        /// </summary>
        [DataMember]
        public string FileWebFullRef
        {
            get; set;
        }
        /// <summary>
        /// 扩展的页数
        /// </summary>
        [DataMember]
        public int PageNum
        {
            get;
            set;
        }
        /// <summary>
        /// 文档类型
        /// </summary>
        [DataMember]
        public string DocumentType
        {
            get;
            set;
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
        /// 文件名称（没有文件类型后缀）
        /// </summary>
        [DataMember]
        public string FileLeafName
        {
            get; set;
        }
        #endregion


        #region 文档扩展属性集合

        /// <summary>
        /// 列表数据集合
        /// </summary>
        [DataMember]
        public SPListItemDataValues DataValues
        {
            get; set;
        }

        #endregion
    }
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPWcfDocument))]
    public class SPWcfDocuments : List<SPWcfDocument> { }

    /// <summary>
    /// 用来传输上传的文件流的相关类
    /// </summary>
    [DataContract]
    public class FileUploadMessage
    {
        #region 属性
        /// <summary>
        /// 文件名称
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
        /// <summary>
        /// 文档库名称
        /// </summary>
        [DataMember]
        public string ListName { get; set; }
        /// <summary>
        /// 文件夹编号
        /// </summary>
        [DataMember]
        public int FolderId { get; set; }
        /// <summary>
        /// 文件夹名称
        /// </summary>
        [DataMember]
        public string FolderName { get; set; }
        /// <summary>
        /// 文件内容流
        /// </summary>
        [DataMember]
        public Byte[] fileData { get; set; }
        /// <summary>
        /// 文件页码
        /// </summary>
        [DataMember]
        public int PageNum { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        [DataMember]
        public string DocumentType { get; set; }
        #endregion

        #region 特殊类型
        [DataMember]
        public SPSetting Setting { get; set; }
        #endregion
    }

    #endregion
}