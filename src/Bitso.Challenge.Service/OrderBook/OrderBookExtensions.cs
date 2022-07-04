using System;
using System.Collections;
using System.Collections.Generic;

namespace Bitso.Challenge.Service
{
    public static class OrderBookExtensions
    {
        public static void AddOrder(this SortedList<long, OrderBookLevel> levels, Order newOrder)
        {
            OrderBookLevel level = null;
            if (!levels.TryGetValue(newOrder.Price, out level))
            {
                level = new OrderBookLevel(newOrder.Price);
                levels.Add(level.Price, level);
            }
            level.AddOrder(newOrder);
        }

        ///
        public static IEnumerable<KeyValuePair<long, OrderBookLevel>> Head(this SortedList<long, OrderBookLevel> list,
            long from, bool inclusive)
        {
            var binarySearchResult =  list.Keys.BinarySearch(from,list.Comparer);

            if (binarySearchResult < 0)
                binarySearchResult = ~binarySearchResult;
            else if (inclusive)
                binarySearchResult++;
            return System.Linq.Enumerable.Take(list, binarySearchResult);
        }

        public static IEnumerable<KeyValuePair<long, OrderBookLevel>> Tail(this SortedList<long, OrderBookLevel> list,
            long from, bool inclusive)
        {
            var binarySearchResult = list.Keys.BinarySearch(from, list.Comparer);

            if (binarySearchResult < 0)
                binarySearchResult = ~binarySearchResult;
            else if (!inclusive)
                binarySearchResult++;
            return System.Linq.Enumerable.Take(list, binarySearchResult);
        }

        public static int BinarySearch(this IList<long> list, long value,IComparer<long> comparer)
        {
            int lower = 0;
            int upper = list.Count - 1;

            while (lower <= upper)
            {
                var middle = lower + ((upper - lower) / 2);
                var comparisonResult = comparer.Compare(value, list[middle]);
                if (comparisonResult < 0)
                {
                    upper = middle - 1;
                }
                else if (comparisonResult > 0)
                {
                    lower = middle + 1;
                }
                else
                {
                    return middle;
                }
            }

            return ~lower;
        }
    }

}



