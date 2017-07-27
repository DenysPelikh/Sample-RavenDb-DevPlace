using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimisticConcurrency.Handlers;

namespace OptimisticConcurrency
{
    class Program
    {   
        static void Main(string[] args)
        {
            var store = SetupRavenDb.SetupEmbeddableStore();

        }
    }
}
