using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

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
        public int ID { get; set; }
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
        public int RadiatorOtherSideId { get; set; }
        //public int LoadLimit { get; set; }
        //public SpectralType SpectralType { get; set; }
        //public bool isHuman { get; set; }
        //public bool isRobot { get; set; }
        //public int ISRU { get; set; }
        //public RobonautType RobonautType { get; set; }

        [Ignore]
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
        public readonly DataTemplate DataTemplate = new DataTemplate(() => 
        {
            var itemStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = 2,
                HeightRequest = 25,
            };
            itemStack.SetBinding(StackLayout.BackgroundColorProperty, "Color");

            Label nameLabel = new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };
            nameLabel.SetBinding(Label.TextProperty, "Name");
            itemStack.Children.Add(nameLabel);

            Label massLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                WidthRequest = 20,
            };
            massLabel.SetBinding(Label.TextProperty, "Mass");
            itemStack.Children.Add(massLabel);

            Label radLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                WidthRequest = 20,
            };
            radLabel.SetBinding(Label.TextProperty, "RadHard");
            itemStack.Children.Add(radLabel);

            var symbolStack = new StackLayout() { Orientation = StackOrientation.Vertical, WidthRequest = 50, HeightRequest = 25, Padding = -3, Spacing = -7.5};
            itemStack.Children.Add(symbolStack);

            Label suppLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            suppLabel.SetBinding(Label.TextProperty, "Supports");
            symbolStack.Children.Add(suppLabel);

            Label reqLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            reqLabel.SetBinding(Label.TextProperty, "Requirements");
            symbolStack.Children.Add(reqLabel);

            Label thrustLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                WidthRequest = 30,
            };
            thrustLabel.SetBinding(Label.TextProperty, "Thrust");
            itemStack.Children.Add(thrustLabel);

            Label fuelLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                WidthRequest = 30,
            };
            fuelLabel.SetBinding(Label.TextProperty, "FuelUse");
            itemStack.Children.Add(fuelLabel);

            Label afterLabel = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                WidthRequest = 30,
            };
            afterLabel.SetBinding(Label.TextProperty, "AfterBurn");
            itemStack.Children.Add(afterLabel);

            return new ViewCell
            {
                View = itemStack,
            };
        });
    }
}
