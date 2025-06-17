using UnityEngine;
using System;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using static MainManager;
using Unity.VisualScripting;


public class Thruster
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Spectral_Type { get; set; }
    public int Mass { get; set; }
    public int Rad_Hard { get; set; }
    public int Thrust { get; set; }
    public double Fuel_Consumption_Double { get; set; }
    public string Fuel_Consumption { get; set; }
    public string Fuel_Type { get; set; }
    public int Bonus_Pivots { get; set; }
    public int Afterburn { get; set; }
    public bool Push { get; set; }
    public bool Solar { get; set; }
    public bool C_Generator { get; set; }
    public bool e_Generator { get; set; }
    public bool X_Reactor { get; set; }
    public bool S_Reactor { get; set; }
    public bool B_Reactor { get; set; }
    public int Therms { get; set; }
    public string Ability { get; set; }

    public Thruster(SqliteDataReader reader)
    {
        this.ID = reader.GetInt32(0);
        this.Name = reader.GetString("Name");
        this.Spectral_Type = reader.GetString("Spectral_Type");
        this.Mass = reader.GetInt32("Mass");
        this.Rad_Hard = reader.GetInt32("Rad_Hard");
        this.Thrust = reader.GetInt32("Thrust");
        this.Fuel_Consumption = reader.GetString("Fuel_Consumption");
        this.Fuel_Type = reader.GetString("Fuel_Type");
        this.Bonus_Pivots = reader.GetInt32("Bonus_Pivots");
        this.Afterburn = reader.GetInt32("Afterburn");
        this.Push = reader.GetBoolean("Push");
        this.Solar = reader.GetBoolean("Solar");
        this.C_Generator = reader.GetBoolean("C_Generator");
        this.e_Generator = reader.GetBoolean("e_Generator");
        this.X_Reactor = reader.GetBoolean("X_Reactor");
        this.S_Reactor = reader.GetBoolean("S_Reactor");
        this.B_Reactor = reader.GetBoolean("B_Reactor");
        this.Therms = reader.GetInt32("Therms");
        this.Ability = reader.GetString("Ability");
    }
}

public static class Helper_functions
{
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
