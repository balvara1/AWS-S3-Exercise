using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Shapes;


namespace _301128209alvarado_Lab1
{
    /// <summary>
    /// Interaction logic for ObjectWindow.xaml
    /// </summary>
    public partial class ObjectWindow : Window
    {
        private ObservableCollection<ObjectItem> objects = new ObservableCollection<ObjectItem>();
        public IConfiguration Configuration { get; private set; }

        public string filePath;


        // AWS login credentials
        private String accessKeyID;
        private String secretKey;
        private BasicAWSCredentials awsCredentials;
        AmazonS3Client s3Client;

        public ObjectWindow()
        {
            InitializeComponent();
            createAWSCredentials();
            LoadBucketList();
            dataGrid_objectList.ItemsSource = objects;
        }

        private void createAWSCredentials()
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            accessKeyID = Configuration.GetSection("AWSCredentials").GetSection("AccesskeyID").Value;
            secretKey = Configuration.GetSection("AWSCredentials").GetSection("Secretaccesskey").Value;

            awsCredentials = new BasicAWSCredentials(accessKeyID, secretKey);
            s3Client = new AmazonS3Client(awsCredentials, RegionEndpoint.USEast1);
        }

        private async void LoadBucketList()
        {
            ListBucketsResponse res = await s3Client.ListBucketsAsync();

            foreach (S3Bucket bucket in res.Buckets)
            {
                comboBox_Bucket.Items.Add(bucket.BucketName);
            }
        }

        private void comboBox_Bucket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objects.Clear();
            LoadObjects();
        }

        private async void LoadObjects()
        {
            var bucketName = comboBox_Bucket.SelectedItem.ToString();

            // Test
            //MessageBox.Show("Bucket Name " + bucketName, "Selected Item");

            ListObjectsRequest request = new ListObjectsRequest();
            request.BucketName = bucketName;


            ListObjectsResponse response = await s3Client.ListObjectsAsync(request);
            foreach (S3Object o in response.S3Objects)
            {
                objects.Add(new ObjectItem()
                {
                    Key = o.Key,
                    Size = o.Size.ToString()

                });
            }
        }

        private void btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            string filepath;

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            bool? response = openFileDialog.ShowDialog();

            if(response == true)
            {
                filepath = openFileDialog.FileName;
                textbox_ObjectName.Text = filepath;
            }    
        }

        private void btn_MainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        public async void btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            filePath = textbox_ObjectName.Text.ToString();

            if (String.IsNullOrEmpty(textbox_ObjectName.Text))
            {
                MessageBox.Show("Select an Object", "Error");
                return;
            }

            try
            {
                string bucketName = getBucketName();

                
                TransferUtility transfer = new TransferUtility(s3Client);
                await transfer.UploadAsync(filePath, bucketName);

                // clear textbox with filepath

                textbox_ObjectName.Clear();

                // clear data grid so as not to overlap

                objects.Clear();

                // reload contents of bucket
                LoadObjects();
            }
            catch (AmazonS3Exception amazonS3Ex)
            {
                string failMsg = $"Failed uploading object.\n{amazonS3Ex.Message}";
                MessageBox.Show(failMsg, "Alert");
            }
        }

        private string getBucketName()
        {
            
            return comboBox_Bucket.SelectedItem.ToString();
        }

    }
}
