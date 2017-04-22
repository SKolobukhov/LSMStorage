using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using LSMStorage.Core;
using NUnit.Framework;
using File = LSMStorage.Core.File;

namespace LSMStorage.Tests
{
    [TestFixture]
    public class MemTableTests
    {
        private static readonly string Directory = "C:\\Lsm_Tests";

        private string filePath;
        private MemTable memTable;
        private OpLogManager opLogManager;
        private OperationSerializer serializer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            System.IO.Directory.CreateDirectory(Directory);
            serializer = new OperationSerializer();
            serializer.AddEmptySerializer<GetOperation>();
            serializer.AddSerializer<PutOperationSerializer>();
            serializer.AddSerializer<RemoveOperationSerializer>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            try
            {
                if (System.IO.Directory.Exists(Directory))
                {
                    foreach (var file in System.IO.Directory.EnumerateFiles(Directory))
                    {
                        System.IO.File.Delete(file);
                    }
                    System.IO.Directory.Delete(Directory, true);
                }
            }
            catch (Exception) { }
        }

        [SetUp]
        public void SetUp()
        {
            filePath = Path.Combine(Directory, Guid.NewGuid().ToString());
            opLogManager = new OpLogManager(new File(filePath), serializer);
            memTable = new MemTable(opLogManager, null, ulong.MaxValue);
        }

        [TearDown]
        public void TearDown()
        {
            opLogManager.Dispose();
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [Test]
        public void Should_add_items()
        {
            var item1 = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1);
            var item2 = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 2);

            memTable.Apply(item1.ToOperation());
            memTable.Apply(item2.ToOperation());

            var itemFromTable1 = memTable.Get(item1.Key);
            var itemFromTable2 = memTable.Get(item2.Key);

            itemFromTable1.Should().Be(item1.Value);
            itemFromTable2.Should().Be(item2.Value);
        }

        [Test]
        public void Should_overwrite_item_with_same_key()
        {
            var key = Guid.NewGuid().ToString();

            var item = Item.CreateItem(key, Guid.NewGuid().ToString(), 1);
            var tombstone = Item.CreateTombStone(item, 2);

            memTable.Apply(item.ToOperation());
            memTable.Apply(tombstone.ToOperation());

            Thread.Sleep(1000);

            memTable.Get(key, false).Should().BeNull();
            memTable.Get(key, true).Should().Be(item.Value);

        }
    }
}
