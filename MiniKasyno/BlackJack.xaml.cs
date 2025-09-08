/*
    Zadanie zaliczeniowe z C#
    Imię i nazwisko ucznia: Tomasz Gradowski
    Data wykonania zadania: 29.02.2025
    Treść zadania: Ksyno - mini
    Opis funkcjonalności aplikacji: Mini kasyno, które zawiera gry BlackJack oraz Ruletkę.
*/

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace MiniKasyno
{
    // Klasa reprezentująca okno gry BlackJack
    public partial class BlackJack : Window
    {
        // Deklaracje zmiennych do zarządzania grą
        private string deckId; // ID talii kart
        private List<string> playerHand = new List<string>(); // Karty gracza
        private List<string> dealerHand = new List<string>(); // Karty krupiera
        private int playerScore = 0; // Wynik gracza
        private int dealerScore = 0; // Wynik krupiera
        private bool gameOver = false; // Flaga informująca, czy gra się zakończyła
        private Image hiddenDealerCard; // Ukryta karta krupiera
        private string hiddenDealerCardValue; // Wartość ukrytej karty
        private string hiddenDealerCardImage; // Obrazek ukrytej karty
        private static readonly HttpClient httpClient = new HttpClient(); // Klient HTTP do komunikacji z API

        // Konstruktor okna BlackJack, inicjalizuje komponenty i timer
        public BlackJack()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Inicjalizacja timera do wyświetlania zegara
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // Metoda obsługująca tick timer'a, aktualizuje zegar
        private void Timer_Tick(object sender, EventArgs e)
        {
            txtClock.Text = DateTime.Now.ToString("HH:mm:ss") + " GMT+1";
        }

        // Rozpoczyna nową grę, resetuje wszystkie zmienne
        private async void StartNewGame()
        {
            try
            {
                gameOver = false;
                ResultText.Text = "";
                playerHand.Clear();
                dealerHand.Clear();
                PlayerCardsPanel.Children.Clear();
                DealerCardsPanel.Children.Clear();
                playerScore = 0;
                dealerScore = 0;
                PlayerScoreText.Text = "Twoje karty: 0";
                DealerScoreText.Text = "Karty krupiera: ?";
                btnPass.Visibility = Visibility.Visible;
                btnDobierz.Visibility = Visibility.Visible;

                // Pobranie decku kart z API
                string deckUrl = "https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=6";
                string response = await httpClient.GetStringAsync(deckUrl);
                JObject json = JObject.Parse(response);
                deckId = json["deck_id"].ToString();

                // Dobranie kart dla gracza i krupiera
                await DrawCard(true);
                await DrawCard(true);
                await DrawDealerCard(false);
                await DrawDealerCard(true);

                DealerScoreText.Text = "Karty krupiera: ?";

                // Jeżeli gracz ma 21 punktów od razu wygrywa
                if (playerScore == 21)
                {
                    ResultText.Text = "Blackjack! Wygrałeś!";
                    gameOver = true;
                }
            }
            catch (Exception ex)
            {
                // Obsługa błędów związanych z rozpoczęciem gry
                MessageBox.Show($"Wystąpił błąd podczas inicjowania gry: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metoda rysująca kartę (dla gracza lub krupiera)
        private async Task DrawCard(bool forPlayer)
        {
            try
            {
                if (gameOver) return;

                // Wysyłanie zapytania do API w celu dobrania karty
                string drawUrl = $"https://deckofcardsapi.com/api/deck/{deckId}/draw/?count=1";
                string response = await httpClient.GetStringAsync(drawUrl);
                JObject json = JObject.Parse(response);
                string cardValue = json["cards"][0]["value"].ToString();
                string cardImage = json["cards"][0]["image"].ToString();

                // Dodanie karty do ręki gracza
                if (forPlayer)
                {
                    playerHand.Add(cardValue);
                    DisplayCard(PlayerCardsPanel, cardImage);
                    playerScore = CalculateScore(playerHand);
                    PlayerScoreText.Text = "Twoje karty: " + playerScore;

                    // Sprawdzenie, czy gracz przekroczył 21 punkty
                    if (playerScore > 21)
                    {
                        ResultText.Text = "Przegrałeś!";
                        gameOver = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Obsługa błędów związanych z dobieraniem kart
                MessageBox.Show($"Nie masz kart. Rozpocznij nową grę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metoda rysująca kartę dla krupiera (może być ukryta)
        private async Task DrawDealerCard(bool hidden)
        {
            try
            {
                if (gameOver) return;

                // Wysyłanie zapytania do API w celu dobrania karty
                string drawUrl = $"https://deckofcardsapi.com/api/deck/{deckId}/draw/?count=1";
                string response = await httpClient.GetStringAsync(drawUrl);
                JObject json = JObject.Parse(response);
                string cardValue = json["cards"][0]["value"].ToString();
                string cardImage = json["cards"][0]["image"].ToString();

                // Obsługa ukrytej karty krupiera
                if (hidden)
                {
                    hiddenDealerCardValue = cardValue;
                    hiddenDealerCardImage = cardImage;

                    hiddenDealerCard = new Image
                    {
                        Width = 80,
                        Height = 120,
                        Margin = new Thickness(5),
                        Source = new BitmapImage(new Uri("https://deckofcardsapi.com/static/img/back.png"))
                    };
                    DealerCardsPanel.Children.Add(hiddenDealerCard);
                }
                else
                {
                    dealerHand.Add(cardValue);
                    DisplayCard(DealerCardsPanel, cardImage);
                    dealerScore = CalculateScore(dealerHand);
                    DealerScoreText.Text = "Karty krupiera: ?";
                }
            }
            catch (Exception ex)
            {
                // Obsługa błędów przy dobieraniu karty krupiera
                MessageBox.Show($"Wystąpił błąd przy dobieraniu karty krupiera: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Obsługuje kliknięcie przycisku "Dobierz kartę" (gracz dobiera kartę)
        private async void Hit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await DrawCard(true);
            }
            catch (Exception ex)
            {
                // Obsługa błędów przy dobieraniu kart
                MessageBox.Show($"Wystąpił błąd podczas hitowania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Obsługuje kliknięcie przycisku "Spasuj" (gracz kończy swoją turę)
        private async void Stand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gameOver || playerHand.Count == 0)
                {
                    MessageBox.Show("Nie masz kart. Rozpocznij nową grę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Dodanie ukrytej karty krupiera do jego ręki
                dealerHand.Add(hiddenDealerCardValue);
                DealerCardsPanel.Children.Remove(hiddenDealerCard);
                DisplayCard(DealerCardsPanel, hiddenDealerCardImage);

                dealerScore = CalculateScore(dealerHand);
                DealerScoreText.Text = "Karty krupiera: " + dealerScore;

                // Krupier dobiera karty, aż osiągnie 17 punktów lub więcej
                while (dealerScore < 17)
                {
                    await DrawDealerCard(false);
                    dealerScore = CalculateScore(dealerHand);
                    DealerScoreText.Text = "Karty krupiera: " + dealerScore;
                }

                // Określenie wyniku gry
                if (dealerScore > 21 || playerScore > dealerScore)
                {
                    ResultText.Text = "Wygrałeś!";
                }
                else if (dealerScore == playerScore)
                {
                    ResultText.Text = "Remis!";
                }
                else
                {
                    ResultText.Text = "Przegrałeś!";
                }

                gameOver = true;
            }
            catch (Exception ex)
            {
                // Obsługa błędów przy zakończeniu tury
                MessageBox.Show($"Wystąpił błąd podczas stania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Rozpoczyna nową grę
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartNewGame();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd przy rozpoczynaniu nowej gry: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Powrót do poprzedniego okna (głównego okna)
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window2 newWindow = new Window2();
                newWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                // Obsługa błędów przy przejściu do głównego okna
                MessageBox.Show($"Wystąpił błąd przy powrocie do głównego okna: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metoda do wyświetlania kart (dodawanie obrazu karty do odpowiedniego panelu)
        private void DisplayCard(StackPanel panel, string imageUrl)
        {
            try
            {
                Image img = new Image
                {
                    Width = 80,
                    Height = 120,
                    Margin = new Thickness(5),
                    Source = new BitmapImage(new Uri(imageUrl))
                };
                panel.Children.Add(img);
            }
            catch (Exception ex)
            {
                // Obsługa błędów przy wyświetlaniu karty
                MessageBox.Show($"Wystąpił błąd przy wyświetlaniu karty: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Obliczanie wyniku (punkty) na podstawie kart w ręce
        private int CalculateScore(List<string> hand)
        {
            try
            {
                int score = 0;
                int aceCount = 0;

                // Przetwarzanie kart
                foreach (var card in hand)
                {
                    if (card == "KING" || card == "QUEEN" || card == "JACK")
                        score += 10; // Wartość kart figurowych
                    else if (card == "ACE")
                    {
                        score += 11;
                        aceCount++; // Liczenie asów
                    }
                    else
                    {
                        score += int.Parse(card); // Liczenie kart numerycznych
                    }
                }

                // Korekta wyniku, jeśli mamy asa (11 punktów) i wynik przekracza 21
                while (score > 21 && aceCount > 0)
                {
                    score -= 10;
                    aceCount--;
                }

                return score;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd przy obliczaniu wyniku: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
    }
}
