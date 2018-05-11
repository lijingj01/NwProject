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

    #region SharePoint实体对象

    #region 基础对象

    /// <summary>
    /// SharePoint实体对象基础类
    /// </summary>
    [DataContract]
    public class SPBaseClass
    {
        #region SharePoint相关参数配置
        private string _spsite = string.Empty;
        /// <summary>
        /// SharePoint主站地址
        /// </summary>
        [DataMember]
        public string SPSite
        {
            get
            {
                return _spsite;
            }
            set
            {
                _spsite = value;

                #region 设置参数
                string[] strBaseSites = SPSite.Split('/');
                string strBaseSite = string.Format("{0}//{1}{2}", strBaseSites[0], strBaseSites[1], strBaseSites[2]);
                SPBaseSite = strBaseSite;
                #endregion
            }
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
        [DataMember]
        public string SPBaseSite
        {
            get;
            set;
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
    public class SPList :SPBaseClass
    {

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DocTemplateUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DefaultViewUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string MobileDefaultViewUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid ID
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ImageUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid Name
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int BaseType
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid FeatureId
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime Created
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime Modified
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Version
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RootFolder
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string WebFullUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Guid WebId
        {
            get;
            set;
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
            get;set;
        }
        #endregion

        #region 构造函数

        public SPList()
        {

        }

        internal SPList(XmlNode ListNode)
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

            Fields = new SPListFields();
            foreach (XmlNode node in ListNode.FirstChild.ChildNodes)
            {
                SPListField field = new SPListField(node);
                Fields.Add(field);
            }

            ListFullUrl = string.Format("{0}{1}", SPSiteUrl, RootFolder);
            ListUrl = RootFolder.Substring(RootFolder.LastIndexOf("/") + 1);
        }

        #endregion

        #region 集合属性
        /// <summary>
        /// 字段集合
        /// </summary>
        [DataMember]
        public SPListFields Fields
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
            get;set;
        }

        #endregion
    }

    /// <summary>
    /// 对应的SharePoint列表对象的字段对象
    /// </summary>
    [DataContract]
    public class SPListField  
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

        public SPListField()
        {

        }
        /// <summary>
        /// 根据Xml创建字段数据
        /// </summary>
        /// <param name="node"></param>
        public SPListField(XmlNode node)
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
    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPListField))]
    public class SPListFields : List<SPListField>
    {
        /// <summary>
        /// 通过显示名称来获取对于的字段数据
        /// </summary>
        /// <param name="strDisplayName">显示名称</param>
        /// <returns></returns>
        public SPListField GetField(string strDisplayName)
        {
            foreach (SPListField field in this)
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

        internal SPCostDocument(XmlNode ndListItems, SPList list, SPCostFolder folder)
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
        internal void XmlLoad(XmlNode node, SPListField pageField)
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
        internal void XmlLoad(XmlNode node, SPListField pageField, SPListField typeField)
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

            foreach (SPListField field in SPList.Fields)
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
        internal SPList SPList
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
        internal SPList SPList
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

        internal SPCostDocuments(XmlNode ndListItems, SPList listItem)
            : this()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            //取得页数的内部编码
            SPListField pageField = listItem.Fields.GetField(PageNumDisplayName);

            //判断有没有附件类型字段
            SPListField typeField = listItem.Fields.GetField(DocumentTypeDisplayName);
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


        internal SPCostDocuments(XmlNode ndListItems, SPList listItem, SPCostFolder folder)
            : this()
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(ndListItems.OwnerDocument.NameTable);
            ns.AddNamespace("rs", "urn:schemas-microsoft-com:rowset");
            ns.AddNamespace("z", "#RowsetSchema");
            XmlNodeList nodes = ndListItems.SelectNodes(@"//z:row", ns);

            //取得页数的内部编码
            SPListField pageField = listItem.Fields.GetField(PageNumDisplayName);

            //判断有没有附件类型字段
            SPListField typeField = listItem.Fields.GetField(DocumentTypeDisplayName);
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
        /// 列表项内容简易数据
        /// </summary>
        [DataMember]
        public Dictionary<string, string> DataItems { get; set; }

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
        internal SPList SPList
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
        private string ItemModified = "ows_Modified";
        /// <summary>
        /// 创建时间
        /// </summary>
        private string ItemCreated = "ows_Created";
        #endregion

        #region 构造函数
        public SPListItem()
        {
            DataItems = new Dictionary<string, string>();
            DataValues = new SPListItemDataValues();
        }
        #endregion

        #region 内部数据构造
        internal void XmlLoad(XmlNode node, SPList listItem)
        {
            this.SPList = listItem;
            CreateDoc(node);
        }

        private void CreateDoc(XmlNode node)
        {

            string strID = node.Attributes[ItemID].Value;
            string strUID = node.Attributes[ItemUniqueId].Value;
            string strModified = DateTime.Now.ToString();
            if (node.Attributes[ItemModified] != null)
            {
                strModified = node.Attributes[ItemModified].Value;
            }
            else if (node.Attributes[ItemCreated] != null)
            {
                strModified = node.Attributes[ItemCreated].Value;
            }

            ID = Convert.ToInt32(strID);
            UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
            Modified = Convert.ToDateTime(strModified);

            foreach (SPListField field in SPList.Fields)
            {
                string strFieldName = "ows_" + field.Name;
                if (node.Attributes[strFieldName] != null)
                {
                    if (!DataItems.ContainsKey(field.DisplayName))
                    {
                        string value = node.Attributes[strFieldName].Value;

                        SPListItemDataValue dv = new SPListItemDataValue();
                        dv.DataName = field.DisplayName;
                        dv.DataType = field.Type;
                        dv.DataValue = value;
                        DataValues.Add(dv);
                        //加入键值对
                        #region 特殊字段处理
                        string strDataValue = dv.DataValue;
                        if (dv.DataType == "URL")
                        {
                            string[] strDvs = strDataValue.Split(',');
                            if (strDvs.Length > 0)
                            {
                                strDataValue = strDvs[0];
                            }
                        }
                        //加入键值对
                        DataItems.Add(dv.DataName, strDataValue);

                        #endregion
                    }
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


    #region 查询专用

    [DataContract]
    /// <summary>
    /// 列表库查询对象
    /// </summary>
    public class SPListSearch
    {
        #region 属性
        /// <summary>
        /// 查询的字段
        /// </summary>
        [DataMember]
        public string SearchFieldName { get; set; }
        /// <summary>
        /// 查询的条件
        /// </summary>
        [DataMember]
        public string SearchFieldValue { get; set; }

        /// <summary>
        /// 查询的判断逻辑
        /// </summary>
        [DataMember]
        public SPListSearchTypeEnum SearchType { get; set; }
        #endregion
    }

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPListSearch))]
    public class SPListSearchs : List<SPListSearch>
    {

    }


    #endregion

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

        /// <summary>
        /// 图片的缩微图路径地址
        /// </summary>
        [DataMember]
        public string ImageSmallUrl { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 图片简称，没有后缀
        /// </summary>
        [DataMember]
        public string FileSmallName
        {
            get;
            set;
        }
        /// <summary>
        /// 图片库信息
        /// </summary>
        internal SPList SPList
        {
            get;
            set;
        }
        /// <summary>
        /// 列表项内容简易数据
        /// </summary>
        [DataMember]
        public Dictionary<string, string> DataItems { get; set; }
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
            ID = 0;
            DataItems = new Dictionary<string, string>();
        }

        public SPImage(XmlNode node) : this()
        {

            string strImageName = node.Attributes[ImageNameKey].Value;
            string strImageUrl = node.Attributes[ImageUrlKey].Value;
            string strID = node.Attributes[ImageIDKey].Value;
            string strUID = node.Attributes[ImageUniqueIdKey].Value;
            string strIcon = node.Attributes[ImageDocIconKey].Value;
            string strModified = node.Attributes[ImageModifiedKey].Value;

            ID = Convert.ToInt32(strID);
            UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
            FileLeafRef = strImageName.Substring(strImageName.IndexOf("#") + 1);
            FileRef = strImageUrl.Substring(strImageUrl.IndexOf("#") + 1);
            DocIcon = strIcon;
            Modified = Convert.ToDateTime(strModified);

            if (node.Attributes[ImageCreatedKey] != null)
            {
                string strCreated = node.Attributes[ImageCreatedKey].Value;
                Created = Convert.ToDateTime(strCreated);
            }
            else
            {
                Created = Modified;
            }

            if (node.Attributes[EncodedAbsUrlKey] != null)
            {
                string strAbsUrl = node.Attributes[EncodedAbsUrlKey].Value;
                EncodedAbsUrl = strAbsUrl.Substring(strAbsUrl.IndexOf("#") + 1);
            }
            else
            {
                EncodedAbsUrl = FileRef;
            }

            #region 定义图片的缩微图地址
            string strSmallFolder = "_w";
            string strNewFileName = strSmallFolder + "/" + FileLeafRef.Replace("." + DocIcon, "_" + DocIcon) + "." + DocIcon;
            ImageSmallUrl = EncodedAbsUrl.Replace(FileLeafRef, strNewFileName);

            #endregion

            this.FileSmallName = this.FileLeafRef.Replace("." + this.DocIcon, "");
        }

        public SPImage(XmlNode node, SPList list) : this()
        {
            this.SPList = list;
            string strImageName = node.Attributes[ImageNameKey].Value;
            string strImageUrl = node.Attributes[ImageUrlKey].Value;
            string strID = node.Attributes[ImageIDKey].Value;
            string strUID = node.Attributes[ImageUniqueIdKey].Value;
            string strIcon = node.Attributes[ImageDocIconKey].Value;
            string strModified = node.Attributes[ImageModifiedKey].Value;

            ID = Convert.ToInt32(strID);
            UniqueId = new Guid(strUID.Substring(strUID.IndexOf("#") + 1));
            FileLeafRef = strImageName.Substring(strImageName.IndexOf("#") + 1);
            FileRef = strImageUrl.Substring(strImageUrl.IndexOf("#") + 1);
            DocIcon = strIcon;
            Modified = Convert.ToDateTime(strModified);

            if (node.Attributes[ImageCreatedKey] != null)
            {
                string strCreated = node.Attributes[ImageCreatedKey].Value;
                Created = Convert.ToDateTime(strCreated);
            }
            else
            {
                Created = Modified;
            }

            if (node.Attributes[EncodedAbsUrlKey] != null)
            {
                string strAbsUrl = node.Attributes[EncodedAbsUrlKey].Value;
                EncodedAbsUrl = strAbsUrl.Substring(strAbsUrl.IndexOf("#") + 1);
            }
            else
            {
                EncodedAbsUrl = this.SPList.SPBaseSite + "/" + FileRef;
            }

            #region 扩展属性
            foreach (SPListField field in SPList.Fields)
            {
                string strFieldName = "ows_" + field.Name;
                if (node.Attributes[strFieldName] != null)
                {
                    if (!DataItems.ContainsKey(field.DisplayName))
                    {
                        string value = node.Attributes[strFieldName].Value;
                        #region 特殊字段处理

                        string strDataValue = value;
                        if (field.Type == "URL")
                        {
                            string[] strDvs = strDataValue.Split(',');
                            if (strDvs.Length > 0)
                            {
                                strDataValue = strDvs[0];
                            }
                        }
                        //加入键值对
                        DataItems.Add(field.DisplayName, strDataValue);

                        #endregion
                    }
                }
            }
            #endregion

            #region 定义图片的缩微图地址
            string strSmallFolder = "_w";
            string strNewFileName = strSmallFolder + "/" + FileLeafRef.Replace("." + DocIcon, "_" + DocIcon) + "." + DocIcon;
            ImageSmallUrl = EncodedAbsUrl.Replace(FileLeafRef, strNewFileName);

            #endregion

            this.FileSmallName = this.FileLeafRef.Replace("." + this.DocIcon, "");
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

    #region 用户对象

    [DataContract]
    public class SPUser
    {
        #region 用户属性

        /// <summary>
        /// 用户编号
        /// </summary>
        [DataMember]
        public int UserID { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 用户邮箱地址
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// 用户登陆账号
        /// </summary>
        [DataMember]
        public string LoginName { get; set; }

        #endregion

        #region 构造函数
        public SPUser()
        {
            UserID = 0;
            UserName = string.Empty;
            Email = string.Empty;
            LoginName = string.Empty;
        }

        internal SPUser(XmlNode ndUserInfo) : this()
        {
            if (ndUserInfo.ChildNodes.Count > 0)
            {
                XmlNode cnode = ndUserInfo.ChildNodes[0];

                UserID = Convert.ToInt32(cnode.Attributes["ID"].Value);
                UserName = cnode.Attributes["Name"].Value;
                Email = cnode.Attributes["Email"].Value;

                string strLogin = cnode.Attributes["LoginName"].Value;
                if (strLogin.IndexOf("|") > -1)
                {
                    string[] strLoginList = strLogin.Split('|');
                    if (strLoginList.Length > 1)
                    {
                        LoginName = strLoginList[1];
                    }
                }
                else
                {
                    LoginName = strLogin;
                }
            }
        }
        #endregion
    }

    [Serializable]
    [CollectionDataContract]
    [KnownType(typeof(SPUser))]
    public class SPUsers : List<SPUser>
    {

    }

    #endregion

    #endregion

    #region SharePoint相关接口枚举

    [DataContract]
    [Flags]
    /// <summary>
    /// 列表库排序枚举
    /// </summary>
    public enum SPListOrderByEnum
    {
        /// <summary>
        /// 顺序排列
        /// </summary>
        [EnumMember]
        Asc = 1,
        /// <summary>
        /// 倒序排列
        /// </summary>
        [EnumMember]
        Desc = 0
    }

    /// <summary>
    /// 列表查询判断条件类型
    /// </summary>
    [DataContract]
    [Flags]
    public enum SPListSearchTypeEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        [EnumMember]
        Eq = 0,
        /// <summary>
        /// 不等于
        /// </summary>
        [EnumMember]
        Neq = 1,
        /// <summary>
        /// 小于
        /// </summary>
        [EnumMember]
        Lt = 2,
        /// <summary>
        /// 小于等于
        /// </summary>
        [EnumMember]
        Leq = 3,
        /// <summary>
        /// 大于
        /// </summary>
        [EnumMember]
        Gt = 4,
        /// <summary>
        /// 大于等于
        /// </summary>
        [EnumMember]
        Geq = 5,
        /// <summary>
        /// 包含
        /// </summary>
        [EnumMember]
        Contains = 6,
        /// <summary>
        /// 以某字符串开头
        /// </summary>
        [EnumMember]
        BeginsWith = 7,
        /// <summary>
        /// 在集合范围内
        /// </summary>
        [EnumMember]
        In = 8,
        /// <summary>
        /// 为空
        /// </summary>
        [EnumMember]
        IsNull = 9,
        /// <summary>
        /// 不为空
        /// </summary>
        [EnumMember]
        IsNotNull = 10,
        /// <summary>
        /// 属于用户组
        /// </summary>
        [EnumMember]
        Membership = 11
    }

    [DataContract]
    [Flags]
    public enum FileType
    {

        [EnumMember]
        Audio = 0,
        [EnumMember]
        Video = 1,
        [EnumMember]
        Picture = 2,
        [EnumMember]
        Other = 3
    }

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