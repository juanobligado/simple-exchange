using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitso.Challenge.Service
{
    public interface IOrderBook
    {
        /**
        * Process new order.
        */
        Order NewOrder( AddOrder order);

        /**
         * Cancel order completely.
         * <p>
         * fills cmd.action  with original original order action
         *
         * @param cmd - order command
         * @return MATCHING_UNKNOWN_ORDER_ID if order was not found, otherwise SUCCESS
         */
        void CancelOrder(DeleteOrder cmd);

        /**
         * Decrease the size of the order by specific number of lots
         */
        void ModifyOrder(ModifyOrder cmd);




    }
}
