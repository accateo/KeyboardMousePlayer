using System;
using System.Windows;

using System.Diagnostics;
using System.Timers;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace KeyboardMousePlayer
{

    public partial class ActionWindow : Window
    {
        public static ActionWindow window_istance;
        private static Timer aTimer;
        private static int timeClock = 400;
       
        public ActionWindow()
        {

            InitializeComponent();
            window_istance = this;
           
            //sistemo la finestra 
            this.Topmost = true;
            Left = System.Windows.SystemParameters.WorkArea.Width - Width;
            Top = System.Windows.SystemParameters.WorkArea.Height - Height;
            this.AllowsTransparency = true;
            // lbldurata.Content += " " + PlayAction.istance.totalTimestamp.ToString();
            pgBarAzioni.Maximum = 100;
            pgBarAzioni.Minimum = 1;
            pgBarAzioni.Value = 1;
            MainWindow.wi.Dispatcher.Invoke(() =>
            {
                StartTimer();
            });
        }
        
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)

        {
          Update();      
        }
        private static void Update() {
                window_istance.Dispatcher.Invoke(() =>
                {
                    ProgressBar pgBarAzion = ActionWindow.window_istance.pgBarAzioni;
                    Label lblSt = ActionWindow.window_istance.lblAggiornamento;
                    Debug.WriteLine(((double)(PlayAction.istance.contatoreAzioni) / (double)(PlayAction.istance.numeroAzioni)) * 100);
                    pgBarAzion.Value = ((double)(PlayAction.istance.contatoreAzioni) / (double)(PlayAction.istance.numeroAzioni)) * 100;
                    lblSt.Content = "Step " + PlayAction.istance.contatoreAzioni.ToString() + " di " + (PlayAction.istance.numeroAzioni ).ToString() + " - "
                + Math.Round(pgBarAzion.Value) + "%"; 
                });
            
        }
        public void StartTimer() {
        
            aTimer = new System.Timers.Timer();
            aTimer.Interval = timeClock;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }

        //nascondo finestra a fine test
        public void HideWindow()
        {            
            ActionWindow.window_istance.Dispatcher.Invoke(() =>
            {
                ActionWindow temp = ActionWindow.window_istance;
                aTimer.Enabled = false;
                temp.Close();
            });
        }
        //gestore mouse enter
        private void window_OnMouseEnter(object sender, MouseEventArgs e)
        {
            //if(this.Visibility==Visibility.Visible)
            //    this.Hide();
        }
        private void window_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //if (this.Visibility == Visibility.Hidden)
            //    this.Show();
        }
    }
}


