using DataPrivacyTrix.SharedKernel.Emails;

namespace DataPrivacyTrix.Ports.Emails;

public interface IEmailsWritePort
{
    Task<AxisResult.AxisResult> CreateAsync(IEmailEntityProperties properties);
}
