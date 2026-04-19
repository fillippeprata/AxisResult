using Axis;
using AxisMediator.Contracts;
using AxisRepository.Postgres;
using DataPrivacyTrix.Driven.Repositories.Postgres.Emails.Scripts;
using DataPrivacyTrix.Ports.Emails;
using DataPrivacyTrix.SharedKernel;
using DataPrivacyTrix.SharedKernel.Emails;
using Microsoft.Extensions.DependencyInjection;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Emails;

internal class EmailsRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow), IEmailsReaderPort, IEmailsWritePort
{
    private const string Select = $"SELECT {EmailsTable.EmailId}, {EmailsTable.EmailAddress}";

    public Task<AxisResult<IEmailEntityProperties>> GetByIdAsync(EmailId emailId)
        => GetAsync<IEmailEntityProperties>(
            $"{Select} FROM {EmailsTable.Table} WHERE {EmailsTable.EmailId} = @id",
            p => p.AddWithValue("id", emailId.ToString()),
            EmailDbEntity.FromReader,
            "EMAIL_NOT_FOUND");

    public Task<AxisResult<IEmailEntityProperties>> GetByEmailAddressAsync(string emailAddress)
        => GetAsync<IEmailEntityProperties>(
            $"{Select} FROM {EmailsTable.Table} WHERE {EmailsTable.EmailAddress} = @address",
            p => p.AddWithValue("address", emailAddress),
            EmailDbEntity.FromReader,
            "EMAIL_NOT_FOUND");

    public Task<AxisResult> CreateAsync(IEmailEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {EmailsTable.Table} ({EmailsTable.EmailId}, {EmailsTable.EmailAddress}) " +
            "VALUES (@id, @address)",
            p =>
            {
                p.AddWithValue("id", properties.EmailId.ToString());
                p.AddWithValue("address", properties.EmailAddress);
            },
            duplicateKeyCode: "EMAIL_ALREADY_EXISTS");
}
