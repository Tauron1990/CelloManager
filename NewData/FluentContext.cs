using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;

namespace NewData
{
    public partial class NewDataContext : OpenAccessContext
    {
        static MetadataContainer metadataContainer = new NewDataMetadataSource().GetModel();

        private static BackendConfiguration GetBackendConfiguration()
        {
            var temp = new BackendConfiguration();

            //temp.Backend = "splite";
            temp.ProviderName = "System.Data.SQLite";
            
            temp.Runtime.Concurrency = TransactionMode.OPTIMISTIC_NO_LOST_UPDATES;

            temp.SecondLevelCache.Enabled = true;
            temp.SecondLevelCache.NumberOfObjects = 250;
            temp.SecondLevelCache.NumberOfObjectsPerQueryResults = 250;

            return temp;
        }

        private static string GetConnection()
        {
            return "connectionId";
        }
        
        public NewDataContext()
            : base(GetConnection(), GetBackendConfiguration(), metadataContainer)
        {
            
        }

        public IQueryable<Product> Products
        {
            get
            {
                return this.GetAll<Product>();
            }
        }

        public void UpdateSchema()
        {
            var handler = this.GetSchemaHandler();
            string script = null;
            try
            {
                script = handler.CreateUpdateDDLScript(null);
            }
            catch
            {
                bool throwException = false;
                try
                {
                    handler.CreateDatabase();
                    script = handler.CreateDDLScript();
                }
                catch
                {
                    throwException = true;
                }
                if (throwException)
                    throw;
            }

            if (string.IsNullOrEmpty(script) == false)
            {
                handler.ExecuteDDLScript(script);
            }
        }
    }
}
