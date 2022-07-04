using System;
using System.Collections.Generic;

namespace Bitso.Challenge.Service.OrderBook
{
    public interface ITradeEventDispatcher
    {
        void NotifyTrade(Order order1,Order destOrder,  long filled,long price);
        void NotifyDelete(Order order);
    }

    public class OrderBook : IOrderBook
    {
        private readonly SortedList<long, OrderBookLevel> askBuckets;
        private readonly SortedList<long, OrderBookLevel> bidBuckets;
        private readonly Dictionary<Guid, Order> idMap ;
        private readonly ITradeEventDispatcher tradeEventDispatcher;

        public OrderBook(ITradeEventDispatcher tradeEventDispatcher)
        {
            askBuckets = new SortedList<long, OrderBookLevel>();
            bidBuckets = new SortedList<long, OrderBookLevel>(new DescendingComparer<long>());
            idMap = new Dictionary<Guid, Order>();
            this.tradeEventDispatcher = tradeEventDispatcher;
        }

 
        /**
         *  Remove has O(n) 
         */
        public void CancelOrder(DeleteOrder cmd)
        {
            if(idMap.TryGetValue(cmd.Id,out var order))
            {
                idMap.Remove(cmd.Id);
                var sideBuckets = GetLevelsBySide(order.Action);
                var orderBucket = sideBuckets[order.Price];
                orderBucket.Remove(order.Id);
                tradeEventDispatcher.NotifyDelete(order);
            }
        }

        /**
         *  o(log(n))
         */
        public Order NewOrder(AddOrder orderMessage)
        {
            //Create New Order
            var order = new Order
            {
                Action = orderMessage.Action,
                Id = Guid.NewGuid(),
                Size = orderMessage.Size,
                Price = orderMessage.Price,
                TimeStamp = DateTime.Now

            };
            var oppositeBuckets = GetOppositeTradeableLevels(order.Action, order.Price);
            long filledSize = TryMatchTrades(order, oppositeBuckets);
            if (filledSize == order.Size)
                return order;

            GetLevelsBySide(order.Action).AddOrder(order);
            idMap.Add(order.Id, order);
            return order;
        }

        private long TryMatchTrades(Order order, IEnumerable<KeyValuePair<long, OrderBookLevel>> oppositeBuckets)
        {
            long filled = 0;
            long orderSize = order.Size;
            foreach(var level in oppositeBuckets)
            {
                long sizeLeft = orderSize - filled;
                var matchResult = level.Value.Match(sizeLeft, order, tradeEventDispatcher);
                matchResult.OrdersToRemove.ForEach(o => idMap.Remove(o.Id));
                filled += matchResult.MatchedVolume;
                if (filled == order.Size)
                    break;
            }
            return filled;
        }



        private SortedList<long, OrderBookLevel> GetLevelsBySide(OrderSide side)
        {
            return side == OrderSide.Buy ? bidBuckets : askBuckets;
        }

        public IEnumerable<KeyValuePair<long, OrderBookLevel>> GetOppositeTradeableLevels(OrderSide side, long price)
        {
            return (side == OrderSide.Buy ? askBuckets : bidBuckets).Head(price,true);
        }

        public void ModifyOrder(ModifyOrder cmd)
        {
            if (this.idMap.TryGetValue(cmd.OrderId, out var order))
            {
                var newSize = Math.Max(cmd.NewAmount, order.Filled);
                var buckets = GetLevelsBySide(order.Action);
                var bookLevel = buckets[order.Price];
                // Should remove Order
                if (newSize == order.Filled)
                {
                    idMap.Remove(order.Id);
                    bookLevel.Remove(order.Id);
                }
                else
                {
                    bookLevel.Resize(order, cmd.NewAmount);
                }
            }
        }
    }

}
