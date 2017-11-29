using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace InterswitchDemoAsp.Controllers
{
    public class HomeController : Controller
    {
        public string MacKey                = "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F";

        public ActionResult Index()
        {
            Random random                   = new Random();

            ViewBag.ProductId               = 6205;
            ViewBag.PayItemId               = 101;
            ViewBag.Amount                  = random.Next(1000, 100000);
            ViewBag.Amount                  = ViewBag.Amount - ViewBag.Amount % 1000;
            ViewBag.Currency                = "566";
            ViewBag.SiteRedirectUrl         = Request.Url.GetLeftPart(UriPartial.Authority)+"/home/callback/";
            ViewBag.TransactionReference    = RandomString();
            ViewBag.CustomerId              = random.Next(0, 100);

            string Hash         = ViewBag.TransactionReference+ViewBag.ProductId+ViewBag.PayItemId+ViewBag.Amount+ViewBag.SiteRedirectUrl+this.MacKey;

            ViewBag.Hash        = HashString(Hash);

            return View();
        }

        public string RandomString(int length = 12)
        {
            char[] CharArray = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

            string RandomString = string.Empty;
            Random Random       = new Random();

            for(int i = 0; i<length; i++)
            {
                RandomString += CharArray.GetValue(Random.Next(1, CharArray.Length));
            }

            return RandomString;
        }

        public string HashString(string Hash)
        {
            return BitConverter.ToString(new SHA512CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(Hash))).Replace("-", String.Empty).ToUpper();
        }


        public ActionResult CallBack()
        {

            var TransactionRef  = Request["txnref"];
            var PayRef          = Request["payRef"];
            var Amount          = Request["amount"];

            var RequeryUrl  = "https://sandbox.interswitchng.com/webpay/api/v1/gettransaction.json?";
            RequeryUrl      += "productId=6205&transactionreference=" + TransactionRef + "&amount=" + Amount;

            var Hash        = HashString(6205 + TransactionRef + this.MacKey);
            var Data        = "";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Hash", Hash);

                using (HttpResponseMessage response = client.GetAsync(RequeryUrl).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        Data = content.ReadAsStringAsync().Result;
                    }
                }
            }


            //Use Data here...

            ViewBag.Response = Data;

            return View();
        }

     
    }
}