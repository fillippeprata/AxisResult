namespace Axis;

//todo: Move to a dedicated AxisTrix.Email project (adapters for SES, SendGrid, SMTP, etc.)
public interface IAxisEmailService
{
    Task<AxisResult> SendAsync(AxisEmailData data);
}
