using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace WorkingWithMessages.DataContracts
{
    [DataContract]
    public class PizzaOrder
    {
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Size { get; set; }
    }
}
