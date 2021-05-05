using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using jMailClient.MyMailServer; 
using Microsoft.Win32;
using System.Windows.Media.Animation;
using jMailMeta;

namespace jMailClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            proxy = new MailServerClient();
            ofd = new OpenFileDialog();
        }

        private MailServerClient proxy;
        private OpenFileDialog ofd;

        private void btn_imgChs_Click(object sender, RoutedEventArgs e)
        {
            bool? ok = ofd.ShowDialog();
            if (ok == true)
            {
                tb_imgpath.Text = ofd.FileName;
                tb_imgpath.Visibility = Visibility.Visible;
                chosenImg.Source = new BitmapImage(new Uri(ofd.FileName));
                chosenImgBackgr.Visibility = Visibility.Visible;
            }
        }

        private void btn_reg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proxy.Registration(tb_reglogin.Text, tb_regpwd.Password, tb_regname.Text, dp_birthd.SelectedDate.Value, tb_imgpath.Text))
                    OpacityAnimation(img_ok);
                else OpacityAnimation(img_error);
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        void OpacityAnimation(UIElement elem)
        {
            DoubleAnimation anim = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(1)));
            anim.AutoReverse = true;
            elem.BeginAnimation(OpacityProperty, anim);
        }

        void InspectRegConditions()
        {
            if (
                tb_reglogin.Text != String.Empty &&
                tb_regpwd.Password != String.Empty &&
                tb_regname.Text != String.Empty &&
                dp_birthd.SelectedDate != null &&
                tb_imgpath.Text != String.Empty
                )
                btn_reg.IsEnabled = true;
            else btn_reg.IsEnabled = false;
        }

        private void tb_reglogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            InspectRegConditions();
        }

        private void tb_regpwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            InspectRegConditions();
        }

        private void tb_regname_TextChanged(object sender, TextChangedEventArgs e)
        {
            InspectRegConditions();
        }

        private void dp_birthd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            InspectRegConditions();
        }

        private void tb_imgpath_TextChanged(object sender, TextChangedEventArgs e)
        {
            InspectRegConditions();
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proxy.Login(tb_login.Text, tb_passwd.Password))
                {
                    toolb_login.Visibility = Visibility.Collapsed;
                    toolb_logged.Visibility = Visibility.Visible;
                    exp_reg.Visibility = Visibility.Collapsed;
                    tab_control.Visibility = Visibility.Visible;
                    menu.Visibility = Visibility.Visible;
                    string[] uData = proxy.GetUserData();
                    if (uData != null)
                    {
                        lbl_lgas.Content = uData[0] + " [" + uData[2] + "] [" + uData[1] + ']';

                        try
                        {
                            img_userImg.Source = new BitmapImage(new Uri(uData[3]));
                        }
                        catch
                        {
                            img_userImg.Source = new BitmapImage(new Uri("img/default_user.png", UriKind.Relative));
                        }
                    }
                    RefreshContacts();
                    RefreshMails();
                }
                else OpacityAnimation(img_lgerror);
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (proxy.LogOut())
                {
                    toolb_login.Visibility = Visibility.Visible;
                    toolb_logged.Visibility = Visibility.Collapsed;
                    exp_reg.Visibility = Visibility.Visible;
                    tab_control.Visibility = Visibility.Collapsed;
                    menu.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        private void tb_to_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_to.Text != String.Empty) btn_sendmsg.IsEnabled = true;
            else btn_sendmsg.IsEnabled = false;
        }

        private void btn_sendmsg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = tb_to.Text.Replace(" ", "");
                if (s[s.Length - 1] == ',') s = s.Remove(s.Length - 1);
                string[] to = s.Split(',');
                if (proxy.SendMessage(to, tb_subject.Text, tb_body.Text))
                {
                    OpacityAnimation(img_sendok);
                    RefreshContacts();
                }
                else OpacityAnimation(img_senderror);
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        void RefreshContacts()
        {
            try
            {
                string[] uData = proxy.GetContacts();
                list_contacts.ItemsSource = uData;
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        private void list_contacts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tb_to.Text += list_contacts.SelectedItems[0] + ", ";
        }

        void RefreshMails()
        {
            try
            {
                string[][] inboxData = proxy.GetInboxMessages();
                List<InboxMsgMeta> inboxMessages = new List<InboxMsgMeta>();
                for (int i = 0; i < inboxData.Length; i++)
                {
                    InboxMsgMeta msg = new InboxMsgMeta(inboxData[i][5] == "1" ? true : false, inboxData[i][0], inboxData[i][2], inboxData[i][3], inboxData[i][4]);
                    inboxMessages.Add(msg);
                }
                dg_inbox.ItemsSource = inboxMessages;

                string[][] sentData = proxy.GetSentMessages();
                List<SentMsgMeta> sentMessages = new List<SentMsgMeta>();
                for (int i = 0; i < sentData.Length; i++)
                {
                    SentMsgMeta msg = new SentMsgMeta(sentData[i][5] == "1" ? true : false, sentData[i][1], sentData[i][2], sentData[i][3], sentData[i][4]);
                    sentMessages.Add(msg);
                }
                dg_sent.ItemsSource = sentMessages;
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        private void tb_srchKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadSearchData();
            }
        }

        void LoadSearchData()
        {
            try
            {
                string[][] msgData = proxy.Filter(tb_srchKey.Text);
                List<InboxMsgMeta> messages = new List<InboxMsgMeta>();
                for (int i = 0; i < msgData.Length; i++)
                {
                    InboxMsgMeta msg = new InboxMsgMeta(msgData[i][5] == "1" ? true : false, msgData[i][0], msgData[i][2], msgData[i][3], msgData[i][4]);
                    messages.Add(msg);
                }
                dg_search.ItemsSource = messages;
            }
            catch (Exception x)
            {
                MessageBox.Show("Server is not available! - " + x.Message);
            }
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            LoadSearchData();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RefreshMails();
            RefreshContacts();
        }
    }
}
