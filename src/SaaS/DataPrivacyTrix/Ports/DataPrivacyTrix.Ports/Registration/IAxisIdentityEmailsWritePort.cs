using Axis;
using DataPrivacyTrix.SharedKernel.Registration;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Ports.Registration;

public interface IAxisIdentityEmailsWritePort
{
    Task<AxisResult> AddEmailAsync(AxisIdentityId axisIdentityId, EmailId emailId);
}
