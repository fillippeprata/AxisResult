using AxisTrix;
using AxisTrix.CQRS.Queries;
using DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;
using DataPrivacyTrix.Ports.Emails;

namespace DataPrivacyTrix.Application.Emails.UseCases.GetByEmailAddress.v1;

internal class GetByEmailAddressHandler(
    IEmailsReaderPort readerPort
) : IAxisQueryHandler<GetByEmailAddressQuery, GetByEmailAddressResponse>
{
    public Task<AxisResult<GetByEmailAddressResponse>> HandleAsync(GetByEmailAddressQuery query)
        => readerPort.GetByEmailAddressAsync(query.Email!)
            .MapAsync(entity => new GetByEmailAddressResponse { EmailId = entity.EmailId });
}
