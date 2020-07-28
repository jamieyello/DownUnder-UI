﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DownUnder.Utilities.Serialization.DataContractResolvers
{
    class DownUnderResolver : DataContractResolver
    {
        // On loading
        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            Console.WriteLine($"DownUnderResolver " + typeName);
            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver);
        }

        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            Console.WriteLine($"bup");
            
            return knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace);
        }
    }
}