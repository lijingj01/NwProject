﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace SPDocumentWcfService.SPDwsWebService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="DwsSoap", Namespace="http://schemas.microsoft.com/sharepoint/soap/dws/")]
    public partial class Dws : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CreateDwsOperationCompleted;
        
        private System.Threading.SendOrPostCallback DeleteDwsOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDwsMetaDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDwsDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback UpdateDwsDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback RemoveDwsUserOperationCompleted;
        
        private System.Threading.SendOrPostCallback RenameDwsOperationCompleted;
        
        private System.Threading.SendOrPostCallback FindDwsDocOperationCompleted;
        
        private System.Threading.SendOrPostCallback CanCreateDwsUrlOperationCompleted;
        
        private System.Threading.SendOrPostCallback CreateFolderOperationCompleted;
        
        private System.Threading.SendOrPostCallback DeleteFolderOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Dws() {
            this.Url = global::SPDocumentWcfService.Properties.Settings.Default.SPDocumentWcfService_SPDwsWebService_Dws;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event CreateDwsCompletedEventHandler CreateDwsCompleted;
        
        /// <remarks/>
        public event DeleteDwsCompletedEventHandler DeleteDwsCompleted;
        
        /// <remarks/>
        public event GetDwsMetaDataCompletedEventHandler GetDwsMetaDataCompleted;
        
        /// <remarks/>
        public event GetDwsDataCompletedEventHandler GetDwsDataCompleted;
        
        /// <remarks/>
        public event UpdateDwsDataCompletedEventHandler UpdateDwsDataCompleted;
        
        /// <remarks/>
        public event RemoveDwsUserCompletedEventHandler RemoveDwsUserCompleted;
        
        /// <remarks/>
        public event RenameDwsCompletedEventHandler RenameDwsCompleted;
        
        /// <remarks/>
        public event FindDwsDocCompletedEventHandler FindDwsDocCompleted;
        
        /// <remarks/>
        public event CanCreateDwsUrlCompletedEventHandler CanCreateDwsUrlCompleted;
        
        /// <remarks/>
        public event CreateFolderCompletedEventHandler CreateFolderCompleted;
        
        /// <remarks/>
        public event DeleteFolderCompletedEventHandler DeleteFolderCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/CreateDws", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CreateDws(string name, string users, string title, string documents) {
            object[] results = this.Invoke("CreateDws", new object[] {
                        name,
                        users,
                        title,
                        documents});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CreateDwsAsync(string name, string users, string title, string documents) {
            this.CreateDwsAsync(name, users, title, documents, null);
        }
        
        /// <remarks/>
        public void CreateDwsAsync(string name, string users, string title, string documents, object userState) {
            if ((this.CreateDwsOperationCompleted == null)) {
                this.CreateDwsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateDwsOperationCompleted);
            }
            this.InvokeAsync("CreateDws", new object[] {
                        name,
                        users,
                        title,
                        documents}, this.CreateDwsOperationCompleted, userState);
        }
        
        private void OnCreateDwsOperationCompleted(object arg) {
            if ((this.CreateDwsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateDwsCompleted(this, new CreateDwsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/DeleteDws", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string DeleteDws() {
            object[] results = this.Invoke("DeleteDws", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void DeleteDwsAsync() {
            this.DeleteDwsAsync(null);
        }
        
        /// <remarks/>
        public void DeleteDwsAsync(object userState) {
            if ((this.DeleteDwsOperationCompleted == null)) {
                this.DeleteDwsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteDwsOperationCompleted);
            }
            this.InvokeAsync("DeleteDws", new object[0], this.DeleteDwsOperationCompleted, userState);
        }
        
        private void OnDeleteDwsOperationCompleted(object arg) {
            if ((this.DeleteDwsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteDwsCompleted(this, new DeleteDwsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/GetDwsMetaData", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetDwsMetaData(string document, string id, bool minimal) {
            object[] results = this.Invoke("GetDwsMetaData", new object[] {
                        document,
                        id,
                        minimal});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetDwsMetaDataAsync(string document, string id, bool minimal) {
            this.GetDwsMetaDataAsync(document, id, minimal, null);
        }
        
        /// <remarks/>
        public void GetDwsMetaDataAsync(string document, string id, bool minimal, object userState) {
            if ((this.GetDwsMetaDataOperationCompleted == null)) {
                this.GetDwsMetaDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDwsMetaDataOperationCompleted);
            }
            this.InvokeAsync("GetDwsMetaData", new object[] {
                        document,
                        id,
                        minimal}, this.GetDwsMetaDataOperationCompleted, userState);
        }
        
        private void OnGetDwsMetaDataOperationCompleted(object arg) {
            if ((this.GetDwsMetaDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDwsMetaDataCompleted(this, new GetDwsMetaDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/GetDwsData", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetDwsData(string document, string lastUpdate) {
            object[] results = this.Invoke("GetDwsData", new object[] {
                        document,
                        lastUpdate});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetDwsDataAsync(string document, string lastUpdate) {
            this.GetDwsDataAsync(document, lastUpdate, null);
        }
        
        /// <remarks/>
        public void GetDwsDataAsync(string document, string lastUpdate, object userState) {
            if ((this.GetDwsDataOperationCompleted == null)) {
                this.GetDwsDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDwsDataOperationCompleted);
            }
            this.InvokeAsync("GetDwsData", new object[] {
                        document,
                        lastUpdate}, this.GetDwsDataOperationCompleted, userState);
        }
        
        private void OnGetDwsDataOperationCompleted(object arg) {
            if ((this.GetDwsDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDwsDataCompleted(this, new GetDwsDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/UpdateDwsData", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string UpdateDwsData(string updates, string meetingInstance) {
            object[] results = this.Invoke("UpdateDwsData", new object[] {
                        updates,
                        meetingInstance});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void UpdateDwsDataAsync(string updates, string meetingInstance) {
            this.UpdateDwsDataAsync(updates, meetingInstance, null);
        }
        
        /// <remarks/>
        public void UpdateDwsDataAsync(string updates, string meetingInstance, object userState) {
            if ((this.UpdateDwsDataOperationCompleted == null)) {
                this.UpdateDwsDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUpdateDwsDataOperationCompleted);
            }
            this.InvokeAsync("UpdateDwsData", new object[] {
                        updates,
                        meetingInstance}, this.UpdateDwsDataOperationCompleted, userState);
        }
        
        private void OnUpdateDwsDataOperationCompleted(object arg) {
            if ((this.UpdateDwsDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UpdateDwsDataCompleted(this, new UpdateDwsDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/RemoveDwsUser", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string RemoveDwsUser(string id) {
            object[] results = this.Invoke("RemoveDwsUser", new object[] {
                        id});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void RemoveDwsUserAsync(string id) {
            this.RemoveDwsUserAsync(id, null);
        }
        
        /// <remarks/>
        public void RemoveDwsUserAsync(string id, object userState) {
            if ((this.RemoveDwsUserOperationCompleted == null)) {
                this.RemoveDwsUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRemoveDwsUserOperationCompleted);
            }
            this.InvokeAsync("RemoveDwsUser", new object[] {
                        id}, this.RemoveDwsUserOperationCompleted, userState);
        }
        
        private void OnRemoveDwsUserOperationCompleted(object arg) {
            if ((this.RemoveDwsUserCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RemoveDwsUserCompleted(this, new RemoveDwsUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/RenameDws", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string RenameDws(string title) {
            object[] results = this.Invoke("RenameDws", new object[] {
                        title});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void RenameDwsAsync(string title) {
            this.RenameDwsAsync(title, null);
        }
        
        /// <remarks/>
        public void RenameDwsAsync(string title, object userState) {
            if ((this.RenameDwsOperationCompleted == null)) {
                this.RenameDwsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRenameDwsOperationCompleted);
            }
            this.InvokeAsync("RenameDws", new object[] {
                        title}, this.RenameDwsOperationCompleted, userState);
        }
        
        private void OnRenameDwsOperationCompleted(object arg) {
            if ((this.RenameDwsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RenameDwsCompleted(this, new RenameDwsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/FindDwsDoc", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string FindDwsDoc(string id) {
            object[] results = this.Invoke("FindDwsDoc", new object[] {
                        id});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void FindDwsDocAsync(string id) {
            this.FindDwsDocAsync(id, null);
        }
        
        /// <remarks/>
        public void FindDwsDocAsync(string id, object userState) {
            if ((this.FindDwsDocOperationCompleted == null)) {
                this.FindDwsDocOperationCompleted = new System.Threading.SendOrPostCallback(this.OnFindDwsDocOperationCompleted);
            }
            this.InvokeAsync("FindDwsDoc", new object[] {
                        id}, this.FindDwsDocOperationCompleted, userState);
        }
        
        private void OnFindDwsDocOperationCompleted(object arg) {
            if ((this.FindDwsDocCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.FindDwsDocCompleted(this, new FindDwsDocCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/CanCreateDwsUrl", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CanCreateDwsUrl(string url) {
            object[] results = this.Invoke("CanCreateDwsUrl", new object[] {
                        url});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CanCreateDwsUrlAsync(string url) {
            this.CanCreateDwsUrlAsync(url, null);
        }
        
        /// <remarks/>
        public void CanCreateDwsUrlAsync(string url, object userState) {
            if ((this.CanCreateDwsUrlOperationCompleted == null)) {
                this.CanCreateDwsUrlOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCanCreateDwsUrlOperationCompleted);
            }
            this.InvokeAsync("CanCreateDwsUrl", new object[] {
                        url}, this.CanCreateDwsUrlOperationCompleted, userState);
        }
        
        private void OnCanCreateDwsUrlOperationCompleted(object arg) {
            if ((this.CanCreateDwsUrlCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CanCreateDwsUrlCompleted(this, new CanCreateDwsUrlCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/CreateFolder", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string CreateFolder(string url) {
            object[] results = this.Invoke("CreateFolder", new object[] {
                        url});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CreateFolderAsync(string url) {
            this.CreateFolderAsync(url, null);
        }
        
        /// <remarks/>
        public void CreateFolderAsync(string url, object userState) {
            if ((this.CreateFolderOperationCompleted == null)) {
                this.CreateFolderOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCreateFolderOperationCompleted);
            }
            this.InvokeAsync("CreateFolder", new object[] {
                        url}, this.CreateFolderOperationCompleted, userState);
        }
        
        private void OnCreateFolderOperationCompleted(object arg) {
            if ((this.CreateFolderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CreateFolderCompleted(this, new CreateFolderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://schemas.microsoft.com/sharepoint/soap/dws/DeleteFolder", RequestNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", ResponseNamespace="http://schemas.microsoft.com/sharepoint/soap/dws/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string DeleteFolder(string url) {
            object[] results = this.Invoke("DeleteFolder", new object[] {
                        url});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void DeleteFolderAsync(string url) {
            this.DeleteFolderAsync(url, null);
        }
        
        /// <remarks/>
        public void DeleteFolderAsync(string url, object userState) {
            if ((this.DeleteFolderOperationCompleted == null)) {
                this.DeleteFolderOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteFolderOperationCompleted);
            }
            this.InvokeAsync("DeleteFolder", new object[] {
                        url}, this.DeleteFolderOperationCompleted, userState);
        }
        
        private void OnDeleteFolderOperationCompleted(object arg) {
            if ((this.DeleteFolderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteFolderCompleted(this, new DeleteFolderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void CreateDwsCompletedEventHandler(object sender, CreateDwsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateDwsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateDwsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void DeleteDwsCompletedEventHandler(object sender, DeleteDwsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeleteDwsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DeleteDwsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GetDwsMetaDataCompletedEventHandler(object sender, GetDwsMetaDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDwsMetaDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDwsMetaDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GetDwsDataCompletedEventHandler(object sender, GetDwsDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDwsDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDwsDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void UpdateDwsDataCompletedEventHandler(object sender, UpdateDwsDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UpdateDwsDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UpdateDwsDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void RemoveDwsUserCompletedEventHandler(object sender, RemoveDwsUserCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RemoveDwsUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RemoveDwsUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void RenameDwsCompletedEventHandler(object sender, RenameDwsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RenameDwsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RenameDwsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void FindDwsDocCompletedEventHandler(object sender, FindDwsDocCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class FindDwsDocCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal FindDwsDocCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void CanCreateDwsUrlCompletedEventHandler(object sender, CanCreateDwsUrlCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CanCreateDwsUrlCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CanCreateDwsUrlCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void CreateFolderCompletedEventHandler(object sender, CreateFolderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CreateFolderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CreateFolderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void DeleteFolderCompletedEventHandler(object sender, DeleteFolderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeleteFolderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DeleteFolderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591