using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using static R_Copy.Pathing;
using MessageBox = System.Windows.MessageBox;

namespace R_Copy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //public void ExecuteCopy(String Source, String Destination, String Threads, params string[] Excluded)
        public void ExecuteCopy(String Source, String Destination, String Threads, String Excluded)
        {
            string Folders = Excluded.Replace(",", "\" \"");
            Process p = new Process();
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.FileName = "cmd.exe";
            Info.Arguments = @"/c ROBOCOPY " + '\u0022'+Source+'\u0022' + " " + '\u0022'+Destination+'\u0022' + " /E" + " /MT:" + Threads;
            if (!string.IsNullOrWhiteSpace(Folders)) {
                Info.Arguments += " /XD " + '\u0022'+Folders+'\u0022';
            }
            p.StartInfo = Info;
            p.Start();
            StartCopy.IsEnabled = false;
            while (!p.HasExited)
            {
                if (p.HasExited)
                {
                    StartCopy.IsEnabled = true;
                    MessageBoxResult finito = MessageBox.Show("Operation finished!", "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
                    switch (finito)
                    {
                        case MessageBoxResult.OK:
                            break;
                    }
                }
            }
        }
        private void StartCopy_Click(object sender, RoutedEventArgs e)
        {
            bool ThreadIsNumber = false;
            bool passedCheck = false;
            bool passedConfirm = false;
            int ThreadAmount = 0;
            string CopyFrom = SourceFrom.Text;
            string CopyTo = DestinationCopy.Text;
            string NumberThreads = NumberOfThreads.Text;
            string RawExcluded = FoldersToExclude.Text;
            string[] ExcludedFolders = FoldersToExclude.Text.Split(',');

            string TestOutput = "Is this correct?\n \n" + "From: " + CopyFrom + "\n" + 
                "To: " + CopyTo + "\n" + 
                "Threads: " + NumberThreads + "\n" + 
                "Excluded folders: \n" + string.Join(Environment.NewLine, ExcludedFolders);
            // string caption = "DEBUG MESSAGE";

            // Show error if content is empty
            if (string.IsNullOrWhiteSpace(SourceFrom.Text))
            {
                MessageBox.Show("You need to choose a source folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passedCheck = false;
            }
            else if (string.IsNullOrWhiteSpace(DestinationCopy.Text))
            {
                MessageBox.Show("You need to choose a destination folder!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passedCheck = false;
            }
            else if (string.IsNullOrWhiteSpace(NumberOfThreads.Text))
            {
                MessageBox.Show("You need to select the amount of threads!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passedCheck = false;
            }
            else
            {
                ThreadIsNumber = int.TryParse(NumberThreads, out ThreadAmount);

                if (ThreadIsNumber)
                {
                    if (ThreadAmount > 128 || ThreadAmount < 5)
                    {
                        MessageBox.Show("Threads needs to be between: 5 (min) and 128 (max)", "Wait", MessageBoxButton.OK, MessageBoxImage.Warning);
                        passedCheck = false;
                    }
                    else
                    {
                        passedCheck = true;
                    }
                }
                else
                {
                    MessageBox.Show("Threads has to be a number...", "Wait", MessageBoxButton.OK, MessageBoxImage.Warning);
                    passedCheck = false;
                }
            }


            if (passedCheck)
            {
                MessageBoxResult confirm = MessageBox.Show(TestOutput, "Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                switch (confirm)
                {
                    case MessageBoxResult.Yes:
                        passedConfirm = true;
                        break;
                    case MessageBoxResult.No:
                        passedConfirm = false;
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(FoldersToExclude.Text))
            {

                ExcludedFolders[0] = "None";
            }

            if (passedCheck && passedConfirm)
            {
                ExecuteCopy(CopyFrom,CopyTo,NumberThreads,RawExcluded);
            }

        }
        // Select soure
        private void SelectSource_Click(object sender, RoutedEventArgs e)
        {
            var FDialog = new System.Windows.Forms.FolderBrowserDialog();
            var res = FDialog.ShowDialog();

            switch (res)
            {
                case System.Windows.Forms.DialogResult.OK:
                    SourceFrom.Text = FDialog.SelectedPath;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    break;
            }
        }

        private void SelectDest_Click(object sender, RoutedEventArgs e)
        {
            var FDialog = new System.Windows.Forms.FolderBrowserDialog();
            var res = FDialog.ShowDialog();

            switch (res)
            {
                case System.Windows.Forms.DialogResult.OK:
                    DestinationCopy.Text = GetUNCPath(FDialog.SelectedPath);
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    break;
            }
        }

        private void excludeHelp_Click(object sender, RoutedEventArgs e)
        {
            string howto = "You can exclude folders by pasting the path to the folder you want excluded, ex: \n" +
                "C:\\Users\\my_user\\this_gets_copied\\this_gets_excluded\n\n" +
                "You can also exclude multiple folders by separating them with comma (,) ex:\n" +
                "C:\\Users\\my_user\\this_gets_copied\\but_not_this,C:\\Users\\my_user\\this_gets_copied\\but_not_this_either,\n" +
                "C:\\Users\\my_user\\this_gets_copied\\but_also_not_this_one";

            MessageBox.Show(howto, "How to exclude", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
