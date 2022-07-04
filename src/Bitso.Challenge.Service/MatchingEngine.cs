using Bitso.Challenge.Service.Commands;
using Bitso.Challenge.Service.OrderBook;

namespace Bitso.Challenge.Service
{
    /*
     *
     * Matching Engine
     * Matching engine should execute any matching trades and update the LOB. After each message (when processing and LOB update is finalized), the matching engine should observe the LOB and take trade execution actions; if it exists.
     * Example Execution:
     * Using the same exemplary order book above,
     * if a new sell order at price of 9 and amount of 55 arrives to exchange,
     * the matching engine should fully fill the bid at 9 with amount of 40 (at Amount 0)
     * and partially fill the bid at Amount 1 (15 is filled and 5 is remaining).
     * The Order object should then be modified to contain the correct amount after the partial fill.
     */
    public class MatchingEngine
    {
        private IOrderBook orderBook;
        private OrderCommandFactory factory;

        public MatchingEngine(ITradeEventDispatcher tradeEventDispatcher)
        {

            orderBook = new OrderBook.OrderBook(tradeEventDispatcher);
            factory = new OrderCommandFactory();
        }

        public void ProcessIncomingMessage(OrderMessage orderMessage)
        {
            factory.Create(orderMessage).Process(orderBook);
        }
    }
}
