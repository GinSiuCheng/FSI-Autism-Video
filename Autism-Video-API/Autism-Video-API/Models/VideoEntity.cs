﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB.Table;

namespace Autism_Video_API.Models
{
    public class VideoEntity : TableEntity
    {
        public string EndTime { get; set; }
        public string FileName { get; set; }

        public string BlobFileUrl { get; set;}
                
        public VideoEntity() { }

        public VideoEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey) { }

        public VideoEntity(string partitionKey, string rowKey, string endTime, string fileName, string StorageConnectionString)
        {
            string TableName = "Videos";
            base.PartitionKey = partitionKey;
            base.RowKey = rowKey;
            this.EndTime = endTime;
            this.FileName = fileName;

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table. 
            // Persist this http connection in the class
            var table = tableClient.GetTableReference(TableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            var io = TableOperation.Insert(this);

            table.Execute(io);
        }

        public void Update(string partitionKey, string rowKey, string url, string StorageConnectionString)
        {
            string TableName = "Videos";

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table. 
            // Persist this http connection in the class
            var table = tableClient.GetTableReference(TableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            TableOperation retrieveOperation = TableOperation.Retrieve<VideoEntity>(partitionKey, rowKey);
            TableResult retrievedResult = table.Execute(retrieveOperation);
            VideoEntity updateEntity = (VideoEntity)retrievedResult.Result;
            if(updateEntity != null)
            {
                //Change the description
                updateEntity.BlobFileUrl = url;

                // Create the InsertOrReplace TableOperation
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);

                // Execute the operation.
                table.Execute(insertOrReplaceOperation);
            }
        }
    }
}