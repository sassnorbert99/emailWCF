using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace jMailServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Jäger's Mail Server";
            DataHandler db = DataHandler.GetDataHandler("jmail.jdb");
            MailServer.SetUsers(db.ReadUsers());
            MailServer.SetMails(db.ReadMails());
            Console.WriteLine("Mail Server is opening...");
            ServiceHost server = new ServiceHost(typeof(MailServer));
            server.Open();
            Console.WriteLine("Mail Server is ready!\n");
            foreach (ServiceEndpoint e in server.Description.Endpoints)
            {
                Console.WriteLine("[Address: {0}]\n\n[Binding: {1}]", e.Address.ToString(), e.Binding.Name);
            }

            Console.ReadLine();
            Console.WriteLine("Mail Server shutting down...");
            server.Close();
        }
    }
}
