using Axis;
using DataPrivacyTrix.Ports.AxisIdentities;
using DataPrivacyTrix.SharedKernel.AxisIdentities;
using DataPrivacyTrix.SharedKernel.Cellphones;
using AxisIdentityEntity = DataPrivacyTrix.Domain.AxisIdentities.Root.AxisIdentityEntity;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Application.AxisIdentities;

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
