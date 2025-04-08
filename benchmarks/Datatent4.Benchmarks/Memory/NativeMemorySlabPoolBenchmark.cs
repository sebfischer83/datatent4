using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Datatent4.Core.Memory.Unmanaged;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datatent4.Benchmarks.Memory
{
    [MemoryDiagnoser]
    [InliningDiagnoser(true, true)]
    [TailCallDiagnoser]
    [ConcurrencyVisualizerProfiler]
    [NativeMemoryProfiler]
    [ThreadingDiagnoser]
    [ExceptionDiagnoser]
    public class NativeMemorySlabPoolBenchmark
    {
        private NativeMemorySlabPool _pool;

        [GlobalSetup]
        public void Setup()
        {
            // Initialisiere den Pool mit einem Null-Logger
            _pool = new NativeMemorySlabPool(NullLogger<NativeMemorySlabPool>.Instance);
        }

        [Benchmark]
        public void RentReturnMemorySlab()
        {
            // Teste die Rent-Methode
            var slab = _pool.Rent();
            _pool.Return(slab);
        }

    }
}
