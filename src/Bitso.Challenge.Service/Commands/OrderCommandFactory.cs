using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitso.Challenge.Service.Commands
{

    public interface IOrderCommand
    {
        void Process(IOrderBook orderBook);
    }

    public class OrderCommandFactory
    {
        public IOrderCommand Create(OrderMessage message)
        {
            switch (message)
            {
                case DeleteOrder order:
                    return new DeleteOrderCommand(order);
                case AddOrder addOrder:
                    return new AddOrderCommand(addOrder);
                case ModifyOrder modifyOrder:
                    return new ModifyOrderCommand(modifyOrder);
            }

            return null;
        }
    }
}
