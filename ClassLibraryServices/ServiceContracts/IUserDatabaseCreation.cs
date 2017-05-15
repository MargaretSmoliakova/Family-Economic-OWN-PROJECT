using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ClassLibraryServices.ServiceContracts
{
    [ServiceContract]
    interface IUserDatabaseCreation
    {
        [OperationContract]
        string MethodForTest();

        [OperationContract]
        void CreateUserDataBase(string loginName);

        [OperationContract]
        void CreateUserDataBaseUsingScript(string loginName);        
    }
}
