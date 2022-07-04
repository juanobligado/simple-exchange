using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.PerformanceData;
using Bitso.Challenge.Service.OrderBook;
using Moq;
using NUnit.Framework;
using System.Diagnostics;

namespace Bitso.Challenge.Service.Test
{
    public class OrderBookTest
    {
        private Mock<ITradeEventDispatcher> dispatcher;
        private OrderBook.OrderBook orderBook;

        [SetUp]
        public void Setup()
        {
            dispatcher = new Mock<ITradeEventDispatcher>(MockBehavior.Strict);
            orderBook = new OrderBook.OrderBook(dispatcher.Object);
        }

        [Test]
        public void InsertOrder()
        {
            var order = orderBook.NewOrder(new AddOrder
            {
                Action = OrderSide.Buy,
                Price = 10,
                Size = 10,
                Symbol = "AAPL"
            });
            Assert.AreNotEqual(Guid.Empty, order.Id);
        }

        [Test]
        public void InsertAndFillOrder()
        {
            var order = orderBook.NewOrder(new AddOrder
            {
                Action = OrderSide.Buy,
                Price = 10,
                Size = 10,
                Symbol = "AAPL"
            });

            dispatcher.Setup(m => m.NotifyTrade(It.IsAny<Order>(), order, 10, 10));
            orderBook.NewOrder(new AddOrder
            {
                Action = OrderSide.Sell,
                Price = 10,
                Size = 10,
                Symbol = "AAPL"
            });

        }

        [Test]
        public void StressTestInsert()
        {
            var tick = new Stopwatch();
            dispatcher.Setup(m => m.NotifyTrade(It.IsAny<Order>(), It.IsAny<Order>(), It.IsAny<long>(), It.IsAny<long>()));
            var random = new Random();

            tick.Start();
            var points = 0;
            for (int priceLevel = 1; priceLevel < 101; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Buy,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                    points++;
                }
            }

            for (int priceLevel = 101; priceLevel < 202; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Sell,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                }
                points++;
            }
            tick.Stop();
            Console.WriteLine($"Inserted {points} orders in {tick.ElapsedMilliseconds} ms ");
        }

        [Test]
        public void StressTestDelete()
        {
            var tick = new Stopwatch();
            var tradesCount = 0;
            dispatcher.Setup(m => m.NotifyDelete(It.IsAny<Order>()))
                .Callback<Order>((o) => { tradesCount++; });
            var random = new Random();


            var points = 0;
            var orders = new Queue<Guid>();
            for (int priceLevel = 1; priceLevel < 101; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    var o = orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Buy,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                    orders.Enqueue(o.Id);
                }
            }

            

            for (int priceLevel = 101; priceLevel < 201; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    var o =orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Sell,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                    orders.Enqueue(o.Id);
                }
                points++;
            }

            tick.Start();
            while (orders.Any())
            {
                orderBook.CancelOrder(new DeleteOrder {Id = orders.Dequeue()});
            }
            tick.Stop();
            Console.WriteLine($"Deleted {tradesCount} orders in {tick.ElapsedMilliseconds} ms");
        }

        [Test]
        public void StressTestChangeSize()
        {
            var tick = new Stopwatch();
            var tradesCount = 0;
            var random = new Random();


            var points = 0;
            var orders = new Queue<Guid>();
            for (int priceLevel = 1; priceLevel < 101; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    var o = orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Buy,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                    orders.Enqueue(o.Id);
                }
            }



            for (int priceLevel = 101; priceLevel < 201; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    var o = orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Sell,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                    orders.Enqueue(o.Id);
                }
                points++;
            }

            tick.Start();
            while (orders.Any())
            {
                var order = orders.Dequeue();
                orderBook.ModifyOrder(new ModifyOrder { OrderId = order , NewAmount = 10 });
                tradesCount++;
            }
            tick.Stop();
            Console.WriteLine($"Changed {tradesCount} orders in {tick.ElapsedMilliseconds} ms");
        }

        [Test]
        public void StressTestFill()
        {
            var tick = new Stopwatch();
            var tradesCount = 0;
            dispatcher.Setup(m => m.NotifyTrade(It.IsAny<Order>(), It.IsAny<Order>(), It.IsAny<long>(), It.IsAny<long>()))
                .Callback<Order, Order, long, long>((o, u, f, s) => { tradesCount++; });
            var random = new Random();


            var points = 0;
            for (int priceLevel = 1; priceLevel < 101; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Buy,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                    points++;
                }
            }

            tick.Start();

            for (int priceLevel = 1; priceLevel < 101; priceLevel++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var size = random.Next(1, 100);
                    orderBook.NewOrder(new AddOrder
                    {
                        Action = OrderSide.Sell,
                        Price = priceLevel,
                        Size = size,
                        Symbol = "AAPL"
                    });
                }
                points++;
            }
            tick.Stop();
            Console.WriteLine($"Filled {tradesCount} orders in {tick.ElapsedMilliseconds} ms");
        }

    }
}
