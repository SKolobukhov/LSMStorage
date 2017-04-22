using System;
using System.IO;
using FluentAssertions;
using LSMStorage.Core;
using NUnit.Framework;
using File = LSMStorage.Core.File;

namespace LSMStorage.Tests
{
    [TestFixture]
    public class OpLogApplier_Tests
    {
        private static readonly string Directory = "C:\\Lsm_Tests";

        private File file;
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
            var filePath = Path.Combine(Directory, Guid.NewGuid().ToString());
            file = new File(filePath);
        }

        [Test]
        public void Should_apply_add_operation_from_opLog()
        {
            var item1 = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1);
            var item2 = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 2);

            using (var opLogManager = new OpLogManager(file, serializer))
            {
                var memTable = new MemTable(opLogManager, null, ulong.MaxValue);
                memTable.Apply(item1.ToOperation());
                memTable.Apply(item2.ToOperation());
            }
            using (var opLogManager = new OpLogManager(file, serializer))
            {
                var opLogApplier = new OpLogApplier(opLogManager);
                var memTable = new MemTable(opLogManager, null, ulong.MaxValue);
                opLogApplier.Apply(memTable);

                var itemForKey1 = memTable.Get(item1.Key);
                var itemForKey2 = memTable.Get(item2.Key);

                itemForKey1.Should().Be(item1.Value);
                itemForKey2.Should().Be(item2.Value);
            }
        }

        [Test]
        public void Should_apply_remove_operation_from_opLog()
        {
            var item = Item.CreateItem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1);
            var tombStone = Item.CreateTombStone(item, 2);

            using (var opLogManager = new OpLogManager(file, serializer))
            {
                var memTable = new MemTable(opLogManager, null, ulong.MaxValue);
                memTable.Apply(item.ToOperation());
                memTable.Apply(tombStone.ToOperation());
            }
            using (var opLogManager = new OpLogManager(file, serializer))
            {
                var opLogApplier = new OpLogApplier(opLogManager);
                var memTable = new MemTable(opLogManager, null, ulong.MaxValue);
                opLogApplier.Apply(memTable);

                memTable.Get(tombStone.Key, false).Should().BeNull();
                memTable.Get(tombStone.Key, true).Should().Be(item.Value);
            }
        }
    }
}
