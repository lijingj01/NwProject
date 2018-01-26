using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SPDocumentWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“SPListItemService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 SPListItemService.svc 或 SPListItemService.svc.cs，然后开始调试。
    public class SPListItemService : ISPListItemService
    {
        public void DoWork()
        {
        }

        #region 列表库操作

        /// <summary>
        /// 获取指定列表库的所有列表项
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">列表库名称</param>
        /// <returns></returns>
        public SPListItems GetSPListItems(SPSetting setting, string ListName)
        {
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            SPListItems items = docHelper.GetSPListItems(ListName);
            return items;
        }
        /// <summary>
        /// 获取指定列表库按条件查询的列表项
        /// </summary>
        /// <param name="setting">配置信息</param>
        /// <param name="ListName">列表库名称</param>
        /// <param name="SearchList">查询条件（key:字段名/value:字段内容）</param>
        /// <returns></returns>
        public SPListItems GetSPListItemsBySearch(SPSetting setting, string ListName, Dictionary<string, string> SearchList)
        {
            throw new NotImplementedException();
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
            SharePointHelper docHelper = new SPDocumentWcfService.SharePointHelper(setting.SPUserId, setting.SPUserPwd, setting.SPUserDomain, setting.SPSite, setting.SPWeb);
            docHelper.UpdateSPListItem(strListName, 1, updateValue);
        }

        #endregion
    }
}
