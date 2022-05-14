
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

namespace ExwhyzeeNotification.BackgroundTwo
{

    public class RequestInfo
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Retries { get; set; }
    }

    public class BackgroundTwo : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        public BackgroundTwo(ILogger<BackgroundTwo> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }
        private async void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //Do your stuff with your Dbcontext
                IQueryable<Message> msgk = from s in _context.Messages
                                                    .Where(x => x.NotificationStatus == NotificationStatus.NotSent || x.NotificationStatus == NotificationStatus.NotDefind)
                                                    .Where(x => x.DateTime == DateTime.UtcNow.AddHours(1))
                                                    .Where(x => x.Retries == 0)
                                           select s;


                foreach (var i in msgk)
                {
                    Queue queue = new Queue();
                    queue.MessageId = i.Id;

                    _context.Queues.Add(queue);

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


        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
