window.BENCHMARK_DATA = {
  "lastUpdate": 1781921638236,
  "repoUrl": "https://github.com/Chris-Wolfgang/ETL-Csv",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "072df2b80cc026c10b36916bfd3bbee93c9f3f79",
          "message": "Merge pull request #16 from Chris-Wolfgang/initial-dev\n\nRelease 0.1.0: CsvExtractor<T> / CsvLoader<T> with parser-agnostic mapping",
          "timestamp": "2026-06-19T22:10:32-04:00",
          "tree_id": "e832d9925a18f74e4f8ccaa6299354c32d7ebadd",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/072df2b80cc026c10b36916bfd3bbee93c9f3f79"
        },
        "date": 1781921637035,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 5046722.739583333,
            "unit": "ns",
            "range": "± 12260.880715080959"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 4751567.901041667,
            "unit": "ns",
            "range": "± 9617.964828018647"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1106617.8515625,
            "unit": "ns",
            "range": "± 144521.48091978196"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1555173.3619791667,
            "unit": "ns",
            "range": "± 115313.1666383202"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1403253.3268229167,
            "unit": "ns",
            "range": "± 112055.67292695708"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 3815326.03125,
            "unit": "ns",
            "range": "± 2513.530102876026"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 4929878.111979167,
            "unit": "ns",
            "range": "± 217858.8763656344"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 4166321.8151041665,
            "unit": "ns",
            "range": "± 23856.16390484782"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 33152447.541666668,
            "unit": "ns",
            "range": "± 51627.833133115644"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 43790927.11111111,
            "unit": "ns",
            "range": "± 4125452.895826811"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 34332556.44444444,
            "unit": "ns",
            "range": "± 303320.97696843545"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2281697.6354166665,
            "unit": "ns",
            "range": "± 926371.3502843822"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 7709915.416666667,
            "unit": "ns",
            "range": "± 4703327.481250187"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 4916928.130208333,
            "unit": "ns",
            "range": "± 1409494.512396116"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 4818776.606770833,
            "unit": "ns",
            "range": "± 16464.260001663122"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 7904374.755208333,
            "unit": "ns",
            "range": "± 1338990.401081619"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 11970453.635416666,
            "unit": "ns",
            "range": "± 2544785.137570072"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 37444926.77777778,
            "unit": "ns",
            "range": "± 4637384.595483597"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 94931386.66666667,
            "unit": "ns",
            "range": "± 24538248.546456523"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 77543064.66666667,
            "unit": "ns",
            "range": "± 14169004.65288269"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 384400.11442057294,
            "unit": "ns",
            "range": "± 239.72615825951956"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1173173.4361979167,
            "unit": "ns",
            "range": "± 31287.123378741522"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1508329.98828125,
            "unit": "ns",
            "range": "± 78749.7898931966"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 4358046.622395833,
            "unit": "ns",
            "range": "± 22157.634403546475"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 34088233.51111111,
            "unit": "ns",
            "range": "± 119645.16590751476"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 324089939.8333333,
            "unit": "ns",
            "range": "± 280473.12785066117"
          }
        ]
      }
    ]
  }
}