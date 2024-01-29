using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace HFCA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        #region test
        private ObservableCollection<Card> activeCards = new ObservableCollection<Card>()
        { 
            new Card()
            {
                Name = "TEST thruster",
                Type = CardType.Thruster,
                Thrust = 2,
                FuelUse = 1.5,
                AfterBurn = 1,
                Mass = 4,
                FreeTurns = 1,
                IsSolarPowered = true,
                IsPushable = false,
            }  
        };
        private ObservableCollection<Card> inactiveCards = new ObservableCollection<Card>();
        #endregion

        private ObservableCollection<Card> ActiveCards { get { return activeCards; } set { activeCards = value; OnPropertyChanged(nameof(ActiveCards)); } }
        private ObservableCollection<Card> InactiveCards { get { return inactiveCards; } set { inactiveCards = value; OnPropertyChanged(nameof(InactiveCards)); } }

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

        private Card ActiveThruster 
        {
            get 
            {
                var at = ActiveCards.SingleOrDefault(x => x.Type == CardType.Thruster);
                return at ?? new Card() 
                {
                    Name = "No Thruster",
                    AfterBurn = 0,
                    FreeTurns = 0,
                    FuelUse = 0,
                    IsPushable = false,
                    IsSolarPowered = false,
                    Mass = 0,
                    RadHard = 0,
                    Thrust = 0,
                    Type = CardType.Thruster,
                };
            }
        }
        private int solarBonus;
        private bool afterburnerUsed = false;
        private bool powerSatPushed = false;

        //MASS
        private int dryMass;
        public int DryMass { get { return dryMass; } set { dryMass = value;  OnPropertyChanged(nameof(massLabelText)); OnPropertyChanged(nameof(burnsLeftAllLabelText)); OnPropertyChanged(nameof(NetThrust)); OnPropertyChanged(nameof(BurnsLeftThisTurn)); } }
        public double WetMass { get { return DryMass + FuelTanks; } }
        public string massLabelText { get => DryMass.ToString() + " / " + ((((int)WetMass == 0) && WetMass > 0) ? ToStepFractionCount() : ((int)WetMass).ToString() + " " + ToStepFractionCount()); }
        
        //RAD HARD
        private int minRadHard;
        public int MinRadHard { get { return minRadHard; } set { minRadHard = value; OnPropertyChanged(nameof(MinRadHard)); } }
        
        //FUEL TANKS
        private double fuelTanks;
        public double FuelTanks { get { return fuelTanks; } set { fuelTanks = value < 0 ? 0 : value + DryMass > 32 ? 32 - DryMass : value; OnPropertyChanged(nameof(FuelTanks)); OnPropertyChanged(nameof(fuelTanksLabelText)); OnPropertyChanged(nameof(massLabelText)); OnPropertyChanged(nameof(burnsLeftAllLabelText)); OnPropertyChanged(nameof(BurnsLeftThisTurn)); OnPropertyChanged(nameof(NetThrust)); } }
        public string fuelTanksLabelText { get => (((int)FuelTanks) == 0 && FuelTanks > 0) ? ToStepFractionCount() : ((int)FuelTanks).ToString() + " " + ToStepFractionCount(); }

        //BURNS LEFT
        private int burnsUsedThisTurn;
                
        public int BurnsLeftThisTurn { get { return Math.Max(0, Math.Min(NetThrust, BurnsLeftAll)) - burnsUsedThisTurn; } }
        public int BurnsLeftAll { get { return ActiveThruster.FuelUse != 0 ? (int)(stepsLeft / ActiveThruster.FuelUse) : 999; } }
        public int stepsLeft { get => possibleSteps.Count(x => x >= DryMass && x < WetMass); }
        public string burnsLeftAllLabelText { get => BurnsLeftAll.ToString() + " (" + stepsLeft.ToString() + ")"; }

        //FREE TURNS
        private int freeTurns;
        public int FreeTurns { get { return freeTurns; } set { freeTurns = value; OnPropertyChanged(nameof(turnAvailabilityLabel)); OnPropertyChanged(nameof(turnAvailability)); } }
        public Color turnAvailability { get => freeTurns > 0 ? Color.Green : BurnsLeftThisTurn > 1 ? Color.DeepPink : Color.DarkRed; }
        public string turnAvailabilityLabel { get => freeTurns > 0 ? "Turn\n(Free)" : "Turn"; }

        //LEFTOVER BURN
        private double leftoverBurnedFuel;
        public double LeftoverBurnedFuel { get => leftoverBurnedFuel; set { leftoverBurnedFuel = value; OnPropertyChanged(nameof(endTurnButtonText)); } }

        //NET THRUST
        private bool isDuringMovement;
        private int currentNetThrust;
        private int finalNetThrust;
        public int NetThrust
        {
            get
            {
                if (isDuringMovement) return currentNetThrust;

                finalNetThrust = ActiveThruster.Thrust;
                if (ActiveThruster.IsSolarPowered)
                {
                    if (solarBonus < -5)
                        return 0;
                    finalNetThrust += solarBonus;
                }
                if (powerSatPushed)
                    ++finalNetThrust;
                if (afterburnerUsed)
                    finalNetThrust += ActiveThruster.AfterBurn;
                if (WetMass < 2)
                    finalNetThrust += 2;
                else if (WetMass < 4.5)
                    finalNetThrust += 1;
                else if (WetMass < 8.2)
                    finalNetThrust += 0;
                else if (WetMass < 17)
                    finalNetThrust -= 1;
                else finalNetThrust -= 2;

                currentNetThrust = Math.Min(finalNetThrust, 15);
                return currentNetThrust;
            }
        }
        
        public string endTurnButtonText { get => LeftoverBurnedFuel > 0d ? "End Turn\n (loose " + Math.Round(1-LeftoverBurnedFuel, 2).ToString() +" step)" : "End\nTurn"; }
        public bool AfterBurnButtonEnable { get => !afterburnerUsed 
                || (ActiveThruster.Type == CardType.Thruster && stepsLeft < ActiveThruster.AfterBurn)
                || ActiveThruster.Type == CardType.GwTwThruster && stepsLeft < 1; }
        public bool isPushable { get => ActiveThruster.IsPushable; }
        public bool isCardSelected { get => SelectedCard != null; }
        public Card SelectedCard;

        public Page1()
        {
            activeCards.CollectionChanged += ActiveCards_CollectionChanged;

            InitializeComponent();
            BindingContext = this;
            SetupPage();

            _ = App.Database;
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

            ListView CardList = new ListView()
            {
                ItemsSource = ActiveCards,
                ItemTemplate = new Card().DataTemplate,
                HasUnevenRows = true,
                BindingContext = this,
            };
            mainPageLayout.Children.Add(CardList);
            mainPageLayout.Children.Add(new BoxView() { BackgroundColor = Color.Black, HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 2 });
            mainPageLayout.Children.Add(new Label()
            {
                Text = "Inactive cards",
            });
            ListView InactiveCardList = new ListView()
            {
                ItemsSource = InactiveCards,
                ItemTemplate = new Card().DataTemplate,
                HasUnevenRows = true,
                BindingContext = this,
            };
            mainPageLayout.Children.Add(InactiveCardList);

            

            var cardButtons = new StackLayout() { Orientation = StackOrientation.Horizontal, HeightRequest = 50};
            mainPageLayout.Children.Add(cardButtons);

            Button AddCardButton = new Button()
            {
                Text = "+",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                WidthRequest = 50,
                CornerRadius = 25,
                BackgroundColor = Color.Yellow,
                FontSize = 18,
                BindingContext = this,
            };
            AddCardButton.Clicked += AddCardButton_Clicked;
            cardButtons.Children.Add(AddCardButton);
            
            Button DeleteCardButton = new Button()
            {
                Text = "X",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                WidthRequest = 50,
                CornerRadius = 25,
                BackgroundColor = Color.Yellow,
                FontSize = 18,
                BindingContext = this,
            };
            DeleteCardButton.SetBinding(Button.IsEnabledProperty, "isCardSelected");
            DeleteCardButton.Clicked += DeleteCardButton_Clicked;
            cardButtons.Children.Add(DeleteCardButton);
            
            Button MoveCardButton = new Button()
            {
                Text = "\\|/",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                WidthRequest = 50,
                CornerRadius = 25,
                BackgroundColor = Color.Yellow,
                FontSize = 15,
                BindingContext = this,
            };
            MoveCardButton.SetBinding(Button.IsEnabledProperty, "isCardSelected");
            MoveCardButton.Clicked += (sender, e) =>
            {
                if (ActiveCards.Contains(SelectedCard))
                    ActiveCards.Remove(SelectedCard);
                else
                {
                    ActiveCards.Add(SelectedCard);
                    CardList.SelectedItem = SelectedCard;
                }
                if (InactiveCards.Contains(SelectedCard))
                    InactiveCards.Remove(SelectedCard);
                else
                {
                    InactiveCards.Add(SelectedCard);
                    InactiveCardList.SelectedItem = SelectedCard;
                }
            };
            cardButtons.Children.Add(MoveCardButton);

            CardList.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem != null)
                {
                    SelectedCard = e.SelectedItem as Card;
                    InactiveCardList.SelectedItem = null;
                    MoveCardButton.Text = "\\|/";
                    OnPropertyChanged(nameof(isCardSelected));
                }
            };
            InactiveCardList.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem != null)
                {
                    SelectedCard = e.SelectedItem as Card;
                    CardList.SelectedItem = null;
                    MoveCardButton.Text = "/|\\";
                    OnPropertyChanged(nameof(isCardSelected));
                }
            };

            mainPageLayout.Children.Add(new BoxView() { BackgroundColor = Color.Black, HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 2 });

            Grid buttonGrid = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 150
            };
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            buttonGrid.RowDefinitions.Add(new RowDefinition() { Height = 75 });
            buttonGrid.RowDefinitions.Add(new RowDefinition() { Height = 75 });
            mainPageLayout.Children.Add(buttonGrid);

            Button EndTurnButton = new Button()
            {
                Text = "End\nturn",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 75,
                WidthRequest = 150,
                BackgroundColor = Color.Blue,
                BorderColor = Color.White,
                BorderWidth = 5,
                FontSize = 18,
                BindingContext = this,
            };
            EndTurnButton.Clicked += EndTurn_Clicked;
            EndTurnButton.SetBinding(Button.TextProperty, "endTurnButtonText");
            buttonGrid.Children.Add(EndTurnButton, 0, 2, 1, 2);
            
            Button TurnButton = new Button()
            {
                Text = "Turn",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                HeightRequest = 75,
                WidthRequest = 150,
                BackgroundColor = Color.Green,
                BorderColor = Color.White,
                BorderWidth = 5,
                FontSize = 18,
                BindingContext = this,
            };
            TurnButton.Clicked += TurnButton_Clicked;
            TurnButton.SetBinding(Button.BackgroundColorProperty, "turnAvailability");
            TurnButton.SetBinding(Button.TextProperty, "turnAvailabilityLabel");
            buttonGrid.Children.Add(TurnButton, 4, 6, 0, 1);

            Button BurnButton = new Button()
            {
                Text = "BURN!",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions= LayoutOptions.Center,
                HeightRequest = 150,
                WidthRequest = 150,
                CornerRadius = 75,
                BackgroundColor = Color.DeepPink,
                BorderColor = Color.White,
                BorderWidth = 8,
                FontSize = 40,
            };
            BurnButton.Clicked += BurnButton_Clicked;
            buttonGrid.Children.Add(BurnButton, 1,5,0,2);

            ActiveCards_CollectionChanged(null, null);
            this.Content = mainPageLayout;
        }

        private void MoveCardButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteCardButton_Clicked(object sender, EventArgs e)
        {
            ActiveCards.Remove(SelectedCard);
            InactiveCards.Remove(SelectedCard);
            SelectedCard = null;
        }

        private CardPickerPage CardPage;

        private void AddCardButton_Clicked(object sender, EventArgs e)
        {
            CardPage = new CardPickerPage();
            CardPage.Disappearing += ReturnFromCardPickerPage;

            Navigation.PushAsync(CardPage);
        }

        private void ReturnFromCardPickerPage(object sender, EventArgs e)
        {
            var SelectedCard = CardPage.SelectedCard;

            if (SelectedCard == null) return;

            if (DryMass + SelectedCard.Mass > 23)
            {
                DisplayAlert("Too heavy", "Chosen card is too heavy", "OK");
                return;
            }

            if ((SelectedCard.Type == CardType.Thruster || SelectedCard.Type == CardType.GwTwThruster)
                && ActiveCards.SingleOrDefault(x => x.Type == CardType.Thruster || x.Type == CardType.GwTwThruster) != null)
                InactiveCards.Add(SelectedCard);
            else
                ActiveCards.Add(SelectedCard);
            OnPropertyChanged(nameof(ActiveCards));

            if (WetMass > 32)
            {
                DisplayAlert("Too heavy", "Adding card changed wet mass over maximum. Reducing fuel tanks by " + (WetMass - 32), "OK");
                FuelTanks -= WetMass - 32;
            }
        }

        private void ActiveCards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var allCards = ActiveCards.Concat(InactiveCards);
            DryMass = allCards.Sum(x => x.Mass);
            MinRadHard = allCards.Any() ? allCards.Min(x => x.RadHard) : 0;
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
                WidthRequest = 50,
                BindingContext= this,
            };
            PowerSatButton.Clicked += PowerSatButton_Clicked;
            PowerSatButton.SetBinding(Button.IsEnabledProperty, "isPushable");
            netThrustStack.Children.Add(PowerSatButton);

            Button AfterBurnButton = new Button()
            {
                Text = ActiveThruster.AfterBurn.ToString(),
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
            
            int stepCount = ActiveThruster.Type == CardType.GwTwThruster ? ActiveThruster.AfterBurn : 1;
            for (int i = 0; i < stepCount; ++i)
            {
                var steps = possibleSteps.IndexOf(WetMass);
                var index = steps > possibleSteps.IndexOf(24) ? steps - 2 : steps - 1;
                FuelTanks = possibleSteps[index] - DryMass;
            }
        }

        private void PowerSatButton_Clicked(object sender, EventArgs e)
        {
            powerSatPushed = !powerSatPushed;
            ((Button)sender).BackgroundColor = powerSatPushed ? Color.Green : Color.DarkRed;
            OnPropertyChanged(nameof(NetThrust));
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
            if (isDuringMovement == false) isDuringMovement = true;

            if (BurnsLeftThisTurn == 0)
            {
                DisplayAlert("Burn failed!", "No more burns left this turn.", "OK");
                return;
            }
            ++burnsUsedThisTurn;

            LeftoverBurnedFuel += ActiveThruster.FuelUse;
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

            OnPropertyChanged(nameof(turnAvailabilityLabel));
        }

        private void TurnButton_Clicked(object sender, EventArgs e)
        {
            if (FreeTurns > 0)
            {
                --FreeTurns;
                return;
            }
            if (BurnsLeftThisTurn < 2)
            {
                DisplayAlert("Turn failed!", "No enough burns left this turn.", "OK");
                return;
            }
            BurnButton_Clicked((object)sender, e);
            BurnButton_Clicked((object)sender, e);
        }
        
        private void EndTurn_Clicked(object sender, EventArgs e)
        {
            isDuringMovement = false;
            afterburnerUsed = false;
            burnsUsedThisTurn = 0;
            OnPropertyChanged(nameof(AfterBurnButtonEnable));
            OnPropertyChanged(nameof(turnAvailabilityLabel));
            OnPropertyChanged(nameof(BurnsLeftThisTurn));
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
            if (ActiveThruster.IsSolarPowered)
                solarBonus = -1 * (((Picker)sender).SelectedIndex - 2);
            else
                solarBonus = 0;
            OnPropertyChanged(nameof(NetThrust));
            OnPropertyChanged(nameof(BurnsLeftThisTurn));
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