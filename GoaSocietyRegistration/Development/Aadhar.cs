using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoaSocietyRegistration.Development
{
    class Aadhar
    {
        [BsonId]
        public ObjectId _Id { get; set; }
        [BsonElement("App_ID")]
        public string App_ID { get; set; }
        [BsonElement("Mem_ID")]
        public string Member_ID { get; set; }
        [BsonElement("Doc_ID")]
        public string Doc_ID { get; set; }
    }
}