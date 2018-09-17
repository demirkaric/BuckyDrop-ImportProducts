using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace BuckyDrop
{
    public partial class Form1 : Form
    {
        private List<string> links = new List<string>();
        private WebRequests webRequests = new WebRequests();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// startButton runs the app (backgroundWorker)
        /// </summary>
        private void startButton_Click(object sender, EventArgs e) => bckWorker.RunWorkerAsync();

        /// <summary>
        /// Adding product links to a list
        /// </summary>
        private void addButton_Click(object sender, EventArgs e) => ProductsToList();

        /// <summary>
        /// Opening Homepage (Login page)
        /// </summary>
        private void GetHomepage() => webRequests.Get("https://www.buckydrop.com/en/login/?ref=https%3A%2F%2Fwww.buckydrop.com%2Fen%2Fadmin%2Fproducts%2Fsearch",
                "https://www.buckydrop.com/en/admin/products/search");

        /// <summary>
        /// Login to BuckyDrop.com
        /// </summary>
        private void Login()
        {
            // Appending data to Richtextbox
            Invoke(new Action(() => richBox.AppendText("Login to buckydrop.com...")));
            Refreshing();

            // Posting Login data
            webRequests.Post("https://www.buckydrop.com/supersell/rest/login/auth",
                "username=" + textBox1.Text + "&password=" + textBox2.Text + "&next=", "application/x-www-form-urlencoded",
                "https://www.buckydrop.com/en/login/?ref=https%3A%2F%2Fwww.buckydrop.com%2Fen%2Fadmin%2Fproducts%2Fsearch", false);
        }

        /// <summary>
        /// Add all products to the import list at buckydrop.com
        /// </summary>
        private void AddToImportList()
        {
            foreach (string link in links)
                Add(SPU(link));
        }

        /// <summary>
        /// Getting SPU code and Platform name for specific porduct
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public string SPU(string link)
        {
            // Appending data to Richtextbox
            Invoke(new Action(() => richBox.AppendText(Environment.NewLine + "Adding: " + link)));
            Refreshing();

            // Requesting product details in Json
            string responseFromServer = webRequests.Post("https://www.buckydrop.com/supersell/rest/micro-shop/check/searchOrUpdateProduct",
               "{\"productLink\":\"" + link + "\",\"language\":1}", "application/json", null, true);

            // Parsing Json, getting SPU Code and Platform name
            var obj = Newtonsoft.Json.Linq.JObject.Parse(responseFromServer);
            var spucode = (string)obj.SelectToken("data.result.spuCode");
            var platform = (string)obj.SelectToken("data.result.platform");

            return spucode + "," + platform;
        }

        /// <summary>
        /// Adding product to the import list at buckydrop.com
        /// </summary>
        /// <param name="input"></param>
        private void Add(string input)
        {
            string spu = "", platform = "";
            // Spliting spu and platform name from the array (using "try-catch" in the case of "index out of range")
            try
            {
                string[] arr = input.Split(',');
                spu = arr[0];
                platform = arr[1];
            }
            catch { }

            // Inserting the product to import list at buckydrop.com
            string responseFromServer = webRequests.Post("https://www.buckydrop.com/supersell/rest/micro-shop/editProduct/createData",
                "{\"platform\":\"" + platform + "\",\"spuCode\":\"" + spu + "\",\"needTimelyGrab\":0}", "application/json;charset=UTF-8",
                "https://www.buckydrop.com/en/admin/products/search/detail/?spu=" + spu + "&platform=" + platform, true);

            // Write response code to Richtextbox
            if (responseFromServer.Contains("Success"))
            {
                Invoke(new Action(() => richBox.AppendText(Environment.NewLine + "Success")));
                Refreshing();
            }
            else
            {
                Invoke(new Action(() => richBox.AppendText(Environment.NewLine + "Not Exist!")));
                Refreshing();
            }
        }

        /// <summary>
        /// Using background worker (Prevents UI thread from freezing)
        /// </summary>
        private void bckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Creating separated threads for every method
            Thread Hmp = new Thread(() => GetHomepage());
            Hmp.Start();
            Hmp.Join();

            Thread Lgn = new Thread(() => Login());
            Lgn.Start();
            Lgn.Join();

            Thread Imp = new Thread(() => AddToImportList());
            Imp.Start();
            Imp.Join();
        }

        /// <summary>
        /// Refreshing and Scrolling to the end of Richtextbox
        /// </summary>
        private void Refreshing()
        {
            Invoke(new Action(() => richBox.SelectionStart = richBox.Text.Length));
            Invoke(new Action(() => richBox.ScrollToCaret()));
            Invoke(new Action(() => richBox.Invalidate()));
            Invoke(new Action(() => richBox.Update()));
            Invoke(new Action(() => richBox.Refresh()));
        }

        /// <summary>
        /// Adding products links from Richtextbox to List<string> links
        /// </summary>
        private void ProductsToList()
        {
            string[] tempArray = richBox.Lines;
            for (int counter = 0; counter < tempArray.Length; counter++)
            {
                if (richBox.Text != "")
                {
                    links.Add(tempArray[counter]);
                }
            }
            // Clearing Richtextbox
            richBox.Clear();
        }
    }
}