using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace AUTO_ARCHIVE.Controllers
{
    public class AccountController : Controller
    {
        public readonly IHostingEnvironment _env;

        public readonly string _clientId;

        public readonly string _email;

        public readonly string _password;

        public AccountController(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _clientId = configuration["Authentication:Cognito:ClientId"];
            _email = configuration["Exade-IT:Email"];
            _password = configuration["Exade-IT:Password"];
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task SignOut()
        {
            try
            {
                string startUrl = null;

                if(_env.IsDevelopment())
                {
                    startUrl = "https://localhost:44329";
                }
                else
                {
                    startUrl = "https://scarfbeta.azurewebsites.net";
                }

                string redirectUrl = "https://exade-it.auth.ca-central-1.amazoncognito.com/logout?response_type=code&client_id=" + _clientId + "&redirect_uri=" + startUrl + "/signin-oidc&state=STATE&scope=email+openid+profile";

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.Response.Redirect(redirectUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IActionResult> EmailForm(string selection, string formSubj, string formDesc)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress")).Value;

            string userName = User.Claims.FirstOrDefault(c => c.Type.Equals("name")).Value.Split(" ")[0];

            var sendClient = new SmtpClient(); var replyClient = new SmtpClient();

            sendClient.Connect("smtp.mail.us-east-1.awsapps.com", 465, true);

            // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism.
            sendClient.AuthenticationMechanisms.Remove("XOAUTH2");

            // Note: only needed if the SMTP server requires authentication
            sendClient.Authenticate(_email, _password);

            var sendMsg = new MimeMessage();

            sendMsg.From.Add(new MailboxAddress(_email));

            if(selection == "support" && formSubj != null && formDesc != null)
            {
                sendMsg.To.Add(new MailboxAddress("support@scarfbeta.de"));

                sendMsg.Subject = "SUPPORT: " + formSubj;

                sendMsg.Body = new TextPart("plain")
                {
                    Text = "User: " + userName + "\nEmail: " + userEmail + "\nMessage: " + formDesc
                };

                await sendClient.SendAsync(sendMsg);

                await SendReply(selection);

                sendClient.Disconnect(true);

                return Redirect(Request.Headers["Referer"].ToString());
            }

            else if (selection == "feedback" && formSubj != null && formDesc != null)
            {
                sendMsg.To.Add(new MailboxAddress("feedback@scarfbeta.de"));

                sendMsg.Subject = "FEEDBACK: " + formSubj;

                sendMsg.Body = new TextPart("plain")
                {
                    Text = "User: " + userName + "\nEmail: " + userEmail + "\nMessage: " + formDesc
                };

                await sendClient.SendAsync(sendMsg);

                await SendReply(selection);

                sendClient.Disconnect(true);

                return Redirect(Request.Headers["Referer"].ToString());
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task SendReply(string typeOfReply)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress")).Value;

            string userName = User.Claims.FirstOrDefault(c => c.Type.Equals("name")).Value.Split(" ")[0];

            var replyClient = new SmtpClient();

            replyClient.Connect("smtp.mail.us-east-1.awsapps.com", 465, true);

            replyClient.AuthenticationMechanisms.Remove("XOAUTH2");

            replyClient.Authenticate(_email, _password);

            var replyMsg = new MimeMessage();

            replyMsg.From.Add(new MailboxAddress(_email));

            replyMsg.To.Add(new MailboxAddress(userEmail));

            if(typeOfReply == "support")
            {
                replyMsg.Subject = "SUPPORT RECEIPT";

                replyMsg.Body = new TextPart("plain")
                {
                    Text = "Hi " + userName + ", \n" + "\tWe have received your support case. A member from the Auto Konnect support team will be with you shortly.\n\nThank you \nAuto Konnect"
                };

                await replyClient.SendAsync(replyMsg);

                replyClient.Disconnect(true);
            }

            else if (typeOfReply == "feedback")
            {
                replyMsg.Subject = "THANK YOU!!";

                replyMsg.Body = new TextPart("plain")
                {
                    Text = "Hi " + userName + ", \n" + "\t Your feedback has been received and we would like to thank you.\n\n Auto Konnect"
                };

                await replyClient.SendAsync(replyMsg);

                replyClient.Disconnect(true);
            }
            
        }
    }
}