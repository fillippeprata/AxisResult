using AxisTrix.Results;
using DataPrivacyTrix.SharedKernel.Emails;

namespace DataPrivacyTrix.Ports.Emails;

public interface IEmailsWritePort
{
    Task<AxisResult> CreateAsync(IEmailEntityProperties properties);
}
