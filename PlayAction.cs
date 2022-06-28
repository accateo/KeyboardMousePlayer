using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace KeyboardMousePlayer
{
   public class PlayAction
    {
        public static PlayAction istance;
        private FileJson file;
        private long startTimestamp;
        private long endTimestamp;
        public volatile int contatoreAzioni=0;
        public volatile  int  numeroAzioni;
        public  long totalTimestamp;
        public static Thread actionsThread;
        public static Thread keyboardThread;
        //variabile che indica se l'esecuzione deve fermarsi
        public static bool stopExec = false;



        //avvio riproduzione registrazione singola
        public void start(string nome)
        {

            istance =this;
            //variabile che controlla se ricevo stop da server
            stopExec = false;
            //inizializzo il  contatore delle azioni eseguite
            contatoreAzioni = 0;
            //prendo il file e lo leggo
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            String path = @"C:\\KeyboardMousePlayer\\Single\\" + nome;
            file = (FileJson)json_serializer.Deserialize<FileJson>(File.ReadAllText(path+".json"));
            //ora di inizio
            startTimestamp = file.startTimestamp;
            //ora di fine
            endTimestamp = file.endTimestamp;
            //ore totali
            totalTimestamp =file.totalTimestamp;
            //azioni totali
            numeroAzioni = file.numeroAzioni;
            // Creating and initializing threads
            actionsThread = new Thread(new ThreadStart(this.riproduciAzioni));
            //hook della tastiera per scorciatoia che blocca esecuzione
            KeyboardHook.StartHook();
            Task.Run(() =>
            {
                actionsThread.Start();
                //aspetto che finisca il thread
                actionsThread.Join();
                //chiama la funzione che chiama la mainwindow e chiude l'action window
                //se azione non interrotta - fine
                if(!stopExec)
                    MainWindow.wi.endAction(1);
            });
          
           
        }
        

        async private void riproduciAzioni()
        {
            //inizializzo i 2 contatori del tempo,uno per quello attuale e uno che rappresenta il tempo di quello dopo
            DateTime timeTempBef = DateTimeOffset.FromUnixTimeMilliseconds(startTimestamp).DateTime;
            DateTime timeTempLat = DateTimeOffset.FromUnixTimeMilliseconds(startTimestamp).DateTime;

            List<SingleAction> actionsList = file.actionsList;
            timeTempLat = DateTimeOffset.FromUnixTimeMilliseconds(actionsList[0].actionUnixTime).DateTime;
            int ms = Math.Abs((int)(timeTempLat.Subtract(timeTempBef)).TotalMilliseconds);
            contatoreAzioni++;
       
            if (ms != 0)
            {
                try
                {
                    Thread.Sleep(ms);


                }
                catch (ThreadInterruptedException tie)
                {
                    return;
                }


            }
            //inizializzo lo strumento per l'input
            InputSimulator simulatore = new InputSimulator();
            //scorro la lista
            for (int contFor = 0; contFor < actionsList.Count - 1; contFor++)
            {
                //prendo i 2 tempi
                SingleAction actionTemp = actionsList[contFor];
                SingleAction actionNext = actionsList[contFor + 1];
                //Debug.WriteLine(contFor + 1);
                timeTempLat = DateTimeOffset.FromUnixTimeMilliseconds(actionTemp.actionUnixTime).DateTime;
                timeTempBef = DateTimeOffset.FromUnixTimeMilliseconds(actionNext.actionUnixTime).DateTime;
                //calcolo il delta tra i 2 tempi
                int delta = Math.Abs((int)(timeTempLat.Subtract(timeTempBef)).TotalMilliseconds);
                //che azione è? mouse o keyboard
                if (actionTemp.type=="mouse")
                {
                    Debug.WriteLine("MOUSE");
                    //eseguo le azioni del mouse 
                    Mousemove.SetCursorPosition(actionTemp.mouseX, actionTemp.mouseY);
                    switch (actionTemp.wParam)
                    {
                        case 512:
                            //mouse move
                            //Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0001);
                            Debug.WriteLine("MOUSE MOVE");
                            break;
                        case 513:
                            //mouse left down
                            Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0002);
                            Debug.WriteLine("CLICK DOWN");
                            break;
                        case 514:
                            //mouse left up
                            Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0004);
                            Debug.WriteLine("CLICK UP");
                            break;
                        case 516:
                            //mouse right down
                            Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0008);
                            break;
                        case 517:
                            // mouse right up
                            Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0010);
                            break;
                    }
                }
                if (actionTemp.type == "keyboard")
                {
                    Debug.WriteLine("KEYBOARD");
                    //stampo il tasto in debug
                    System.Windows.Forms.Keys key = (Keys)uint.Parse(actionTemp.keyboardVK);
                    Debug.WriteLine(key.ToString());
                    //simulo il tasto
                    switch (int.Parse(actionTemp.keyboardFlags))
                    {
                        case 0:
                            simulatore.Keyboard.KeyDown((VirtualKeyCode)int.Parse(actionTemp.keyboardVK));
                            break;
                        case 1:
                            simulatore.Keyboard.KeyDown((VirtualKeyCode)int.Parse(actionTemp.keyboardVK));
                            break;
                        case 32:
                            simulatore.Keyboard.KeyDown((VirtualKeyCode)int.Parse(actionTemp.keyboardVK));
                            break;
                        default:
                            simulatore.Keyboard.KeyUp((VirtualKeyCode)int.Parse(actionTemp.keyboardVK));
                            break;

                    }
                }
                //aggiorno il contatore delle azion 
                contatoreAzioni++;

                //applico il delta time 
                if (delta > 0)
                {
                    try
                    {
                        Thread.Sleep(delta);

                    }
                    catch (ThreadInterruptedException tie)
                    {
                        return;
                    }
                }

            }
            SingleAction mt = actionsList[actionsList.Count - 1];
            if (mt.type=="mouse")
            {
                //eseguo le azioni del mouse 
                Mousemove.SetCursorPosition(mt.mouseX, mt.mouseY);
                switch (mt.wParam)
                {
                    case 512:
                        //mouse move
                        //Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0001);
                        Debug.WriteLine("MOUSE MOVE");
                        break;
                    case 513:
                        //mouse left down
                        Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0002);
                        Debug.WriteLine("CLICK DOWN");
                        break;
                    case 514:
                        //mouse left up 
                        Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0008);
                        break;
                    case 517:
                        // mouse right up
                        Mousemove.MouseEvent((Mousemove.MouseEventFlags)0x0010);
                        break;
                }
            }
            if (mt.type == "keyboard")
            {
                //stampo il tasto in debug
                System.Windows.Forms.Keys key = (Keys)uint.Parse(mt.keyboardVK);
                Debug.WriteLine(key.ToString());
                //simulo il tasto
                switch (int.Parse(mt.keyboardFlags))
                {
                    case 0:
                        simulatore.Keyboard.KeyDown((VirtualKeyCode)int.Parse(mt.keyboardVK));
                        break;
                    case 1:
                        simulatore.Keyboard.KeyDown((VirtualKeyCode)int.Parse(mt.keyboardVK));
                        break;
                    case 32:
                        simulatore.Keyboard.KeyDown((VirtualKeyCode)int.Parse(mt.keyboardVK));
                        break;
                    default:
                        simulatore.Keyboard.KeyUp((VirtualKeyCode)int.Parse(mt.keyboardVK));
                        break;

                }
            }

            
            //interrompo il thread
            Thread.CurrentThread.Interrupt();
        }

        
        //funzione che blocca azione in esecuzione
        public static void Interrupt()
        {
            //fermo esecuzione future azioni
            stopExec = true; 


        }

        
    }
}