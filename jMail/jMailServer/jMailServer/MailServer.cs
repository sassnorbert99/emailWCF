using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using jMailMeta;
using System.Text.RegularExpressions;
using System.IO;

namespace jMailServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    class MailServer : IMailServer
    {
        static MailServer()
        {
            users = new List<User>();
            messages = new List<Mail>();       
        }

        private static DataHandler db = DataHandler.GetDataHandler();
        private static List<User> users;
        private static List<Mail> messages;
        private User currentUser;

        public static void SetUsers(List<User> Users)
        {
            users = Users;
        }

        public static void SetMails(List<Mail> Messages)
        {
            messages = Messages;
        }

        public bool Registration(string LoginName, string Password, string Name, DateTime Birthday, string ImagePath)
        {
            if (GetUserByName(LoginName) != null) return false;
            User u = new User(LoginName, Password, Name, Birthday, ImagePath);
            lock (users)
            {
                users.Add(u);
            }
            db.InsertUser(u);
            return true;
        }

        User GetUserByName(string UserName)
        {
            lock (users)
            {
                foreach (User u in users)
                {
                    if (u.LoginName == UserName) return u;
                }
                return null;
            }
        }

        User GetUserByMailAddr(string MailAddress)
        {
            lock (users)
            {
                foreach (User u in users)
                {
                    if (u.MailAddress == MailAddress) return u;
                }
                return null;
            }
        }

        User AuthenticateUser(string LoginName, string Password)
        {
            lock (users)
            {
                foreach (User u in users)
                {
                    if (u.LoginName == LoginName && u.Password == Password) return u;
                }
                return null;
            }
        }

        public bool Login(string LoginName, string Password)
        {
            if (currentUser != null) return false;
            currentUser = AuthenticateUser(LoginName, Password);
            if (currentUser == null) return false;
            return true;
        }

        bool NotLoggedIn
        {
            get { return currentUser == null; }
        }

        public string[] GetUserData()
        {
            if (NotLoggedIn) return null;
            string[] data = new string[5];
            data[0] = currentUser.LoginName;
            data[1] = currentUser.MailAddress;
            data[2] = currentUser.Name;
            data[3] = currentUser.ImagePath;
            return data;
        }

        public bool SendMessage(List<string> To, string Subject, string Body)
        {
            if (NotLoggedIn) return false;
            Mail message = new Mail(To, Subject, Body);
            message.From = currentUser.MailAddress;
            message.SentDate = DateTime.Now;
            for (int i = 0; i < message.To.Count; i++)
            {
                if (!currentUser.Contacts.Contains(message.To[i]))
                    currentUser.Contacts.Add(message.To[i]);
                lock (users)
                {
                    User u = GetUserByMailAddr(message.To[i]);
                    if (u != null && !u.Contacts.Contains(message.From))
                        u.Contacts.Add(message.From);
                }
            }
            lock (messages)
            {
                messages.Add(message);
            }
            db.InsertMail(message);
            return true;
        }

        public List<string[]> GetInboxMessages()
        {
            List<string[]> msgs = new List<string[]>();
            lock (messages)
            {
                string[] mailData;
                foreach (Mail m in messages)
                {

                    if (m.To.Contains(currentUser.MailAddress))
                    {
                        mailData = new string[6];
                        mailData[0] = m.From;
                        StringBuilder stb = new StringBuilder();
                        foreach (string t in m.To)
                        {
                            stb.Append(t);
                            stb.Append(", ");
                        }
                        mailData[1] = stb.ToString();
                        mailData[2] = m.Subject;
                        mailData[3] = m.SentDate.ToShortDateString();
                        mailData[4] = m.Message;
                        if (m.Unread) mailData[5] = "1";
                        else mailData[5] = "0";
                        msgs.Add(mailData);
                    }
                }
            }
            return msgs;
        }

        public List<string[]> GetSentMessages()
        {
            List<string[]> msgs = new List<string[]>();
            lock (messages)
            {
                string[] mailData;

                foreach (Mail m in messages)
                {

                    if (m.From == currentUser.MailAddress)
                    {
                        mailData = new string[6];
                        mailData[0] = m.From;
                        StringBuilder stb = new StringBuilder();
                        foreach (string t in m.To)
                        {
                            stb.Append(t);
                            stb.Append(", ");
                        }
                        mailData[1] = stb.ToString();
                        mailData[2] = m.Subject;
                        mailData[3] = m.SentDate.ToShortDateString();
                        mailData[4] = m.Message;
                        if (m.Unread) mailData[5] = "1";
                        else mailData[5] = "0";
                        msgs.Add(mailData);
                    }
                }
            }
            return msgs;
        }

        public List<string[]> Filter(string Keyword)
        {
            List<string[]> msgs = new List<string[]>();
            lock (messages)
            {
                string[] mailData;
                foreach (Mail m in messages)
                {

                    if (m.To.Contains(currentUser.MailAddress))
                    {
                        if (
                            Regex.IsMatch(m.From, Keyword) ||
                            ListItemsMatch(m.To, Keyword) ||
                            Regex.IsMatch(m.Subject, Keyword) ||
                            Regex.IsMatch(m.SentDate.ToShortDateString(), Keyword) ||
                            Regex.IsMatch(m.Message, Keyword)
                            )
                        {
                            mailData = new string[6];
                            mailData[0] = m.From;
                            StringBuilder stb = new StringBuilder();
                            foreach (string t in m.To)
                            {
                                stb.Append(t);
                                stb.Append(", ");
                            }
                            mailData[1] = stb.ToString();
                            mailData[2] = m.Subject;
                            mailData[3] = m.SentDate.ToShortDateString();
                            mailData[4] = m.Message;
                            if (m.Unread) mailData[5] = "1";
                            else mailData[5] = "0";
                            msgs.Add(mailData);
                        }
                    }
                }
            }
            return msgs;
        }

        bool ListItemsMatch(List<string> Items, string KeyWord)
        {
            foreach (string s in Items)
            {
                if (Regex.IsMatch(s, KeyWord)) return true;
            }
            return false;
        }

        public List<string> GetContacts()
        {
            return currentUser.Contacts;
        }

        public bool DeleteMessage()
        {
            if (NotLoggedIn) return false;
            return false;
        }

        public bool LogOut()
        {
            if (NotLoggedIn) return false;
            currentUser = null;
            return true;
        }
    }
}
