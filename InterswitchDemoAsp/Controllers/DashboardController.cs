using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Backoffice.Controllers
{
    public class DashboardController : Controller
    {
        public string MacKey  = "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRankAdvancementCard(int rankid)
        {
            return PartialView("Cards/RankAdvancement");
        }

        public ActionResult Interswitch()
        {
            var TransactionRef  = "PfMXkWm7sE61";
            var Amount          = 55000;

            var RequeryUrl  = "https://sandbox.interswitchng.com/webpay/api/v1/gettransaction.json?productId=6205&transactionreference=" + TransactionRef + "&amount=" + Amount;

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

            ViewBag.Data = Data;

            return View();
        }

        public string HashString(string Hash)
        {
            return BitConverter.ToString(new SHA512CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(Hash))).Replace("-", String.Empty).ToUpper();
        }
    }
}