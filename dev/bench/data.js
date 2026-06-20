window.BENCHMARK_DATA = {
  "lastUpdate": 1781972295979,
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
      },
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
          "id": "1187515a9c169f20966303fe9f67cff9589b752f",
          "message": "Merge pull request #112 from Chris-Wolfgang/dependabot/github_actions/github-actions-91b150d450\n\nBump the github-actions group with 3 updates",
          "timestamp": "2026-06-20T09:10:04-04:00",
          "tree_id": "2ff48d3863013f4e01eb76a83ccc5df991962dbd",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/1187515a9c169f20966303fe9f67cff9589b752f"
        },
        "date": 1781961219801,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6485893.338541667,
            "unit": "ns",
            "range": "± 171293.26949457458"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6269279.223958333,
            "unit": "ns",
            "range": "± 11783.310084988017"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1325128.0221354167,
            "unit": "ns",
            "range": "± 73617.49766303274"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1726494.1276041667,
            "unit": "ns",
            "range": "± 146381.91293294085"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1647529.2161458333,
            "unit": "ns",
            "range": "± 25534.845448085984"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 4900177.002604167,
            "unit": "ns",
            "range": "± 15883.931604052877"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 6192342.5625,
            "unit": "ns",
            "range": "± 111450.81353555614"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5385167.200520833,
            "unit": "ns",
            "range": "± 114481.52220474948"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 42405750.666666664,
            "unit": "ns",
            "range": "± 193626.7033611515"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 59362292.5,
            "unit": "ns",
            "range": "± 3889839.196622856"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 43839949.02777778,
            "unit": "ns",
            "range": "± 981049.9166007075"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2402504.1692708335,
            "unit": "ns",
            "range": "± 142100.60871871174"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3352061.2994791665,
            "unit": "ns",
            "range": "± 805936.9559055154"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2683477.6770833335,
            "unit": "ns",
            "range": "± 176260.23216377414"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6230178.8984375,
            "unit": "ns",
            "range": "± 16291.553510572336"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8213403.848958333,
            "unit": "ns",
            "range": "± 737131.7813074075"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7172516.208333333,
            "unit": "ns",
            "range": "± 217702.16373706283"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 44072033.333333336,
            "unit": "ns",
            "range": "± 382969.56002446637"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 72565600.33333333,
            "unit": "ns",
            "range": "± 12944800.05363314"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 48378684.666666664,
            "unit": "ns",
            "range": "± 370863.54765465966"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 503062.98046875,
            "unit": "ns",
            "range": "± 8872.877662360728"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1465622.2955729167,
            "unit": "ns",
            "range": "± 16851.68763613238"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1867725.7994791667,
            "unit": "ns",
            "range": "± 21436.654823213863"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5510703.486979167,
            "unit": "ns",
            "range": "± 24805.968108692483"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45275712.93939394,
            "unit": "ns",
            "range": "± 232677.2074760359"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 413712930,
            "unit": "ns",
            "range": "± 1061306.911938295"
          }
        ]
      },
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
          "id": "64ad5a93dd70ce582b4b930b862d8458d29acccb",
          "message": "Merge pull request #113 from Chris-Wolfgang/dependabot/nuget/dotnet-dependencies-8d32cd9afb\n\nBump the dotnet-dependencies group with 6 updates",
          "timestamp": "2026-06-20T12:14:52-04:00",
          "tree_id": "a7191ce8ac92dcc0fa0ca96785bdd66cfd24b1ca",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/64ad5a93dd70ce582b4b930b862d8458d29acccb"
        },
        "date": 1781972295151,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6620350.770833333,
            "unit": "ns",
            "range": "± 11232.705397483332"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6152669.377604167,
            "unit": "ns",
            "range": "± 43047.22620696137"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1243158.9075520833,
            "unit": "ns",
            "range": "± 56614.28798913404"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 2631276.5911458335,
            "unit": "ns",
            "range": "± 1299292.3300873365"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 2219864.6119791665,
            "unit": "ns",
            "range": "± 934579.5315858965"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 4948477.739583333,
            "unit": "ns",
            "range": "± 18967.06106810055"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8012560.177083333,
            "unit": "ns",
            "range": "± 642630.8459540973"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5413110.578125,
            "unit": "ns",
            "range": "± 25107.240985434775"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 42382503.222222224,
            "unit": "ns",
            "range": "± 379504.2317454774"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 63863475.42857143,
            "unit": "ns",
            "range": "± 2771264.8180314284"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 44923843.09090909,
            "unit": "ns",
            "range": "± 333851.41863631393"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2159886.6067708335,
            "unit": "ns",
            "range": "± 48353.49888997414"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3328205.0442708335,
            "unit": "ns",
            "range": "± 924531.2688512105"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3648508.0052083335,
            "unit": "ns",
            "range": "± 1430125.5300588985"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6103225.276041667,
            "unit": "ns",
            "range": "± 53192.82597360004"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8888787.385416666,
            "unit": "ns",
            "range": "± 1062553.1384043845"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6595097.442708333,
            "unit": "ns",
            "range": "± 70210.92723776167"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 44920327.333333336,
            "unit": "ns",
            "range": "± 1441265.0119631307"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 71509935.33333333,
            "unit": "ns",
            "range": "± 12304739.097233078"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 75746435.66666667,
            "unit": "ns",
            "range": "± 25946755.77983402"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 572679.1494140625,
            "unit": "ns",
            "range": "± 25957.72713782533"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1501146.1100260417,
            "unit": "ns",
            "range": "± 59003.86726517667"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1969531.1588541667,
            "unit": "ns",
            "range": "± 63798.70853074535"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5609815.638020833,
            "unit": "ns",
            "range": "± 54578.753446886265"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 43848111.833333336,
            "unit": "ns",
            "range": "± 290628.15584718005"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 436462491,
            "unit": "ns",
            "range": "± 2510527.5980934766"
          }
        ]
      }
    ]
  }
}