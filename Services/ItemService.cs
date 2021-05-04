using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using doggo.Models;
using doggo.Helpers;
using doggo.Data;

namespace doggo.Services
{
    public interface IItemService
    {
        IEnumerable<StockSummaryDTO> StockSummary();
        StockSummaryDTO StockSummaryById(int id);
        IEnumerable<StockRecordDTO> GetStockRecordById(int id);
        ItemInfoView FullItemInfo(int id);
        Task<IEnumerable<ItemDTO>> GetItems();
        Task AddById(int id, int amount);
        Task DeleteById(int id, int amount);
        IEnumerable<HistoryView> GetHistoryById(int userId);
        Task<Backpass> DeleteReservationById(int id);
        Task<Backpass> BatchDeleteReservation(int itemId, int userId, DateTime reserveDate, List<int> slots);
        Task<TimeTableView> GetReservationByItemId(int id);
        Task<TimeTableView> GetReservationByItemId(int id, DateTime reserveDate);
        Task<ReserveAvailable> GetReserveAvailables(DateTime selectedDate);
    }

    public class ItemService : IItemService
    {
        private readonly AppSettings _appSettings;
        private readonly DBContext db;

        public ItemService(IOptions<AppSettings> appSettings, DBContext context)
        {
            _appSettings = appSettings.Value;
            db = context;
        }
        
        public IEnumerable<StockSummaryDTO> StockSummary(){
            var res = (
                from i in db.Item
                join s in (from rs in db.ItemStock group rs by rs.ItemId into ss
                select new {
                    ItemId=ss.Key,
                    Current=ss.Sum(d => (Int32)(d.Type=="Increment"?d.Amount:(d.Amount * -1))),
                    Increment=ss.Sum(d => (Int32)(d.Type=="Increment"?d.Amount:0)),
                    Decrement=ss.Sum(d => (Int32)(d.Type=="Decrement"?d.Amount:0))
                }) on i.Id equals s.ItemId into gis
                from s in gis.DefaultIfEmpty()
                select new StockSummaryDTO {
                    Id=i.Id,
                    Name=i.Name,
                    Location=i.Location,
                    Current=s!=null?s.Current:0,
                    Increment=s!=null?s.Increment:0,
                    Decrement=s!=null?s.Decrement:0
                }
            );

            return res;
        }

        
        public StockSummaryDTO StockSummaryById(int id){
            return StockSummary().FirstOrDefault(m=> m.Id == id);
        }
        public IEnumerable<StockRecordDTO> GetStockRecordById(int id){
            var res = (
                from i in db.ItemStock
                where i.ItemId==id
                orderby i.Id descending
                select new StockRecordDTO {
                    Id=i.Id,
                    Type=i.Type,
                    Amount=i.Amount,
                    Snapshot=(
                        from s in db.ItemStock
                        where s.ItemId==id & s.Id<=i.Id
                        group s by s.ItemId into ss
                        select new{
                            Snapshot=ss.Sum(d => (Int32)(d.Type=="Increment"?d.Amount:(d.Amount * -1)))
                        }).First().Snapshot,
                    CreatedDate=i.CreatedDate
                }
            );

            return res;
        }

