using System;
using System.IO;
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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;

namespace _301128209alvarado_Lab1
{
    /// <summary>
    /// Interaction logic for BucketWindow.xaml
    /// </summary>
    public partial class BucketWindow : Window
    {
        private ObservableCollection<BucketObject> buckets = new ObservableCollection<BucketObject>();
        public IConfiguration Configuration { get; private set; }

        // AWS login credentials
        private String accessKeyID;
        private String secretKey;
        private BasicAWSCredentials awsCredentials;
        AmazonS3Client s3Client;

        public BucketWindow()
        {
            InitializeComponent();
            createAWSCredentials();
            LoadBucketList();
            dataGrid_bucketList.ItemsSource = buckets;
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
                buckets.Add(new BucketObject()
                {
                    BucketName = bucket.BucketName,
                    CreateDate = bucket.CreationDate.ToString()
                });
            }
        }


        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            string BucketName = textbox_BucketName.Text.ToString();

            if (String.IsNullOrEmpty(textbox_BucketName.Text))
            {
                MessageBox.Show("Enter Bucket Name", "Error");
                return;
            }

            createNewBucket(BucketName);

        }

        private async void createNewBucket(string BucketName)
        {
            try
            {
                // put bucket request to insert
                PutBucketRequest putBucketRequest = new PutBucketRequest();
                putBucketRequest.BucketName = BucketName;

                PutBucketResponse bucketCreateResponse = await s3Client.PutBucketAsync(BucketName);

                // reload bucket list
                buckets.Clear();
                LoadBucketList();
            }
            catch (AmazonS3Exception amazonS3Ex)
            {
                String failed = $"Failed to Create Bucket.\n{amazonS3Ex.Message}";
                MessageBox.Show(failed, "Error");
            }
        }

        private void mainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();

        }
    }
}
