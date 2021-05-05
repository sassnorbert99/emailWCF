using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using jMailMeta;
using System.Data;

namespace jMailServer
{
    class DataHandler
    {
        private DataHandler(string fileName)
        {
            try
            {
                database = SQLiteHelper.GetSQLiteHelper();
                string basepath = System.AppDomain.CurrentDomain.BaseDirectory;
                string path = basepath + fileName;
                if (!File.Exists(path)) database.NewFile(path);
                else database.OpenFile(path);
                Console.WriteLine("Database connection succesful");
            }
            catch (Exception e)
            {
                database = null;
                Console.WriteLine("Database connection failed due to: " + e.Message);
            }
            //CreateNewTable();
        }

        private static SQLiteHelper database;
        private static DataHandler self;

        public static DataHandler GetDataHandler(string FileName)
        {
            if (self == null) self = new DataHandler(FileName);
            return self;
        }
        public static DataHandler GetDataHandler()
        {
            return self;
        }

        public void CreateNewTable()
        {
            try
            {
                string q;
                q = "CREATE TABLE User (login VARCHAR(50) UNIQUE, pwd VARCHAR(50), name VARCHAR(50), birth VARCHAR(20), imgpath VARCHAR(50), contacts VARCHAR(255))";
                database.ExecuteNonQuery(q);

                q = "CREATE TABLE Mail (mfrom VARCHAR(50), mto VARCHAR(255), subject VARCHAR(50), message VARCHAR(255), sentdate VARCHAR(20), unread NUMERIC)";
                database.ExecuteNonQuery(q);
                Console.WriteLine("Tables created!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Table creation failed due to: " + e.Message);
            }
        }

        public bool InsertUser(User user)
        {
            try
            {
                string q = "INSERT INTO User (login, pwd, name, birth, imgpath) VALUES ('" + user.LoginName + "', '" + user.Password + "', '" + user.Name + "', '" + user.BirthDay.ToShortDateString() + "', '" + user.ImagePath + "')";
                database.ExecuteNonQuery(q);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Insertion failed due to: " + e.Message);
                return false;
            }
        }

        public bool InsertMail(Mail mail)
        {
            try
            {
                StringBuilder to = new StringBuilder();
                for (int i = 0; i < mail.To.Count; i++)
                {
                    to.Append(mail.To[i]);
                    if (i != mail.To.Count - 1) to.Append(',');
                }
                string q = "INSERT INTO Mail (mfrom, mto, subject, message, sentdate, unread) VALUES ('" + mail.From + "', '" + to.ToString() + "', '" + mail.Subject + "', '" + mail.Message + "', '" + mail.SentDate.ToShortDateString() + "', '" + (mail.Unread ? "1" : "0") + "')";
                database.ExecuteNonQuery(q);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Insertion failed due to: " + e.Message);
                return false;
            }
        }

        public List<User> ReadUsers()
        {
            List<User> users = new List<User>();
            try
            {
                string q = "SELECT * FROM User";
                DataTable dt = database.ExecuteDataQuery(q);
                UserData2List(dt, users);
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine("Query failed due to: " + e.Message);
                return null;
            }
        }

        public List<Mail> ReadMails()
        {
            List<Mail> mails = new List<Mail>();
            try
            {
                string q = "SELECT * FROM Mail";
                DataTable dt = database.ExecuteDataQuery(q);
                MailData2List(dt, mails);
                return mails;
            }
            catch (Exception e)
            {
                Console.WriteLine("Query failed due to: " + e.Message);
                return null;
            }
        }

        void UserData2List(DataTable data, List<User> list)
        {
            foreach (DataRow row in data.Rows)
            {
                string[] d = row["birth"].ToString().Split('.');
                DateTime dt = new DateTime(int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]));
                User rek = new User(row["login"].ToString(), row["pwd"].ToString(), row["name"].ToString(), dt, row["imgpath"].ToString());
                list.Add(rek);
            }
        }

        void MailData2List(DataTable data, List<Mail> list)
        {
            foreach (DataRow row in data.Rows)
            {
                string[] d = row["sentdate"].ToString().Split('.');
                DateTime dt = new DateTime(int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]));
                Mail m = new Mail(row["mto"].ToString().Split(',').ToList<string>(), row["subject"].ToString(), row["message"].ToString());
                m.From = row["mfrom"].ToString();
                m.SentDate = dt;
                list.Add(m);
            }
        }

    }
}
