using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HFCA;
using SQLite;

namespace HFCA
{
    public class CardDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public CardDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
#if DEBUG
            _database.DropTableAsync<Card>().Wait();
#endif
            var result = _database.CreateTableAsync<Card>().Result;
            if (result == CreateTableResult.Created)
                _database.InsertAllAsync(Data);
        }

        public ObservableCollection<Card> GetCards()
        {
            return new ObservableCollection<Card>(_database.Table<Card>().ToListAsync().Result);
        }

        public Task<Card> GetCard(int id)
        {
            return _database.GetAsync<Card>(id);
        }

        private List<Card> Data => new List<Card>()
        {
            new Card()
            {
                Name = "Test Thruster",
                AfterBurn = 2,
                FreeTurns = 1,
                FuelUse = 2,
                IsPushable = false,
                IsSolarPowered = false,
                Mass = 4,
                RadHard = 5,
                Thrust = 3,
                Type = CardType.Thruster,
                Requirements = "Xt",
            },
            new Card()
            {
                Name = "Test Generator",
                Mass = 2,
                RadHard = 3,
                Type = CardType.Generator,
                Supports = "e",
                Requirements = "t",
            },
            new Card()
            {
                Name = "Test Reactor",
                IsThrustModificator = true,
                AfterBurn = 2,
                FuelUse = 1/4d,
                IsSolarPowered = true,
                Mass = 2,
                RadHard = 7,
                Type = CardType.Reactor,
                Supports = "X",
                Requirements = "e",
            },
            new Card()
            {
                Name = "Test Robonaut",
                AfterBurn = 1,
                FuelUse = 4,
                Mass = 3,
                RadHard = 1,
                Thrust = 5,
                Type = CardType.Robonaut,
                Requirements = "X",
            },
        };
    }
}