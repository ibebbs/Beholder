using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Examiner.Tests.Face.Data.Provider
{
    [TestFixture]
    public class Should
    {
        private const string ConnectionString = "BlahBlahConnectionString";
        private const string ContainerName = "BlahBlahContainerName";
        private static readonly IEnumerable<string> CategoriesToIgnore = new[] { "none" };

        private (Persistence.IProvider persistenceProvider, Examiner.Face.Data.Provider) CreateSubject(Examiner.Face.Configuration configuration = null)
        {
            configuration ??= new Examiner.Face.Configuration
            {
                ConnectionString = ConnectionString,
                CategorizedFacesContainerName = ContainerName,
                CategoriesToIgnore = CategoriesToIgnore
            };

            var options = A.Fake<IOptions<Examiner.Face.Configuration>>(builder => builder.ConfigureFake(fake => A.CallTo(() => fake.Value).Returns(configuration)));
            var logger = A.Fake<ILogger<Examiner.Face.Data.Provider>>();
            var persistenceProvider = A.Fake<Persistence.IProvider>();
            var subject = new Examiner.Face.Data.Provider(persistenceProvider, options, logger);

            return (persistenceProvider, subject);
        }

        [Test]
        public async Task GetLatestChangeFromConfiguredContainer()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            var latestChange = await subject.GetDateOfLastChangeAsync(CancellationToken.None);

            A.CallTo(() => persistenceProvider.GetDateOfLastChangeAsync(ConnectionString, ContainerName, A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task ReturnAnEqualNumberOfItemsForEachName()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            var allItems = new[]
            {
                "Bob/e71d895b-b944-4642-b04d-2334de1ceb01.png",
                "Bob/b262626d-150f-438b-af20-1e105f987d3d.png",
                "Dave/ef5f09bf-b890-4859-94a4-4a4032cf9545.png",
                "Dave/1ff40435-aafd-44d2-aea1-178699a0e5ec.png",
                "Dave/053483ef-9b69-4de1-b9f7-4d65a712e362.png",
                "Colin/66af9bda-8e65-44b2-bc78-f5da0323cfdb.png",
                "Colin/0c14ec89-1ce4-4821-a5ce-333cc41bd4f7.png",
                "Colin/9914cf7f-c5cd-4688-850a-2c14fd7dd6bb.png",
                "Colin/479534b6-e679-4137-a202-93e8c60cf3ab.png"
            };

            A.CallTo(() => persistenceProvider.GetItems(ConnectionString, ContainerName, A<CancellationToken>.Ignored))
                .Returns(allItems.ToAsyncEnumerable());
            A.CallTo(() => persistenceProvider.GetBlobItemAsync(ConnectionString, ContainerName, A<string>.Ignored, A<Func<Stream, Task<Examiner.Face.Data.Item>>>.Ignored, A<CancellationToken>.Ignored))
                .ReturnsLazily(call => call.GetArgument<Func<Stream, Task<Examiner.Face.Data.Item>>>(3).Invoke(new MemoryStream()));

            var items = await subject.GetBalancedItems(CancellationToken.None).ToListAsync();

            var result = items
                .GroupBy(item => item.Name)
                .Select(group => (Name: group.Key, Count: group.Count()))
                .Aggregate(
                    (Count: (int?)null, AllEqual: true),
                    (tuple, item) => tuple.Count.HasValue
                        ? (tuple.Count, AllEqual: tuple.AllEqual && tuple.Count == item.Count)
                        : (item.Count, AllEqual: true));

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.AllEqual, Is.True);
        }


        [Test]
        public async Task NotIncludeIgnoredCategories()
        {
            (var persistenceProvider, var subject) = CreateSubject();

            var allItems = new[]
            {
                "bob/e71d895b-b944-4642-b04d-2334de1ceb01.png",
                "bob/b262626d-150f-438b-af20-1e105f987d3d.png",
                "dave/ef5f09bf-b890-4859-94a4-4a4032cf9545.png",
                "dave/1ff40435-aafd-44d2-aea1-178699a0e5ec.png",
                "dave/053483ef-9b69-4de1-b9f7-4d65a712e362.png",
                "none/66af9bda-8e65-44b2-bc78-f5da0323cfdb.png"
            };

            A.CallTo(() => persistenceProvider.GetItems(ConnectionString, ContainerName, A<CancellationToken>.Ignored))
                .Returns(allItems.ToAsyncEnumerable());
            A.CallTo(() => persistenceProvider.GetBlobItemAsync(ConnectionString, ContainerName, A<string>.Ignored, A<Func<Stream, Task<Examiner.Face.Data.Item>>>.Ignored, A<CancellationToken>.Ignored))
                .ReturnsLazily(call => call.GetArgument<Func<Stream, Task<Examiner.Face.Data.Item>>>(3).Invoke(new MemoryStream()));

            var items = await subject.GetBalancedItems(CancellationToken.None).ToListAsync();

            var result = items
                .GroupBy(item => item.Name)
                .Select(group => (Name: group.Key, Count: group.Count()))
                .Aggregate(
                    (Count: (int?)null, AllEqual: true),
                    (tuple, item) => tuple.Count.HasValue
                        ? (tuple.Count, AllEqual: tuple.AllEqual && tuple.Count == item.Count)
                        : (item.Count, AllEqual: true));

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.AllEqual, Is.True);
        }
    }
}
