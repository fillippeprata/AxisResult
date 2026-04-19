using Axis;
using DataPrivacyTrix.Domain.Registration.Root;
using DataPrivacyTrix.Ports.Registration;
using DataPrivacyTrix.SharedKernel.Cellphones;
using DataPrivacyTrix.SharedKernel.Registration;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Application.Registration;

internal interface IAxisIdentityAggregateApplication : IAxisIdentityEntityProperties
{
    Task<AxisResult> IsValidAsync();
    Task<AxisResult> AddCellphoneAsync(CellphoneId cellphoneId);
    Task<AxisResult> AddEmailAsync(EmailId emailId);
}

internal class AxisIdentityAggregateApplication(
    IAxisIdentityEntityProperties properties,
    IAxisIdentityCellphonesWritePort cellphonesWriter,
    IAxisIdentityEmailsWritePort emailsWriter
) : AxisIdentityEntity(properties), IAxisIdentityAggregateApplication
{
    public new Task<AxisResult> IsValidAsync()
        => base.IsValidAsync();

    public Task<AxisResult> AddCellphoneAsync(CellphoneId cellphoneId)
        => AddCellphoneAsync()
            .ThenAsync(() => cellphonesWriter.AddCellphoneAsync(AxisIdentityId, cellphoneId));

    public Task<AxisResult> AddEmailAsync(EmailId emailId)
        => AddEmailAsync()
            .ThenAsync(() => emailsWriter.AddEmailAsync(AxisIdentityId, emailId));
}
