using Axis;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AxisEmail.MimeKit;

//todo: Move to a dedicated EmailTrix project (use AWS SES or another adapter instead of sending directly)
public class AxisEmailService(IOptions<AxisEmailSettings> emailSettings, IAxisLogger<AxisEmailService> logger) : IAxisEmailService
{
    private readonly AxisEmailSettings _trixSettings = emailSettings.Value;

    public async Task<AxisResult> SendAsync(AxisEmailData emailTrixData)
    {
        try
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_trixSettings.Sender.Name, _trixSettings.Sender.Address));
            message.Subject = emailTrixData.Subject;
            message.Body = new TextPart(emailTrixData.BodyTextType) { Text = emailTrixData.Body };
            foreach(var (name, emailAddress) in emailTrixData.To)
                message.To.Add(new MailboxAddress(name, emailAddress));
            foreach(var (name, emailAddress) in emailTrixData.Cc)
                message.Cc.Add(new MailboxAddress(name, emailAddress));

            using var client = new SmtpClient();
            var secureSocketOptions = _trixSettings.Smtp.SslEnabled ? SecureSocketOptions.Auto : SecureSocketOptions.None;
            await client.ConnectAsync(_trixSettings.Smtp.Host, _trixSettings.Smtp.Port, secureSocketOptions);
            await client.AuthenticateAsync(_trixSettings.Sender.Address, _trixSettings.Sender.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR_SENDING_EMAIL");
            return AxisError.InternalServerError("ERROR_SENDING_EMAIL");
        }

        return AxisResult.Ok();
    }

}
