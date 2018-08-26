﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ISynergy.Framework.Utilities
{
    public class X509CertUtilities
    {
        public static X509Certificate2 FindCertFromStore(StoreName name, StoreLocation location, X509FindType findType, object findValue, bool validOnly)
        {
            using (var store = new X509Store(name, location))
            {
                store.Open(OpenFlags.ReadOnly);
                var certificateCollection = store.Certificates.Find(findType, findValue, validOnly);
                return certificateCollection.Count == 0 ? null : certificateCollection[0];
            }
        }
    }
}
