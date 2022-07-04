using System;

namespace Bitso.Challenge.Service
{
    public class Order 
    {
        public Guid Id { get; set; }
        public OrderSide Action { get; set; } 
        public long Price { get; set; }
        public long Size { get; set; }
        public long Filled { get; set; }
        public DateTime TimeStamp { get; set; }
    }


}
