// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore.Cosmos.Sql.Metadata;
using Newtonsoft.Json.Linq;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class CosmosSqlTestStore : TestStore
    {
        private readonly DocumentClient _documentClient;
        private readonly string _dataFilePath;

        public static CosmosSqlTestStore Create(string name) => new CosmosSqlTestStore(name, shared: false);

        public static CosmosSqlTestStore GetOrCreate(string name) => new CosmosSqlTestStore(name);

        public static CosmosSqlTestStore GetOrCreate(string name, string dataFilePath)
            => new CosmosSqlTestStore(name, dataFilePath: dataFilePath);

        public static CosmosSqlTestStore GetOrCreateInitialized(string name)
            => (CosmosSqlTestStore)new CosmosSqlTestStore(name).Initialize(null, (Func<DbContext>)null, null);

        public static CosmosSqlTestStore CreateInitialized(string name)
            => (CosmosSqlTestStore)Create(name).Initialize(null, (Func<DbContext>)null, null);

        private CosmosSqlTestStore(string name, bool shared = true, string dataFilePath = null)
            : base(name, shared)
        {
            ConnectionUri = new Uri(TestEnvironment.DefaultConnection);
            AuthToken = TestEnvironment.AuthToken;

            _documentClient = new DocumentClient(ConnectionUri, AuthToken);

            if (dataFilePath != null)
            {
                _dataFilePath = Path.Combine(
                    Path.GetDirectoryName(typeof(CosmosSqlTestStore).GetTypeInfo().Assembly.Location),
                    dataFilePath);
            }
        }

        public Uri ConnectionUri { get; private set; }
        public string AuthToken { get; private set; }

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed)
        {
            using (var context = createContext())
            {
                if (_dataFilePath == null)
                {
                    Clean(context);
                    seed(context);
                }
                else
                {
                    CreateFromFile(context).GetAwaiter().GetResult();
                }
            }
        }

        private async Task CreateFromFile(DbContext context)
        {
            if (await context.Database.EnsureCreatedAsync())
            {
                var seedData = JArray.Parse(File.ReadAllText(_dataFilePath));
                var collectionUri = UriFactory.CreateDocumentCollectionUri(
                    Name, context.Model.CosmosSql().DefaultCollection);

                foreach (var entityData in seedData)
                {
                    var entityName = (string)entityData["Name"];
                    var entityFullName = (string)entityData["FullName"];

                    if (entityFullName != null)
                    {
                        foreach (var document in entityData["Data"])
                        {
                            document["id"] = $"{entityFullName}|{document["id"]}";
                            document["Discriminator"] = entityName;
                            await _documentClient.CreateDocumentAsync(collectionUri, document);
                        }
                    }
                }
            }
        }

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
        {
            return builder.UseCosmosSql(
                ConnectionUri,
                AuthToken,
                Name);
        }

        public override void Clean(DbContext context)
        {
            context.Database.EnsureDeletedAsync().GetAwaiter().GetResult();
            context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
        }

        private void DeleteDatabase()
        {
            try
            {
                _documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(Name)).GetAwaiter().GetResult();
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Ignore as database may not have existed.
            }
        }

        public override void Dispose()
        {
            if (_dataFilePath == null)
            {
                DeleteDatabase();
            }

            _documentClient.Dispose();
            base.Dispose();
        }
    }
}
