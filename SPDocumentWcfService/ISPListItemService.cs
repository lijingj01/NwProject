using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SPDocumentWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ISPListItemService”。
    [ServiceContract]
    public interface ISPListItemService
    {
        [OperationContract]
        void DoWork();

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
