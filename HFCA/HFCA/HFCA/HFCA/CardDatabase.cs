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
                ID = 1,
                OtherSideId = 2,
                SpectralType = SpectralType.V,
                Name = "Ablative Plate\r\n",
                Type = CardType.Thruster,
                Mass = 1,
                RadHard = 7,
                Thrust = 2,
                FuelUse = 2,
                FreeTurns = 0,
                AfterBurn = 1,
                IsPushable = true,
                IsSolarPowered = false,
                Requirements = "XB",
            },
            new Card()
            {
                ID = 2,
                OtherSideId = 1,
                Name = "Ablative Nozzle",
                SpectralType= SpectralType.V,
                Mass = 0,
                RadHard = 8,
                Type = CardType.Thruster,
                Thrust= 3,
                FuelUse = 2,
                FreeTurns = 0,
                AfterBurn = 1,
                IsPushable = true,
                IsSolarPowered = false,
                Requirements = "X~B",
            },
            new Card()
            {
                ID = 3,
                OtherSideId = 4,
                Name = "De Laval Nozzle",
                SpectralType= SpectralType.M,
                Mass = 0,
                RadHard = 6,
                Type = CardType.Thruster,
                Thrust= 5,
                FuelUse = 4,
                FreeTurns = 0,
                AfterBurn = 2,
                IsPushable = false,
                IsSolarPowered = false,
                Requirements = "~B",
            },
            new Card()
            {
                ID = 4,
                OtherSideId = 3,
                Name = "Magnetic Nozzle",
                SpectralType= SpectralType.M,
                Mass = 0,
                RadHard = 5,
                Type = CardType.Thruster,
                Thrust= 3,
                FuelUse = 1,
                FreeTurns = 0,
                AfterBurn = 3,
                IsPushable = false,
                IsSolarPowered = false,
                Requirements = "~",
            },
            
        };
    }
}