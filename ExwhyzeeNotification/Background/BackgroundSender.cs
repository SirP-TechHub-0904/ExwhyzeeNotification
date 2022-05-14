
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ExwhyzeeNotification.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ExwhyzeeNotification.Background
{

    public class RequestInfo
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Retries { get; set; }
    }

    public class BackgroundSender : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        public BackgroundSender(ILogger<BackgroundSender> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(40));

            return Task.CompletedTask;
        }
        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //Do your stuff with your Dbcontext
                IQueryable<Queue> msg = from s in _context.Queues.Include(x=>x.Message)
                                                    .Where(x => x.Message.NotificationStatus == NotificationStatus.NotSent || x.Message.NotificationStatus == NotificationStatus.NotDefind)
                                                   .Where(x=>x.Message.Retries < 5)
                                                    .Take(7)
                                                   
                                          select s;


                var c = msg.Count();
                var cf = msg.ToList();

                foreach (var i in msg)
                {
                    if (i.Message.NotificationType == NotificationType.Email)
                    {
                        //
                        bool result = await SendEmail(i.Message.Receipient, i.Message.Receipient, i.Message.Title);
                        if (result == true)
                        {
                            i.Message.NotificationStatus = NotificationStatus.Sent;
                        }
                        else
                        {
                            i.Message.NotificationStatus = NotificationStatus.NotSent;
                            i.Message.Retries = i.Message.Retries + 1;
                        }
                    }else if (i.Message.NotificationType == NotificationType.SMS)
                    {
                        i.Message.Content = i.Message.Content.Replace("0", "O");
                        i.Message.Content = i.Message.Content.Replace("yahoo", "yah0o");
                        i.Message.Content = i.Message.Content.Replace("Services", "Servics");
                        i.Message.Content = i.Message.Content.Replace("gmail", "g -mail");
                        string response = "";

                        var getApi = "http://account.kudisms.net/api/?username=peterahioma2020@gmail.com&password=nation@123&message=@@message@@&sender=@@sender@@&mobiles=@@recipient@@";
                        string apiSending = getApi.Replace("@@sender@@", "Ahioma").Replace("@@recipient@@", HttpUtility.UrlEncode(i.Message.Receipient)).Replace("@@message@@", HttpUtility.UrlEncode(i.Message.Content));

                        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiSending);
                        httpWebRequest.Method = "GET";
                        httpWebRequest.ContentType = "application/json";

                        //getting the respounce from the request
                        HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                        Stream responseStream = httpWebResponse.GetResponseStream();
                        StreamReader streamReader = new StreamReader(responseStream);
                        response = await streamReader.ReadToEndAsync();

                        //
                        
                        if (response.Contains("OK"))
                        {
                            i.Message.NotificationStatus = NotificationStatus.Sent;
                        }
                        else
                        {
                            i.Message.NotificationStatus = NotificationStatus.NotSent;
                            i.Message.Retries = i.Message.Retries + 1;
                        }
                    }

                    try
                    {

                        var iod = await _context.Messages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == i.Id);
                        iod.NotificationStatus = i.Message.NotificationStatus;
                        iod.Retries = i.Message.Retries;
                        //_context.Entry(iod).State = EntityState.Detached;
                        _context.Attach(iod).State = EntityState.Modified;


                    }

                    catch (Exception webex)
                    {

                    }


                }
                await _context.SaveChangesAsync();
            }
        }
        //private async void DoWork(object state)
        //{
        //    _logger.LogInformation("Timed Background Service is working.");
        //    await _senderService.Notification();
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public async Task<bool> SendEmail(string recipient, string message, string title)
        {
            try
            {


                //create the mail message 
                MailMessage mail = new MailMessage();


                //mail.Body = message;
                ////set the addresses 
                //mail.From = new MailAddress("noreply@pestroglobal.com", "PESTROGLOBAL LTD"); //IMPORTANT: This must be same as your smtp authentication address.
                //mail.To.Add(recipient);

                ////set the content 
                //mail.Subject = title.Replace("\r\n", "");

                //mail.IsBodyHtml = true;
                ////send the message 
                //SmtpClient smtp = new SmtpClient("mail.pestroglobal.com");

                ////IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                //NetworkCredential Credentials = new NetworkCredential("noreply@pestroglobal.com", "ASD@1k123");
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = Credentials;
                //smtp.Port = 25;    //alternative port number is 8889
                //smtp.EnableSsl = false;
                //smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }



        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
