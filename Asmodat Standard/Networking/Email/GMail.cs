using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Networking
{
    public class GmailClientSecretJson
    {
        public GmailClientSecretInstalledJson installed { get; set; }
    }

    public class GmailClientSecretInstalledJson
    {
        public string client_id { get; set; }
        public string project_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_secret { get; set; }
        public string redirect_uris { get; set; }
    }

    public class Gmail
    {
        public GmailService Service { get; private set; }

        public BaseClientService.Initializer Initializer { get; private set; }

        public UserCredential Credentials { get; private set; }

        public Gmail(string user, GmailClientSecretInstalledJson gsecret) : this(
            User: user,
            project_id: gsecret.project_id,
            client_id: gsecret.client_id,
            client_secret: gsecret.client_secret)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="User">e.g</param>
        /// <param name="project_id">e.g XYZ1</param>
        /// <param name="client_id">e.g 504304721897-2ef8devll02lc732dg2aukte3fjtq7a3.apps.googleusercontent.com</param>
        /// <param name="client_secret">e.g R_g2-feEwk1jorX4GF3oDkzo</param>
        public Gmail(string User, string project_id, string client_id, string client_secret)
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = client_id,
                ClientSecret = client_secret
            },
            new[] { GmailService.Scope.GmailSend },
            user: User, taskCancellationToken: CancellationToken.None).Result;

            Service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = project_id
            });
        }

        public async Task Send(string from, string to, string subject, string body, bool HTML = false)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            mailMessage.To.Add(to);
            mailMessage.ReplyToList.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = HTML;

            /*foreach (System.Net.Mail.Attachment attachment in email.Attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }*/

            var mimeMessage = MimeMessage.CreateFromMailMessage(mailMessage);

            var gmailMessage = new Message
            {
                Raw = Base64UrlEncode(mimeMessage.ToString())
            };

            var request = Service.Users.Messages.Send(gmailMessage, to);

            var msg = await request.ExecuteAsync();
        }

        private static string Base64UrlEncode(string input)
        {
            var inputbytes = System.Text.Encoding.UTF8.GetBytes(input);
            return System.Convert.ToBase64String(inputbytes).Replace('+', '-').Replace('/', '-').Replace("=", "");
        }
    }
}
