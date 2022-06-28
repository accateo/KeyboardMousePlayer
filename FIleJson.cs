
using System.Collections.Generic;

namespace KeyboardMousePlayer
{
    
    public class ActionType {
        public string type;
    }
   
    public class FileJson : ActionType
    {
        public string nomeFile;
        public string descrizione;
        public long startTimestamp;
        public long endTimestamp;
        public long totalTimestamp;
        public int numeroAzioni;
        public double risX;
        public double risY;
        //public List<MouseAction> MouseList;
        //public List<KeyboardAction> KeyboardList;
        public List<SingleAction> actionsList;

        public override string ToString()
        {
            return this.nomeFile;
        }

    }
    
}
