using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AsmodatStandard.Networking
{
   
    public class SmtpMailSetup
    {
        public string host { get; set; } = "smtp.gmail.com";
        public int port { get; set; } = 587;
        public bool ssl { get; set; } = true;
        public string login { get; set; }
        public string password { get; set; }
    }

    /// <summary>
    /// Requres setting up Allow Users to Manage Less Secure App access:
    /// https://admin.google.com/AdminHome?hl=en_GB&pli=1&fral=1#ServiceSettings/notab=1&service=securitysetting&subtab=lesssecureappsaccess
    /// </summary>
    public class SmtpMail
    {
        private SmtpClient _client;
        public string From { get; set; }

        public SmtpMail(SmtpMailSetup _setup) : this(
            host: _setup.host, 
            port: _setup.port, 
            login: _setup.login, 
            password: _setup.password, 
            ssl: _setup.ssl)
        {
           
        }

        public SmtpMail(string host, int port, string login, string password, bool ssl)
        {
            _client = new SmtpClient(host, port);
            _client.UseDefaultCredentials = false;
            _client.EnableSsl = ssl;
            _client.Credentials = new NetworkCredential(login, password);
        }

        public async Task Send(
            string from, 
            string to, 
            string body, 
            string subject, 
            IEnumerable<string> attachments = null, 
            bool recursive = false, bool isBodyHTML = false, 
            bool throwIfAttachementNotFound = false,
            bool throwIfAttachementTooBig = false,
            long attachementMaxSize = (25*1024*1024))
        {
            using (MailMessage mm = new MailMessage())
            {
                mm.From = new MailAddress(from);

                var recipients = to.Split(',').Where(x => !x.IsNullOrWhitespace()).ToArray();

                if (recipients.IsNullOrEmpty())
                    throw new System.Exception("Failed to send email, no recipients were specified");

                foreach(var recipient in recipients)
                {
                    if(!recipient.IsValidEmailAddress())
                        throw new System.Exception($"Recipient address '{recipient}' is not a valid email address");
                    mm.To.Add(recipient);
                }

                mm.Body = body;
                mm.Subject = subject;
                mm.IsBodyHtml = isBodyHTML;

                var attachFiles = new List<string>();
                if (!attachments.IsNullOrEmpty())
                {
                    foreach (var attachment in attachments)
                    {
                        if (File.Exists(attachment))
                            attachFiles.Add(attachment);

                        if (Directory.Exists(attachment))
                        {
                            long totalSize = 0;
                            var files = DirectoryInfoEx.GetFiles(attachment.ToDirectoryInfo(), recursive: recursive)?.OrderBy(x => x.Length)?.ToArray();
                            if (!files.IsNullOrEmpty())
                                foreach (var file in files)
                                {
                                    totalSize += attachementMaxSize;
                                    if (totalSize <= attachementMaxSize)
                                        attachFiles.Add(file.FullName);
                                    else if (throwIfAttachementTooBig)
                                        throw new System.Exception($"Total size of all attachements exeded maximum of {attachementMaxSize}B.");
                                }
                        }
                    }
                }

                attachments = attachFiles.Select(x => x.ToRuntimePath()).DistinctArray();

                foreach (var attachement in attachments)
                {
                    if (File.Exists(attachement))
                        mm.Attachments.Add(new Attachment(attachement));
                    else if (throwIfAttachementNotFound)
                        throw new System.Exception($"File not found: '{attachement ?? "undefined"}', could not attach to email.");
                }
                
                await _client.SendMailAsync(mm);
            }
        }
    }
}
