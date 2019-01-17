using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TopicsAndSubscriptions.Models
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime OrderDate { get; set; }

        [DataMember]
        public int Items { get; set; }

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public string Priority { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public bool HasLoyaltyCard { get; set; }

    }
}
