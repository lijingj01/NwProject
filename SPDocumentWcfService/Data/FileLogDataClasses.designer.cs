﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SPDocumentWcfService.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="SharePointFileLogDB")]
	public partial class FileLogDataClassesDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 可扩展性方法定义
    partial void OnCreated();
    partial void InsertFolders(Folders instance);
    partial void UpdateFolders(Folders instance);
    partial void DeleteFolders(Folders instance);
    partial void InsertFiles(Files instance);
    partial void UpdateFiles(Files instance);
    partial void DeleteFiles(Files instance);
    #endregion
		
		public FileLogDataClassesDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["SharePointFileLogDBConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public FileLogDataClassesDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FileLogDataClassesDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FileLogDataClassesDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public FileLogDataClassesDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Folders> Folders
		{
			get
			{
				return this.GetTable<Folders>();
			}
		}
		
		public System.Data.Linq.Table<Files> Files
		{
			get
			{
				return this.GetTable<Files>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Folders")]
	public partial class Folders : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _SPSite;
		
		private string _SPWeb;
		
		private string _ListName;
		
		private int _FolderId;
		
		private string _FolderName;
		
		private System.Guid _FolderUniqueId;
		
		private string _FileLeafRef;
		
		private string _FileRef;
		
		private string _ParentUrl;
		
		private System.DateTime _Created;
		
		private string _CreateUser;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnSPSiteChanging(string value);
    partial void OnSPSiteChanged();
    partial void OnSPWebChanging(string value);
    partial void OnSPWebChanged();
    partial void OnListNameChanging(string value);
    partial void OnListNameChanged();
    partial void OnFolderIdChanging(int value);
    partial void OnFolderIdChanged();
    partial void OnFolderNameChanging(string value);
    partial void OnFolderNameChanged();
    partial void OnFolderUniqueIdChanging(System.Guid value);
    partial void OnFolderUniqueIdChanged();
    partial void OnFileLeafRefChanging(string value);
    partial void OnFileLeafRefChanged();
    partial void OnFileRefChanging(string value);
    partial void OnFileRefChanged();
    partial void OnParentUrlChanging(string value);
    partial void OnParentUrlChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnCreateUserChanging(string value);
    partial void OnCreateUserChanged();
    #endregion
		
		public Folders()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SPSite", DbType="VarChar(200) NOT NULL", CanBeNull=false)]
		public string SPSite
		{
			get
			{
				return this._SPSite;
			}
			set
			{
				if ((this._SPSite != value))
				{
					this.OnSPSiteChanging(value);
					this.SendPropertyChanging();
					this._SPSite = value;
					this.SendPropertyChanged("SPSite");
					this.OnSPSiteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SPWeb", DbType="VarChar(200) NOT NULL", CanBeNull=false)]
		public string SPWeb
		{
			get
			{
				return this._SPWeb;
			}
			set
			{
				if ((this._SPWeb != value))
				{
					this.OnSPWebChanging(value);
					this.SendPropertyChanging();
					this._SPWeb = value;
					this.SendPropertyChanged("SPWeb");
					this.OnSPWebChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ListName", DbType="NVarChar(200) NOT NULL", CanBeNull=false)]
		public string ListName
		{
			get
			{
				return this._ListName;
			}
			set
			{
				if ((this._ListName != value))
				{
					this.OnListNameChanging(value);
					this.SendPropertyChanging();
					this._ListName = value;
					this.SendPropertyChanged("ListName");
					this.OnListNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FolderId", DbType="Int NOT NULL")]
		public int FolderId
		{
			get
			{
				return this._FolderId;
			}
			set
			{
				if ((this._FolderId != value))
				{
					this.OnFolderIdChanging(value);
					this.SendPropertyChanging();
					this._FolderId = value;
					this.SendPropertyChanged("FolderId");
					this.OnFolderIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FolderName", DbType="NVarChar(300) NOT NULL", CanBeNull=false)]
		public string FolderName
		{
			get
			{
				return this._FolderName;
			}
			set
			{
				if ((this._FolderName != value))
				{
					this.OnFolderNameChanging(value);
					this.SendPropertyChanging();
					this._FolderName = value;
					this.SendPropertyChanged("FolderName");
					this.OnFolderNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FolderUniqueId", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid FolderUniqueId
		{
			get
			{
				return this._FolderUniqueId;
			}
			set
			{
				if ((this._FolderUniqueId != value))
				{
					this.OnFolderUniqueIdChanging(value);
					this.SendPropertyChanging();
					this._FolderUniqueId = value;
					this.SendPropertyChanged("FolderUniqueId");
					this.OnFolderUniqueIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileLeafRef", DbType="NVarChar(600) NOT NULL", CanBeNull=false)]
		public string FileLeafRef
		{
			get
			{
				return this._FileLeafRef;
			}
			set
			{
				if ((this._FileLeafRef != value))
				{
					this.OnFileLeafRefChanging(value);
					this.SendPropertyChanging();
					this._FileLeafRef = value;
					this.SendPropertyChanged("FileLeafRef");
					this.OnFileLeafRefChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileRef", DbType="NVarChar(600) NOT NULL", CanBeNull=false)]
		public string FileRef
		{
			get
			{
				return this._FileRef;
			}
			set
			{
				if ((this._FileRef != value))
				{
					this.OnFileRefChanging(value);
					this.SendPropertyChanging();
					this._FileRef = value;
					this.SendPropertyChanged("FileRef");
					this.OnFileRefChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParentUrl", DbType="NVarChar(600) NOT NULL", CanBeNull=false)]
		public string ParentUrl
		{
			get
			{
				return this._ParentUrl;
			}
			set
			{
				if ((this._ParentUrl != value))
				{
					this.OnParentUrlChanging(value);
					this.SendPropertyChanging();
					this._ParentUrl = value;
					this.SendPropertyChanged("ParentUrl");
					this.OnParentUrlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreateUser", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string CreateUser
		{
			get
			{
				return this._CreateUser;
			}
			set
			{
				if ((this._CreateUser != value))
				{
					this.OnCreateUserChanging(value);
					this.SendPropertyChanging();
					this._CreateUser = value;
					this.SendPropertyChanged("CreateUser");
					this.OnCreateUserChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Files")]
	public partial class Files : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _ListName;
		
		private int _FolderId;
		
		private string _FolderName;
		
		private string _FileLeafRef;
		
		private System.Guid _UniqueId;
		
		private string _FileWebFullRef;
		
		private System.DateTime _Created;
		
		private string _CreateUser;
		
		private System.DateTime _Modified;
		
		private string _ModifieUser;
		
		private bool _IsDel;
		
		private int _UserTaskId;
		
		private int _PageNum;
		
		private string _DocumentType;
		
		private System.Data.Linq.Binary _FileData;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnListNameChanging(string value);
    partial void OnListNameChanged();
    partial void OnFolderIdChanging(int value);
    partial void OnFolderIdChanged();
    partial void OnFolderNameChanging(string value);
    partial void OnFolderNameChanged();
    partial void OnFileLeafRefChanging(string value);
    partial void OnFileLeafRefChanged();
    partial void OnUniqueIdChanging(System.Guid value);
    partial void OnUniqueIdChanged();
    partial void OnFileWebFullRefChanging(string value);
    partial void OnFileWebFullRefChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnCreateUserChanging(string value);
    partial void OnCreateUserChanged();
    partial void OnModifiedChanging(System.DateTime value);
    partial void OnModifiedChanged();
    partial void OnModifieUserChanging(string value);
    partial void OnModifieUserChanged();
    partial void OnIsDelChanging(bool value);
    partial void OnIsDelChanged();
    partial void OnUserTaskIdChanging(int value);
    partial void OnUserTaskIdChanged();
    partial void OnPageNumChanging(int value);
    partial void OnPageNumChanged();
    partial void OnDocumentTypeChanging(string value);
    partial void OnDocumentTypeChanged();
    partial void OnFileDataChanging(System.Data.Linq.Binary value);
    partial void OnFileDataChanged();
    #endregion
		
		public Files()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ListName", DbType="NVarChar(32) NOT NULL", CanBeNull=false)]
		public string ListName
		{
			get
			{
				return this._ListName;
			}
			set
			{
				if ((this._ListName != value))
				{
					this.OnListNameChanging(value);
					this.SendPropertyChanging();
					this._ListName = value;
					this.SendPropertyChanged("ListName");
					this.OnListNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FolderId", DbType="Int NOT NULL")]
		public int FolderId
		{
			get
			{
				return this._FolderId;
			}
			set
			{
				if ((this._FolderId != value))
				{
					this.OnFolderIdChanging(value);
					this.SendPropertyChanging();
					this._FolderId = value;
					this.SendPropertyChanged("FolderId");
					this.OnFolderIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FolderName", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string FolderName
		{
			get
			{
				return this._FolderName;
			}
			set
			{
				if ((this._FolderName != value))
				{
					this.OnFolderNameChanging(value);
					this.SendPropertyChanging();
					this._FolderName = value;
					this.SendPropertyChanged("FolderName");
					this.OnFolderNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileLeafRef", DbType="NVarChar(600) NOT NULL", CanBeNull=false)]
		public string FileLeafRef
		{
			get
			{
				return this._FileLeafRef;
			}
			set
			{
				if ((this._FileLeafRef != value))
				{
					this.OnFileLeafRefChanging(value);
					this.SendPropertyChanging();
					this._FileLeafRef = value;
					this.SendPropertyChanged("FileLeafRef");
					this.OnFileLeafRefChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UniqueId", DbType="UniqueIdentifier NOT NULL")]
		public System.Guid UniqueId
		{
			get
			{
				return this._UniqueId;
			}
			set
			{
				if ((this._UniqueId != value))
				{
					this.OnUniqueIdChanging(value);
					this.SendPropertyChanging();
					this._UniqueId = value;
					this.SendPropertyChanged("UniqueId");
					this.OnUniqueIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileWebFullRef", DbType="NVarChar(1000) NOT NULL", CanBeNull=false)]
		public string FileWebFullRef
		{
			get
			{
				return this._FileWebFullRef;
			}
			set
			{
				if ((this._FileWebFullRef != value))
				{
					this.OnFileWebFullRefChanging(value);
					this.SendPropertyChanging();
					this._FileWebFullRef = value;
					this.SendPropertyChanged("FileWebFullRef");
					this.OnFileWebFullRefChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreateUser", DbType="VarChar(32) NOT NULL", CanBeNull=false)]
		public string CreateUser
		{
			get
			{
				return this._CreateUser;
			}
			set
			{
				if ((this._CreateUser != value))
				{
					this.OnCreateUserChanging(value);
					this.SendPropertyChanging();
					this._CreateUser = value;
					this.SendPropertyChanged("CreateUser");
					this.OnCreateUserChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Modified", DbType="DateTime NOT NULL")]
		public System.DateTime Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				if ((this._Modified != value))
				{
					this.OnModifiedChanging(value);
					this.SendPropertyChanging();
					this._Modified = value;
					this.SendPropertyChanged("Modified");
					this.OnModifiedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ModifieUser", DbType="VarChar(32) NOT NULL", CanBeNull=false)]
		public string ModifieUser
		{
			get
			{
				return this._ModifieUser;
			}
			set
			{
				if ((this._ModifieUser != value))
				{
					this.OnModifieUserChanging(value);
					this.SendPropertyChanging();
					this._ModifieUser = value;
					this.SendPropertyChanged("ModifieUser");
					this.OnModifieUserChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IsDel", DbType="Bit NOT NULL")]
		public bool IsDel
		{
			get
			{
				return this._IsDel;
			}
			set
			{
				if ((this._IsDel != value))
				{
					this.OnIsDelChanging(value);
					this.SendPropertyChanging();
					this._IsDel = value;
					this.SendPropertyChanged("IsDel");
					this.OnIsDelChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UserTaskId", DbType="Int NOT NULL")]
		public int UserTaskId
		{
			get
			{
				return this._UserTaskId;
			}
			set
			{
				if ((this._UserTaskId != value))
				{
					this.OnUserTaskIdChanging(value);
					this.SendPropertyChanging();
					this._UserTaskId = value;
					this.SendPropertyChanged("UserTaskId");
					this.OnUserTaskIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PageNum", DbType="Int NOT NULL")]
		public int PageNum
		{
			get
			{
				return this._PageNum;
			}
			set
			{
				if ((this._PageNum != value))
				{
					this.OnPageNumChanging(value);
					this.SendPropertyChanging();
					this._PageNum = value;
					this.SendPropertyChanged("PageNum");
					this.OnPageNumChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DocumentType", DbType="NVarChar(30)")]
		public string DocumentType
		{
			get
			{
				return this._DocumentType;
			}
			set
			{
				if ((this._DocumentType != value))
				{
					this.OnDocumentTypeChanging(value);
					this.SendPropertyChanging();
					this._DocumentType = value;
					this.SendPropertyChanged("DocumentType");
					this.OnDocumentTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileData", DbType="VarBinary(MAX)", UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary FileData
		{
			get
			{
				return this._FileData;
			}
			set
			{
				if ((this._FileData != value))
				{
					this.OnFileDataChanging(value);
					this.SendPropertyChanging();
					this._FileData = value;
					this.SendPropertyChanged("FileData");
					this.OnFileDataChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
