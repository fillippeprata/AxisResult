using AxisTrix.Results;
using DataPrivacyTrix.SharedKernel.Emails;

namespace DataPrivacyTrix.Ports.Emails;

public interface IEmailReadersPort
{
    Task<AxisResult<IEmailEntityProperties>> GetByEmailAsync(string emailAddress);
}
