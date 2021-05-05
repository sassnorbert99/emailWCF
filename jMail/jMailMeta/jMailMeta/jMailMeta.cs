using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace jMailMeta
{
    [DataContract]
    public class User
    {
        public User(string LoginName, string Password, string Name, DateTime Birthday, string ImagePath, List<string> Contacts)
        {
            loginName = LoginName;
            password = Password;
            name = Name;
            mailAddress = loginName + "@jmail.com";
            birthDay = Birthday;
            imagePath = ImagePath;
            contacts = Contacts;
        }
        public User(string LoginName, string Password, string Name, DateTime Birthday, string ImagePath) :
            this(LoginName, Password, Name, Birthday, ImagePath, new List<string>()) { }


        private string loginName;
        private string password;
        private string name;
        private string mailAddress;
        private DateTime birthDay;
        private string imagePath;
        private List<string> contacts;

        public string LoginName
        {
            get { return loginName; }
        }

        public string Password
        {
            get { return password; }
        }
        public string Name
        {
            get { return name; }
        }
        public string MailAddress
        {
            get { return mailAddress; }
        }
        public DateTime BirthDay
        {
            get { return birthDay; }
        }
        public string ImagePath
        {
            get { return imagePath; }
        }
        public List<string> Contacts
        {
            get { return contacts; }
        }
    }

    [DataContract]
    public class Mail
    {
        public Mail(List<string> To, string Subject, string Body)
        {
            to = To;
            subject = Subject;
            message = Body;
            unread = true;
        }

        private string from;
        private List<string> to;
        private string subject;
        private string message;
        private DateTime sentDate;
        private bool unread;

        public string From
        {
            get { return from; }
            set { from = value; }
        }
        public List<string> To
        {
            get { return to; }
        }
        public string Subject
        {
            get { return subject; }
        }
        public string Message
        {
            get { return message; }
        }
        public DateTime SentDate
        {
            get { return sentDate; }
            set { sentDate = value; }
        }
        public bool Unread
        {
            get { return unread; }
            set { unread = value; }
        }
    }
}
