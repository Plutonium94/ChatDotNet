using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;

namespace WpfApplication1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ChatController controller;

        public MainWindow() {
            InitializeComponent();
            controller = ChatController.getInstance();
            this.DataContext = controller.Persons;

            lstPersonnes.ItemsSource = controller.Persons;
            ObservableCollection<string> chatRoomNames = new ObservableCollection<string>();
            foreach (ChatRoom c in controller.ChatRooms) {
                chatRoomNames.Add(c.Nom);
            }
            chatRoomList.ItemsSource = chatRoomNames;

            currentRoomMessages.ItemsSource = controller.CurrentRoomMessages;

            ObservableCollection<Message> crm = new ObservableCollection<Message>();
            controller.SelectedChatRoom = chatRoomList.SelectedItem as ChatRoom;
            
            
        }

        public void SendClick(object sender, RoutedEventArgs e) {
            Console.WriteLine("Bouton clique");
            string destinaire = lstPersonnes.SelectedItem as string;
            if (destinaire == "") {
                controller.SendPrivateMessage(composeMessage.Text, destinaire);
            } else {
                destinaire = chatRoomList.SelectedItem as string;
                controller.SendChatRoomMessage(composeMessage.Text, destinaire);
            }
        }

        /** Send Goodbye to everybody when pressing x button */
        public void DataWindow_Closing(object sender, CancelEventArgs cae) {
            Console.WriteLine("Saying GoodBye to everybody");
            controller.SendGoodBye();

        }
    }

    
}
