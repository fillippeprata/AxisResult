using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Emails.v1;
using DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;
using DataPrivacyTrix.Ports.AxisIdentities;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.GetAxisIdentityByEmail.v1;

internal class GetAxisIdentityByEmailHandler(
    IEmailsMediator emailsMediator,
    IAxisIdentitiesReaderPort readerPort
) : IAxisQueryHandler<GetAxisIdentityByEmailQuery, GetAxisIdentityByEmailResponse>
{
    public async Task<AxisResult<GetAxisIdentityByEmailResponse>> HandleAsync(GetAxisIdentityByEmailQuery query)
    {
        var emailResult = await emailsMediator.GetByEmailAddressAsync(new GetByEmailAddressQuery { Email = query.EmailAddress });

        if (emailResult.IsFailure)
            return AxisResult.Error<GetAxisIdentityByEmailResponse>(emailResult.Errors);

        return await readerPort.GetByEmailIdAsync((EmailId)emailResult.Value.EmailId)
            .MapAsync(entity => new GetAxisIdentityByEmailResponse
            {
                AxisIdentityId = entity.AxisIdentityId,
                DisplayName = entity.DisplayName
            });
    }
}
