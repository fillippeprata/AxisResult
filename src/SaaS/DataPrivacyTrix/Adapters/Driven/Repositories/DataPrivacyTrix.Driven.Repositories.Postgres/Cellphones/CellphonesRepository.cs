using Axis;
using AxisMediator.Contracts;
using AxisRepository.Postgres;
using DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones.Scripts;
using DataPrivacyTrix.Ports.Cellphones;
using DataPrivacyTrix.SharedKernel;
using DataPrivacyTrix.SharedKernel.Cellphones;
using Microsoft.Extensions.DependencyInjection;
using CountryId = Axis.Localization.CountryId;

namespace DataPrivacyTrix.Driven.Repositories.Postgres.Cellphones;

internal class CellphonesRepository(
    IAxisMediator mediator,
    IAxisLogger<PostgresRepositoryBase> logger,
    [FromKeyedServices(ApplicationConfig.AppKey)] IPostgresUnitOfWork uow
) : PostgresRepositoryBase(mediator, logger, uow), ICellphonesReaderPort, ICellphonesWritePort
{
    private const string Select = $"SELECT {CellphonesTable.CellphoneId}, {CellphonesTable.CountryId}, {CellphonesTable.CellphoneNumber}";

    public Task<AxisResult<ICellphoneEntityProperties>> GetByIdAsync(CellphoneId cellphoneId)
        => GetAsync<ICellphoneEntityProperties>(
            $"{Select} FROM {CellphonesTable.Table} WHERE {CellphonesTable.CellphoneId} = @id",
            p => p.AddWithValue("id", cellphoneId.ToString()),
            CellphoneDbEntity.FromReader,
            "CELLPHONE_NOT_FOUND");

    public Task<AxisResult<ICellphoneEntityProperties>> GetByCellphoneNumberAsync(CountryId countryId, string cellphoneNumber)
        => GetAsync<ICellphoneEntityProperties>(
            $"{Select} FROM {CellphonesTable.Table} " +
            $"WHERE {CellphonesTable.CountryId} = @countryId AND {CellphonesTable.CellphoneNumber} = @number",
            p =>
            {
                p.AddWithValue("countryId", countryId.ToString());
                p.AddWithValue("number", cellphoneNumber);
            },
            CellphoneDbEntity.FromReader,
            "CELLPHONE_NOT_FOUND");

    public Task<AxisResult> CreateAsync(ICellphoneEntityProperties properties)
        => ExecuteAsync(
            $"INSERT INTO {CellphonesTable.Table} ({CellphonesTable.CellphoneId}, {CellphonesTable.CountryId}, {CellphonesTable.CellphoneNumber}) " +
            "VALUES (@id, @countryId, @number)",
            p =>
            {
                p.AddWithValue("id", properties.CellphoneId.ToString());
                p.AddWithValue("countryId", properties.CountryId.ToString());
                p.AddWithValue("number", properties.CellphoneNumber);
            },
            duplicateKeyCode: "CELLPHONE_ALREADY_EXISTS");
}
