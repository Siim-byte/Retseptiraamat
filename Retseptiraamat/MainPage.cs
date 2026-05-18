using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace Retseptiraamat
{
    public class Pealeht : TabbedPage
    {
        public Pealeht()
        {
            BarBackgroundColor = Colors.WhiteSmoke;
            BarTextColor = Colors.DarkSlateGray;
            SelectedTabColor = Colors.Green;

            Children.Add(new UusRetseptPage { Title = "Uus retsept" });
            Children.Add(new MinuRetseptidPage { Title = "Minu retseptid" });
        }
    }

    // Lisamise vorm
    public class UusRetseptPage : ContentPage
    {
        private Entry entryNimi;
        private Entry entryKategooria;
        private Entry entryPilt;

        public UusRetseptPage()
        {
            BackgroundColor = Colors.AliceBlue;

            var layout = new VerticalStackLayout
            {
                Padding = new Thickness(20),
                Spacing = 15,
                VerticalOptions = LayoutOptions.Center
            };

            var pealkiri = new Label
            {
                Text = "Lisa uus retsept",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.Black
            };

            entryNimi = new Entry { Placeholder = "Retsepti nimi, nt Pasta Carbonara", BackgroundColor = Colors.White, TextColor = Colors.Black };
            entryKategooria = new Entry { Placeholder = "Kategooria, nt Pearoad, Magustoidud", BackgroundColor = Colors.White, TextColor = Colors.Black };
            entryPilt = new Entry { Placeholder = "Pildi URL", BackgroundColor = Colors.White, TextColor = Colors.Black };

            var nuppSalvesta = new Button
            {
                Text = "Salvesta retsept",
                BackgroundColor = Colors.Green,
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 50,
                CornerRadius = 8
            };
            nuppSalvesta.Clicked += OnSalvestaClicked;

            layout.Children.Add(pealkiri);
            layout.Children.Add(entryNimi);
            layout.Children.Add(entryKategooria);
            layout.Children.Add(entryPilt);
            layout.Children.Add(nuppSalvesta);

            Content = new ScrollView { Content = layout };
        }

        private async void OnSalvestaClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entryNimi.Text) ||
                string.IsNullOrWhiteSpace(entryKategooria.Text) ||
                string.IsNullOrWhiteSpace(entryPilt.Text))
            {
                await DisplayAlert("Hoiatus", "Palun täida kõik väljad enne salvestamist!", "OK");
                return;
            }

            var uusRetsept = new Retsept
            {
                Nimi = entryNimi.Text.Trim(),
                Kategooria = entryKategooria.Text.Trim(),
                PildiLink = entryPilt.Text.Trim()
            };

            Failihaldur.SalvestaRetsept(uusRetsept);

            await DisplayAlert("Edukas", "Retsept on edukalt salvestatud!", "OK");

            entryNimi.Text = string.Empty;
            entryKategooria.Text = string.Empty;
            entryPilt.Text = string.Empty;
        }
    }

    public class MinuRetseptidPage : ContentPage
    {
        private ListView listViewRetseptid;

        public MinuRetseptidPage()
        {
            BackgroundColor = Colors.White;

            var grid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            var pealkiri = new Label
            {
                Text = "Minu Retseptid",
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(15),
                TextColor = Colors.Black
            };
            Grid.SetRow(pealkiri, 0);

            listViewRetseptid = new ListView
            {
                IsGroupingEnabled = true,
                HasUnevenRows = true,
                SelectionMode = ListViewSelectionMode.None
            };
            Grid.SetRow(listViewRetseptid, 1);

            listViewRetseptid.GroupHeaderTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.DarkSlateGray,
                    VerticalOptions = LayoutOptions.Center
                };
                label.SetBinding(Label.TextProperty, "Nimetus");

                var headerGrid = new Grid { Padding = new Thickness(15, 8), BackgroundColor = Colors.LightGray };
                headerGrid.Children.Add(label);

                return new ViewCell { View = headerGrid };
            });

            listViewRetseptid.ItemTemplate = new DataTemplate(() =>
            {
                var rowGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(80) },
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                    Padding = new Thickness(10)
                };

                var pilt = new Image { WidthRequest = 70, HeightRequest = 70, Aspect = Aspect.AspectFill };
                pilt.SetBinding(Image.SourceProperty, "PildiLink");
                Grid.SetColumn(pilt, 0);

                var nimiLabel = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    TextColor = Colors.Black,
                    VerticalOptions = LayoutOptions.Center
                };
                nimiLabel.SetBinding(Label.TextProperty, "Nimi");

                var textStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Padding = new Thickness(10, 0, 0, 0) };
                textStack.Children.Add(nimiLabel);
                Grid.SetColumn(textStack, 1);

                rowGrid.Children.Add(pilt);
                rowGrid.Children.Add(textStack);

                var cell = new ViewCell { View = rowGrid };

                var kustutaMenuu = new MenuItem
                {
                    Text = "Kustuta",
                    IsDestructive = true
                };
                kustutaMenuu.Clicked += OnKustutaClicked;

                cell.ContextActions.Add(kustutaMenuu);

                return cell;
            });

            grid.Children.Add(pealkiri);
            grid.Children.Add(listViewRetseptid);

            Content = grid;
        }

        private void OnKustutaClicked(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var kustutatavRetsept = menuItem.BindingContext as Retsept;
            if (kustutatavRetsept == null) return;

            var koikRetseptid = Failihaldur.LoeRetseptid();

            var eemaldatav = koikRetseptid.FirstOrDefault(r => r.Nimi == kustutatavRetsept.Nimi && r.Kategooria == kustutatavRetsept.Kategooria);
            if (eemaldatav != null)
            {
                koikRetseptid.Remove(eemaldatav);

                Failihaldur.SalvestaKoikRetseptid(koikRetseptid);
                UuendaNimekirja(koikRetseptid);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var koikRetseptid = Failihaldur.LoeRetseptid();
            UuendaNimekirja(koikRetseptid);
        }

        private void UuendaNimekirja(List<Retsept> retseptid)
        {
            var grupeeritud = retseptid
                .GroupBy(r => r.Kategooria)
                .Select(g => new RetseptiKategooria(g.Key, g.ToList()))
                .ToList();

            listViewRetseptid.ItemsSource = grupeeritud;
        }
    }
}
