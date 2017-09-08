using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1 
{
    public class ChatRoom 
    {

        private string nom;
        private string description;
        private List<Message> messages;

        public ChatRoom(string nom, string description) {
            this.nom = nom;
            this.description = description;
            this.messages = new List<Message>();
        }

        public string Nom {
            get { return nom; }
            set { nom = value; }
        }

        public string Description {
            get { return description; }
            set { description = value; }
        }

        public List<Message> Messages {
            get { return messages; }
            set { messages = value; }
        }

        public void AddMessage(Message m) {
            messages.Add(m);
        }
    }
}
