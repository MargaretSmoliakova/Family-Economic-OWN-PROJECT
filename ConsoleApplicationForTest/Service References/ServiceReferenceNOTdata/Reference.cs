﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplicationForTest.ServiceReferenceNOTdata {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReferenceNOTdata.IUserDatabaseCreation")]
    public interface IUserDatabaseCreation {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserDatabaseCreation/MethodForTest", ReplyAction="http://tempuri.org/IUserDatabaseCreation/MethodForTestResponse")]
        string MethodForTest();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserDatabaseCreation/MethodForTest", ReplyAction="http://tempuri.org/IUserDatabaseCreation/MethodForTestResponse")]
        System.Threading.Tasks.Task<string> MethodForTestAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBase", ReplyAction="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBaseResponse")]
        void CreateUserDataBase(string loginName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBase", ReplyAction="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBaseResponse")]
        System.Threading.Tasks.Task CreateUserDataBaseAsync(string loginName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBaseUsingScript", ReplyAction="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBaseUsingScriptResponse")]
        void CreateUserDataBaseUsingScript(string loginName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBaseUsingScript", ReplyAction="http://tempuri.org/IUserDatabaseCreation/CreateUserDataBaseUsingScriptResponse")]
        System.Threading.Tasks.Task CreateUserDataBaseUsingScriptAsync(string loginName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUserDatabaseCreationChannel : ConsoleApplicationForTest.ServiceReferenceNOTdata.IUserDatabaseCreation, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UserDatabaseCreationClient : System.ServiceModel.ClientBase<ConsoleApplicationForTest.ServiceReferenceNOTdata.IUserDatabaseCreation>, ConsoleApplicationForTest.ServiceReferenceNOTdata.IUserDatabaseCreation {
        
        public UserDatabaseCreationClient() {
        }
        
        public UserDatabaseCreationClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UserDatabaseCreationClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UserDatabaseCreationClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UserDatabaseCreationClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string MethodForTest() {
            return base.Channel.MethodForTest();
        }
        
        public System.Threading.Tasks.Task<string> MethodForTestAsync() {
            return base.Channel.MethodForTestAsync();
        }
        
        public void CreateUserDataBase(string loginName) {
            base.Channel.CreateUserDataBase(loginName);
        }
        
        public System.Threading.Tasks.Task CreateUserDataBaseAsync(string loginName) {
            return base.Channel.CreateUserDataBaseAsync(loginName);
        }
        
        public void CreateUserDataBaseUsingScript(string loginName) {
            base.Channel.CreateUserDataBaseUsingScript(loginName);
        }
        
        public System.Threading.Tasks.Task CreateUserDataBaseUsingScriptAsync(string loginName) {
            return base.Channel.CreateUserDataBaseUsingScriptAsync(loginName);
        }
    }
}
