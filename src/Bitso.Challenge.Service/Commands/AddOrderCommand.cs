namespace Bitso.Challenge.Service.Commands
{
    public class AddOrderCommand : IOrderCommand
    {
        private readonly AddOrder deleteOrder;

        public AddOrderCommand(AddOrder deleteOrder)
        {
            this.deleteOrder = deleteOrder;
        }
        public void Process(IOrderBook orderBook)
        {
            orderBook.NewOrder(deleteOrder);
        }
    }
}
