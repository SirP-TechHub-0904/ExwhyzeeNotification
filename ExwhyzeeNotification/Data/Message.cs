using Microsoft.AspNetCore.Identity;
using System;

namespace ExwhyzeeNotification.Data
{
    public class Message
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
        public bool Repeat { get; set; }
        public int Minute { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public NotifyStatus NotifyStatus { get; set; }
        public NotificationType NotificationType { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int Retries { get; set; }
        public string Receipient { get; set; }
    }
}
