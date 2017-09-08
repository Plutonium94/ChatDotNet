using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WpfApplication1 
{

    public delegate void messageDelegate(String message); 
    public class ChatController 
    {

        private ObservableCollection<ChatRoom> allChatRooms;
        private ObservableCollection<string> currentRoomMessages;
        private List<Pair> pairs;
        private ChatRoom selectedChatRooom;
        private UdpClient listener;
        private Socket sender;
        private bool do_receive = true;

        private static ChatController instance;
        private static DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Message));

        public static ChatController getInstance() 
        {
            if (instance == null)
                instance = new ChatController();
            return instance;
        }

        private ChatController() 
        {
            pairs = new List<Pair>();
            Pair p = new Pair();
            p.addr = "192.168.0.144";
            p.port = "2323";

            pairs.Add(p);//Kevin
            pairs.Add(new Pair("192.168.0.143", "2323"));//Remi
            pairs.Add(new Pair("192.168.0.161", "2323"));//Vaki
            pairs.Add(new Pair("192.168.0.127", "2323"));//Sacha

            allChatRooms = new ObservableCollection<ChatRoom>();
            currentRoomMessages = new ObservableCollection<string>();
            initDefaultChatRooms();

            sender = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);

            Thread receiverThread = new Thread(receive);
            receiverThread.Start();
            manageNetwork();
        }


        public void manageNetwork() 
        {
            Message m = new Message( "192.168.0.170",pairs);
            
            MemoryStream mstream = new MemoryStream();
            ser.WriteObject(mstream, m);
            mstream.Position = 0;
            StreamReader sr = new StreamReader(mstream);
            String jsonmessage = sr.ReadToEnd();
            foreach(Pair p in pairs) {
                sendMessageUdp(p.addr, p.port, jsonmessage);
            }
        }


        
        public void receive() {
            listener = new UdpClient(2323);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 2323);

            while (do_receive) {
                try {

                    byte[] bytesRecu = listener.Receive(ref ep);
                    string msgTxtRecu = Encoding.ASCII.GetString(bytesRecu, 0, bytesRecu.Length);
                    App.Current.Dispatcher.Invoke(new messageDelegate(IncomingMessage),msgTxtRecu);
                    
                } catch (ObjectDisposedException obe) {
                    Console.WriteLine(obe.ToString());
                } catch(SocketException se) {
                    Console.WriteLine(se.ToString());
                }
            }
            
        }



        private void initDefaultChatRooms() {
            Message m1 = new Message("MESSAGE", "laurent", "Ceci est un message", "banque", "moi");
            Message m2 = new Message("MESSAGE", "yannis", "faire le tpt", "melissa", "lui");
            Message m5 = new Message("MESSAGE", "david", "chercher la salle", "fali", "vaki");
            ChatRoom c1 = new ChatRoom("Grasse", "Cette salle est tres petite");
            foreach (Message m in new Message[] { m1, m2, m5 }) {
                c1.AddMessage(m);
            }

            Message m3 = new Message("MESSAGE", "michel", "on a gagne", "sacha", "oualid");
            Message m4 = new Message("MESSAGE", "kevin", "il faut dormir", "clement", "walid");

            ChatRoom c2 = new ChatRoom("Nice", "la plus belle description");
            c2.AddMessage(m3); c2.AddMessage(m4);

            foreach (ChatRoom cr in new ChatRoom[] { c1, c2 }) {
                allChatRooms.Add(cr);
            }
        }

        public void IncomingMessage(string msg) 
        {
            Console.WriteLine(msg);

            currentRoomMessages.Add(msg);
            byte[] byteMessage = Encoding.ASCII.GetBytes(msg);
            MemoryStream mstream = new MemoryStream(byteMessage);
            mstream.Position = 0;

            Message receivedMessage = (Message)ser.ReadObject(mstream);

            /* Repondre Ping par Pong */
            if (receivedMessage.type == "PING") {
                Message m = new Message("PONG", "Orange", "", receivedMessage.addr_source, "Daniel");
                mstream = new MemoryStream();
                ser.WriteObject(mstream, m);
                mstream.Position = 0;
                StreamReader sr = new StreamReader(mstream);
                String jsonmessage = sr.ReadToEnd();
                sendMessageUdp(receivedMessage.addr_source, receivedMessage.port_source, jsonmessage);
            }

        }

        public ObservableCollection<ChatRoom> ChatRooms {
            get { return allChatRooms; }
            set { allChatRooms = value; }
        }

        public ChatRoom SelectedChatRoom {
            get { return selectedChatRooom; }
            set { selectedChatRooom = value; }
        }

        public ObservableCollection<string> Persons {
            get {
                ObservableCollection<string> res = new ObservableCollection<string>();
                foreach (ChatRoom cr in allChatRooms) {
                    foreach (Message m in cr.Messages) {
                        res.Add(m.nickname);
                    }
                }
                return res;
            }
        }

        public void sendMessageUdp(String ip, String port, String message) 
        {
            IPAddress target = IPAddress.Parse(ip);
            IPEndPoint ep = new IPEndPoint(target,Int32.Parse(port));
            byte[] msg = Encoding.ASCII.GetBytes(message);
            sender.SendTo(msg, ep);
            Console.WriteLine("Sent " + Encoding.ASCII.GetString(msg) + " to " + ip + ":" + port);
        }


        public ObservableCollection<string> CurrentRoomMessages {
            get { return currentRoomMessages; }
            set { currentRoomMessages = value; }
        }

        public void SendGoodBye() {
            foreach (Pair p in pairs) {
                Message m = new Message("GOODBYE", "Orange", "", "192.168.0.170", "Daniel");
                MemoryStream mstream = new MemoryStream();
                ser.WriteObject(mstream, m);
                mstream.Position = 0;
                StreamReader sr = new StreamReader(mstream);
                String jsonmessage = sr.ReadToEnd();
                sendMessageUdp(p.addr, p.port, jsonmessage);
            }
        }

        public void SendPrivateMessage(string msgTxt, string destinaire) {
            
        }

        public void SendChatRoomMessage(string msgTxt, string chatroomName) 
        {
            foreach (Pair p in pairs) 
            {
                Message m = new Message("MESSAGE", "Orange", msgTxt, p.addr, "Daniel");
                MemoryStream mstream = new MemoryStream();
                ser.WriteObject(mstream, m);
                mstream.Position = 0;
                StreamReader sr = new StreamReader(mstream);
                String jsonmessage = sr.ReadToEnd();
                sendMessageUdp(p.addr, p.port, jsonmessage);
            }


        }


    }
}
