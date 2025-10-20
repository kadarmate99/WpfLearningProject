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

    #region public IEnumerable<Customer> GetAll()
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
    #endregion

    #region public IEnumerable<Customer> SaveAll(IEnumerable<Customer> customers)
    [Fact]
    public void SaveAll_WhenFileDoesNotExist_CreatesNewFile()
    {
        // Arrange
        File.Delete(_testFilePath);
        var emptyCustomerList = new List<Customer>();

        // Act
        _repository.SaveAll(emptyCustomerList);

        // Assert
        Assert.True(File.Exists(_testFilePath));
    }

    [Fact]
    public void SaveAll_WhenValidDataGiven_WritesCustomersToFile()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer{ Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true},
            new Customer{ Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false}
        };
        var expectedJson = JsonSerializer.Serialize(customers, _jsonOptions);

        // Act
        var returnedCustomers = _repository.SaveAll(customers);

        // Assert
        Assert.Equal(expectedJson, File.ReadAllText(_testFilePath));
        Assert.Equal(customers, returnedCustomers);
    }

    [Fact]
    public void SaveAll_WhenFileAlreadyHasData_OverwritesWithNewData()
    {
        // Arrange
        var oldCustomers = new List<Customer>
        {
            new Customer{ Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true},
            new Customer{ Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false}
        };
        var newCustomers = new List<Customer>
        {
            new Customer{ Id = 1, FirstName = "Julia", LastName = "Anna", IsDeveloper = false},
            new Customer{ Id = 2, FirstName = "Anna", LastName = "Developer", IsDeveloper = true}
        };
        var expectedJson = JsonSerializer.Serialize(newCustomers, _jsonOptions);
        _repository.SaveAll(oldCustomers); // make file exist with data in it

        // Act
        var returnedCustomers = _repository.SaveAll(newCustomers); // write new customers to the same file

        // Assert
        Assert.Equal(expectedJson, File.ReadAllText(_testFilePath));
        Assert.Equal(newCustomers, returnedCustomers);
    }
    #endregion

    #region public IEnumerable<Customer> Add(Customer customer)
    [Fact]
    public void Add_WhenFileDoesNotExist_AddsCustomerWithId1()
    {
        // Arrange
        File.Delete(_testFilePath);
        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        var expectedCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true }
        };

        // Act
        var returnedCustomers = _repository.Add(customer);
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(result, returnedCustomers);
        Assert.Equal(expectedCustomers, returnedCustomers);
    }

    [Fact]
    public void Add_WhenEmptyFileExists_AddsCustomerWithId1()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        var expectedCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true }
        };

        // Act
        var returnedCustomers = _repository.Add(customer);
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(result, returnedCustomers);
        Assert.Equal(expectedCustomers, returnedCustomers);
    }


    [Fact]
    public void Add_WhenFileAlreadyHasData_AddCustomerWithIncrementedId()
    {
        // Arrange
        var oldCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false }
        };
        var newCustomer = new Customer { FirstName = "Test", LastName = "Developer", IsDeveloper = true };

        var expectedCustomers = new List<Customer>(oldCustomers)
        {
            new Customer { Id = 3, FirstName = "Test", LastName = "Developer", IsDeveloper = true }
        };

        _repository.SaveAll(oldCustomers); // make file exist with data in it

        // Act
        var returnedCustomers = _repository.Add(newCustomer).ToList();
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(result, returnedCustomers);
        Assert.Equal(expectedCustomers, returnedCustomers);
        Assert.Equal("Test", returnedCustomers[2].FirstName); // test if new Customer is really the last item
        Assert.Equal(3, returnedCustomers[2].Id); // test if id was assigned as expected
    }

    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    [InlineData(-5)]
    public void Add_WhenNewCustomerHasPredefinedId_IdIsOverwritten(int predefinedId)
    {
        // Arrange
        var oldCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false }
        };
        var newCustomer = new Customer { Id = predefinedId, FirstName = "Test", LastName = "Developer", IsDeveloper = true };

        var expectedCustomers = new List<Customer>(oldCustomers)
        {
            new Customer { Id = 3, FirstName = "Test", LastName = "Developer", IsDeveloper = true }
        };

        _repository.SaveAll(oldCustomers); // make file exist with data in it

        // Act
        var returnedCustomers = _repository.Add(newCustomer).ToList();
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(result, returnedCustomers);
        Assert.Equal(expectedCustomers, returnedCustomers);
        Assert.Equal("Test", returnedCustomers[2].FirstName); // test if new Customer is really the last item
        Assert.Equal(3, returnedCustomers[2].Id); // test if id was assigned as expected
    }

    #endregion

    #region public IEnumerable<Customer> Delete(Customer customer)
    [Fact]
    public void Delete_WhenFileDoesNotExist_ThrowsException()
    {
        // Arrange
        File.Delete(_testFilePath);
        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Delete(customer));
        Assert.Equal($"File not found or empty: {_testFilePath}", ex.Message);
    }

    public void Delete_WhenFileIsEmpty_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Delete(customer));
        Assert.Equal($"File not found or empty: {_testFilePath}", ex.Message);
    }

    [Theory]
    [InlineData(0, "Zsombor", "Okos", true)]
    [InlineData(1, "Zsomb", "Okos", true)]
    [InlineData(1, "Zsombor", "", true)]
    [InlineData(1, "Zsombor", "Okos", false)]
    public void Delete_WhenCustomerDoesNotExist01_ThrowsException(int id, string firstName, string lastName, bool isDeveloper)
    {
        // Arrange
        var existingCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false }
        };
        _repository.SaveAll(existingCustomers);

        var customer = new Customer { Id = id, FirstName = firstName, LastName = lastName, IsDeveloper = isDeveloper };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Delete(customer));
        Assert.Equal($"The specified customer ({customer.FirstName} {customer.LastName}, ID {customer.Id}) does not exist.", ex.Message);
    }

    [Fact]
    public void Delete_WhenCustomerDoesNotExist02_ThrowsException()
    {
        // Arrange
        var existingCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false }
        };
        _repository.SaveAll(existingCustomers);

        var customer = new Customer { Id = 1, LastName = "Okos", IsDeveloper = true }; // property missing

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Delete(customer));
        Assert.Equal($"The specified customer ({customer.FirstName} {customer.LastName}, ID {customer.Id}) does not exist.", ex.Message);
    }

    [Fact]
    public void Delete_WhenCustomerExists_DeleteThatCustomer()
    {
        // Arrange
        var existingCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false },
            new Customer { Id = 3, FirstName = "Julia", LastName = "Developer", IsDeveloper = true },
            new Customer { Id = 4, FirstName = "Anna", LastName = "Tester", IsDeveloper = false }
        };
        _repository.SaveAll(existingCustomers);

        var customer = new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false };

        var expectedCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 3, FirstName = "Julia", LastName = "Developer", IsDeveloper = true },
            new Customer { Id = 4, FirstName = "Anna", LastName = "Tester", IsDeveloper = false }
        };

        // Act
        var returnedCustomers = _repository.Delete(customer);

        // Assert
        Assert.Equal(expectedCustomers, returnedCustomers);
    }
    #endregion

    #region public IEnumerable<Customer> Update(Customer customer)
    [Fact]
    public void Update_WhenFileDoesNotExist_ThrowsException()
    {
        // Arrange
        File.Delete(_testFilePath);
        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Update(customer));
        Assert.Equal($"File not found or empty: {_testFilePath}", ex.Message);
    }

    public void Update_WhenFileIsEmpty_ThrowsException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");
        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Update(customer));
        Assert.Equal($"File not found or empty: {_testFilePath}", ex.Message);
    }

    [Theory]
    [InlineData(0, "Zsombor", "Okos", true)]
    [InlineData(4, "Zsombor", "Okos", true)]
    public void Update_WhenCustomerDoesNotExist_ThrowsException(int id, string firstName, string lastName, bool isDeveloper)
    {
        // Arrange
        var existingCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false }
        };
        _repository.SaveAll(existingCustomers);

        var customer = new Customer { Id = id, FirstName = firstName, LastName = lastName, IsDeveloper = isDeveloper };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Update(customer));
        Assert.Equal($"The specified customer ({customer.FirstName} {customer.LastName}, ID {customer.Id}) does not exist.", ex.Message);
    }

    [Fact]
    public void Update_WhenCustomerDoesNotExist02_ThrowsException()
    {
        // Arrange
        var existingCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false }
        };
        _repository.SaveAll(existingCustomers);

        var customer = new Customer { FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _repository.Update(customer));
        Assert.Equal($"The specified customer ({customer.FirstName} {customer.LastName}, ID {customer.Id}) does not exist.", ex.Message);
    }

    [Fact]
    public void Update_WhenCustomerExists_UpdateThatCustomer()
    {
        // Arrange
        var existingCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Buta", IsDeveloper = false },
            new Customer { Id = 3, FirstName = "Julia", LastName = "Developer", IsDeveloper = true },
            new Customer { Id = 4, FirstName = "Anna", LastName = "Tester", IsDeveloper = false }
        };
        _repository.SaveAll(existingCustomers);

        var customer = new Customer { Id = 2, FirstName = "Máté", LastName = "Okos", IsDeveloper = true };

        var expectedCustomers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "Zsombor", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 2, FirstName = "Máté", LastName = "Okos", IsDeveloper = true },
            new Customer { Id = 3, FirstName = "Julia", LastName = "Developer", IsDeveloper = true },
            new Customer { Id = 4, FirstName = "Anna", LastName = "Tester", IsDeveloper = false }
        };

        // Act
        var returnedCustomers = _repository.Update(customer);

        // Assert
        Assert.Equal(expectedCustomers, returnedCustomers);
    }
    #endregion
}
