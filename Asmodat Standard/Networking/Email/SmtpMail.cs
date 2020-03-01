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

        public async Task Send(string from, string to, string body, string subject, IEnumerable<string> attachments = null, bool recursive = false, bool isBodyHTML = false, bool throwIfAttachementNotFound = false)
        {
            using (MailMessage mm = new MailMessage())
            {
                mm.From = new MailAddress(from);
                mm.To.Add(to);
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
                            var files = DirectoryInfoEx.GetFiles(attachment.ToDirectoryInfo(), recursive: recursive);
                            if (!files.IsNullOrEmpty())
                                foreach (var file in files)
                                    attachFiles.Add(file.FullName);
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
