using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using DnsClient;
using JetBrains.Annotations;
using MailKit.Net.Smtp;
using MimeKit;
using Syncfusion.Pdf;
using Syncfusion.XPS;
using Tauron.Application.CelloManager.Resources;

namespace Tauron.Application.CelloManager.Logic.RefillPrinter.Rule
{
    [PublicAPI]
    public class MailHelper
    {
        public static bool MailOrder(FlowDocument document, string email, string dns)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            string server = GetServer(email, dns);

            if (string.IsNullOrWhiteSpace(server)) return false;

            MimeMessage message = GetMessage(email, document);

            return message != null && SendMail(message, server);
        }

        public static string GetServer(string email, string dns)
        {
            try
            {
                var client = new LookupClient(IPAddress.Parse(dns));

                var response = client.Query(email.Split('@')[1], QueryType.MX);

                var anser = response.Answers.MxRecords().OrderByDescending(r => r.Preference).FirstOrDefault();

                return anser?.Exchange;
            }
            catch
            {
                return null;
            }
        }

        public static MimeMessage GetMessage(string email, FlowDocument doc)
        {
            try
            {

                var msg = new MimeMessage
                          {
                              Priority = MessagePriority.Urgent,
                              Subject  = UIResources.EmailerMessageSubject
                          };

                msg.From.Add(new MailboxAddress("Cello Manager", "cello@blueprint.de"));
                msg.To.Add(InternetAddress.Parse(ParserOptions.Default, email));
                byte[] pdfDocument = ConvertToPdf(doc);

                BodyBuilder builder = new BodyBuilder {TextBody = UIResources.EmailerMessageBody};
                builder.Attachments.Add("Order.pdf", pdfDocument);

                msg.Body = builder.ToMessageBody();

                return msg;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ConvertToPdf(FlowDocument doc)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XamlToXps converter1 = new XamlToXps();
                converter1.LoadXaml(doc);
                converter1.Save(stream);
                stream.Position = 0;


                XPSToPdfConverter converter2 = new XPSToPdfConverter();
                PdfDocument       document   = converter2.Convert(stream.ToArray());

                using (MemoryStream targetStream = new MemoryStream())
                {
                    document.Save(targetStream);
                    targetStream.Position = 0;
                    return targetStream.ToArray();
                }
            }
        }

        public static bool SendMail(MimeMessage message, string server)
        {
            try
            {
                using (var smclient = new SmtpClient())
                {
                    smclient.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
                    smclient.Connect(server, 25, false);

                    //mclient.Authenticate("joey", "password");
                    smclient.Send(message);
                    smclient.Disconnect(true);
                    return true;
                }
            }
            catch(Exception e)
            {
                CommonApplication.Current.Container.Resolve<IDialogFactory>()
                    .ShowMessageBox(CommonApplication.Current.MainWindow, e.Message, "Error", MsgBoxButton.Ok, MsgBoxImage.Warning, null);

                return false;
            }
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors) => !sslpolicyerrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors);
    }
}