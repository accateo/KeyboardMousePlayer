using System;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Nancy.Json;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace KeyboardMousePlayer
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static System.Windows.Forms.TextBox textBox;
        public static string actionName = "";
        public static MainWindow wi;
        private const string pathAzioni = "C:\\KeyboardMousePlayer\\Single\\";
        private const string configFilePath = "C:\\KeyboardMousePlayer\\player_config.json";
        
        PlayAction plAction;
        //variabile che indica se stop ricevuto da server 0 successo 1 bloccato da server 2 bloccato localmente
        private int serverCommandStopped = -1;
        //stato visualizzazione finestra
        private bool windowShowState = false;
        public object SelectedItem
        {
            get; set;
        }
        public MainWindow()
        {

            InitializeComponent();
            creaDirectory();
            //gestisco indirizzo ip server
            if (File.Exists(configFilePath))
            {
                //leggo ip del server da file di configurazione
                JObject data = JObject.Parse(File.ReadAllText(configFilePath));
                windowShowState = (Int32.Parse(data["progressWindow"].ToString()) == 0 ? false : true);
                if(windowShowState)
                    progressCheckbox.IsChecked = true;
                
            }
            

            
            actionDataGridUpdate();
            //creo istante di registrazioni singole
            plAction = new PlayAction();
            wi = this;
            

        }
        //crea le varie directory se non esistono
        private void creaDirectory()
        {  

            
            if (!File.Exists(pathAzioni))
            {
                Directory.CreateDirectory(pathAzioni);
            }
            
        }

        //funzione che rende la textbox dell'input box valida solo per i numeri
        private static void textBox_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "[^0-9]"))
            {
                System.Windows.Forms.MessageBox.Show("Inserisci solo numeri");
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
            }
        }
        
        
        
        
        //bottone che fa partire la singola registrazione selezionata dall'utente
        private void playAzioneSingola_Click_1(object sender, RoutedEventArgs e)
        {
            //Vedo se si è selezionato un elemento della listbox
            var curItem = dgSingle.SelectedItem;
            //verifico se si è selezionato un elemento o la listbox è vuota
            if (curItem == null)
            {
                System.Windows.MessageBox.Show("Per favore selezionare un'azione");
            }
            else
            {
                //metto il file selezionato nella listbox dentro la variabile nome
                actionName = ((dgFormat) dgSingle.SelectedItem).nomeFile;
                ActionWindow actionWindow = new ActionWindow();
                ShowInTaskbar = true;
                WindowState = System.Windows.WindowState.Minimized;
                if (windowShowState)
                    actionWindow.Show();
                plAction.start(actionName);

            }
        }
        
        //aggiorna la data grid della azioni
        private void actionDataGridUpdate()
        {

            //apro il popup per selezionare il file (dal bottone seleziona file)
            string[] files = Directory.GetFiles(pathAzioni);
            List<dgFormat> lista = new List<dgFormat>();


            foreach (string path in files)
            {
                FileJson temp = JsonConvert.DeserializeObject<FileJson>(File.ReadAllText(path), new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });  

                dgFormat row = new dgFormat();
                
                //da millisecondi a minuti
                TimeSpan t = TimeSpan.FromMilliseconds(temp.totalTimestamp);
                string answer = string.Format("{1:D2}:{2:D2}",
                                        t.Hours,
                                        t.Minutes,
                                        t.Seconds,
                                        t.Milliseconds);

                //riempio celle 
                row.tempo = answer;
                row.nomeFile = temp.nomeFile;
                string stringaTemp = temp.descrizione;
                row.descrizione = stringaTemp;
                lista.Add(row);

            }
            dgSingle.ItemsSource = lista;
            //non editabile
            dgSingle.IsReadOnly = true;
            
            
        }
        
        //bottone che fa partire il refresh della datagrid delle azioni
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            actionDataGridUpdate();
            dgSingle.Columns[0].Header = "Nome";
            dgSingle.Columns[1].Header = "Descrizione";
            dgSingle.Columns[2].Header = "Tempo (min)";
            System.Windows.MessageBox.Show("Lista di registrazioni ricaricata correttamente");
        }
        
        
        
        

        //riapro finestra principale
        public void reopenWindow()
        {
            WindowState = System.Windows.WindowState.Normal;
            this.Topmost = true;
            Activate();

        }
        //chiude la action window e ripare la main window
        //commandStopped - 1 azione finita 0 azione bloccata
        public void endAction(int commandStopped)
        {
            
            wi.Dispatcher.Invoke(() =>
            {
                //nascondo finestra in basso a destra 
                ActionWindow.window_istance.HideWindow();
                //ferma il keyboard hook
                KeyboardHook.StopHook();
                //riapro finestra principale
                reopenWindow();
                if (commandStopped == 1)
                {
                    System.Windows.Forms.MessageBox.Show("Registrazione riprodotta con successo");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Esecuzione bloccata");
                }

            });

        }
        
        
        
        
        
        //funzione che serve per le descrizioni della datagrid
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgSingle.Columns[0].Header = "Nome";
            dgSingle.Columns[1].Header = "Descrizione";
            dgSingle.Columns[2].Header = "Tempo (min)";
        }
        //gestione checkbox
        private void progressCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //aggiorno stato
            windowShowState = true;
            progressCheckbox.IsChecked = true;
        }
        private void progressCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            //aggiorno stato
            windowShowState = false;
            progressCheckbox.IsChecked = false;
        }
        
        

     
        }
    }

    


