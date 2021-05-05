using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using jMailMeta;

namespace jMailServer
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IMailServer
    {
        [OperationContract(IsInitiating = true)]
        bool Login(string LoginName, string Password);

        [OperationContract()]
        string[] GetUserData();

        [OperationContract()]
        bool Registration(string LoginName, string Password, string Name, DateTime Birthday, string ImagePath);

        [OperationContract()]
        bool SendMessage(List<string> To, string Subject, string Body);

        [OperationContract()]
        List<string[]> GetInboxMessages();

        [OperationContract()]
        List<string[]> GetSentMessages();

        [OperationContract()]
        List<string[]> Filter(string Keyword);

        [OperationContract()]
        List<string> GetContacts();

        [OperationContract()]
        bool DeleteMessage();

        [OperationContract()]
        bool LogOut();
    }
}
