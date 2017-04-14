using System;
using System.IO;

namespace LSMStorage.Core
{
    public class DatabaseManager
    {
        private readonly IMemTable memTable;
        private readonly DiskTablesMerger diskTablesMerger;
        private readonly DirectoryInfo databaseDirectory;

        public DatabaseManager(IMemTable memTable, DiskTablesMerger diskTablesMerger, DirectoryInfo databaseDirectory, MergeMethod mergeMethod)
        {
            this.memTable = memTable;
            this.diskTablesMerger = diskTablesMerger;
            this.databaseDirectory = databaseDirectory;
            MergeMethod = mergeMethod;

            ItemsTreshold = 10;
        }

        public void Apply(IOperation operation)
        {
            memTable.Apply(operation);
        }

        public void RestoreMemoryCopyFromSnapshot() { throw new NotImplementedException();}

        public int ItemsTreshold { get; private set; }
        public MergeMethod MergeMethod { get; }
    }
}