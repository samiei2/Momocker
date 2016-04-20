using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnScreenKeyboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents m_Events;
        private SQLiteConnection m_dbConnection;
        private HashSet<string> trie;
        private bool _loadCompleted;

        public MainWindow()
        {
            InitializeComponent();
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = 100;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            this.Left = 0;
            trie = new HashSet<string>();
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            Thread _dataLoadThread = new Thread(() => {
                m_dbConnection =
                new SQLiteConnection("Data Source=EngDictionary.db;Version=3;");
                m_dbConnection.Open();
                string sql = "select * from words";
                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    trie.Add(reader["name"].ToString());
                }
                _loadCompleted = true;
                PopulateKeyboard();
            });
            _dataLoadThread.Start();
            //_dataLoadThread.Join();
        }

        private void PopulateKeyboard()
        {
            HashSet<string> wordsSet = new HashSet<string>(trie);
            List<char> keys = GetKeySet(wordsSet);
            try
            {
                stackpanel.Dispatcher.BeginInvoke((MethodInvoker)(() => {
                    foreach (char key in keys)
                    {
                        KeyboardButton button = new KeyboardButton();
                        button.label.Content = key;
                        this.stackpanel.Children.Add(button);
                    }
                }));
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private List<char> GetKeySet(HashSet<string> wordsSet)
        {
            if (wordsSet.Count == 0)
                return new List<char>();
            List<char> keys = new List<char>();
            string firstWord = wordsSet.First();
            var firstchar = firstWord.First();
            wordsSet.RemoveWhere((string word) => { return word.First() == firstchar; });
            keys.Add(firstchar);
            keys.AddRange(GetKeySet(wordsSet));
            return keys;
        }

        private List<string> GetKeySet(HashSet<string> wordsSet,string prefix)
        {
            List<string> keys = new List<string>();
            string firstWord = wordsSet.First();
            return keys;
        }
    }
}
