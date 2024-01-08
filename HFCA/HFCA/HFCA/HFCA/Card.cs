using System;
using System.Collections.Generic;
using System.Text;

namespace HFCA
{
    public enum SpectralType
    {
        C,S,M,V,D
    }
    public enum CardType
    {
        Crew,
        Thruster,
        RegolitThruster,
        Radiator,
        Reactor,
        Generator,
        Robonaut,
        Rafinery,
        Bernal,
        Colonist,
        GwTwThruster,
        Freighter
    }

    public class Card
    {
        public CardType Type { get; set; }

        public double FuelUse { get; set; }
        public int Thrust { get; set; }
        public int AfterBurn { get; set; }
        public int Mass { get; set; }
        public int RadHard { get; set; }
        public string Name { get; set; }
        public string Requirements { get; set; }
        public string Supports { get; set; }
        public int FreeTurns { get; set; }
        public bool IsSolarPowered { get; set; }
        public bool IsThrustModificator { get; set; }
        public bool IsPushable { get; set; }
        public Card RadiatorOtherSide { get; set; }
        //public int LoadLimit { get; set; }
        //public SpectralType SpectralType { get; set; }
        //public bool isHuman { get; set; }
        //public bool isRobot { get; set; }
        //public int ISRU { get; set; }
        //public RobonautType RobonautType { get; set; }
    }
}
