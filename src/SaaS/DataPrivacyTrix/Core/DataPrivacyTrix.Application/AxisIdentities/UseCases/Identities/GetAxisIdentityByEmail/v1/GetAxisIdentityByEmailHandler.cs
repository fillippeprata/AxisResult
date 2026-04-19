using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.AxisIdentities.v1.Identities.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Emails.v1;
using DataPrivacyTrix.Contracts.Emails.v1.GetEmailByAddress;
using DataPrivacyTrix.Ports.AxisIdentities;

namespace DataPrivacyTrix.Application.AxisIdentities.UseCases.Identities.GetAxisIdentityByEmail.v1;

internal class GetAxisIdentityByEmailHandler(
    IEmailsMediator emailsMediator,
    IAxisIdentitiesReaderPort readerPort
) : IAxisQueryHandler<GetAxisIdentityByEmailQuery, GetAxisIdentityByEmailResponse>
{
    public Task<AxisResult<GetAxisIdentityByEmailResponse>> HandleAsync(GetAxisIdentityByEmailQuery query) =>
        emailsMediator.GetByEmailAddressAsync(new GetEmailByAddressQuery{ Email = query.EmailAddress })
            .ZipAsync(email => readerPort.GetByEmailIdAsync(email.EmailId))
            .MapAsync((_, entity) => new GetAxisIdentityByEmailResponse
            {
                AxisIdentityId = entity.AxisIdentityId,
                DisplayName = entity.DisplayName
            });
}
