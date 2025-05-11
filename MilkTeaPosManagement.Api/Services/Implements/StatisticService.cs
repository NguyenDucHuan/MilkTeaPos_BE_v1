using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Helper;
using MilkTeaPosManagement.Api.Models.OrderItemModels;
using MilkTeaPosManagement.Api.Services.Interfaces;
using MilkTeaPosManagement.DAL.UnitOfWorks;
using MilkTeaPosManagement.Domain.Models;
using System.Globalization;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MilkTeaPosManagement.Api.Services.Implements
{
    public class StatisticService(IUnitOfWork uow) : IStatisticService
    {
        private readonly IUnitOfWork _uow = uow;
        public async Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByDate(DateTime? date)
        {
            var dateData = date == null ? DateTime.Now : date;
            var starttime = new TimeOnly(08, 00, 00);
            var orderResult = new List<object>();
            var amountResult = new List<object>();
            decimal? totalProfit = 0;
            var totalOrigin = 0;
            for (var i = 0; i < 6; i++)
            {
                //var orders = await _uow.GetRepository<Order>().GetListAsync(predicate: o => o.CreateAt.Value.Year == dateData.Value.Year && o.CreateAt.Value.Month == dateData.Value.Month && o.CreateAt.Value.Day == dateData.Value.Day 
                //                                                                            && (o.CreateAt.Value.Hour == starttime.Hour || o.CreateAt.Value.Hour == starttime.AddHours(1).Hour));
                var orders = new List<Order>();
                var statuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: stt => stt.OrderStatus == "SUCCESS" && stt.UpdatedAt.Value.Year == dateData.Value.Year && stt.UpdatedAt.Value.Month == dateData.Value.Month && stt.UpdatedAt.Value.Day == dateData.Value.Day
                                                                                            && (stt.UpdatedAt.Value.Hour == starttime.Hour || stt.UpdatedAt.Value.Hour == starttime.AddHours(1).Hour));
                foreach(var status in statuses)
                {
                    var od = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == status.OrderId);
                    orders.Add(od);
                }
                decimal ? totalAmount = 0;
                //if (orders != null && orders.Count > 0)
                //{
                    foreach (var order in orders)
                    {
                        totalAmount += order.TotalAmount;
                    }
                    orderResult.Add(new
                    {
                        label = starttime.ToString("HH:mm") + " - " + starttime.AddHours(2).ToString("HH:mm"),
                        value = orders.Count
                    });
                    totalProfit += totalAmount;
                    totalOrigin += orders.Count;
                    amountResult.Add(new
                    {
                        label = starttime.ToString("HH:mm") + " - " + starttime.AddHours(2).ToString("HH:mm"),
                        value = totalAmount
                    });
                //}                
                starttime = starttime.AddHours(2);
            }
            
            return (orderResult, amountResult, totalOrigin, totalProfit);
        }
        public async Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByWeek(DateTime? fromDate, DateTime? toDate)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            DayOfWeek firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            int diff = DateTime.Now.DayOfWeek - firstDayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            DateTime startOfWeek = DateTime.Now.AddDays(-diff).Date;
            DateTime endOfWeek = startOfWeek.AddDays(6);

            fromDate = fromDate == null ? startOfWeek : fromDate;
            toDate = toDate == null ? endOfWeek : toDate;

            var orderResult = new List<object>();
            var amountResult = new List<object>();
            decimal? totalProfit = 0;
            var totalOrigin = 0;

            while(fromDate.Value.Date <= toDate.Value.Date)
            {
                //var orders = await _uow.GetRepository<Order>().GetListAsync(predicate: o => o.CreateAt.Value.Year == fromDate.Value.Year && o.CreateAt.Value.Month == fromDate.Value.Month && o.CreateAt.Value.Day == fromDate.Value.Day
                //                                                                            && o.CreateAt.Value.Day == fromDate.Value.Day);
                var orders = new List<Order>();
                var statuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: stt => stt.OrderStatus == "SUCCESS" && stt.UpdatedAt.Value.Year == fromDate.Value.Year && stt.UpdatedAt.Value.Month == fromDate.Value.Month && stt.UpdatedAt.Value.Day == fromDate.Value.Day);
                foreach (var status in statuses)
                {
                    var od = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == status.OrderId);
                    orders.Add(od);
                }
                decimal? totalAmount = 0;
                //if (orders != null && orders.Count > 0)
                //{
                    foreach (var order in orders)
                    {
                        totalAmount += order.TotalAmount;
                    }
                    orderResult.Add(new
                    {
                        label = fromDate.Value.ToString("dd/MM"),
                        value = orders.Count
                    });
                    totalProfit += totalAmount;
                    totalOrigin += orders.Count;
                    amountResult.Add(new
                    {
                        label = fromDate.Value.ToString("dd/MM"),
                        value = totalAmount
                    });
                //}
                fromDate = fromDate.Value.AddDays(1);
            }
            return (orderResult, amountResult, totalOrigin, totalProfit);
        }
        public async Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByMonth(int? month, int? year)
        {
            year = year == null ? DateTime.Now.Year : year;
            month = month == null ? DateTime.Now.Month : month;
            var startDate = new DateTime(year.Value, month.Value, 1, 0, 0, 0);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var orderResult = new List<object>();
            var amountResult = new List<object>();
            decimal? totalProfit = 0;
            var totalOrigin = 0;

            while (startDate.Month == month && startDate.Year == year)
            {
                //var orders = await _uow.GetRepository<Order>().GetListAsync(predicate: o => o.CreateAt.Value.Year == startDate.Year && o.CreateAt.Value.Month == startDate.Month
                //                                                                            && o.CreateAt.Value.Day == startDate.Day);
                var orders = new List<Order>();
                var statuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: stt => stt.OrderStatus == "SUCCESS" && stt.UpdatedAt.Value.Year == startDate.Year && stt.UpdatedAt.Value.Month == startDate.Month && stt.UpdatedAt.Value.Day == startDate.Day);
                foreach (var status in statuses)
                {
                    var od = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == status.OrderId);
                    orders.Add(od);
                }
                decimal? totalAmount = 0;
                //if (orders != null && orders.Count > 0)
                //{
                    foreach (var order in orders)
                    {
                        totalAmount += order.TotalAmount;
                    }
                    orderResult.Add(new
                    {
                        label = startDate.ToString("dd/MM"),
                        value = orders.Count
                    });
                    totalProfit += totalAmount;
                    totalOrigin += orders.Count;
                    amountResult.Add(new
                    {
                        label = startDate.ToString("dd/MM"),
                        value = totalAmount
                    });
                //}
                startDate = startDate.AddDays(1);
            }
            return (orderResult, amountResult, totalOrigin, totalProfit);
        }
        public async Task<(List<object>, List<object>, int, decimal?)> OrderStatisticsByYear(int? year)
        {
            year = year == null ? DateTime.Now.Year : year;
            var startDate = new DateTime(year.Value, 1, 1, 0, 0, 0);

            var orderResult = new List<object>();
            var amountResult = new List<object>();
            decimal? totalProfit = 0;
            var totalOrigin = 0;

            while (startDate.Year == year)
            {
                //var orders = await _uow.GetRepository<Order>().GetListAsync(predicate: o => o.CreateAt.Value.Year == startDate.Year
                //                                                                            && o.CreateAt.Value.Month == startDate.Month);
                var orders = new List<Order>();
                var statuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: stt => stt.OrderStatus == "SUCCESS" && stt.UpdatedAt.Value.Year == startDate.Year && stt.UpdatedAt.Value.Month == startDate.Month);
                foreach (var status in statuses)
                {
                    var od = await _uow.GetRepository<Order>().SingleOrDefaultAsync(predicate: o => o.OrderId == status.OrderId);
                    orders.Add(od);
                }
                decimal? totalAmount = 0;
                //if (orders != null && orders.Count > 0)
                //{
                    foreach (var order in orders)
                    {
                        totalAmount += order.TotalAmount;
                    }
                    orderResult.Add(new
                    {
                        label = startDate.ToString("MM"),
                        value = orders.Count
                    });
                    totalProfit += totalAmount;
                    totalOrigin += orders.Count;
                    amountResult.Add(new
                    {
                        label = startDate.ToString("MM"),
                        value = totalAmount
                    });
                //}
                startDate = startDate.AddMonths(1);
            }
            return (orderResult, amountResult, totalOrigin, totalProfit);
        }
        public async Task<IEnumerable<SellProductModel>> GetBestSeller(int number)
        {
            //var productSum = await _dbSet.GroupBy(oi => oi.ProductId)
            //                            .Select(group => new
            //                            {
            //                                ProductId = group.Key,
            //                                Sum = group.Sum(oi => oi.Quantity)
            //                            }).OrderBy(oi => oi.Sum).Take(number).ToListAsync();
            //var bestSellers = new List<object>();
            //foreach (var group in productSum)
            //{
            //    var product = await _uow.GetRepository<Product>().SingleOrDefaultAsync(predicate: p => p.ProductId == group.ProductId);
            //    bestSellers.Add(new
            //    {
            //        Product = product,
            //        Quatity = group.Sum
            //    });
            //}
            //return bestSellers;
            var productSum = new List<SellProductModel>();
            var products = await _uow.GetRepository<Product>().GetListAsync(predicate: o => o.Status == true);

            var statuses = await _uow.GetRepository<Orderstatusupdate>().GetListAsync(predicate: stt => stt.OrderStatus == "SUCCESS");
            var orderItems = new List<Orderitem>();
            foreach (var status in statuses)
            {
                var items = await _uow.GetRepository<Orderitem>().GetListAsync(predicate: oi => oi.OrderId == status.OrderId);
                foreach(var item in items)
                {
                    orderItems.Add(item);
                }
            }

            foreach (var product in products)
            {
                int? sum = 0;
                var orderItemsOfProduct = orderItems.Where(oi => oi.ProductId == product.ProductId);
                foreach (var item in orderItemsOfProduct)
                {
                    sum += item.Quantity;
                }
                productSum.Add(new SellProductModel
                {
                    Product = product,
                    TotalQuantitySold = sum
                });
            }
            var bestSellers = productSum.OrderByDescending(ps => ps.TotalQuantitySold).Take(number);
            return bestSellers;
        }
    }
}
