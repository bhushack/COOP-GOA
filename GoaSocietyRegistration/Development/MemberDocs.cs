using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration
{
    public class MemberDocs
    {
        [BsonId]
        public ObjectId _Id { get; set; }

        [BsonElement("App_ID")]
        public Int64 App_ID { get; set; }
        [BsonElement("Mem_ID")]
        public Int64 Member_ID { get; set; }
        [BsonElement("Doc_ID")]
        public string Doc_ID { get; set; }

        [BsonElement("Doc_CT")]
        public string Doc_CT { get; set; }
        [BsonElement("Doc Name")]
        public string doc_name { get; set; }

        [BsonElement("Doc Content")]
        public byte[] DocContent { get; set; }

        [BsonElement("Upload At")]
        public string time_stamp { get; set; }      

        public bool Active { get; set; }

        public String UpdatedBy { get; set; }

        public String IpAddress { get; set; }

        public String MacAddress { get; set; }
    }
}