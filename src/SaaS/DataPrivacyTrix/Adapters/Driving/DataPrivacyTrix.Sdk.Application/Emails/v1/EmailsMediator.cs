using Axis;
using AxisMediator.Contracts;
using DataPrivacyTrix.Contracts.Emails.v1;
using DataPrivacyTrix.Contracts.Emails.v1.AddEmail;
using DataPrivacyTrix.Contracts.Emails.v1.GetByEmailAddress;

namespace DataPrivacyTrix.Sdk.Application.Emails.v1;

internal class EmailsMediator(IAxisMediator mediator) : IEmailsMediator
{
    public Task<AxisResult<AddEmailResponse>> AddAsync(AddEmailCommand command)
        => mediator.Cqrs.ExecuteAsync<AddEmailCommand, AddEmailResponse>(command);

    public Task<AxisResult<GetByEmailAddressResponse>> GetByEmailAddressAsync(GetByEmailAddressQuery query)
        => mediator.Cqrs.QueryAsync<GetByEmailAddressQuery, GetByEmailAddressResponse>(query);
}
