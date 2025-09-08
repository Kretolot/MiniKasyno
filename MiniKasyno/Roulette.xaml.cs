/*
    Zadanie zaliczeniowe z C#
    Imię i nazwisko ucznia: Tomasz Gradowski
    Data wykonania zadania: 29.02.2025
    Treść zadania: Ksyno - mini
    Opis funkcjonalności aplikacji: Mini kasyno, które zawiera gry BlackJack oraz Ruletkę.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MiniKasyno
{

    public partial class Roulette : Window
    {
        private DispatcherTimer clockTimer; // Timer do aktualizacji zegara
        private Random random; // Generator liczb losowych
        private int selectedNumber = -1; // Wybrana liczba przez gracza
        private string selectedColor = null; // Wybrany kolor przez gracza
        private int resultNumber; // Wylosowana liczba
        private string resultColor; // Wylosowany kolor


        /// Konstruktor klasy Roulette.
        /// Inicjalizuje komponenty oraz zegar.

        public Roulette()
        {
            InitializeComponent();
            random = new Random();
            InitializeClock();
        }


        /// Inicjalizuje zegar, który aktualizuje godzinę na ekranie.

        private void InitializeClock()
        {
            clockTimer = new DispatcherTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(1);
            clockTimer.Tick += Timer_Tick2;
            clockTimer.Start();
        }


        /// Obsługuje zdarzenie tyknięcia zegara, aktualizując czas.

        private void Timer_Tick2(object sender, EventArgs e)
        {
            txtClock.Text = DateTime.Now.ToString("HH:mm:ss") + " GMT+1";
        }


        /// Obsługuje kliknięcie w numer ruletki przez użytkownika.

        private void Number_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                selectedNumber = int.Parse(textBlock.Text);
                selectedColor = null; // Reset wyboru koloru
                SpinRoulette();
            }
        }


        /// Obsługuje kliknięcie w kolor (czerwony lub czarny) przez użytkownika.

        private void Color_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                selectedColor = button.Content.ToString().ToLower(); // "red" lub "black"
                selectedNumber = -1; // Reset wyboru liczby
                SpinRoulette();
            }
        }


        /// Rozpoczyna losowanie numeru oraz koloru w ruletce.

        private void SpinRoulette()
        {
            resultNumber = random.Next(0, 37); // Losowanie liczby od 0 do 36
            resultColor = GetColorForNumber(resultNumber);
            PlayVideo(resultNumber.ToString());
        }


        /// Odtwarza wideo reprezentujące wylosowany wynik.

        private void PlayVideo(string videoName)
        {
            try
            {
                // Pobieranie katalogu aplikacji
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string videoPath = System.IO.Path.Combine(appDirectory, "images", $"{videoName}.mp4");

                if (!System.IO.File.Exists(videoPath))
                {
                    MessageBox.Show($"Nie znaleziono pliku wideo: {videoPath}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                imgRoulette.Visibility = Visibility.Collapsed; // Ukrywa obraz ruletki
                mediaElement.Visibility = Visibility.Visible; // Pokazuje wideo

                mediaElement.Stop();
                mediaElement.Source = new Uri(videoPath, UriKind.Absolute);
                mediaElement.Play();

                // Obsługa zdarzenia zakończenia wideo
                mediaElement.MediaEnded -= MediaElement_MediaEnded;
                mediaElement.MediaEnded += MediaElement_MediaEnded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można odtworzyć wideo: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Obsługuje zakończenie odtwarzania wideo
        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            CheckResult();
        }



        /// Sprawdza, czy użytkownik wygrał lub przegrał.

        private void CheckResult()
        {
            bool win = false;

            if (selectedNumber != -1 && selectedNumber == resultNumber)
            {
                win = true;
            }
            else if (!string.IsNullOrEmpty(selectedColor) && selectedColor == resultColor)
            {
                win = true;
            }

            string message = win ? "GRATULACJE! WYGRAŁEŚ!" : "Niestety, przegrałeś.";
            MessageBox.Show($"Wylosowana liczba: {resultNumber} ({resultColor.ToUpper()})\n{message}", "Wynik");
        }


        /// Zwraca kolor dla danego numeru ruletki.

        private string GetColorForNumber(int number)
        {
            if (number == 0) return "green"; // 0 jest zawsze zielony

            if ((number >= 1 && number <= 10) || (number >= 19 && number <= 28))
            {
                return number % 2 == 0 ? "black" : "red";
            }
            else
            {
                return number % 2 == 0 ? "red" : "black";
            }
        }


        /// Obsługuje kliknięcie w przycisk powrotu.

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Window2 newWindow = new Window2();
            newWindow.Show();
            this.Close();
        }
    }
}