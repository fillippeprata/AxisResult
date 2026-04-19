using Axis;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Ports.AxisIdentities;

public interface IAxisIdentityEmailsWritePort
{
    Task<AxisResult> AddEmailAsync(AxisIdentityId axisIdentityId, EmailId emailId);
}
