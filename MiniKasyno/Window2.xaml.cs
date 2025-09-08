/*
Zadanie zaliczeniowe z C#
Imię i nazwisko ucznia: Tomasz Gradowski
Data wykonania zadania: 29.02.2025
Treść zadania: Ksyno - mini
Opis funkcjonalności aplikacji: Mini kasyno, które zawiera gry BlackJack oraz Ruletkę.
*/

using System;
using System.Windows;
using System.Windows.Threading;

namespace MiniKasyno
{
    
    /// Klasa Window2 - reprezentuje drugie okno aplikacji.
    /// Zawiera funkcjonalność wyboru gry oraz zegar wyświetlający aktualny czas.
    
    public partial class Window2 : Window
    {
        
        /// Konstruktor inicjalizujący komponenty okna oraz uruchamiający zegar.
        
        public Window2()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Tworzenie i uruchamianie timera do aktualizacji zegara
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        
        /// Obsługuje zdarzenie aktualizacji zegara i wyświetla bieżący czas.
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            txtClock.Text = DateTime.Now.ToString("HH:mm:ss") + (" GMT+1");
        }

        
        /// Obsługuje kliknięcie przycisku powrotu do głównego okna.
        
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }

        
        /// Obsługuje kliknięcie przycisku otwierającego grę w Ruletkę.
        
        private void btnRoulette_Click(object sender, RoutedEventArgs e)
        {
            Roulette newWindow = new Roulette();
            newWindow.Show();
            this.Close();
        }

        
        /// Obsługuje kliknięcie przycisku otwierającego grę w Black Jacka.
        
        private void btnBlackJack_Click(object sender, RoutedEventArgs e)
        {
            BlackJack newWindow = new BlackJack();
            newWindow.Show();
            this.Close();
        }
    }
}
