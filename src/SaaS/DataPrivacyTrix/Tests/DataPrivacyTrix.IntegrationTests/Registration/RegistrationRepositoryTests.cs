using DataPrivacyTrix.Contracts.Cellphones.v1;
using DataPrivacyTrix.Contracts.Cellphones.v1.AddCellphone;
using DataPrivacyTrix.Contracts.Emails.v1;
using DataPrivacyTrix.Contracts.Emails.v1.AddEmail;
using DataPrivacyTrix.Contracts.Registration.v1;
using DataPrivacyTrix.Contracts.Registration.v1.AddCellphoneToAxisIdentity;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Registration.v1.GetAxisIdentityById;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByCellphone;
using DataPrivacyTrix.Contracts.Registration.v1.RegisterAxisIdentityByEmail;
using DataPrivacyTrix.Contracts.Registration.v1.SharedData;
using DataPrivacyTrix.IntegrationTests.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace DataPrivacyTrix.IntegrationTests.Registration;

public class RegistrationRepositoryTests(PostgresFixture fixture) : DatabaseTestBase(fixture)
{
    private const string BrCountryId = "BR";
    private const string ValidCpfA = "39053344705";
    private const string ValidCpfB = "11144477735";
    private const string ValidCpfC = "52998224725";
    private const string ValidCpfD = "71428793860";
    private const string ValidCpfE = "15350946056";

    private static IRegistrationMediator Registration(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IRegistrationMediator>();

    private static ICellphonesMediator Cellphones(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<ICellphonesMediator>();

    private static IEmailsMediator Emails(IServiceScope scope)
        => scope.ServiceProvider.GetRequiredService<IEmailsMediator>();

    [Fact]
    public async Task RegisterByCellphoneHappyPathAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        const string cellphoneNumber = "11987654301";
        string cellphoneId;

        using (var scope = serviceProvider.CreateScope())
        {
            var addResponse = await Cellphones(scope).AddAsync(new AddCellphoneCommand
            {
                CountryId = BrCountryId,
                CellphoneNumber = cellphoneNumber
            });
            Assert.True(addResponse.IsSuccess, $"Failed: {string.Join("; ", addResponse.Errors.Select(e => e.Code))}");
            cellphoneId = addResponse.Value.CellphoneId;
        }

        string axisIdentityId;
        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Registration(scope).RegisterByCellphoneAsync(new RegisterAxisIdentityByCellphoneCommand
            {
                Data = ValidData(ValidCpfA, "User A"),
                CellphoneId = cellphoneId
            });
            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            axisIdentityId = response.Value.AxisIdentityId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Registration(scope).GetByCellphoneAsync(new GetAxisIdentityByCellphoneQuery
            {
                CountryId = BrCountryId,
                CellphoneNumber = cellphoneNumber
            });
            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            Assert.Equal(axisIdentityId, response.Value.AxisIdentityId);
            Assert.Equal("User A", response.Value.DisplayName);
        }
    }

    [Fact]
    public async Task RegisterByEmailHappyPathAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var emailAddress = $"user-b-{Guid.NewGuid():N}@example.com";
        string emailId;

