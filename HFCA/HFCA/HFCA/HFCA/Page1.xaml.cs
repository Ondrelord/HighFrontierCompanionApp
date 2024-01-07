using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace HFCA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        #region test
        private double fuelUse = 1.5;
        private bool solarPowered = true;
        private int afterburnerStrenght = 1;
        
        #endregion

        private readonly List<double> possibleSteps = new List<double>{
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

        private readonly List<string> HelioZones = new List<string>(){
            "Merkur",
            "Venus",
            "Earth",
            "Mars",
            "Ceres",
            "Jupiter",
            "Saturn",
            "Uran",
            "Neptun"
        };

        private int dryMass;
        private int minRadHard;
        private int netThrust;
        private double fuelTanks;
        private int burnsLeftThisTurn;
        private int freeTurns;
        private double leftoverBurnedFuel;
        
        private int solarBonus;
        private bool afterburnerUsed = false;
        private bool powerSatPushed = false;

        public int DryMass { get { return dryMass; } set { dryMass = value; OnPropertyChanged(nameof(DryMass)); OnPropertyChanged(nameof(massLabelText)); OnPropertyChanged(nameof(burnsLeftAllLabelText)); } }
        public double WetMass { get { return DryMass + FuelTanks; } }
        public int MinRadHard { get { return minRadHard; } set { minRadHard = value; OnPropertyChanged(nameof(MinRadHard)); } }
        public double FuelTanks { get { return fuelTanks; } set { fuelTanks = value; OnPropertyChanged(nameof(FuelTanks)); OnPropertyChanged(nameof(fuelTanksLabelText)); OnPropertyChanged(nameof(massLabelText)); OnPropertyChanged(nameof(burnsLeftAllLabelText)); OnPropertyChanged(nameof(NetThrust)); } }
        public int BurnsLeftThisTurn { get { return burnsLeftThisTurn; } set { burnsLeftThisTurn = value; OnPropertyChanged(nameof(BurnsLeftThisTurn)); OnPropertyChanged(nameof(turnAvailability)); } }
        public int BurnsLeftAll { get { return (int)(stepsLeft / fuelUse); } }
        public int FreeTurns { get { return freeTurns; } set { freeTurns = value; OnPropertyChanged(nameof(turnAvailabilityLabel)); OnPropertyChanged(nameof(turnAvailability)); } }
        public double LeftoverBurnedFuel { get => leftoverBurnedFuel; set { leftoverBurnedFuel = value; OnPropertyChanged(nameof(endTurnButtonText)); } }
        public int NetThrust { 
            get 
            {
                var final = netThrust;
                if (solarPowered)
                {
                    if (solarBonus < -5)
                        return 0;
                    final += solarBonus;
                }
                if (powerSatPushed)
                    ++final;
                if (afterburnerUsed)
                    final += afterburnerStrenght;

                return Math.Min(final, 15); // max value 15
            } 
            set 
            { 
                netThrust = value; 
                OnPropertyChanged(nameof(NetThrust));
                OnPropertyChanged(nameof(BurnsLeftThisTurn));
            } }


        public string fuelTanksLabelText { get => (((int)FuelTanks) == 0 && FuelTanks > 0) ? ToStepFractionCount() : ((int)FuelTanks).ToString() + " " + ToStepFractionCount(); }
        public string massLabelText { get => DryMass.ToString() + " / " + ((((int)WetMass == 0) && WetMass > 0) ? ToStepFractionCount() :((int)WetMass).ToString() + " " + ToStepFractionCount()); }
        public int stepsLeft { get => possibleSteps.Count(x => x >= DryMass && x < WetMass); }
        public string burnsLeftAllLabelText { get => BurnsLeftAll.ToString() + " (" + stepsLeft.ToString() + ")"; }
        public Color turnAvailability { get => freeTurns > 0 ? Color.Green : BurnsLeftThisTurn > 1 ? Color.DeepPink : Color.DarkRed; }
        public string turnAvailabilityLabel { get => freeTurns > 0 ? "Turn\n(Free)" : "Turn"; }
        public string endTurnButtonText { get => LeftoverBurnedFuel > 0d ? "End\nTurn\n Leftover Burn : " + Math.Round(LeftoverBurnedFuel, 2).ToString() : "End\nTurn"; }
        public bool AfterBurnButtonEnable { get => !afterburnerUsed; }

        public Page1()
        {
            //TEST data
            DryMass = 1;
            MinRadHard = 1;
            NetThrust = 3;
            fuelTanks = 1;
            freeTurns = 1;
            burnsLeftThisTurn = Math.Min(NetThrust, BurnsLeftAll);

            InitializeComponent();
            BindingContext = this;
            SetupPage();

        }

        private void SetupPage()
        {
            StackLayout mainPageLayout = new StackLayout()
            {
                Spacing = 0,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            Grid grid = new Grid();
            mainPageLayout.Children.Add(grid);

            grid.ColumnSpacing = 0;
            grid.RowSpacing = 0;
            for (int i = 0; i < 8; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < 4; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = 50 });
            grid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 4; j++)
                    grid.Children.Add(new BoxView() { Color = new Color(0.1 * i, 0.1 * j, 0.05 * i + 0.05 * j) }, i, j);

            SetMass(grid);
            SetMinRadHard(grid);
            SetNetThrust(grid);
            SetFuelTanks(grid);
            SetBurnsLeft(grid);

            ListView CardList = new ListView();
            mainPageLayout.Children.Add(CardList);
            
            Grid buttonGrid = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 200
            };
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.RowDefinitions.Add(new RowDefinition() { Height = 100 });
            buttonGrid.RowDefinitions.Add(new RowDefinition() { Height = 100 });
            mainPageLayout.Children.Add(buttonGrid);

            Button EndTurnButton = new Button()
            {
                Text = "End\nturn",
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = 100,
                WidthRequest = 150,
                BackgroundColor = Color.Blue,
                BorderColor = Color.White,
                BorderWidth = 8,
                BindingContext = this,
            };
            EndTurnButton.Clicked += EndTurn_Clicked;
            EndTurnButton.SetBinding(Button.TextProperty, "endTurnButtonText");
            buttonGrid.Children.Add(EndTurnButton, 0, 2, 1, 2);
            
            Button TurnButton = new Button()
            {
                Text = "Turn",
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = 100,
                WidthRequest = 150,
                BackgroundColor = Color.Green,
                BorderColor = Color.White,
                BorderWidth = 8,
                BindingContext = this,
            };
            TurnButton.Clicked += TurnButton_Clicked;
            TurnButton.SetBinding(Button.BackgroundColorProperty, "turnAvailability");
            TurnButton.SetBinding(Button.TextProperty, "turnAvailabilityLabel");
            buttonGrid.Children.Add(TurnButton, 3, 5, 0, 1);

            Button BurnButton = new Button()
            {
                Text = "BURN!",
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 200,
                WidthRequest = 200,
                CornerRadius = 100,
                BackgroundColor = Color.DeepPink,
                BorderColor = Color.White,
                BorderWidth = 8
            };
            BurnButton.Clicked += BurnButton_Clicked;
            buttonGrid.Children.Add(BurnButton, 1,4,0,2);

            

            this.Content = mainPageLayout;
        }

        private void SetBurnsLeft(Grid grid)
        {
            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = "Burns left this turn",
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
            }, 4, 6, 2, 3);
            Label burnsLeftThisTurnLabel = new Label()
            {
                Text = BurnsLeftThisTurn.ToString(),
                BindingContext = this,
                BackgroundColor = Color.LightGray,
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 30,
            };
            burnsLeftThisTurnLabel.SetBinding(Label.TextProperty, "BurnsLeftThisTurn");
            
            grid.Children.Add(burnsLeftThisTurnLabel, 4, 6, 3, 4);


            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = "All Burns (steps) left",
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 18,
            }, 6, 8, 2, 3);
            Label burnsLeftAllLabel = new Label()
            {
                Text = burnsLeftAllLabelText,
                BindingContext = this,
                BackgroundColor = Color.LightGray,
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 30,
            };
            burnsLeftAllLabel.SetBinding(Label.TextProperty, "burnsLeftAllLabelText");
            grid.Children.Add(burnsLeftAllLabel, 6, 8, 3, 4);


        }

        private void SetMass(Grid grid)
        {
            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = "Mass\nDry/Wet",
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
            }, 0, 2, 0, 1);
            Label MassLabel = new Label
            {
                Text = DryMass.ToString() + " / " + WetMass.ToString(),
                BackgroundColor = Color.LightGray,
                BindingContext = this,
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
            };
            MassLabel.SetBinding(Label.TextProperty, "massLabelText");
            grid.Children.Add(MassLabel, 0, 2, 1, 2);
        }

        private void SetMinRadHard(Grid grid)
        {
            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = "Min. Rad. Hardness",
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
            }, 2, 4, 0, 1);
            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = MinRadHard.ToString(),
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 30,
            }, 2, 4, 1, 2);

        }

        private void SetNetThrust(Grid grid)
        {
            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = "Net thrust",
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
            }, 4, 8, 0, 1);

            StackLayout netThrustStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
            };
            grid.Children.Add(netThrustStack, 4, 8, 1, 2);

            Picker HelioZonePicker = new Picker()
            {
                Title = "HelioZone",
                ItemsSource = HelioZones,
                BindingContext = this,
                WidthRequest = 50,
                
            };
            HelioZonePicker.SelectedIndexChanged += HelioZonePicked;
            netThrustStack.Children.Add(HelioZonePicker);
            Label NetThrustLabel = new Label
            {
                Text = NetThrust.ToString(),
                BackgroundColor = Color.LightGray,
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 30,
                WidthRequest = 80,
            };
            NetThrustLabel.SetBinding(Label.TextProperty, "NetThrust");
            netThrustStack.Children.Add(NetThrustLabel);

            Button PowerSatButton = new Button()
            {
                Text = "P-Sat",
                BackgroundColor = Color.DarkRed,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = 50
            };
            PowerSatButton.Clicked += PowerSatButton_Clicked;
            netThrustStack.Children.Add(PowerSatButton);

            Button AfterBurnButton = new Button()
            {
                Text = afterburnerStrenght.ToString(),
                BackgroundColor = Color.OrangeRed,
                IsEnabled = !afterburnerUsed,
                BindingContext = this,
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = 50
            };
            AfterBurnButton.SetBinding(Button.IsEnabledProperty, "AfterBurnButtonEnable");
            AfterBurnButton.Clicked += AfterBurnButton_Clicked;
            netThrustStack.Children.Add(AfterBurnButton);
        }

        private void AfterBurnButton_Clicked(object sender, EventArgs e)
        {
            afterburnerUsed = true;
            OnPropertyChanged(nameof(AfterBurnButtonEnable));

            var steps = possibleSteps.IndexOf(WetMass);
            var index = steps > possibleSteps.IndexOf(24) ? steps - 2 : steps - 1;
            FuelTanks = possibleSteps[index] - DryMass;

            BurnsLeftThisTurn = Math.Min(NetThrust, BurnsLeftAll);
        }

        private void PowerSatButton_Clicked(object sender, EventArgs e)
        {
            powerSatPushed = !powerSatPushed;
            ((Button)sender).BackgroundColor = powerSatPushed ? Color.Green : Color.DarkRed;
            OnPropertyChanged(nameof(NetThrust));
            BurnsLeftThisTurn = Math.Min(NetThrust, BurnsLeftAll);
        }

        private void SetFuelTanks(Grid grid)
        {
            grid.Children.Add(new Label
            {
                BackgroundColor = Color.LightGray,
                Text = "Fuel Tanks",
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 20,
            }, 0, 4, 2, 3);

            StackLayout fuelTanksStack = new StackLayout() { HorizontalOptions = LayoutOptions.CenterAndExpand, Orientation = StackOrientation.Horizontal };
            grid.Children.Add(fuelTanksStack, 0, 4, 3, 4);
            Button FTMinus = new Button()
            {
                Text = "-",
                CornerRadius = 25,
                WidthRequest = 50,
                FontSize = 25,
            };
            FTMinus.Clicked += (sender, args) => FuelTanks = possibleSteps.OrderBy(x => Math.Abs((WetMass - 1.03) - x)).First() - DryMass;
            fuelTanksStack.Children.Add(FTMinus);
            Label FTLabel = new Label
            {
                Text = FuelTanks.ToString(),
                BindingContext = this,
                BackgroundColor = Color.LightGray,
                TextColor = Color.Green,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 30,
                WidthRequest = 70
            };
            FTLabel.SetBinding(Label.TextProperty, "fuelTanksLabelText");
            fuelTanksStack.Children.Add(FTLabel);
            Button FTPlus = new Button()
            {
                Text = "+",
                CornerRadius = 25,
                WidthRequest = 50,
                FontSize = 25,
            };
            FTPlus.Clicked += (sender, args) => FuelTanks = possibleSteps.OrderBy(x => Math.Abs((WetMass + 1.02) - x)).First() - DryMass;
            fuelTanksStack.Children.Add(FTPlus);

        }

        private void BurnButton_Clicked(object sender, EventArgs e)
        {
            if (BurnsLeftThisTurn == 0)
            {
                DisplayAlert("Burn failed!", "No more burns left this turn.", "OK");
                return;
            }
            --BurnsLeftThisTurn;

            LeftoverBurnedFuel += fuelUse;
            int burnedFuel = (int)LeftoverBurnedFuel;
            LeftoverBurnedFuel -= burnedFuel;

            var stepOrig = possibleSteps.IndexOf(WetMass);

            var heavySteps = possibleSteps.IndexOf(24);
            if (stepOrig > heavySteps)
            {
                double heavyStepsCount =  stepOrig - heavySteps;
                double modulo = heavyStepsCount / 2;
                double doublePricedSteps = Math.Min(burnedFuel, modulo);
                int bonusBurns = (int)Math.Ceiling(doublePricedSteps);
                burnedFuel += bonusBurns;
            }

            var stepNew = stepOrig - burnedFuel;
            FuelTanks = possibleSteps[stepNew] - DryMass;
        }

        private void TurnButton_Clicked(object sender, EventArgs e)
        {
            if (FreeTurns > 0)
            {
                --FreeTurns;
                //if (freeTurns == 0) ((Button)sender).BackgroundColor = Color.DeepPink;
                return;
            }
            if (burnsLeftThisTurn < 2)
            {
                DisplayAlert("Turn failed!", "No enough burns left this turn.", "OK");
                return;
            }
            BurnButton_Clicked((object)sender, e);
            BurnButton_Clicked((object)sender, e);

            //if (freeTurns == 0 && burnsLeftThisTurn < 2) ((Button)sender).BackgroundColor = Color.DarkRed;
        }
        private void EndTurn_Clicked(object sender, EventArgs e)
        {
            afterburnerUsed = false;
            BurnsLeftThisTurn = Math.Min(NetThrust, BurnsLeftAll);
            OnPropertyChanged(nameof(AfterBurnButtonEnable));
            OnPropertyChanged(nameof(NetThrust));

            if (LeftoverBurnedFuel > 0)
            {
                var steps = possibleSteps.IndexOf(WetMass);
                var index = steps > possibleSteps.IndexOf(24) ? steps - 2 : steps - 1;
                FuelTanks = possibleSteps[index] - DryMass;
            }
            LeftoverBurnedFuel = 0;

        }
        private void HelioZonePicked (object sender, EventArgs e)
        {
            if (solarPowered)
                solarBonus = -1 * (((Picker)sender).SelectedIndex - 2);
            else
                solarBonus = 0;
            OnPropertyChanged(nameof(NetThrust));
        }


        private string ToStepFractionCount()
        {
            int floorValue = (int)Math.Floor(WetMass);
            int ceilValue = (int)Math.Ceiling(WetMass);

            var indexFV = possibleSteps.IndexOf(floorValue);
            var indexCV = possibleSteps.IndexOf(ceilValue);
            var indexPV = possibleSteps.IndexOf(WetMass);

            if (indexPV - indexFV == 0) return "";
            return (indexPV - indexFV).ToString() + "/" + (indexCV - indexFV).ToString();
        }
    }    
}