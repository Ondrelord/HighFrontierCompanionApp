using UnityEngine;
using System;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using static MainManager;
using Unity.VisualScripting;


public class ThrustTriangle
{
    public int Thrust { get; set; }
    public double Fuel_Consumption_Double { get => Helper_functions.FractionToDouble(Fuel_Consumption); }
    public string Fuel_Consumption { get; set; }
    public string Fuel_Type { get; set; } //"Water", "Dirt", "Isotope", "Modification"
    public int Bonus_Pivots { get; set; }
    public int Afterburn { get; set; }
    public bool Push { get; set; }
    public bool Solar { get; set; }
    public bool Pacman { get; set; }
}

public class Thruster
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Spectral_Type { get; set; }
    public int Mass { get; set; }
    public int Rad_Hard { get; set; }
   
    public bool C_Generator { get; set; }
    public bool e_Generator { get; set; }
    public bool X_Reactor { get; set; }
    public bool S_Reactor { get; set; }
    public bool B_Reactor { get; set; }
    public int Therms { get; set; }
    public string Ability { get; set; }

    public ThrustTriangle ThrustTriangle { get; set; }

    public Thruster(SqliteDataReader reader)
    {
        this.ID = reader.GetInt32(0);
        this.Name = reader.GetString("Name");
        this.Spectral_Type = reader.GetString("Spectral_Type");
        this.Mass = reader.GetInt32("Mass");
        this.Rad_Hard = reader.GetInt32("Rad_Hard");

        this.C_Generator = reader.GetBoolean("C_Generator");
        this.e_Generator = reader.GetBoolean("e_Generator");
        this.X_Reactor = reader.GetBoolean("X_Reactor");
        this.S_Reactor = reader.GetBoolean("S_Reactor");
        this.B_Reactor = reader.GetBoolean("B_Reactor");
        this.Therms = reader.GetInt32("Therms");
        this.Ability = reader.GetString("Ability");
        
        this.ThrustTriangle.Thrust = reader.GetInt32("Thrust");
        this.ThrustTriangle.Fuel_Consumption = reader.GetString("Fuel_Consumption");
        this.ThrustTriangle.Fuel_Type = reader.GetString("Fuel_Type");
        this.ThrustTriangle.Bonus_Pivots = reader.GetInt32("Bonus_Pivots");
        this.ThrustTriangle.Afterburn = reader.GetInt32("Afterburn");
        this.ThrustTriangle.Push = reader.GetBoolean("Push");
        this.ThrustTriangle.Solar = reader.GetBoolean("Solar");
        this.ThrustTriangle.Pacman = false;
    }
}

public static class Helper_functions
{
    public enum HelioZones 
    {
        Merkur   = 2,
        Venus    = 1,
        Earth    = 0,
        Mars     = -1,
        Ceres    = -2,
        Jupiter  = -3,
        Saturn   = -4,
        Uran     = -5,
        Neptun   = -999
    };

    public readonly static List<double> PossibleSteps = new List<double>{
        1, 1+1/9d, 1+2/9d, 1+3/9d, 1+4/9d, 1+5/9d, 1+6/9d, 1+7/9d, 1+8/9d,
        2, 2+1/6d, 2+2/6d, 2+3/6d, 2+4/6d, 2+5/6d,
        3, 3+1/4d, 3+2/4d, 3+3/4d,
        4, 4+1/3d, 4.6d, //2/3
        5, 5.2d, 5+2/3d,
        6, 6+1/2d,
        7, 7+1/2d,
        8, 8+1/2d,
        9, 9+1/2d,
        10, 10.45d,
        11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23,
        24, 25, 26, 27, 28, 29, 30, 31, 32
        };

    /// <summary>
    /// String representation of the wet mass in steps.
    /// </summary>
    public string ToStepFractionCount()
    {
        int floorValue = (int)Math.Floor(WetMass);
        int ceilValue = (int)Math.Ceiling(WetMass);

        var indexFV = possibleSteps.IndexOf(floorValue);
        var indexCV = possibleSteps.IndexOf(ceilValue);
        var indexPV = possibleSteps.IndexOf(WetMass);

        if (indexPV - indexFV == 0) return "";
        return (indexPV - indexFV).ToString() + "/" + (indexCV - indexFV).ToString();
    }