        using (var scope = serviceProvider.CreateScope())
        {
            var addResponse = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailAddress });
            Assert.True(addResponse.IsSuccess, $"Failed: {string.Join("; ", addResponse.Errors.Select(e => e.Code))}");
            emailId = addResponse.Value.EmailId;
        }

        string axisIdentityId;
        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
            {
                Data = ValidData(ValidCpfB, "User B"),
                EmailId = emailId
            });
            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            axisIdentityId = response.Value.AxisIdentityId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Registration(scope).GetByEmailAsync(new GetAxisIdentityByEmailQuery { EmailAddress = emailAddress });
            Assert.True(response.IsSuccess, $"Failed: {string.Join("; ", response.Errors.Select(e => e.Code))}");
            Assert.Equal(axisIdentityId, response.Value.AxisIdentityId);
            Assert.Equal("User B", response.Value.DisplayName);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var response = await Registration(scope).GetByIdAsync(new GetAxisIdentityByIdQuery { AxisIdentityId = axisIdentityId });
            Assert.True(response.IsSuccess);
            Assert.Equal("User B", response.Value.DisplayName);
            Assert.Equal(ValidCpfB, response.Value.Document);
        }
    }

    [Fact]
    public async Task AddCellphoneToExistingIdentityAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var emailAddress = $"user-c-{Guid.NewGuid():N}@example.com";
        var firstPhone = "11987654302";
        var secondPhone = "11987654303";
        string axisIdentityId;
        string secondCellphoneId;

        using (var scope = serviceProvider.CreateScope())
        {
            var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailAddress });
            Assert.True(emailResp.IsSuccess);
            var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
            {
                Data = ValidData(ValidCpfC, "User C"),
                EmailId = emailResp.Value.EmailId
            });
            Assert.True(regResp.IsSuccess);
            axisIdentityId = regResp.Value.AxisIdentityId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var phoneResp = await Cellphones(scope).AddAsync(new AddCellphoneCommand
            {
                CountryId = BrCountryId,
                CellphoneNumber = firstPhone
            });
            Assert.True(phoneResp.IsSuccess);

            var addResp = await Registration(scope).AddCellphoneAsync(new AddCellphoneToAxisIdentityCommand
            {
                AxisIdentityId = axisIdentityId,
                CellphoneId = phoneResp.Value.CellphoneId
            });
            Assert.True(addResp.IsSuccess, $"Failed: {string.Join("; ", addResp.Errors.Select(e => e.Code))}");
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var phoneResp = await Cellphones(scope).AddAsync(new AddCellphoneCommand
            {
                CountryId = BrCountryId,
                CellphoneNumber = secondPhone
            });
            Assert.True(phoneResp.IsSuccess);
            secondCellphoneId = phoneResp.Value.CellphoneId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var addResp = await Registration(scope).AddCellphoneAsync(new AddCellphoneToAxisIdentityCommand
            {
                AxisIdentityId = axisIdentityId,
                CellphoneId = secondCellphoneId
            });
            Assert.True(addResp.IsSuccess);
        }
    }

    [Fact]
    public async Task CellphoneCannotBeLinkedToTwoIdentitiesAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var emailA = $"user-d-a-{Guid.NewGuid():N}@example.com";
        var emailB = $"user-d-b-{Guid.NewGuid():N}@example.com";
        var cellphoneNumber = "11987654304";
        string sharedCellphoneId;
        string identityB;

        using (var scope = serviceProvider.CreateScope())
        {
            var phoneResp = await Cellphones(scope).AddAsync(new AddCellphoneCommand
            {
                CountryId = BrCountryId,
                CellphoneNumber = cellphoneNumber
            });
            Assert.True(phoneResp.IsSuccess);
            sharedCellphoneId = phoneResp.Value.CellphoneId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailA });
            Assert.True(emailResp.IsSuccess);
            var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
            {
                Data = ValidData(ValidCpfD, "User D-A"),
                EmailId = emailResp.Value.EmailId
            });
            Assert.True(regResp.IsSuccess);
            var addResp = await Registration(scope).AddCellphoneAsync(new AddCellphoneToAxisIdentityCommand
            {
                AxisIdentityId = regResp.Value.AxisIdentityId,
                CellphoneId = sharedCellphoneId
            });
            Assert.True(addResp.IsSuccess);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailB });
            Assert.True(emailResp.IsSuccess);
            var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
            {
                Data = ValidData(ValidCpfE, "User D-B"),
                EmailId = emailResp.Value.EmailId
            });
            Assert.True(regResp.IsSuccess);
            identityB = regResp.Value.AxisIdentityId;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var addResp = await Registration(scope).AddCellphoneAsync(new AddCellphoneToAxisIdentityCommand
            {
                AxisIdentityId = identityB,
                CellphoneId = sharedCellphoneId
            });
            Assert.True(addResp.IsFailure);
            Assert.Contains(addResp.Errors, x => x.Code == "CELLPHONE_ALREADY_REGISTERED");
        }
    }

    [Fact]
    public async Task RegisterShouldFailForInvalidCpfAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var emailAddress = $"user-invalid-{Guid.NewGuid():N}@example.com";

        using var scope = serviceProvider.CreateScope();
        var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailAddress });
        Assert.True(emailResp.IsSuccess);

        var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
        {
            Data = ValidData("12345678900", "Invalid User"),
            EmailId = emailResp.Value.EmailId
        });

        Assert.True(regResp.IsFailure);
        Assert.Contains(regResp.Errors, x => x.Code == "DOCUMENT_INVALID");
    }

    [Fact]
    public async Task RegisterShouldFailWhenDocumentAlreadyRegisteredAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var emailOne = $"dup-a-{Guid.NewGuid():N}@example.com";
        var emailTwo = $"dup-b-{Guid.NewGuid():N}@example.com";
        const string sharedCpf = "87748248800";

        using (var scope = serviceProvider.CreateScope())
        {
            var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailOne });
            Assert.True(emailResp.IsSuccess);
            var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
            {
                Data = ValidData(sharedCpf, "First"),
                EmailId = emailResp.Value.EmailId
            });
            Assert.True(regResp.IsSuccess);
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailTwo });
            Assert.True(emailResp.IsSuccess);
            var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
            {
                Data = ValidData(sharedCpf, "Second"),
                EmailId = emailResp.Value.EmailId
            });
            Assert.True(regResp.IsFailure);
            Assert.Contains(regResp.Errors, x => x.Code == "DOCUMENT_ALREADY_REGISTERED");
        }
    }

    [Fact]
    public async Task RegisterShouldFailForUnsupportedCountryAsync()
    {
        var serviceProvider = DependencyInjection.ServiceProviderWithPostgres(Fixture.ConnectionString);
        var emailAddress = $"us-{Guid.NewGuid():N}@example.com";

        using var scope = serviceProvider.CreateScope();
        var emailResp = await Emails(scope).AddAsync(new AddEmailCommand { EmailAddress = emailAddress });
        Assert.True(emailResp.IsSuccess);

        var data = ValidData(ValidCpfA, "US User") with { CountryId = "US" };
        var regResp = await Registration(scope).RegisterByEmailAsync(new RegisterAxisIdentityByEmailCommand
        {
            Data = data,
            EmailId = emailResp.Value.EmailId
        });

        Assert.True(regResp.IsFailure);
        Assert.Contains(regResp.Errors, x => x.Code == "COUNTRY_ID_DOCUMENT_NOT_IMPLEMENTED");
    }

    private static RegisterAxisIdentityData ValidData(string document, string displayName) => new()
    {
        IsIndividual = true,
        Document = document,
        CountryId = BrCountryId,
        DisplayName = displayName,
        DefaultLanguage = "pt-BR",
        SecurityLevel = "Normal"
    };
}
