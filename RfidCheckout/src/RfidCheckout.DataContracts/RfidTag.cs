using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RfidCheckout.DataContracts
{
    [DataContract]
    public class RfidTag
    {
        public RfidTag()
        {
            TagId = Guid.NewGuid().ToString("N");
        }

        [DataMember]
        public string TagId { get; set; }
        [DataMember]
        public string Product { get; set; }
        [DataMember]
        public double Price { get; set; }
    }
}
