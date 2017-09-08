using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace WpfApplication1 {

    [DataContract]
    public class Message {
        
        private string messageType;
        private string nickname_;
        private string msg_;
        private string timestamp_;
        private string destinaire;
        private string hash_;
        private string rootedby_;
        private string addr_source_;
        private string port_source_ = "2323";
        private List<Pair> pairs_;

        public Message() { }

        public Message(string mt, string nn, string m, string d, string rb) {
            messageType = mt;
            nickname_ = nn;
            timestamp_ = "" + DateTime.UtcNow.Subtract(new DateTime(1970,1,1,0,0,0)).TotalSeconds;
            destinaire = d;
            rootedby_ = rb;
            msg_ = m;
            hash_ = Encoding.ASCII.GetString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(messageType + nickname_ + msg + timestamp_ + destinaire)));
            
        }

        public Message(string addr_source, List<Pair> pairs) {
            this.pairs_ = pairs;
            this.addr_source_ = addr_source;
            this.messageType = "HELLO_A";
        }

        [DataMember(IsRequired=false,EmitDefaultValue=false)]
        public string addr_source {
            get { return addr_source_;  }
            set { addr_source_ = value;  }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<Pair> pairs {
            get { return pairs_;  }
            set { pairs_ = value; }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string port_source {
            get { return port_source_; }
            set { port_source_ = value; }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string type {
            get {
                return messageType;
            }
            set {
                messageType = value;
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string timestamp {
            get { return "" + timestamp_;  }
            set { timestamp_ =  value;  }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string hash {
            get { return hash_; }
            set { hash_ = value; }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string nickname {
            get { return nickname_; }
            set { nickname_ = value; }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string destinataire {
            get { return destinaire;  }
            set { destinaire = value;  }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string msg {
            get { return msg_;  }
            set { msg_= value;  }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string rootedby {
            get { return rootedby_;  }
            set { rootedby_ = value;  }
        }

        public override string ToString() {
            return "<" + nickname_ + "> : " + msg; 
        }
    }


    
}
