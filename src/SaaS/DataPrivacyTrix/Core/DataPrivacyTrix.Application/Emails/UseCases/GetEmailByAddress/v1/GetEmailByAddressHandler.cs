using Axis;
using AxisMediator.Contracts.CQRS.Queries;
using DataPrivacyTrix.Contracts.Emails.v1.GetEmailByAddress;
using DataPrivacyTrix.Ports.Emails;

namespace DataPrivacyTrix.Application.Emails.UseCases.GetEmailByAddress.v1;

internal class GetEmailByAddressHandler(
    IEmailsReaderPort readerPort
) : IAxisQueryHandler<GetEmailByAddressQuery, GetEmailByAddressResponse>
{
    public Task<AxisResult<GetEmailByAddressResponse>> HandleAsync(GetEmailByAddressQuery query)
        => readerPort.GetByEmailAddressAsync(query.Email!)
            .MapAsync(entity => new GetEmailByAddressResponse { EmailId = entity.EmailId });
}
