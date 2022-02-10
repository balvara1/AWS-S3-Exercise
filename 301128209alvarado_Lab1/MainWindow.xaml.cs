using System;
using System.Collections.Generic;
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

namespace _301128209alvarado_Lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void createBucket_Click(object sender, RoutedEventArgs e)
        {
            BucketWindow bucketWindow = new BucketWindow();
            bucketWindow.Show();
            this.Close();
        }

        private void objectLevel_Click(object sender, RoutedEventArgs e)
        {
            ObjectWindow objectWindow = new ObjectWindow();
            objectWindow.Show();
            this.Close();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