    /// <summary>
    /// Converts a string representation of a fraction to a double.
    /// </summary>
    public static double FractionToDouble(string fraction)
    {
        double result;

        if (double.TryParse(fraction, out result))
        {
            return result;
        }

        string[] split = fraction.Split(new char[] { ' ', '/' });

        if (split.Length == 2 || split.Length == 3)
        {
            int a, b;

            if (int.TryParse(split[0], out a) && int.TryParse(split[1], out b))
            {
                if (split.Length == 2)
                {
                    return (double)a / b;
                }

                int c;

                if (int.TryParse(split[2], out c))
                {
                    return a + (double)b / c;
                }
            }
        }

        throw new FormatException("Not a valid fraction.");
    }
}

public class MainManager : MonoBehaviour
{
    public ShipStack ActiveShipStack;


    public GameObject CardPrefab;
    public GameObject CardContent;




    // Start is called before the first frame update
    void Start()
    {
        string connection = "URI=file:" + Application.persistentDataPath + "/" + "HF_card_DB";
        using (var dbcon = new SqliteConnection(connection))
        {
            dbcon.Open();

            var command = dbcon.CreateCommand();
            command.CommandText =
            @"
                SELECT rowid, * 
                FROM Thrusters
            "
            ;
            //SELECT ROW_NUMBER() OVER(ORDER BY(SELECT 1)) AS RowId, *
            //    FROM Thrusters
            //    WHERE ROWID = $id
            //command.Parameters.AddWithValue("$id", 1);

            using (var reader = command.ExecuteReader())
            {
                var deck = new List<Thruster>();
                while (reader.Read())
                {
                    deck.Add(new Thruster(reader));
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {

    }
}

public class Card<T>
{
    public T Front { get; set; }
    public T Back { get; set; }

    
}

public class Deck<Card, T>
{
    private List<Card<T>> _cards;
    public List<Card<T>> Cards
    {
        get
        {
            if (_cards == null)
            {
                FillDeck();
            }
            return _cards;
        }
        set
        {
            _cards = value;
        }
    }

    private void FillDeck()
    {
        var type = nameof(T);

        string connection = "URI=file:" + Application.persistentDataPath + "/" + "HF_card_DB";
        using (var dbcon = new SqliteConnection(connection))
        {
            dbcon.Open();

            var command = dbcon.CreateCommand();
            command.CommandText =
            @"
                SELECT rowid, * 
                FROM " + type //TODO: rename csv to "Thruster"
            ;
            //SELECT ROW_NUMBER() OVER(ORDER BY(SELECT 1)) AS RowId, *
            //    FROM Thrusters
            //    WHERE ROWID = $id
            //command.Parameters.AddWithValue("$id", 1);

            using (var reader = command.ExecuteReader())
            {
                var _cards = new List<Card<T>>();
                while (reader.Read())
                {
                    var newCard = Card<T>();
                    newCard.Front = new T(reader); //read front side of the card
                    reader.Read();
                    newCard.Front = new T(reader); // read back side of the card
                    _cards.Add(newCard);
                }

            }
        }
    }
}

public class ShipStack : MonoBehaviour
{
    public ThrustTriangle ActiveThrusterTriangle // Currently active thruster modified by reactors and generators
    {
        get
        {
            var activeThruster = ActiveCards.Single(x => x.ThrustTriangle != null && x.ThrustTriangle.Fuel_Type != "Modification").ThrustTriangle;

            foreach (var card in ActiveCards)
            {
                if (card.ThrustTriangle != null && card.ThrustTriangle.Fuel_Type == "Modification")
                {
                    activeThruster.Thrust += card.ThrustTriangle.Thrust;
                    activeThruster.Fuel_Consumption = (activeThruster.Fuel_Consumption_Double * card.ThrustTriangle.Fuel_Consumption_Double).ToString();
                    activeThruster.Solar = activeThruster.Solar || card.ThrustTriangle.Solar;
                    activeThruster.Pacman = activeThruster.Pacman || card.ThrustTriangle.Pacman;
                }
            }
            return activeThruster;
        }
    } 

    public List<Card> ActiveCards { get; set; }
    public List<Card> CargoCards { get; set; } // Cards that does not affect thrusters or needs to be functional to be used

    public bool isPushed { get; set; } // specify if ship has used power sat push
    public bool AfterburnUsed { get; set; } // spcify if afterburn is added to the net thrust or not
    public bool isSolarPowered { get; set; }
    public Helper_functions.HelioZones HelioZone { get; set; }


    // Mass
    public int DryMass { get; set; }
    public double WetMass { get => DryMass + FuelTanks; }
    private double _fuelTanks;
    public double FuelTanks { get => _fuelTanks; set => _fuelTanks = value < 0 ? 0 : value + DryMass > 32 ? 32 - DryMass : value; }

    // Rad_hard
    public int Rad_Hard { get; set; }

    // Burns
    public int BurnsUsedThisTurn;
    public int BurnsLeftThisTurn { get { return Math.Max(0, Math.Min(NetThrust, BurnsLeftAll)) - BurnsUsedThisTurn; } }
    public int BurnsLeftAll { get { return ActiveThrusterTriangle.Fuel_Consumption_Double != 0 ? (int)(stepsLeft / ActiveThrusterTriangle.Fuel_Consumption_Double) : 999; } }
    public int stepsLeft { get => Helper_functions.PossibleSteps.Count(x => x >= DryMass && x < WetMass); }

    // Free turns
    public int BonusPivotsLeft { get; set; }

    public double LeftoverBurnedFuel { get; set; }

    // NET Thrust
    public bool isDuringMovement;
    public int RemainingThrust;
    public int finalNetThrust; // can be higher than 15
    public int NetThrust
    {
        get
        {
            if (isDuringMovement) return RemainingThrust;

            finalNetThrust = ActiveThrusterTriangle.ThrustTriangle.Thrust;
            if (ActiveThrusterTriangle.ThrustTriangle.IsSolarPowered)
            {
                if (solarBonus < -5)
                    return 0;
                finalNetThrust += solarBonus;
            }
            if (isPushed)
                ++finalNetThrust;
            if (afterburnerUsed)
                if (ActiveThrusterTriangle.Fuel_Type == "Isotope")
                    finalNetThrust += ActiveThrusterTriangle.Afterburn;
                else
                    ++finalNetThrust;
            if (WetMass < 2)
                finalNetThrust += 2;
            else if (WetMass < 4.5)
                finalNetThrust += 1;
            else if (WetMass < 8.2)
                finalNetThrust += 0;
            else if (WetMass < 17)
                finalNetThrust -= 1;
            else finalNetThrust -= 2;

            RemainingThrust = Math.Min(finalNetThrust, 15);
            return RemainingThrust;
        }
    }

    public void BurnButton_Clicked(object sender, EventArgs e)
    {
        if (isDuringMovement == false) isDuringMovement = true;

        if (BurnsLeftThisTurn == 0)
        {
            DisplayAlert("Burn failed!", "No more burns left this turn.", "OK");
            return;
        }
        ++burnsUsedThisTurn;

        BurnFuel(ActiveThrusterTriangle.Fuel_Consumption_Double);
    }
    
    public void BurnFuel(double fuelUsed)
    {
        LeftoverBurnedFuel += fuelUsed;
        int burnedFuel = (int)LeftoverBurnedFuel;
        LeftoverBurnedFuel -= burnedFuel;

        var stepOrig = possibleSteps.IndexOf(WetMass);

        var heavySteps = possibleSteps.IndexOf(24);
        if (stepOrig > heavySteps)
        {
            double heavyStepsCount = stepOrig - heavySteps;
            double modulo = heavyStepsCount / 2;
            double doublePricedSteps = Math.Min(burnedFuel, modulo);
            int bonusBurns = (int)Math.Ceiling(doublePricedSteps);
            burnedFuel += bonusBurns;
        }

        var stepNew = stepOrig - burnedFuel;
        FuelTanks = possibleSteps[stepNew] - DryMass;
    }

    // use afterburner
    public void AfterBurnButton_Clicked(object sender, EventArgs e)
    {
        if (isDuringMovement)
        {
            // TODO: spawn warning message "Afterburnes should be used before start of movement"
            // Choice "cancel" or "use anyway" => to ignore warning
            return;
        }

        afterburnerUsed = true;

        int stepCount = ActiveThrusterTriangle.Fuel_Type == "Isotope" ? 1 : ActiveThrusterTriangle.Afterburn;
        BurnFuel(stepCount);
    }
    

    // TODO: setting an auto powersat use
    // power sat button switch
    public void PowerSatButton_Clicked(object sender, EventArgs e)
    {
        isPushed = !isPushed;
    }

}   


//TODO: some notification system for user and his actions
//TODO: UNDO button
