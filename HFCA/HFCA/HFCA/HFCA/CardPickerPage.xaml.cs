using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HFCA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardPickerPage : ContentPage
    {
        private ObservableCollection<Card> allCards = new ObservableCollection<Card>()
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
        public ObservableCollection<Card> AllCards {
            get
            {
                if (typePicker.SelectedIndex == -1) return allCards;
                return new ObservableCollection<Card>(allCards.Where(x => x.Type == (CardType)typePicker.SelectedIndex));
            } }

        private Picker typePicker = null;

        public CardPickerPage()
        {
            InitializeComponent();
            var MainStack = new StackLayout();

            var pickerStack = new StackLayout() { Orientation = StackOrientation.Horizontal, };
            MainStack.Children.Add(pickerStack);
            typePicker = new Picker()
            {
                Title = "Pick card type",
                ItemsSource = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList(),
                BindingContext = this,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            typePicker.SelectedIndexChanged += TypePicker_SelectedIndexChanged;
            pickerStack.Children.Add(typePicker);
            Button cancelChoiceButton = new Button()
            {
                Text = "X",
                CornerRadius = 15,
                HeightRequest = 30,
                WidthRequest = 30,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                TextColor = Color.White,
                BackgroundColor = Color.DarkRed,
                FontSize = 9
            };
            cancelChoiceButton.Clicked += (sender, e) => typePicker.SelectedIndex = -1;
            pickerStack.Children.Add(cancelChoiceButton);

            #region Title
            var TitleStack = new StackLayout() { Orientation = StackOrientation.Horizontal, HeightRequest = 40};
            MainStack.Children.Add(TitleStack);
            TitleStack.Children.Add(new Label()
            {
                Text = "Name",
                WidthRequest = 200,
                VerticalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "Mss",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "RH",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "Supp\nReq",
                WidthRequest = 50,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "Thr",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "FU",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "AB",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            #endregion

            var CardListView = new ListView()
            {
                ItemsSource = AllCards,
                SelectionMode = ListViewSelectionMode.Single,
                ItemTemplate = new DataTemplate(() => {
                    var itemStack = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                    };
                    itemStack.SetBinding(StackLayout.BackgroundColorProperty, "Color");
                    
                    Label nameLabel = new Label()
                    {
                        WidthRequest = 200,
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

                    var symbolStack = new StackLayout() { Orientation = StackOrientation.Vertical, WidthRequest = 50 };
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
                    
                }),
                BindingContext = this,
            };
            CardListView.SetBinding(ListView.ItemsSourceProperty, "AllCards");
            MainStack.Children.Add(CardListView);



            var buttonStack = new StackLayout() { Orientation = StackOrientation.Horizontal, VerticalOptions = LayoutOptions.End };
            MainStack.Children.Add(buttonStack);
            Button ChooseButton = new Button()
            {
                Text = "Choose",
                HorizontalOptions = LayoutOptions.StartAndExpand,
            };
            ChooseButton.Clicked += ChooseButton_Clicked;
            buttonStack.Children.Add(ChooseButton);
            Button BackButton = new Button()
            {
                Text = "Back",
                HorizontalOptions = LayoutOptions.EndAndExpand,
            };
            BackButton.Clicked += BackButton_Clicked;
            buttonStack.Children.Add(BackButton);


            this.BindingContext = this;
            this.Content = MainStack;
        }

        private void TypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AllCards));
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChooseButton_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}