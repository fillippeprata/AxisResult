using Axis;
using AxisMediator.Contracts;
using AxisRepository.Postgres;
using DataPrivacyTrix.Driven.Repositories.Postgres.AxisIdentities.Scripts;
using DataPrivacyTrix.Ports.AxisIdentities;
using DataPrivacyTrix.SharedKernel;
using DataPrivacyTrix.SharedKernel.AxisIdentities;
using DataPrivacyTrix.SharedKernel.Cellphones;
using Microsoft.Extensions.DependencyInjection;
using AxisIdentityId = DataPrivacyTrix.SharedKernel.AxisIdentities.AxisIdentityId;
using CountryId = Axis.Localization.CountryId;
using EmailId = DataPrivacyTrix.SharedKernel.Emails.EmailId;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.AxisIdentities;

internal class AxisIdentitiesRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow),
    IAxisIdentitiesReaderPort,
    IAxisIdentitiesWritePort,
    IAxisIdentityCellphonesWritePort,
    IAxisIdentityEmailsWritePort
{
    private const string Select = $"SELECT " +
                                  $"{AxisIdentitiesTable.AxisIdentityId}, " +
                                  $"{AxisIdentitiesTable.IsIndividual}, " +
                                  $"{AxisIdentitiesTable.Document}, " +
                                  $"{AxisIdentitiesTable.CountryId}, " +
                                  $"{AxisIdentitiesTable.DisplayName}, " +
                                  $"{AxisIdentitiesTable.DefaultLanguage}, " +
                                  $"{AxisIdentitiesTable.SecurityLevel}";

    private const string SelectAliased = $"SELECT " +
                                         $"I.{AxisIdentitiesTable.AxisIdentityId}, " +
                                         $"I.{AxisIdentitiesTable.IsIndividual}, " +
                                         $"I.{AxisIdentitiesTable.Document}, " +
                                         $"I.{AxisIdentitiesTable.CountryId}, " +
                                         $"I.{AxisIdentitiesTable.DisplayName}, " +
                                         $"I.{AxisIdentitiesTable.DefaultLanguage}, " +
                                         $"I.{AxisIdentitiesTable.SecurityLevel}";

    public Task<AxisResult<IAxisIdentityEntityProperties>> GetByIdAsync(AxisIdentityId axisIdentityId)
        => GetAsync<IAxisIdentityEntityProperties>(
            $"{Select} FROM {AxisIdentitiesTable.Table} WHERE {AxisIdentitiesTable.AxisIdentityId} = @id",
            p => p.AddWithValue("id", axisIdentityId.ToString()),
            AxisIdentityDbEntity.FromReader,
            "AXIS_IDENTITY_NOT_FOUND");

    public Task<AxisResult<IAxisIdentityEntityProperties>> GetByDocumentAsync(CountryId countryId, string document)
        => GetAsync<IAxisIdentityEntityProperties>(
            $"{Select} FROM {AxisIdentitiesTable.Table} " +
            $"WHERE {AxisIdentitiesTable.CountryId} = @countryId AND {AxisIdentitiesTable.Document} = @document",
            p =>
            {
                p.AddWithValue("countryId", countryId.ToString());
                p.AddWithValue("document", document);
            },
            AxisIdentityDbEntity.FromReader,
            "AXIS_IDENTITY_NOT_FOUND");

    public Task<AxisResult<IAxisIdentityEntityProperties>> GetByCellphoneIdAsync(CellphoneId cellphoneId)
        => GetAsync<IAxisIdentityEntityProperties>(
            $"{SelectAliased} FROM {AxisIdentitiesTable.Table} I " +
            $"INNER JOIN {AxisIdentityCellphonesTable.Table} IC " +
            $"ON I.{AxisIdentitiesTable.AxisIdentityId} = IC.{AxisIdentityCellphonesTable.AxisIdentityId} " +
            $"WHERE IC.{AxisIdentityCellphonesTable.CellphoneId} = @cellphoneId",
            p => p.AddWithValue("cellphoneId", cellphoneId.ToString()),
            AxisIdentityDbEntity.FromReader,
            "AXIS_IDENTITY_NOT_FOUND");

    public Task<AxisResult<IAxisIdentityEntityProperties>> GetByEmailIdAsync(EmailId emailId)
        => GetAsync<IAxisIdentityEntityProperties>(
            $"{SelectAliased} FROM {AxisIdentitiesTable.Table} I " +
            $"INNER JOIN {AxisIdentityEmailsTable.Table} IE " +
            $"ON I.{AxisIdentitiesTable.AxisIdentityId} = IE.{AxisIdentityEmailsTable.AxisIdentityId} " +
            $"WHERE IE.{AxisIdentityEmailsTable.EmailId} = @emailId",
            p => p.AddWithValue("emailId", emailId.ToString()),
            AxisIdentityDbEntity.FromReader,
            "AXIS_IDENTITY_NOT_FOUND");

    public Task<AxisResult> CreateAsync(IAxisIdentityEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {AxisIdentitiesTable.Table} (" +
            $"{AxisIdentitiesTable.AxisIdentityId}, " +
            $"{AxisIdentitiesTable.IsIndividual}, " +
            $"{AxisIdentitiesTable.Document}, " +
            $"{AxisIdentitiesTable.CountryId}, " +
            $"{AxisIdentitiesTable.DisplayName}, " +
            $"{AxisIdentitiesTable.DefaultLanguage}, " +
            $"{AxisIdentitiesTable.SecurityLevel}) " +
            "VALUES (@id, @isIndividual, @document, @countryId, @displayName, @defaultLanguage, @securityLevel)",
            p =>
            {
                p.AddWithValue("id", properties.AxisIdentityId.ToString());
                p.AddWithValue("isIndividual", properties.IsIndividual);
                p.AddWithValue("document", properties.Document);
                p.AddWithValue("countryId", properties.CountryId.ToString());
                p.AddWithValue("displayName", properties.DisplayName);
                p.AddWithValue("defaultLanguage", properties.DefaultLanguage);
                p.AddWithValue("securityLevel", properties.SecurityLevel.ToString());
            },
            duplicateKeyCode: "DOCUMENT_ALREADY_REGISTERED");

    public Task<AxisResult> AddCellphoneAsync(AxisIdentityId axisIdentityId, CellphoneId cellphoneId)
        => ExecuteAsync(
            $"INSERT INTO {AxisIdentityCellphonesTable.Table} (" +
            $"{AxisIdentityCellphonesTable.AxisIdentityId}, {AxisIdentityCellphonesTable.CellphoneId}) " +
            "VALUES (@identityId, @cellphoneId)",
            p =>
            {
                p.AddWithValue("identityId", axisIdentityId.ToString());
                p.AddWithValue("cellphoneId", cellphoneId.ToString());
            },
            duplicateKeyCode: "CELLPHONE_ALREADY_REGISTERED");

    public Task<AxisResult> AddEmailAsync(AxisIdentityId axisIdentityId, EmailId emailId)
        => ExecuteAsync(
            $"INSERT INTO {AxisIdentityEmailsTable.Table} (" +
            $"{AxisIdentityEmailsTable.AxisIdentityId}, {AxisIdentityEmailsTable.EmailId}) " +
            "VALUES (@identityId, @emailId)",
            p =>
            {
                p.AddWithValue("identityId", axisIdentityId.ToString());
                p.AddWithValue("emailId", emailId.ToString());
            },
            duplicateKeyCode: "EMAIL_ALREADY_REGISTERED");
}
