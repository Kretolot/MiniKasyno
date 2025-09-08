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
    
    /// Klasa MainWindow - reprezentuje główne okno aplikacji.
    /// Zawiera funkcjonalność wyświetlania zegara oraz przejścia do ekranu wyboru gry.
    
    public partial class MainWindow : Window
    {
        
        /// Konstruktor inicjalizujący komponenty okna oraz uruchamiający zegar.
        
        public MainWindow()
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

        
        /// Obsługuje kliknięcie przycisku przejścia do drugiego okna aplikacji.
        
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            Window2 newWindow = new Window2();
            newWindow.Show(); // Otwiera nowe okno
            this.Close(); // Zamyka aktualne okno
        }
    }
}