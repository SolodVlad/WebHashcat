using DLL.Repository;
using DLLTest.Repository.Factory;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DLLTest.Repository
{
    [TestFixture]
    internal class CurrencyRepositoryTest
    {
        private string _code;
        private string _name;
        private double _course;

        private IRepository<Currency> _repository;

        [SetUp]
        public void SetUp()
        {
            InitialiseParameters();
            _repository = RepositoryFactory.Instance<CurrencyRepository>();
        }

        private void InitialiseParameters()
        {
            _code = "UAH";
            _name = "Hryvnia";
            _course = 37;
        }

        private async Task<string> CreateAsync()
        {
            //Arrange
            var currency = new Currency()
            {
                Code = _code,
                Name = _name,
                Course = _course
            };

            //Act
            await _repository.CreateAsync(currency);
            await ContextSingleton.Context.SaveChangesAsync();
            var currencies = await _repository.GetAllAsync();

            //Assert
            Assert.IsTrue(currencies.Any(), "Create currency is failed");
            return currency.Code;
        }

        private async Task UpdateAsync(string code)
        {
            //Arrange
            var currency = (await _repository.FindByConditionAsync(currency => currency.Code == code)).First();
            currency.Code = "USD";
            currency.Name = "Dollar";
            currency.Course = 1;

            //Act
            await ContextSingleton.Context.SaveChangesAsync();
            var updateCurrency = (await _repository.FindByConditionAsync(currency => currency.Code == code)).First();

            //Assert
            Assert.AreNotEqual(updateCurrency.Code, _code, "Update code in currency is failed");
            Assert.AreNotEqual(updateCurrency.Name, _name, "Update name in currency is failed");
            Assert.AreNotEqual(updateCurrency.Course, _course, "Update course in currency is failed");
        }

        private async Task GetAllAsync()
        {
            //Act
            var currencies = await _repository.GetAllAsync();

            //Assert
            Assert.IsNotNull(currencies, "Get all currencies throw null reference exception");
            Assert.IsTrue(currencies.Any(), "Get all currencies is failed");
        }

        private async Task GetByCodeAsync(string code)
        {
            //Act
            var currency = (await _repository.FindByConditionAsync(currency => currency.Code == code)).First();

            //Assert
            Assert.IsNotNull(currency, "Currency is not found");

            Assert.AreEqual(currency.Code, _code, "Code no match");
            Assert.AreEqual(currency.Name, _name, "Name no match");
            Assert.AreEqual(currency.Course, _course, "Course no match");
        }

        private async Task DeleteAsync(string code)
        {
            //Arrange
            Currency currency = (await _repository.FindByConditionAsync(currency => currency.Code == code)).First();

            //Act
            await _repository.RemoveAsync(currency);
            await ContextSingleton.Context.SaveChangesAsync();

            //Assert
            currency = (await _repository.FindByConditionAsync(currency => currency.Code == code)).FirstOrDefault();
            Assert.IsNull(currency, "Delete currency is failed");
        }

        [Test]
        public async Task CurrencyCrud()
        {
            var id = await CreateAsync();
            await GetAllAsync();
            await GetByCodeAsync(id);
            //await UpdateAsync(id);
            await DeleteAsync(id);
        }
    }
}