window.BENCHMARK_DATA = {
  "lastUpdate": 1786461980615,
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
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "1875684f4b5dbfe3262bf9f7b8a70c971437d27c",
          "message": "Bump the dotnet-dependencies group with 5 updates\n\nBumps Meziantou.Analyzer from 3.0.105 to 3.0.115\nBumps Microsoft.VisualStudio.Threading.Analyzers from 17.14.15 to 18.7.23\nBumps Wolfgang.Etl.Abstractions from 0.13.0 to 0.14.1\nBumps Wolfgang.Etl.TestKit from 0.8.0 to 0.9.0\nBumps Wolfgang.Etl.TestKit.Xunit from 0.8.0 to 0.9.0\n\n---\nupdated-dependencies:\n- dependency-name: Meziantou.Analyzer\n  dependency-version: 3.0.115\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: Microsoft.VisualStudio.Threading.Analyzers\n  dependency-version: 18.7.23\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: dotnet-dependencies\n- dependency-name: Wolfgang.Etl.Abstractions\n  dependency-version: 0.14.1\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: Wolfgang.Etl.TestKit\n  dependency-version: 0.9.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: Wolfgang.Etl.TestKit.Xunit\n  dependency-version: 0.9.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>",
          "timestamp": "2026-06-27T13:49:24-04:00",
          "tree_id": "568f9809abf62d10a6b4aed85589ac4d7f94ee92",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/1875684f4b5dbfe3262bf9f7b8a70c971437d27c"
        },
        "date": 1782582778824,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7000119.369791667,
            "unit": "ns",
            "range": "± 17466.055451711378"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6205543.541666667,
            "unit": "ns",
            "range": "± 56075.81035930482"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1279457.7096354167,
            "unit": "ns",
            "range": "± 49111.70900187867"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 2225267.0104166665,
            "unit": "ns",
            "range": "± 92759.85102635219"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1636012.2369791667,
            "unit": "ns",
            "range": "± 98470.4414033307"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5209733.2421875,
            "unit": "ns",
            "range": "± 14621.211200881202"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8448894.3125,
            "unit": "ns",
            "range": "± 596172.0557811081"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5662835.174479167,
            "unit": "ns",
            "range": "± 110477.552673205"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 45068329.72727273,
            "unit": "ns",
            "range": "± 350509.0955599051"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 77407581.66666667,
            "unit": "ns",
            "range": "± 7102318.73297439"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 45975638.06060606,
            "unit": "ns",
            "range": "± 619736.4269202184"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2140463.1197916665,
            "unit": "ns",
            "range": "± 62889.870449929236"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3114731.0651041665,
            "unit": "ns",
            "range": "± 380864.46741862624"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2548631.3971354165,
            "unit": "ns",
            "range": "± 132596.24247146636"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6166156.348958333,
            "unit": "ns",
            "range": "± 16568.104134874447"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8243994.994791667,
            "unit": "ns",
            "range": "± 437495.4954157107"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7088400.34375,
            "unit": "ns",
            "range": "± 172766.09284927"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 43466120.166666664,
            "unit": "ns",
            "range": "± 432314.11771537235"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 70904225.5,
            "unit": "ns",
            "range": "± 12033963.448014665"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 67724734.5,
            "unit": "ns",
            "range": "± 25648318.104158424"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 581816.7275390625,
            "unit": "ns",
            "range": "± 31529.45082583492"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1511510.2565104167,
            "unit": "ns",
            "range": "± 28021.786546578336"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1895921.9856770833,
            "unit": "ns",
            "range": "± 24209.637437427784"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6329896.6953125,
            "unit": "ns",
            "range": "± 66161.18235341103"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 46954812.090909086,
            "unit": "ns",
            "range": "± 170365.56803688014"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 446131170.6666667,
            "unit": "ns",
            "range": "± 10552445.895989912"
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
          "id": "5cbc060737b7c41bd48b2cd75a650bdd5489df05",
          "message": "Merge pull request #125 from Chris-Wolfgang/vNext\n\nRelease 0.2.0: CsvLoader deadlock fix + governance docs + security infra",
          "timestamp": "2026-06-27T21:28:52-04:00",
          "tree_id": "b323b68211b2563d7bed745ceb647e05d17adc73",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/5cbc060737b7c41bd48b2cd75a650bdd5489df05"
        },
        "date": 1782610353149,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7148093.864583333,
            "unit": "ns",
            "range": "± 48714.04889670163"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6468743.403645833,
            "unit": "ns",
            "range": "± 30142.726504065577"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1452460.5807291667,
            "unit": "ns",
            "range": "± 46496.503407465905"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1745554.9518229167,
            "unit": "ns",
            "range": "± 88223.81300323535"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1870294.8984375,
            "unit": "ns",
            "range": "± 89806.01599969032"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5633016.098958333,
            "unit": "ns",
            "range": "± 53330.35403814151"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 6956365.015625,
            "unit": "ns",
            "range": "± 544992.3066634131"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 6069336.630208333,
            "unit": "ns",
            "range": "± 36479.13115641462"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 47021430.27272727,
            "unit": "ns",
            "range": "± 165381.6908059249"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 54920419.370370366,
            "unit": "ns",
            "range": "± 1788106.4333890846"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 48057307.3939394,
            "unit": "ns",
            "range": "± 457146.50862892607"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2245927.2135416665,
            "unit": "ns",
            "range": "± 183101.88555497257"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3550372.1614583335,
            "unit": "ns",
            "range": "± 1285085.8156247607"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2602783.03515625,
            "unit": "ns",
            "range": "± 80884.31864505631"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6159230.760416667,
            "unit": "ns",
            "range": "± 11912.485232030329"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 7608156.557291667,
            "unit": "ns",
            "range": "± 185232.71214586313"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7281350.03125,
            "unit": "ns",
            "range": "± 474957.6841333072"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 47068724.55555555,
            "unit": "ns",
            "range": "± 2040353.9172644175"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 61852940.88888889,
            "unit": "ns",
            "range": "± 1779044.4760636815"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 48979082.666666664,
            "unit": "ns",
            "range": "± 2558957.973381895"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 388396.01953125,
            "unit": "ns",
            "range": "± 8531.874744705232"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1512606.2649739583,
            "unit": "ns",
            "range": "± 11217.125436984616"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2074536.4557291667,
            "unit": "ns",
            "range": "± 11868.011017942656"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6172330.635416667,
            "unit": "ns",
            "range": "± 25705.24421066787"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 46584486.18181818,
            "unit": "ns",
            "range": "± 269240.79353357153"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 469691294.6666667,
            "unit": "ns",
            "range": "± 10086974.665667469"
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
          "id": "4894082ea1e9b9a39e3026a368d470f9e1114918",
          "message": "Merge pull request #136 from Chris-Wolfgang/dependabot/nuget/dotnet-dependencies-f91895ad07\n\nBump the dotnet-dependencies group with 8 updates",
          "timestamp": "2026-07-09T16:50:34-04:00",
          "tree_id": "69e48a69050df60461bdbd8fc53473b64c1fa0ad",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/4894082ea1e9b9a39e3026a368d470f9e1114918"
        },
        "date": 1783630468080,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6843577.322916667,
            "unit": "ns",
            "range": "± 14504.700790670444"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6270269.135416667,
            "unit": "ns",
            "range": "± 36184.552543014535"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1337914.7291666667,
            "unit": "ns",
            "range": "± 6893.326720581104"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1928393.6875,
            "unit": "ns",
            "range": "± 81544.33371993012"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1673047.7096354167,
            "unit": "ns",
            "range": "± 70152.60096506022"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5144048.6015625,
            "unit": "ns",
            "range": "± 16607.522283278366"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8435494.182291666,
            "unit": "ns",
            "range": "± 518004.94050852716"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5524031.872395833,
            "unit": "ns",
            "range": "± 65916.06454272586"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 43973791.05555555,
            "unit": "ns",
            "range": "± 116385.90373421417"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 77786933.38095237,
            "unit": "ns",
            "range": "± 7063802.621102282"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 45590632.90909091,
            "unit": "ns",
            "range": "± 1369436.511466748"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 1930979.9661458333,
            "unit": "ns",
            "range": "± 2917.649351872771"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3391114.6041666665,
            "unit": "ns",
            "range": "± 1102941.2416752232"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3007608.4166666665,
            "unit": "ns",
            "range": "± 358617.0768007408"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 5995623.825520833,
            "unit": "ns",
            "range": "± 63174.32656443174"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8682900.744791666,
            "unit": "ns",
            "range": "± 283217.9513170448"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6542408.5234375,
            "unit": "ns",
            "range": "± 81905.8156290507"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 42376461.80555555,
            "unit": "ns",
            "range": "± 279073.7116882332"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 74920147.83333333,
            "unit": "ns",
            "range": "± 13894085.11980226"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 73078700.33333333,
            "unit": "ns",
            "range": "± 28647824.826385237"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 590705.0257161459,
            "unit": "ns",
            "range": "± 38923.839923088715"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1500930.3522135417,
            "unit": "ns",
            "range": "± 63739.496984815385"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1907227.3359375,
            "unit": "ns",
            "range": "± 16387.01316284286"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5811218.434895833,
            "unit": "ns",
            "range": "± 10323.223436109321"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45872499.96969697,
            "unit": "ns",
            "range": "± 349462.03993969434"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 444186565.6666667,
            "unit": "ns",
            "range": "± 8511994.411136108"
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
          "id": "4fdd3ecfdc63e54dd11ee1a0fe8e174973cd6eac",
          "message": "Merge pull request #144 from Chris-Wolfgang/dependabot/github_actions/github-actions-b93e283e24\n\nBump the github-actions group with 2 updates",
          "timestamp": "2026-07-20T19:49:11-04:00",
          "tree_id": "091561b01c7d28b5e49b6ff244a2f911e10ecfb7",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/4fdd3ecfdc63e54dd11ee1a0fe8e174973cd6eac"
        },
        "date": 1784591568505,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6714274.330729167,
            "unit": "ns",
            "range": "± 54785.82488241842"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 5948520.3828125,
            "unit": "ns",
            "range": "± 5590.464425534256"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1243712.96484375,
            "unit": "ns",
            "range": "± 48289.51995205108"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1720139.9375,
            "unit": "ns",
            "range": "± 74119.79956466782"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 2614271.4583333335,
            "unit": "ns",
            "range": "± 948117.4110547277"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5108303.255208333,
            "unit": "ns",
            "range": "± 22010.51978541214"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 6353529.6015625,
            "unit": "ns",
            "range": "± 99672.16502353024"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5552486.84375,
            "unit": "ns",
            "range": "± 83148.12600402966"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 43425096.97222222,
            "unit": "ns",
            "range": "± 128681.65744986202"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 58340276.85185186,
            "unit": "ns",
            "range": "± 2661400.433891478"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 45010119.333333336,
            "unit": "ns",
            "range": "± 490300.7209443673"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2241934.1510416665,
            "unit": "ns",
            "range": "± 51466.13539641608"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3095462.4036458335,
            "unit": "ns",
            "range": "± 171564.49999239933"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2850274.2838541665,
            "unit": "ns",
            "range": "± 239441.98413927335"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6104335.638020833,
            "unit": "ns",
            "range": "± 11551.510148682217"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 7871487.963541667,
            "unit": "ns",
            "range": "± 529407.5288251984"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6704179.377604167,
            "unit": "ns",
            "range": "± 143743.4021336873"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 43820849.88888889,
            "unit": "ns",
            "range": "± 348078.4124083294"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 72271377.83333333,
            "unit": "ns",
            "range": "± 12832220.64132417"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 52042970.333333336,
            "unit": "ns",
            "range": "± 2190831.797978586"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 515399.4580078125,
            "unit": "ns",
            "range": "± 17093.263971520642"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1505793.6419270833,
            "unit": "ns",
            "range": "± 25471.808876514933"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1909262.078125,
            "unit": "ns",
            "range": "± 42959.28808777312"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6107069.721354167,
            "unit": "ns",
            "range": "± 25929.361510881034"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45246171.545454554,
            "unit": "ns",
            "range": "± 202547.09374209956"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 426391514,
            "unit": "ns",
            "range": "± 1726683.1355703338"
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
          "id": "e7483c414d40a18c18529a4a466051e337a959df",
          "message": "Merge pull request #146 from Chris-Wolfgang/vNext\n\nCSV pipeline extensions: CsvExtractor / CsvLoader over EtlPipeline (#14)",
          "timestamp": "2026-07-21T21:46:24-04:00",
          "tree_id": "3061f769ac3cae68df2e861c5ac723732be6a3d2",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/e7483c414d40a18c18529a4a466051e337a959df"
        },
        "date": 1784685004847,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7037601.442708333,
            "unit": "ns",
            "range": "± 54072.191035917"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6103811.557291667,
            "unit": "ns",
            "range": "± 38916.85398290224"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1254756.4453125,
            "unit": "ns",
            "range": "± 68223.88632775027"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 2348206.2291666665,
            "unit": "ns",
            "range": "± 976754.102688059"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1641035.4895833333,
            "unit": "ns",
            "range": "± 144844.73348244798"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5072827.1328125,
            "unit": "ns",
            "range": "± 2887.446421141168"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8450888.734375,
            "unit": "ns",
            "range": "± 384446.6322780762"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5573888.455729167,
            "unit": "ns",
            "range": "± 112152.9900377746"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 45877580.696969695,
            "unit": "ns",
            "range": "± 71280.42090364847"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 95907826,
            "unit": "ns",
            "range": "± 3347190.5945743513"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 48334413.666666664,
            "unit": "ns",
            "range": "± 360766.63465072477"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 1957575.1497395833,
            "unit": "ns",
            "range": "± 16077.474540861997"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 2970419.6744791665,
            "unit": "ns",
            "range": "± 253172.03557706374"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3300522.6276041665,
            "unit": "ns",
            "range": "± 866210.4061138453"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6213567.8828125,
            "unit": "ns",
            "range": "± 15040.413308980038"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 9476063.21875,
            "unit": "ns",
            "range": "± 725737.6792521914"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6654730.166666667,
            "unit": "ns",
            "range": "± 21906.046305692675"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 52159096.666666664,
            "unit": "ns",
            "range": "± 3865713.8161544735"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 84289948,
            "unit": "ns",
            "range": "± 20494419.9090945"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 67747917.16666667,
            "unit": "ns",
            "range": "± 25724766.846572965"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 575879.3704427084,
            "unit": "ns",
            "range": "± 15359.656438743645"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1580134.9563802083,
            "unit": "ns",
            "range": "± 6424.968211146678"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1970109.5859375,
            "unit": "ns",
            "range": "± 4588.696994162154"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5809210.106770833,
            "unit": "ns",
            "range": "± 40495.66953549718"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 47396402.94444444,
            "unit": "ns",
            "range": "± 561394.9396401669"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 430564656,
            "unit": "ns",
            "range": "± 802966.3768633155"
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
          "id": "931c32c4ac0bb40c9ac3ef6ae5f4b66f818d8a98",
          "message": "Merge pull request #155 from Chris-Wolfgang/dependabot/nuget/dotnet-dependencies-8f606db41f\n\nBump the dotnet-dependencies group with 3 updates",
          "timestamp": "2026-07-27T21:30:42-04:00",
          "tree_id": "d9390334ff5ed1491fbbb65614adb86cca944d73",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/931c32c4ac0bb40c9ac3ef6ae5f4b66f818d8a98"
        },
        "date": 1785202469075,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6680983.747395833,
            "unit": "ns",
            "range": "± 11514.308104498747"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6083802.114583333,
            "unit": "ns",
            "range": "± 5789.998302533698"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1299063.9752604167,
            "unit": "ns",
            "range": "± 88898.23083187979"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1726921.8333333333,
            "unit": "ns",
            "range": "± 92290.81185955416"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1623280.2526041667,
            "unit": "ns",
            "range": "± 115793.24119777248"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5098836.486979167,
            "unit": "ns",
            "range": "± 15514.058349437355"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 6603271.270833333,
            "unit": "ns",
            "range": "± 363242.8392472707"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5584224.611979167,
            "unit": "ns",
            "range": "± 75951.65139413116"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 44227692.86111111,
            "unit": "ns",
            "range": "± 211951.53693877632"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 60570650.291666664,
            "unit": "ns",
            "range": "± 4596890.938938825"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 45919489.90909091,
            "unit": "ns",
            "range": "± 442812.3393605105"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2054148.3893229167,
            "unit": "ns",
            "range": "± 2687.0137186565034"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3268526.6458333335,
            "unit": "ns",
            "range": "± 260861.51244468303"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3207069.8255208335,
            "unit": "ns",
            "range": "± 765278.2548638139"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6152290.0859375,
            "unit": "ns",
            "range": "± 26647.710672865247"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8347666.463541667,
            "unit": "ns",
            "range": "± 518806.9930752366"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6762964.21875,
            "unit": "ns",
            "range": "± 24811.622534547696"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 44932403.88888889,
            "unit": "ns",
            "range": "± 489781.47451018443"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 79197917.33333333,
            "unit": "ns",
            "range": "± 1490078.9372988208"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 48791648.666666664,
            "unit": "ns",
            "range": "± 964703.5542306938"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 509673.6728515625,
            "unit": "ns",
            "range": "± 12197.275697684618"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1513288.3046875,
            "unit": "ns",
            "range": "± 16302.623291966951"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2041560.3880208333,
            "unit": "ns",
            "range": "± 6426.751684516592"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6194542.682291667,
            "unit": "ns",
            "range": "± 15899.048210524103"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 46521914.15151515,
            "unit": "ns",
            "range": "± 132327.61319278763"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 430296749.3333333,
            "unit": "ns",
            "range": "± 1138336.3009802215"
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
          "id": "4324420dd7d19ac43b0a6c5a196fc98ef94b11db",
          "message": "Merge pull request #178 from Chris-Wolfgang/vNext-plus\n\nRelease 0.3.0",
          "timestamp": "2026-08-08T08:22:48-04:00",
          "tree_id": "d5e7fb31b693f07280cc04beda274019e9a1adab",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/4324420dd7d19ac43b0a6c5a196fc98ef94b11db"
        },
        "date": 1786192005089,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7050211.518229167,
            "unit": "ns",
            "range": "± 11097.044354589883"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6105011.2265625,
            "unit": "ns",
            "range": "± 13653.584771399863"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1215645.4381510417,
            "unit": "ns",
            "range": "± 7301.437570855371"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 2103506.734375,
            "unit": "ns",
            "range": "± 100407.7843757556"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1670002.0638020833,
            "unit": "ns",
            "range": "± 93677.32557704802"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5337930.786458333,
            "unit": "ns",
            "range": "± 31011.567286750404"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 7722487.052083333,
            "unit": "ns",
            "range": "± 756317.4340053803"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5631433.182291667,
            "unit": "ns",
            "range": "± 76631.52708676753"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 45320233.81818181,
            "unit": "ns",
            "range": "± 110569.47794636006"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 74569630.75,
            "unit": "ns",
            "range": "± 2970338.242853957"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 47605592.45454546,
            "unit": "ns",
            "range": "± 649578.4393310725"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 1946683.0221354167,
            "unit": "ns",
            "range": "± 5743.099324958919"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3536817.3333333335,
            "unit": "ns",
            "range": "± 161310.0871877995"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3013696.3098958335,
            "unit": "ns",
            "range": "± 308694.84079154336"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6298622.90625,
            "unit": "ns",
            "range": "± 101613.73741756365"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8356821.557291667,
            "unit": "ns",
            "range": "± 656440.1613233745"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7132194.947916667,
            "unit": "ns",
            "range": "± 208853.07952399828"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 42369396.02777778,
            "unit": "ns",
            "range": "± 144449.29357160412"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 91660220,
            "unit": "ns",
            "range": "± 23896959.127859574"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 48822658.166666664,
            "unit": "ns",
            "range": "± 425457.75173991517"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 609024.4388020834,
            "unit": "ns",
            "range": "± 9108.922824960757"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1522943.634765625,
            "unit": "ns",
            "range": "± 50835.20625595955"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2045073.9166666667,
            "unit": "ns",
            "range": "± 22115.277320829446"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6009075.4140625,
            "unit": "ns",
            "range": "± 30643.074983653263"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 46211112.93939394,
            "unit": "ns",
            "range": "± 208411.0847376062"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 444559983.6666667,
            "unit": "ns",
            "range": "± 4394832.200857086"
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
          "id": "8645ac2751e52620dc0e2a3f7badd0f3cc8dffad",
          "message": "Merge pull request #189 from Chris-Wolfgang/vNext\n\nRelease 0.4.0",
          "timestamp": "2026-08-09T19:29:09-04:00",
          "tree_id": "5b6b437ad0b705e952d77932e1e11298da24d6ef",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/8645ac2751e52620dc0e2a3f7badd0f3cc8dffad"
        },
        "date": 1786318369978,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6977607.6171875,
            "unit": "ns",
            "range": "± 61019.07200889807"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6102827.21875,
            "unit": "ns",
            "range": "± 50539.349917710024"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1212297.2610677083,
            "unit": "ns",
            "range": "± 5846.296839713014"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1740045.1028645833,
            "unit": "ns",
            "range": "± 154256.83622614306"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1657240.8111979167,
            "unit": "ns",
            "range": "± 47024.14190495013"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5268321.7109375,
            "unit": "ns",
            "range": "± 53651.3212915583"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 7073222.255208333,
            "unit": "ns",
            "range": "± 653511.6091975234"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5626022.489583333,
            "unit": "ns",
            "range": "± 19300.6261812044"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 45041151.88888889,
            "unit": "ns",
            "range": "± 1293058.9946826184"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 58821893.25,
            "unit": "ns",
            "range": "± 4661780.696297908"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 46386620.48484849,
            "unit": "ns",
            "range": "± 768834.6693971314"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2262608.796875,
            "unit": "ns",
            "range": "± 60683.69788821327"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3940680.4296875,
            "unit": "ns",
            "range": "± 1444366.2348411235"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3075071.515625,
            "unit": "ns",
            "range": "± 411641.0369358898"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6348972.541666667,
            "unit": "ns",
            "range": "± 107424.57972940402"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8154899.526041667,
            "unit": "ns",
            "range": "± 668882.9277820351"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6740626.817708333,
            "unit": "ns",
            "range": "± 85643.7103142872"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 47160706.88888889,
            "unit": "ns",
            "range": "± 4103385.386720555"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 60974338.11111111,
            "unit": "ns",
            "range": "± 4162873.395625834"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 49704321.44444445,
            "unit": "ns",
            "range": "± 2031931.468094152"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 498804.2783203125,
            "unit": "ns",
            "range": "± 19628.741363753616"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1483290.8033854167,
            "unit": "ns",
            "range": "± 23169.438110336847"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1954672.0794270833,
            "unit": "ns",
            "range": "± 40369.722623945396"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5876850.953125,
            "unit": "ns",
            "range": "± 38669.0798486071"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45987028.63636363,
            "unit": "ns",
            "range": "± 169666.25469125627"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 432147074.3333333,
            "unit": "ns",
            "range": "± 531464.421983573"
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
          "id": "81b49f2ea93e5f57710d3749bc0a4fdce92ac7fd",
          "message": "Merge pull request #191 from Chris-Wolfgang/chore/packagevalidation-baseline-0.4.0\n\nbuild: bump PackageValidation baseline to 0.4.0",
          "timestamp": "2026-08-09T21:27:12-04:00",
          "tree_id": "ca30fd74fa37422d9f407376b0f9b04e7ebe85ca",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/81b49f2ea93e5f57710d3749bc0a4fdce92ac7fd"
        },
        "date": 1786325443764,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6345414.1640625,
            "unit": "ns",
            "range": "± 66222.84127854771"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 5895707.0703125,
            "unit": "ns",
            "range": "± 20417.673026346343"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1314686.6848958333,
            "unit": "ns",
            "range": "± 62372.52368696809"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 2742538.6770833335,
            "unit": "ns",
            "range": "± 1353714.504998328"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1792459.0494791667,
            "unit": "ns",
            "range": "± 108888.96291248492"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 4635011.854166667,
            "unit": "ns",
            "range": "± 18210.507465657258"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 5852962.765625,
            "unit": "ns",
            "range": "± 112963.68424263498"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5598675.604166667,
            "unit": "ns",
            "range": "± 35616.39665694852"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 38341696,
            "unit": "ns",
            "range": "± 95385.159122909"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 50109753.666666664,
            "unit": "ns",
            "range": "± 2830979.709790293"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 40691309.25,
            "unit": "ns",
            "range": "± 942261.7235885316"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 1939869.0416666667,
            "unit": "ns",
            "range": "± 12028.077746873172"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 6327433.729166667,
            "unit": "ns",
            "range": "± 2016661.9601930124"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 4929507.072916667,
            "unit": "ns",
            "range": "± 634217.1380991208"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 5953070.794270833,
            "unit": "ns",
            "range": "± 22768.352086702253"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 31001670.944444444,
            "unit": "ns",
            "range": "± 1880119.4195797646"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 15277048.592592591,
            "unit": "ns",
            "range": "± 6425224.1355449"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 40065744.77777778,
            "unit": "ns",
            "range": "± 1441815.2588729295"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 67526702.44444445,
            "unit": "ns",
            "range": "± 8499475.346309064"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 74403223.22222222,
            "unit": "ns",
            "range": "± 20777679.449194793"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 404785.9358723958,
            "unit": "ns",
            "range": "± 11251.294140822294"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1663307.056640625,
            "unit": "ns",
            "range": "± 16932.581761327427"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1999056.7395833333,
            "unit": "ns",
            "range": "± 30986.937193131704"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5899301.877604167,
            "unit": "ns",
            "range": "± 12669.040726200998"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 41288674.11111111,
            "unit": "ns",
            "range": "± 168513.09573999397"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 385342578.3333333,
            "unit": "ns",
            "range": "± 1831211.4770821345"
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
          "id": "ba8b73bf2eae4c07ba9799a5cc0dc58976c0b3da",
          "message": "Merge pull request #198 from Chris-Wolfgang/vNext\n\nRelease 0.5.0 — polymorphic rows & streaming validation",
          "timestamp": "2026-08-11T11:22:30-04:00",
          "tree_id": "fc5529c4f50ef66a1cdfced1a78bfa529267ce85",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/ba8b73bf2eae4c07ba9799a5cc0dc58976c0b3da"
        },
        "date": 1786461971922,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7379291.651041667,
            "unit": "ns",
            "range": "± 3603.0292596263316"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6592785.091145833,
            "unit": "ns",
            "range": "± 22816.85517791576"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1331764.8125,
            "unit": "ns",
            "range": "± 41019.2847567371"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1780440.1966145833,
            "unit": "ns",
            "range": "± 130299.22842804571"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1640575.60546875,
            "unit": "ns",
            "range": "± 88228.5306898974"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5620375.200520833,
            "unit": "ns",
            "range": "± 4639.370189163131"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 9006591.5,
            "unit": "ns",
            "range": "± 348141.7029472805"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5689655.846354167,
            "unit": "ns",
            "range": "± 31863.212177631332"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 44490444.77777778,
            "unit": "ns",
            "range": "± 285195.61652173195"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 78671417.33333333,
            "unit": "ns",
            "range": "± 3990123.067514328"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 46648675.78787879,
            "unit": "ns",
            "range": "± 530964.6123283356"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2231388.7526041665,
            "unit": "ns",
            "range": "± 25603.290445344763"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3253182.1223958335,
            "unit": "ns",
            "range": "± 364851.42595822783"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3104463.3515625,
            "unit": "ns",
            "range": "± 370171.9149632305"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6529090.611979167,
            "unit": "ns",
            "range": "± 52891.55849366873"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 9452039.067708334,
            "unit": "ns",
            "range": "± 648908.1308427032"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 8032588.291666667,
            "unit": "ns",
            "range": "± 134316.05116732215"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 52131865.333333336,
            "unit": "ns",
            "range": "± 496770.10636745574"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 80869916.16666667,
            "unit": "ns",
            "range": "± 12264471.070562005"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 53392114.77777777,
            "unit": "ns",
            "range": "± 4141672.1067910134"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 565933.6100260416,
            "unit": "ns",
            "range": "± 8501.040248643269"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1496866.93359375,
            "unit": "ns",
            "range": "± 50385.96984737413"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2030192.8151041667,
            "unit": "ns",
            "range": "± 31825.907301786847"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5852555.151041667,
            "unit": "ns",
            "range": "± 14099.61071267944"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 48516075.333333336,
            "unit": "ns",
            "range": "± 178254.24248792688"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 484528038,
            "unit": "ns",
            "range": "± 4607865.672395735"
          }
        ]
      }
    ]
  }
}