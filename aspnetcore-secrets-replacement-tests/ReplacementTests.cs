using AspnetCoreSecretsReplacement.Core.Services;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace AspnetCoreSecretsReplacement.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Tests that the replacement of a key within a string results in a new string with the value in its place.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestDefaultSecretsReplacement()
        {
            var key1 = "TestKey1";
            var secret1 = "TestSecret1";
            var key2 = "TestKey2";
            var secret2 = "TestSecret2";

            //Original string uses the "{{key}}" syntax to identify the key. 
            //The resulting string will replace both the double-curly braces and the key with the value from the key vault
            var originalString = "This is a string that contains '{{TestKey1}}' and '{{TestKey2}}'.";
            var convertedString = "This is a string that contains 'TestSecret1' and 'TestSecret2'.";


            var secretsRetrievalServiceMock = new Mock<ISecretsRetrievalService>();
            secretsRetrievalServiceMock.Setup(x => x.GetAsync(key1)).Returns(Task.FromResult(secret1));
            secretsRetrievalServiceMock.Setup(x => x.GetAsync(key2)).Returns(Task.FromResult(secret2));

            DefaultSecretsReplacementService service = new DefaultSecretsReplacementService(
                secretsRetrievalServiceMock.Object);

            var result1 = await service.Replace(originalString);
            Assert.AreEqual(result1, convertedString);

            //Tests multiple times to make sure it is still present because the internals of the service uses a local "cache". 
            var result2 = await service.Replace(originalString);
            Assert.AreEqual(result2, convertedString);
        }
    }
}