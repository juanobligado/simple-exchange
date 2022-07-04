namespace Bitso.Challenge.Service.Commands
{
    public class DeleteOrderCommand : IOrderCommand
    {
        private readonly DeleteOrder deleteOrder;

        public DeleteOrderCommand(DeleteOrder deleteOrder)
        {
            this.deleteOrder = deleteOrder;
        }
        public void Process(IOrderBook orderBook)
        {
             orderBook.CancelOrder(deleteOrder);
        }
    }

    public class ModifyOrderCommand : IOrderCommand
    {
        private readonly ModifyOrder modifyOrder;

        public ModifyOrderCommand(ModifyOrder modifyOrder)
        {
            this.modifyOrder = modifyOrder;
        }
        public void Process(IOrderBook orderBook)
        {
            orderBook.ModifyOrder(modifyOrder);
        }
    }
}
