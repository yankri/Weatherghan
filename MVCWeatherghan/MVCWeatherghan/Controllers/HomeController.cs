using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Rotativa;
using System.Net.Mail;
using MVCWeatherghan.Models;
using System.Net;
using System.Threading;

namespace MVCWeatherghan.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() //homepage
        {
            Weatherdata WD = new Weatherdata();
            ViewBag.Title = "Home";
            return View(WD);
        }
        public ActionResult NewHome()
        {
            return View();
        }
        public ActionResult DisplayPattern(string zipcode, string year) //parameters are query strings from the index view input boxes
        {
            Weatherdata wd = new Weatherdata();
            wd.ZipCode = zipcode;
            wd.Year = year;
            wd.PatternRows = wd.GetPattern(wd.ZipCode, wd.Year);
            return View(wd.PatternRows);
        }

        public ActionResult DisplayPatternPDFView(string zipcode, string year)//parameters are query strings from the index view input boxes
        {
            Weatherdata wd = new Weatherdata();
            wd.ZipCode = zipcode;
            wd.Year = year;
            wd.PatternRows = wd.PatternPDFView(wd.ZipCode, wd.Year);
            return View(wd.PatternRows);

        }
        public ActionResult Patterns() //pattern suggestions page
        {
            return View();
        }

        public ActionResult Resources()
        {
            return View();
        }
        public ActionResult About() //not used yet
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            return View(new ContactForm());
        }

        [HttpPost]
        public ActionResult Contact(ContactForm e) //not used yet
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var toAddress = "weatherghan@gmail.com";
                    var fromAddress = e.Email.ToString();
                    var subject = String.Format("Test enquiry from {0} {1}", e.FirstName, e.LastName);
                    var message = new StringBuilder();
                    message.Append(String.Format("Name {0} {1} \n", e.FirstName, e.LastName));
                    message.Append("Email: " + e.Email + "\n");
                    message.Append("Message: " + e.Message + "\n\n");
                    message.Append(e.Message);

                    //start email Thread
                    var tEmail = new Thread(() =>
                   SendEmail(toAddress, fromAddress, subject, message.ToString()));
                    tEmail.Start();
                    return View("Success");
                }
                catch (Exception)
                {
                    return View("Error");
                }
        }
            return View();
    }
        public void SendEmail(string toAddress, string fromAddress, string subject, string message)
        {
            try
            {
                using (var mail = new MailMessage())
                {
                    const string email = "weatherghan@gmail.com";
                    const string password = "rileybung11";

                    var loginInfo = new NetworkCredential(email, password);

                    mail.From = new MailAddress(fromAddress);
                    mail.To.Add(new MailAddress(toAddress));
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;

                    try
                    {
                        using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtpClient.EnableSsl = true;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = loginInfo;
                            smtpClient.SendMailAsync(mail);
                        }
                    }
                    finally
                    {
                        //dispose the client
                        mail.Dispose();
                    }
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                {
                    var status = t.StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Response.Write("Delivery failed - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                        //resend
                        //smtpClient.Send(message);
                    }
                    else
                    {
                        Response.Write(String.Format("Failed to deliver message to {0}", t.FailedRecipient));
                    }
                }
            }
            catch (SmtpException Se)
            {
                // handle exception here
                Response.Write(Se.ToString());
            }

            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }

        }
        public ActionResult PatternToPDF(string zip, string viewyear)
    {
        return new ActionAsPdf("DisplayPatternPDFView", new { zipcode = zip, year = viewyear }) { FileName = "MyWeatherghanPattern.pdf" };
    }

    public ActionResult Gallery() //gallery page
    {
        return View();
    }
}
}