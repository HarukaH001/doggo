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
        Task<ItemInfoView> AddById(int id, int amount);
        Task<ItemInfoView> DeleteById(int id, int amount);
        IEnumerable<HistoryView> GetHistoryById(int userId);
        Task<Backpass> DeleteReservationById(int id);
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
        public async Task<ItemInfoView> AddById(int id, int amount){
            ItemStockDTO isd = new ItemStockDTO {
                ItemId=id,
                Type="Increment",
                Amount=amount,
                CreatedDate=DateTime.Now
            };

            db.Add(isd);
            await db.SaveChangesAsync();

            return FullItemInfo(id);
        }
        public async Task<ItemInfoView> DeleteById(int id, int amount){
            ItemStockDTO isd = new ItemStockDTO {
                ItemId=id,
                Type="Decrement",
                Amount=amount,
                CreatedDate=DateTime.Now
            };

            db.Add(isd);
            await db.SaveChangesAsync();

            return FullItemInfo(id);
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
    }
}