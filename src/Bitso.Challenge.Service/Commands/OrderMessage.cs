using System;

namespace Bitso.Challenge.Service
{
    public class OrderMessage
    {
        public string Symbol { get; set; }
    }
    public class AddOrder : OrderMessage
    {
        public OrderSide Action { get; set; }
        public long Size { get; set; }
        public long Price { get;  set; }
    }
    public class DeleteOrder : OrderMessage
    {
        public Guid Id { get; set; }
    }
    public class ModifyOrder : OrderMessage
    {
        public Guid OrderId { get; set; }
        public long NewAmount { get; set; }
    }
}
