using System;
using System.Collections.Generic;
using Bitso.Challenge.Service.DataStructures;
using Bitso.Challenge.Service.OrderBook;

namespace Bitso.Challenge.Service
{
    /**
     * Holds Price Level Orders
     */
    public class OrderBookLevel 
    {
        private readonly LinkedDictionary<Guid, Order> entries;
        public OrderBookLevel(long price)
        {
            this.Price = price;
            this.TotalVolume = 0;
            this.entries = new LinkedDictionary<Guid, Order>();
        }

        public long Price { get; private set; }
        public long TotalVolume { get; private set; }
        public void AddOrder(Order newOrder)
        {
            entries.Add(newOrder.Id, newOrder);
            TotalVolume += (newOrder.Size - newOrder.Filled);
        }

        public Order Remove(Guid OrderId)
        {
            if (!entries.TryGetValue(OrderId, out var order)) return null;

            entries.Remove(OrderId);
            TotalVolume -= (order.Size - order.Filled);
            return order;
        }


        public MatchResult Match(long sizeToMatch, Order order, ITradeEventDispatcher tradeEventDispatcher)
        {
            var matchResult = new MatchResult();
            var entriesToRemove = new List<KeyValuePair<Guid, Order>>();
            foreach (var entry in entries)
            {
                var otherSideOrder = entry.Value;
                var tradeFill = Math.Min(sizeToMatch, otherSideOrder.Size - otherSideOrder.Filled);
                otherSideOrder.Filled += tradeFill;
                order.Filled += tradeFill;
                matchResult.MatchedVolume += tradeFill;
                sizeToMatch -= tradeFill;
                TotalVolume -= tradeFill;
                var fullMatch = (otherSideOrder.Size == otherSideOrder.Filled);
                if (fullMatch)
                {
                    entriesToRemove.Add(entry);

                }

                
                tradeEventDispatcher.NotifyTrade(order, otherSideOrder,tradeFill,Price);

                if (order.Filled == order.Size)
                    break;

            }
            foreach(var item in entriesToRemove)
            {
                entries.Remove(item);
            }
            return matchResult;
        }

        internal void Resize(Order order, long newAmount)
        {
            entries[order.Id].Size = newAmount;
        }
    }

    public class MatchResult
    {
        public MatchResult()
        {
            OrdersToRemove = new List<Order>();
        }
        public long MatchedVolume { get; set; }
        public List<Order> OrdersToRemove { get; set; }
        
        
    }
}