        public ItemInfoView FullItemInfo(int id){
            var StockSummary = StockSummaryById(id);
            var StockRecord = GetStockRecordById(id);

            return new ItemInfoView {
                StockSummary=StockSummary,
                StockRecord=StockRecord
            };
        }
        public async Task<IEnumerable<ItemDTO>> GetItems(){
            return await db.Item.ToListAsync();
        }
        public async Task AddById(int id, int amount){
            ItemStockDTO isd = new ItemStockDTO {
                ItemId=id,
                Type="Increment",
                Amount=amount,
                CreatedDate=DateTime.Now
            };

            db.Add(isd);
            await db.SaveChangesAsync();
        }
        public async Task DeleteById(int id, int amount){
            ItemStockDTO isd = new ItemStockDTO {
                ItemId=id,
                Type="Decrement",
                Amount=amount,
                CreatedDate=DateTime.Now
            };

            db.Add(isd);
            await db.SaveChangesAsync();
        }
        public IEnumerable<HistoryView> GetHistoryById(int userId){
            var res = (
                from rec in db.ReservationRecord
                join item in db.Item
                on rec.ItemId equals item.Id
                where rec.UserId==userId
                orderby rec.Id descending
                select new HistoryView {
                    Id=rec.Id,
                    ItemName=item.Name,
                    ItemLocation=item.Location,
                    Timeslot=rec.Timeslot,
                    ReserveDate=rec.ReserveDate,
                    CreatedDate=rec.CreatedDate
                } 
            );

            return res;
        }
        public async Task<Backpass> DeleteReservationById(int id)
        {
            var record = await db.ReservationRecord.FirstOrDefaultAsync(d=>d.Id==id);
            db.Remove(record);
            await db.SaveChangesAsync();
            return new Backpass
            {
                Error = false,
                Data = "Deleted"
            };
        }
        public async Task<Backpass> BatchDeleteReservation(int itemId, int userId, DateTime reserveDate, List<int> slots)
        {
            slots.ForEach(iter => {
                var res = db.ReservationRecord.FirstOrDefault(record => record.ItemId==itemId & record.UserId==userId & record.ReserveDate==reserveDate & record.Timeslot==iter);
                db.ReservationRecord.Remove(res);
            });
            await db.SaveChangesAsync();
            
            return new Backpass
            {
                Error = false,
                Data = "Deleted"
            };
        }
        public async Task<TimeTableView> GetReservationByItemId(int id){
            return await GetReservationByItemId(id, DateTime.Today);
        }
        public async Task<TimeTableView> GetReservationByItemId(int id, DateTime reserveDate){
            var res = db.TimeTable.FromSqlRaw(
                "select i.Id," +
                        "i.UserId," +
                        "u.Name," +
                        "group_concat(i.Timeslot order by i.Timeslot ASC) as Timeslot " +
                "from `ReservationRecord` as i "  +
                "inner join `User` as u " +
                "on i.UserId=u.Id " +
                "where i.ItemId=" + id + " and i.ReserveDate='" + (reserveDate.ToString("yyyy-MM-ddTHH:mm:ssZ")) + "' " +
                "group by i.UserId, i.ReserveDate " +
                "order by i.UserId"
                );
            List<TimeTable> tb = new List<TimeTable>();
            
            await res.ForEachAsync(iter => {
                var lst = iter.Timeslot.Split(',').Select(Int32.Parse).ToList();
                tb.Add(new TimeTable{
                    UserId=iter.UserId,
                    Name=iter.Name,
                    Timeslot=lst
                });
            });

            return new TimeTableView{
                ItemId=id,
                ItemName=db.Item.FirstOrDefault(d=>d.Id==id).Name,
                ReserveDate=reserveDate,
                Table=tb
            };
        }

        public async Task<ReserveAvailable> GetReserveAvailables(DateTime selectedDate){   
            var records = db.ReservationRecord.Where(rec => rec.ReserveDate == selectedDate);
            var stocks = StockSummary();
            Console.WriteLine(stocks);
            ReserveAvailable reserveAvailable = new ReserveAvailable{
                reserveList = new List<ReserveItem>()
            };
            stocks.ToList().ForEach(item => {
                ReserveItem reserveItem = new ReserveItem{
                    itemId = item.Id,
                    name = item.Name,
                    location = item.Location,
                    amount = new ReserveItemAmount{
                        t0910 = item.Current,
                        t1011 = item.Current,
                        t1112 = item.Current,
                        t1213 = item.Current,
                        t1314 = item.Current,
                        t1415 = item.Current,
                        t1516 = item.Current,
                        t1617 = item.Current,
                        t1718 = item.Current,
                        t1819 = item.Current
                    }
                };
                reserveAvailable.reserveList.Add(reserveItem);
            });

            reserveAvailable.reserveList.ForEach(reserveItem => {
                records.ToList().ForEach(rec => {
                    if(rec.ItemId == reserveItem.itemId){
                        switch (rec.Timeslot)
                        {
                            case 1:
                                reserveItem.amount.t0910 -= 1;
                                break;
                            case 2:
                                reserveItem.amount.t1011 -= 1;
                                break;
                            case 3:
                                reserveItem.amount.t1112 -= 1;
                                break;
                            case 4:
                                reserveItem.amount.t1213 -= 1;
                                break;
                            case 5:
                                reserveItem.amount.t1314 -= 1;
                                break;
                            case 6:
                                reserveItem.amount.t1415 -= 1;
                                break;
                            case 7:
                                reserveItem.amount.t1516 -= 1;
                                break;
                            case 8:
                                reserveItem.amount.t1617 -= 1;
                                break;
                            case 9:
                                reserveItem.amount.t1718 -= 1;
                                break;
                            default:
                                reserveItem.amount.t1819 -= 1;
                                break;
                        }
                    }
                });
            });

            return reserveAvailable;
        }
    }
}