using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Color Color
        {
            get
            {
                switch (Type)
                {
                    case CardType.Thruster:
                        return Color.Yellow;
                    case CardType.Generator:
                        return Color.Orange;
                    case CardType.Reactor:
                        return Color.MediumPurple;
                    case CardType.Robonaut:
                        return Color.DeepPink;
                    case CardType.Radiator:
                        return Color.LightBlue;
                    case CardType.Rafinery:
                        return Color.Gray;
                    case CardType.GwTwThruster:
                        return Color.Yellow;
                    case CardType.Colonist:
                        return Color.DarkGray;
                    case CardType.Freighter:
                        return Color.Blue;
                    case CardType.Bernal:
                        return Color.LightYellow;
                    default:
                        return Color.White;
                }
            }
        }
    }
}
