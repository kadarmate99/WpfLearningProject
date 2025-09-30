using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using WiredBrainCoffee.CustomersApp.Configuration;
using WiredBrainCoffee.CustomersApp.Model;
using WiredBrainCoffee.CustomersApp.Repository;

namespace Wired_B_rainCoffee.CustomersApp.Repository;

public class CustomerJsonRepositoryTest : IDisposable
{
    private string _testFilePath;
    private RepoConfig _config;
    private JsonSerializerOptions _jsonOptions;
    private CustomerJsonRepository _repository;

    public CustomerJsonRepositoryTest()
    {
        // Create test file for each test
        _testFilePath = Path.GetTempFileName();

        _config = new RepoConfig
        {
            CustomersFilePath = _testFilePath,
        };

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        _repository = new CustomerJsonRepository(Options.Create(_config), _jsonOptions);
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }

    [Fact]
    public void GetAll_WhenFileDoesNotExist_ReturnsEmptyCollection()
    {
        // Arrange
        File.Delete(_testFilePath);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    [Theory]
    public void GetAll_WhenFileIsNullOrWhiteSpace_ReturnsEmptyCollection(string? fileContent)
    {
        // Arrange
        File.WriteAllText(_testFilePath, fileContent);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [InlineData("[ ]")] // empty json
    [Theory]
    public void GetAll_WhenFileContainsEmptyJson_ReturnsEmptyCollection(string? fileContent)
    {
        // Arrange
        File.WriteAllText(_testFilePath, fileContent);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_WhenJsonContainsExtraFields_ReturnsCustomers()
    {
        // Arrange
        var jsonWithExtraField = "[ {\"Id\": 1, \"FirstName\": \"Julia\", \"LastName2\": \"ExtraLastName\", \"LastName\": \"Developer\",  \"IsDeveloper\": true} ]";
        File.WriteAllText(_testFilePath, jsonWithExtraField);

        var expectedCustomers = new List<Customer>
        {
            new Customer{ Id = 1, FirstName = "Julia", LastName = "Developer", IsDeveloper = true},
        };
        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(expectedCustomers, result);
    }

    [Fact]
    public void GetAll_WhenValidFileExists_ReturnsCustomers()
    {
        // Arrange
        var expectedCustomers = new List<Customer>
        {
            new Customer{ Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true},
            new Customer{ Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false}
        };
        var json = JsonSerializer.Serialize(expectedCustomers, _jsonOptions);
        File.WriteAllText(_testFilePath, json);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(expectedCustomers, result);
    }

    [Theory]
    [InlineData("[ { invalid json } ]")] // totally broken
    [InlineData("[ {\"Id\": \"Julia\", \"FirstName\": \"Julia\", \"LastName\": \"Developer\", \"IsDeveloper\": true} ]")] // string for id
    [InlineData("[ {\"Id\": 2, \"FirstName\": 123, \"LastName\": \"Tester\", \"IsDeveloper\": false} ]")] // number instead of string
    [InlineData("[ {\"Id\": 3, \"FirstName\": \"Anna\", \"IsDeveloper\": true} ]")] // missing required prop
    [InlineData("[ {\"Id\": 4, \"FirstName\": \"John\", \"LastName\": \"Smith\", \"IsDeveloper\": true}, {\"Id\": \"Aaa\" } ]")] // mixed valid + invalid
    public void GetAll_WhenFileContainsInvalidJSON_ThrowsException(string invalidJson)
    {
        // Arrange
        File.WriteAllText(_testFilePath, invalidJson);

        // Act & Assert
        Assert.Throws<JsonException>(() => _repository.GetAll());
    }
}
