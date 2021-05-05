using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jMailClient
{
    class InboxMsgMeta
    {
        public InboxMsgMeta(bool Unread, string From, string Subject, string Date, string Message)
        {
            this.Unread = Unread;
            this.From = From;
            this.Subject = Subject;
            this.Date = Date;
            this.Message = Message;
        }

        public InboxMsgMeta() { }

        public bool Unread { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
    }

    class SentMsgMeta
    {
        public SentMsgMeta(bool Unread, string To, string Subject, string Date, string Message)
        {
            this.Unread = Unread;
            this.To = To;
            this.Subject = Subject;
            this.Date = Date;
            this.Message = Message;
        }

        public SentMsgMeta() { }

        public bool Unread { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
    }
}
