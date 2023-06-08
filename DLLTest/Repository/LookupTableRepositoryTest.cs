using DLL.Repository;
using DLLTest.Repository.Factory;
using Domain.Models;
using NUnit.Framework;

namespace DLLTest.Repository
{
    [TestFixture]
    internal class LookupTableRepositoryTest
    {
        private string _value;
        private string _md5;
        private string _sha1;
        private string _sha256;
        private string _sha384;
        private string _sha512;

        private IRepository<DataLookupTable> _repository;

        [SetUp]
        public void SetUp()
        {
            InitialiseParameters();
            _repository = RepositoryFactory.Instance<LookupTableRepository>();
        }

        private void InitialiseParameters()
        {
            _value = "Hello World";
            _md5 = "b10a8db164e0754105b7a99be72e3fe5";
            _sha1 = "0a4d55a8d778e5022fab701977c5d840bbc486d0";
            _sha256 = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";
            _sha384 = "99514329186b2f6ae4a1329e7ee6c610a729636335174ac6b740f9028396fcc803d0e93863a7c3d90f86beee782f4f3f";
            _sha512 = "2c74fd17edafd80e8447b0d46741ee243b7eb74dd2149a0ab1b9246fb30382f27e853d8585719e0e67cbda0daa8f51671064615d645ae27acb15bfb1447f459b";
        }

        private async Task<string> CreateAsync()
        {
            //Arrange
            var dataLookupTable = new DataLookupTable()
            {
                Value = _value,
                MD5 = _md5,
                SHA1 = _sha1,
                SHA256 = _sha256,
                SHA384 = _sha384,
                SHA512 = _sha512,
            };

            //Act
            await _repository.CreateAsync(dataLookupTable);
            await ContextSingleton.Context.SaveChangesAsync();
            var lookupTable = await _repository.GetAllAsync();

            //Assert
            Assert.IsTrue(lookupTable.Any(), "Create data for lookup table is failed");
            return dataLookupTable.SHA512;
        }

        private async Task GetAllAsync()
        {
            //Act
            var lookupTable = await _repository.GetAllAsync();

            //Assert
            Assert.IsNotNull(lookupTable, "Get lookup table throw null reference exception");
            Assert.IsTrue(lookupTable.Any(), "Get lookup table is failed");
        }

        private async Task GetBySHA512Async(string SHA512)
        {
            //Act
            var dataLookupTable = (await _repository.FindByConditionAsync(dataLookupTable => dataLookupTable.SHA512 == SHA512)).First();

            //Assert
            Assert.IsNotNull(dataLookupTable, "Currency is not found");

            Assert.AreEqual(dataLookupTable.Value, _value, "Value no match");
            Assert.AreEqual(dataLookupTable.MD5, _md5, "MD5 no match");
            Assert.AreEqual(dataLookupTable.SHA1, _sha1, "SHA1 no match");
            Assert.AreEqual(dataLookupTable.SHA256, _sha256, "SHA256 no match");
            Assert.AreEqual(dataLookupTable.SHA384, _sha384, "SHA384 no match");
            Assert.AreEqual(dataLookupTable.SHA512, _sha512, "SHA512 no match");
        }

        private async Task DeleteAsync(string SHA512)
        {
            //Arrange
            var dataLookupTable = (await _repository.FindByConditionAsync(lookupTable => lookupTable.SHA512 == SHA512)).First();

            //Act
            _repository.RemoveAsync(dataLookupTable);
            await ContextSingleton.Context.SaveChangesAsync();

            //Assert
            dataLookupTable = (await _repository.FindByConditionAsync(lookupTable => lookupTable.SHA512 == SHA512)).FirstOrDefault();
            Assert.IsNull(dataLookupTable, "Delete data for lookup table is failed");
        }

        [Test]
        public async Task LookupTableCrud()
        {
            var SHA512 = await CreateAsync();
            await GetAllAsync();
            await GetBySHA512Async(SHA512);
            await DeleteAsync(SHA512);
        }
    }
}