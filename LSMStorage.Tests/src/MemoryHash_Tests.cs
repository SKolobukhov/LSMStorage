using System;
using System.IO;
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
        private OperationSerializer serializer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            System.IO.Directory.CreateDirectory(Directory);
            serializer = new OperationSerializer();
            serializer.AddSerializer<AddOperationSerializer>();
            serializer.AddSerializer<RemoveOperationSerializer>();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.Delete(Directory, true);
            }
        }

        [SetUp]
        public void SetUp()
        {
            filePath = Path.Combine(Directory, Guid.NewGuid().ToString());
            memTable = new MemTable(new OpLogManager(new File(filePath), serializer));
        }

        [TearDown]
        public void TearDown()
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [Test]
        public void Should_add_items()
        {
            var item1 = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var item2 = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            memTable.Apply(item1.ToOperation());
            memTable.Apply(item2.ToOperation());

            var itemFromTable1 = memTable.Get(item1.Key);
            var itemFromTable2 = memTable.Get(item2.Key);

            itemFromTable1.Should().Be(item1);
            itemFromTable2.Should().Be(item2);
        }

        [Test]
        public void Should_overwrite_item_with_same_key()
        {
            var key = Guid.NewGuid().ToString();

            var item = Item.CreateItem(key, Guid.NewGuid().ToString());
            var tombstone = Item.CreateTombStone(key);

            memTable.Apply(item.ToOperation());
            memTable.Apply(tombstone.ToOperation());

            var itemFromTable = memTable.Get(key);

            itemFromTable.Should().Be(tombstone);
        }
    }
}
