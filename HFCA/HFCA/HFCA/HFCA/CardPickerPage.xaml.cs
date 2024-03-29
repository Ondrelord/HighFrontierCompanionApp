﻿using System;
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
        public Card SelectedCard { get => CardListView.SelectedItem as Card; }

        private ObservableCollection<Card> allCards;
        public ObservableCollection<Card> AllCards {
            get
            {
                ObservableCollection<Card> ret = allCards; 
                
                if (typePicker.SelectedIndex != -1)
                    ret = new ObservableCollection<Card>(allCards.Where(x => x.Type == (CardType)typePicker.SelectedIndex));

                if (sidePicker.SelectedIndex == 0) ret = new ObservableCollection<Card>(ret.Where(x => x.ID%2 == 1));
                if (sidePicker.SelectedIndex == 1) ret = new ObservableCollection<Card>(ret.Where(x => x.ID%2 == 0));

                return ret;
            }
            set => allCards = value;
        }

        private Picker typePicker = null;
        private Picker sidePicker = null;
        private ListView CardListView;

        public CardPickerPage()
        {
            AllCards = App.Database.GetCards();

            InitializeComponent();
            var MainStack = new StackLayout();

            var pickerStack = new StackLayout() { Orientation = StackOrientation.Horizontal, };
            MainStack.Children.Add(pickerStack);
            typePicker = new Picker()
            {
                Title = "Card type",
                ItemsSource = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList(),
                BindingContext = this,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            typePicker.SelectedIndexChanged += TypePicker_SelectedIndexChanged;
            pickerStack.Children.Add(typePicker);
            sidePicker = new Picker()
            {
                Title = "Side",
                ItemsSource = Enum.GetValues(typeof(Side)).Cast<Side>().ToList(),
                BindingContext = this,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            sidePicker.SelectedIndexChanged += TypePicker_SelectedIndexChanged;
            pickerStack.Children.Add(sidePicker);
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
            cancelChoiceButton.Clicked += (sender, e) => sidePicker.SelectedIndex = -1;
            pickerStack.Children.Add(cancelChoiceButton);

            #region Title
            var TitleStack = new StackLayout() { Orientation = StackOrientation.Horizontal, HeightRequest = 50, Padding = -10};
            MainStack.Children.Add(TitleStack);
            TitleStack.Children.Add(new Label()
            {
                Text = "Name",
                HorizontalOptions= LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 10
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "Mss",
                WidthRequest = 28,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "RH",
                WidthRequest = 25,
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
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "FU",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            TitleStack.Children.Add(new Label()
            {
                Text = "AB",
                WidthRequest = 30,
                HeightRequest = 30,
                Rotation = 90,
                VerticalTextAlignment = TextAlignment.End,
                HorizontalTextAlignment = TextAlignment.Center,
            });
            #endregion

            CardListView = new ListView()
            {
                ItemsSource = AllCards,
                SelectionMode = ListViewSelectionMode.Single,
                ItemTemplate = new Card().DataTemplate,
                HasUnevenRows = true,
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
            CardListView.SelectedItem = null;
            Navigation.PopAsync();
        }

        private void ChooseButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}