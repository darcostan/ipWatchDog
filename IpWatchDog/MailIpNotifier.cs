using System;
using MailKit;
using IpWatchDog.Log;
using MailKit.Net.Smtp;
using MimeKit;

namespace IpWatchDog
{
	class MailIpNotifier : IIpNotifier
	{
		ILog _log;
		AppConfig _config;

		public MailIpNotifier(ILog log, AppConfig config)
		{
			_log = log;
			_config = config;
		}

		public void OnIpChanged(string oldIp, string newIp)
		{
			var msg = GetMessage(oldIp, newIp, _config);
			_log.Write(LogLevel.Warning, msg.Body.ToString());

			try
			{
				var smtpClient = new SmtpClient();
				smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
				smtpClient.Connect(_config.SmtpHost, 587, false);
				smtpClient.Authenticate("dalibor1988@yandex.com", "hjzbtidgqxtesflh");
				smtpClient.Send(msg);
				smtpClient.Disconnect(true);


			}
			catch (Exception ex)
			{
				_log.Write(LogLevel.Error, "Error sending e-mail. {0}", ex);
			}
		}

		private static MimeMessage GetMessage(string oldIp, string newIp, AppConfig config)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("IP-service", config.MailFrom));
			message.To.Add(new MailboxAddress("Dalibor Stankovic", config.MailTo));
			message.Subject = config.Subject;

			message.Body = new TextPart("plain")
			{
				Text = @"Ip je promenjen sa " + oldIp + " na " + newIp
			};
			return message;
		}
	}
}
