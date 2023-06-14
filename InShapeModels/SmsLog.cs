using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InShapeModels
{
    public class SmsLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string FullName { get; set; }
        public DateTime Date { get; set; }
        public string SMS { get; set; }
        public MessageType MessageType { get; set; }
        public string Response { get; set; }

        //public string ResponseMSG
        //{
        //    get
        //    {
        //        switch (Response)
        //        {
        //            case "":
        //                return "";
        //            default:
        //                return string.Empty;
        //        }
        //    }
        //}
    }

    public class SmsReport
    {
        public DateTime Date { get; set; }

        //public MessageType MessageType { get; set; }

        public List<SmsLog> SentSMS { get; set; }
    }

}
