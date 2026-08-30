window.BENCHMARK_DATA = {
  "lastUpdate": 1788051494689,
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
          "id": "231fb807d97fb07175bca316e2ee8343a6869cc1",
          "message": "Merge pull request #199 from Chris-Wolfgang/chore/packagevalidation-baseline-0.5.0\n\nBump PackageValidation baseline to 0.5.0",
          "timestamp": "2026-08-11T13:15:01-04:00",
          "tree_id": "00c4efb07bd6da72152b06e5d51be9eb42330c64",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/231fb807d97fb07175bca316e2ee8343a6869cc1"
        },
        "date": 1786468707419,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 5941069.234375,
            "unit": "ns",
            "range": "± 18888.442950304667"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6348851.588541667,
            "unit": "ns",
            "range": "± 391755.81249718514"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1257289.2252604167,
            "unit": "ns",
            "range": "± 45548.281529580425"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1907453.8177083333,
            "unit": "ns",
            "range": "± 256519.4026079958"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1680235.9361979167,
            "unit": "ns",
            "range": "± 5699.897455237129"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 4598055.372395833,
            "unit": "ns",
            "range": "± 6731.663343693227"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 5580369.786458333,
            "unit": "ns",
            "range": "± 113061.9443604849"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5408149.90625,
            "unit": "ns",
            "range": "± 38606.251195308054"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 36889664.547619045,
            "unit": "ns",
            "range": "± 92696.12761811966"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 50945126.185185194,
            "unit": "ns",
            "range": "± 5099143.504242271"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 37909946.48717949,
            "unit": "ns",
            "range": "± 57074.358470803236"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2238740.3489583335,
            "unit": "ns",
            "range": "± 111799.83283027075"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3568855.4088541665,
            "unit": "ns",
            "range": "± 208711.5062374634"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3341349.3828125,
            "unit": "ns",
            "range": "± 316724.5508451339"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6088173.692708333,
            "unit": "ns",
            "range": "± 34183.209607847166"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8361421.145833333,
            "unit": "ns",
            "range": "± 242685.58397296068"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 8249726.838541667,
            "unit": "ns",
            "range": "± 737035.470815478"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 47625929.166666664,
            "unit": "ns",
            "range": "± 3627922.8629170274"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 79004607.66666667,
            "unit": "ns",
            "range": "± 7575404.097987353"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 61247504.5,
            "unit": "ns",
            "range": "± 7317695.757795909"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 404531.2353515625,
            "unit": "ns",
            "range": "± 6880.869068836323"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1628676.4635416667,
            "unit": "ns",
            "range": "± 35848.382746599134"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2112104.6484375,
            "unit": "ns",
            "range": "± 28144.575439374792"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5124520.106770833,
            "unit": "ns",
            "range": "± 13239.220152536735"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 38211760.333333336,
            "unit": "ns",
            "range": "± 364755.0046778952"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 382871762,
            "unit": "ns",
            "range": "± 4268198.114702386"
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
          "id": "45fe2db45a36d68159c62df1f785655c6d53f7d4",
          "message": "Merge pull request #203 from Chris-Wolfgang/vNext\n\nRelease 0.6.0",
          "timestamp": "2026-08-13T13:20:42-04:00",
          "tree_id": "a4e74a38e3cfc4f8e9cc634b3716bbca14aacbc7",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/45fe2db45a36d68159c62df1f785655c6d53f7d4"
        },
        "date": 1786641870839,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7180891.997395833,
            "unit": "ns",
            "range": "± 24100.308954892524"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6784463.453125,
            "unit": "ns",
            "range": "± 38272.64089465168"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1205332.2584635417,
            "unit": "ns",
            "range": "± 2034.8520972904494"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1731297.1848958333,
            "unit": "ns",
            "range": "± 91052.27180339783"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1437225.693359375,
            "unit": "ns",
            "range": "± 7897.019890830331"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5270574.338541667,
            "unit": "ns",
            "range": "± 15891.102968391535"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8260083.921875,
            "unit": "ns",
            "range": "± 246344.6967203911"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5647794.171875,
            "unit": "ns",
            "range": "± 60638.24015132665"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 46842935.80555555,
            "unit": "ns",
            "range": "± 1740804.1917103317"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 71594249.33333333,
            "unit": "ns",
            "range": "± 15306547.123321038"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 48459074.21212121,
            "unit": "ns",
            "range": "± 1014623.24939318"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2222202.8177083335,
            "unit": "ns",
            "range": "± 43020.2750888546"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 2993751.1536458335,
            "unit": "ns",
            "range": "± 83747.70042549048"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3024781.8723958335,
            "unit": "ns",
            "range": "± 334246.54555342055"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6607926.450520833,
            "unit": "ns",
            "range": "± 34957.434094269855"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 10002417,
            "unit": "ns",
            "range": "± 283237.9248043849"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7305972.671875,
            "unit": "ns",
            "range": "± 183127.8309439744"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 50879428.333333336,
            "unit": "ns",
            "range": "± 1311265.644659801"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 78269844.66666667,
            "unit": "ns",
            "range": "± 11052979.470311945"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 54076988.88888889,
            "unit": "ns",
            "range": "± 3481800.6318697357"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 579560.9986979166,
            "unit": "ns",
            "range": "± 7812.790711164066"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1521744.255859375,
            "unit": "ns",
            "range": "± 38131.6181649743"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2099088.3854166665,
            "unit": "ns",
            "range": "± 69474.19318121651"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5810420.005208333,
            "unit": "ns",
            "range": "± 15047.404563533644"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 47036249.57575757,
            "unit": "ns",
            "range": "± 221456.7990032412"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 473406409,
            "unit": "ns",
            "range": "± 11492166.831266286"
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
          "id": "c29b4859e35f3597c0dfbe3a1ff388d2e2796ecb",
          "message": "Merge pull request #214 from Chris-Wolfgang/chore/baseline-0.6.0\n\nchore(release): advance PackageValidation baseline to 0.6.0",
          "timestamp": "2026-08-14T14:31:52-04:00",
          "tree_id": "a13c81482c7477d3a0266c3a1c7c4683e29f1747",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/c29b4859e35f3597c0dfbe3a1ff388d2e2796ecb"
        },
        "date": 1786732530254,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7063680.544270833,
            "unit": "ns",
            "range": "± 24444.664251265058"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6989690.145833333,
            "unit": "ns",
            "range": "± 16971.084122185828"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1214230.9485677083,
            "unit": "ns",
            "range": "± 9756.391020031411"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1994903.2369791667,
            "unit": "ns",
            "range": "± 48138.79027231931"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1672303.5924479167,
            "unit": "ns",
            "range": "± 45616.794981233434"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5421472.7578125,
            "unit": "ns",
            "range": "± 17778.827360746887"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8083897.807291667,
            "unit": "ns",
            "range": "± 805828.208605756"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5693793.010416667,
            "unit": "ns",
            "range": "± 66142.81485425502"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 46916328.424242415,
            "unit": "ns",
            "range": "± 120432.5910951317"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 88737175.83333333,
            "unit": "ns",
            "range": "± 3597970.907999894"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 49104785.1,
            "unit": "ns",
            "range": "± 544359.3180534076"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2347975.0520833335,
            "unit": "ns",
            "range": "± 231621.18839359976"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3337529.8958333335,
            "unit": "ns",
            "range": "± 836109.8736584304"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2937925.484375,
            "unit": "ns",
            "range": "± 159530.90141770517"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6476533.520833333,
            "unit": "ns",
            "range": "± 11266.828583543507"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8971223.817708334,
            "unit": "ns",
            "range": "± 877266.0278284923"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7237549.59375,
            "unit": "ns",
            "range": "± 45639.14602273279"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 51776350.666666664,
            "unit": "ns",
            "range": "± 1126832.69887341"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 109751611.33333333,
            "unit": "ns",
            "range": "± 45407510.16237484"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 50203331.666666664,
            "unit": "ns",
            "range": "± 408177.3484248105"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 560301.0891927084,
            "unit": "ns",
            "range": "± 19466.161681276786"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1506112.0533854167,
            "unit": "ns",
            "range": "± 37345.49127080219"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2056431.875,
            "unit": "ns",
            "range": "± 23295.52827325382"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6301804.197916667,
            "unit": "ns",
            "range": "± 9047.89509743079"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 48333972.42424243,
            "unit": "ns",
            "range": "± 371252.6189660205"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 467936386.6666667,
            "unit": "ns",
            "range": "± 3958542.409567735"
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
          "id": "dbe1e768de3adcf01bb64b988c882513c48d9ffe",
          "message": "Merge pull request #204 from Chris-Wolfgang/chore/inspectcode-noise-floor\n\nchore(inspectcode): retire 7 remaining alerts on main",
          "timestamp": "2026-08-18T13:16:45-04:00",
          "tree_id": "5b2fd03ad4b993bb9ded021acc9602b2d6ae9930",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/dbe1e768de3adcf01bb64b988c882513c48d9ffe"
        },
        "date": 1787073634605,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6826801.8984375,
            "unit": "ns",
            "range": "± 26859.33373667592"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6669521.854166667,
            "unit": "ns",
            "range": "± 57486.35098317459"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1271988.4765625,
            "unit": "ns",
            "range": "± 48142.27469005191"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1725425.7265625,
            "unit": "ns",
            "range": "± 112706.945009115"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1598937.5546875,
            "unit": "ns",
            "range": "± 99647.60919353359"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5153708.372395833,
            "unit": "ns",
            "range": "± 5054.909147486153"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 6881118.067708333,
            "unit": "ns",
            "range": "± 435381.82900634944"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5577055.322916667,
            "unit": "ns",
            "range": "± 51964.47677578477"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 44855837.30303031,
            "unit": "ns",
            "range": "± 208137.73527202447"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 85343679,
            "unit": "ns",
            "range": "± 1790085.1413888112"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 48266960.48484849,
            "unit": "ns",
            "range": "± 658911.6148932867"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2316614.2942708335,
            "unit": "ns",
            "range": "± 121863.3452941131"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3300119.4817708335,
            "unit": "ns",
            "range": "± 410103.9717886398"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2966844.1432291665,
            "unit": "ns",
            "range": "± 187257.1866865047"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6511928.934895833,
            "unit": "ns",
            "range": "± 29065.342404046667"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8109597.75,
            "unit": "ns",
            "range": "± 356459.9083021389"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6906071.3046875,
            "unit": "ns",
            "range": "± 99597.07301403068"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 44962335.36363637,
            "unit": "ns",
            "range": "± 1046891.4678653765"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 74123995.83333333,
            "unit": "ns",
            "range": "± 2572851.4318932374"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 46824216.81818182,
            "unit": "ns",
            "range": "± 1212140.5953844567"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 523275.6302083333,
            "unit": "ns",
            "range": "± 18208.67294979087"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1516513.90234375,
            "unit": "ns",
            "range": "± 13855.580592361546"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1953107.9270833333,
            "unit": "ns",
            "range": "± 69791.78185112205"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5797440.888020833,
            "unit": "ns",
            "range": "± 18511.270500918807"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45723572.878787875,
            "unit": "ns",
            "range": "± 153924.2604716062"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 450744412,
            "unit": "ns",
            "range": "± 9636860.521988891"
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
          "id": "d0610ebe3fde92b06e4f97c3ad7c9243ee432003",
          "message": "Merge pull request #227 from Chris-Wolfgang/protected/release-0.6.1-workflow-hardening\n\nchore(security): protected-file split ahead of the 0.6.1 release PR",
          "timestamp": "2026-08-20T22:02:19-04:00",
          "tree_id": "22b78c208c42397d1742c2063c3b52c35f06cd94",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/d0610ebe3fde92b06e4f97c3ad7c9243ee432003"
        },
        "date": 1787277974664,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6930492.705729167,
            "unit": "ns",
            "range": "± 65207.32080107152"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6576913.854166667,
            "unit": "ns",
            "range": "± 9696.099805955064"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1284241.6588541667,
            "unit": "ns",
            "range": "± 74726.50939929792"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 2347383.0885416665,
            "unit": "ns",
            "range": "± 773526.7475069127"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1664866.8333333333,
            "unit": "ns",
            "range": "± 139249.83448572533"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5455706.6640625,
            "unit": "ns",
            "range": "± 16269.812096328627"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8798096.671875,
            "unit": "ns",
            "range": "± 418477.65639826225"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5786900.354166667,
            "unit": "ns",
            "range": "± 92348.79738057012"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 45388666.44444445,
            "unit": "ns",
            "range": "± 102061.97321304663"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 73769748.66666667,
            "unit": "ns",
            "range": "± 5174447.09900009"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 47970764.76666667,
            "unit": "ns",
            "range": "± 413965.8190348721"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2315998.7135416665,
            "unit": "ns",
            "range": "± 139442.0409006644"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3155126.3098958335,
            "unit": "ns",
            "range": "± 51628.10744909964"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2899905.0338541665,
            "unit": "ns",
            "range": "± 142935.87443186354"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6736658.484375,
            "unit": "ns",
            "range": "± 30153.22708898777"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8870486.411458334,
            "unit": "ns",
            "range": "± 879363.7527754531"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7176172.15625,
            "unit": "ns",
            "range": "± 136067.44709243174"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 51780206.833333336,
            "unit": "ns",
            "range": "± 659898.67934637"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 77658149.83333333,
            "unit": "ns",
            "range": "± 14682411.349942908"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 48658712.73333333,
            "unit": "ns",
            "range": "± 916404.196349848"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 582719.09765625,
            "unit": "ns",
            "range": "± 51284.24669872663"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1526144.1236979167,
            "unit": "ns",
            "range": "± 29664.080184891507"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1904948.4544270833,
            "unit": "ns",
            "range": "± 28432.05158153623"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5790666.53125,
            "unit": "ns",
            "range": "± 51215.96706273904"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 47494976.151515156,
            "unit": "ns",
            "range": "± 1055433.925403218"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 445588130.3333333,
            "unit": "ns",
            "range": "± 2029686.7311150096"
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
          "id": "e71bb5a18a2d8248476b4ae4e6b2000c6fe66729",
          "message": "Merge pull request #225 from Chris-Wolfgang/vNext\n\nchore(release): prepare 0.6.1",
          "timestamp": "2026-08-20T22:38:03-04:00",
          "tree_id": "72ada5d47a91a80b5938975fd867ae5be8a352ba",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/e71bb5a18a2d8248476b4ae4e6b2000c6fe66729"
        },
        "date": 1787280126657,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 4356539.182291667,
            "unit": "ns",
            "range": "± 15975.509457612208"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 4371143.1875,
            "unit": "ns",
            "range": "± 12444.580501542629"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1100138.9934895833,
            "unit": "ns",
            "range": "± 138971.5348372591"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1186164.8815104167,
            "unit": "ns",
            "range": "± 62989.530514299644"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1399799.7252604167,
            "unit": "ns",
            "range": "± 137147.36429466645"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 3335673.86328125,
            "unit": "ns",
            "range": "± 13148.973771969266"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 4299361.885416667,
            "unit": "ns",
            "range": "± 171065.58393101877"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 4852599.458333333,
            "unit": "ns",
            "range": "± 159899.335772693"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 30131038.260416668,
            "unit": "ns",
            "range": "± 13665.181083621874"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 39365435.7948718,
            "unit": "ns",
            "range": "± 1344359.8947912895"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 28593607.6875,
            "unit": "ns",
            "range": "± 69281.80345762386"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 1601333.640625,
            "unit": "ns",
            "range": "± 83842.16994644483"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 2301146.4947916665,
            "unit": "ns",
            "range": "± 107444.62827139188"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3886968.4635416665,
            "unit": "ns",
            "range": "± 2553069.2221407834"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 4271049.356770833,
            "unit": "ns",
            "range": "± 14483.426727749773"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8921853.933333334,
            "unit": "ns",
            "range": "± 2867739.1874382887"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 9737962.247395834,
            "unit": "ns",
            "range": "± 2008019.1044929654"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 32109311.333333332,
            "unit": "ns",
            "range": "± 265303.83693657885"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 83446671.55555557,
            "unit": "ns",
            "range": "± 7341021.890463337"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 93357367.66666667,
            "unit": "ns",
            "range": "± 32333759.80883737"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 360026.9134114583,
            "unit": "ns",
            "range": "± 10892.780660312583"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1252129.49609375,
            "unit": "ns",
            "range": "± 5589.000488629664"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1461820.9778645833,
            "unit": "ns",
            "range": "± 12798.82639635719"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 4258766.244791667,
            "unit": "ns",
            "range": "± 31365.795847362908"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 34413656.833333336,
            "unit": "ns",
            "range": "± 1847766.590166763"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 279987937.3333333,
            "unit": "ns",
            "range": "± 1775733.6369130178"
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
          "id": "1bd87e6b43d5ddc1cdecd996ca286b90523f83bf",
          "message": "Merge pull request #228 from Chris-Wolfgang/chore/packagevalidation-baseline-0.6.1\n\nchore(release): advance PackageValidation baseline to 0.6.1",
          "timestamp": "2026-08-21T12:20:14-04:00",
          "tree_id": "c7784779a0e89f316c712ff29e520779a5526045",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/1bd87e6b43d5ddc1cdecd996ca286b90523f83bf"
        },
        "date": 1787329443457,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6585921.598958333,
            "unit": "ns",
            "range": "± 7994.664520436856"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6450900.505208333,
            "unit": "ns",
            "range": "± 6334.058347304202"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1218326.640625,
            "unit": "ns",
            "range": "± 1622.7215036180087"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1990752.2838541667,
            "unit": "ns",
            "range": "± 194580.95776371064"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1605115.4856770833,
            "unit": "ns",
            "range": "± 7358.875562050229"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5110795.078125,
            "unit": "ns",
            "range": "± 9262.439629898916"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 7490356.71875,
            "unit": "ns",
            "range": "± 577891.8532545867"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5635222.088541667,
            "unit": "ns",
            "range": "± 23231.2138681188"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 44724045,
            "unit": "ns",
            "range": "± 204840.8960651433"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 57676577.88888889,
            "unit": "ns",
            "range": "± 668408.6384659741"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 45388168.54545454,
            "unit": "ns",
            "range": "± 176292.1427734703"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2251884.6380208335,
            "unit": "ns",
            "range": "± 18005.663590967553"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3263835.2890625,
            "unit": "ns",
            "range": "± 353494.02086997114"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3125298.9635416665,
            "unit": "ns",
            "range": "± 632932.8849150959"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6586292.416666667,
            "unit": "ns",
            "range": "± 3694.548879621097"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8750341.90625,
            "unit": "ns",
            "range": "± 889682.2801854528"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6913522.5,
            "unit": "ns",
            "range": "± 101644.73689388022"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 45436680.916666664,
            "unit": "ns",
            "range": "± 754338.7847560232"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 84073244,
            "unit": "ns",
            "range": "± 4378013.964608952"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 49994645.666666664,
            "unit": "ns",
            "range": "± 3211243.923669475"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 537317.8590494791,
            "unit": "ns",
            "range": "± 15194.742973413953"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1544951.6842447917,
            "unit": "ns",
            "range": "± 14937.596301594653"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2085520.5989583333,
            "unit": "ns",
            "range": "± 13761.747985586524"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6213576.588541667,
            "unit": "ns",
            "range": "± 15082.12779764312"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45184911.63636363,
            "unit": "ns",
            "range": "± 154742.96472089566"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 428722313.3333333,
            "unit": "ns",
            "range": "± 2226869.1919096494"
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
          "id": "adc9b0307e04832ef0012db21643f50dbaa46fc1",
          "message": "Merge pull request #230 from Chris-Wolfgang/dependabot/nuget/dotnet-dependencies-1710a68599\n\nBump the dotnet-dependencies group with 5 updates",
          "timestamp": "2026-08-21T17:56:02-04:00",
          "tree_id": "435bd16b9c12ecad039f7cc1b48239335f02ee57",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/adc9b0307e04832ef0012db21643f50dbaa46fc1"
        },
        "date": 1787349567738,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 5292956.432291667,
            "unit": "ns",
            "range": "± 19225.211138266684"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 5268357.515625,
            "unit": "ns",
            "range": "± 47066.556684414434"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1108640.67578125,
            "unit": "ns",
            "range": "± 66130.1046469191"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1523989.0130208333,
            "unit": "ns",
            "range": "± 55254.284882162836"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1507271.7721354167,
            "unit": "ns",
            "range": "± 68471.82294908859"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 3926981.4661458335,
            "unit": "ns",
            "range": "± 10515.631754793096"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 4807009.723958333,
            "unit": "ns",
            "range": "± 144557.38297954778"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 4623659.7421875,
            "unit": "ns",
            "range": "± 49490.15818465723"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 33515592.645833332,
            "unit": "ns",
            "range": "± 139937.67019284444"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 41399136.416666664,
            "unit": "ns",
            "range": "± 884113.834999593"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 32826733.1875,
            "unit": "ns",
            "range": "± 225823.37946491086"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2398393.8489583335,
            "unit": "ns",
            "range": "± 495634.00152239174"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3744061.4661458335,
            "unit": "ns",
            "range": "± 1093836.3855696677"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3159642.6302083335,
            "unit": "ns",
            "range": "± 770854.0839141713"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 4980301.619791667,
            "unit": "ns",
            "range": "± 12397.992888278175"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 10141960.138888888,
            "unit": "ns",
            "range": "± 1948990.7937794628"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 5524250.434895833,
            "unit": "ns",
            "range": "± 112359.9168438239"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 37676073.666666664,
            "unit": "ns",
            "range": "± 391480.887455712"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 54381546.88888889,
            "unit": "ns",
            "range": "± 6443821.856038839"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 43647160.97222222,
            "unit": "ns",
            "range": "± 825280.4957121676"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 332607.0813802083,
            "unit": "ns",
            "range": "± 10389.762016041988"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1440456.3600260417,
            "unit": "ns",
            "range": "± 14375.269003622943"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1687431.2825520833,
            "unit": "ns",
            "range": "± 25789.100905778334"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 4316841.375,
            "unit": "ns",
            "range": "± 20813.867173158964"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 32624418.622222226,
            "unit": "ns",
            "range": "± 84988.89372425458"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 319372872.5,
            "unit": "ns",
            "range": "± 1184734.4611220905"
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
          "id": "0b1079a5bf2cf50ed898f695d5fef4c5f1e81e55",
          "message": "Merge pull request #235 from Chris-Wolfgang/vNext\n\nRelease v0.7.0 — CsvValidationResult two-ctor API + alert cleanup",
          "timestamp": "2026-08-22T12:27:36-04:00",
          "tree_id": "a236d207ef5516c98e21044870fc38a43b582dde",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/0b1079a5bf2cf50ed898f695d5fef4c5f1e81e55"
        },
        "date": 1787416272931,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7036003.864583333,
            "unit": "ns",
            "range": "± 51207.539159631924"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6556153.505208333,
            "unit": "ns",
            "range": "± 44455.59905309951"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1217003.3411458333,
            "unit": "ns",
            "range": "± 2115.9047040900773"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1708430.7018229167,
            "unit": "ns",
            "range": "± 113413.98524414352"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1735226.1015625,
            "unit": "ns",
            "range": "± 55227.74957835013"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5250802.783854167,
            "unit": "ns",
            "range": "± 75719.67978017489"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 6743449.416666667,
            "unit": "ns",
            "range": "± 70506.7572657624"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5630953.440104167,
            "unit": "ns",
            "range": "± 84733.71281738127"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 44350563.38888889,
            "unit": "ns",
            "range": "± 70767.32058009361"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 62819908.25,
            "unit": "ns",
            "range": "± 2718982.243995079"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 47599704.696969695,
            "unit": "ns",
            "range": "± 700782.6607046316"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2359860.7005208335,
            "unit": "ns",
            "range": "± 150940.2047436689"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3139561.6744791665,
            "unit": "ns",
            "range": "± 649700.3328769113"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3273002.5729166665,
            "unit": "ns",
            "range": "± 726102.4566445127"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 7015145.489583333,
            "unit": "ns",
            "range": "± 648810.5485442017"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 7845623.234375,
            "unit": "ns",
            "range": "± 167601.26039176277"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7127629.510416667,
            "unit": "ns",
            "range": "± 106084.21979494077"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 50982763.666666664,
            "unit": "ns",
            "range": "± 4477655.225852598"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 67360607.66666667,
            "unit": "ns",
            "range": "± 861663.529748769"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 52089780.55555555,
            "unit": "ns",
            "range": "± 3053443.2956213043"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 514650.095703125,
            "unit": "ns",
            "range": "± 12829.579733439352"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1533874.4375,
            "unit": "ns",
            "range": "± 27502.465487575453"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1954025.8020833333,
            "unit": "ns",
            "range": "± 42927.005059890565"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6270636.640625,
            "unit": "ns",
            "range": "± 6046.664333585032"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 46182491.696969695,
            "unit": "ns",
            "range": "± 357603.26224319875"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 441945063.3333333,
            "unit": "ns",
            "range": "± 11736967.730877612"
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
          "id": "caae8d4dbf560666d9764add440c66b362262971",
          "message": "Merge pull request #238 from Chris-Wolfgang/chore/baseline-0.7.0\n\nchore(pack): advance PackageValidation baseline to 0.7.0",
          "timestamp": "2026-08-22T14:53:54-04:00",
          "tree_id": "7202bbe419a71b3b1cc248919a167b99c108f935",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/caae8d4dbf560666d9764add440c66b362262971"
        },
        "date": 1787425052325,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7635613.981770833,
            "unit": "ns",
            "range": "± 35221.28357027072"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 7018402.005208333,
            "unit": "ns",
            "range": "± 130991.99178905871"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1290327.3151041667,
            "unit": "ns",
            "range": "± 76035.63543371779"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1920680.8177083333,
            "unit": "ns",
            "range": "± 101223.30663558526"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1702364.1119791667,
            "unit": "ns",
            "range": "± 146311.52588947385"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5437262.278645833,
            "unit": "ns",
            "range": "± 45061.409822589456"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8583513.614583334,
            "unit": "ns",
            "range": "± 364242.9254918597"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5816784.7421875,
            "unit": "ns",
            "range": "± 11193.06191147179"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 47065450.303030305,
            "unit": "ns",
            "range": "± 70437.37704217549"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 113002652,
            "unit": "ns",
            "range": "± 16655473.435591197"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 49351339.57575757,
            "unit": "ns",
            "range": "± 993707.3594976604"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2249724.0494791665,
            "unit": "ns",
            "range": "± 47815.94852447569"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3108113.5208333335,
            "unit": "ns",
            "range": "± 65258.20278141589"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3058026.1015625,
            "unit": "ns",
            "range": "± 494573.47670299374"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6871203.638020833,
            "unit": "ns",
            "range": "± 64610.55561180641"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 9664515.682291666,
            "unit": "ns",
            "range": "± 666075.8706140332"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7700762.958333333,
            "unit": "ns",
            "range": "± 515980.97955404455"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 51004131,
            "unit": "ns",
            "range": "± 584647.2279766236"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 99001981.66666667,
            "unit": "ns",
            "range": "± 7872342.971497452"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 54942459.111111104,
            "unit": "ns",
            "range": "± 1484081.9969944137"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 549446.4498697916,
            "unit": "ns",
            "range": "± 11086.026817941854"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1540510.458984375,
            "unit": "ns",
            "range": "± 53723.11747372426"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1953229.16015625,
            "unit": "ns",
            "range": "± 40609.34253322115"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6172652.5234375,
            "unit": "ns",
            "range": "± 42882.45937200105"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 49203397.03030303,
            "unit": "ns",
            "range": "± 275877.64532392623"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 472758791.6666667,
            "unit": "ns",
            "range": "± 5809410.16309077"
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
          "id": "239a9f0324d67f70b30ef4ea5e8aaefa8932dec1",
          "message": "Merge pull request #244 from Chris-Wolfgang/vNext\n\nRelease v0.7.1 — analyzer noise cleanup (0.7.x line)",
          "timestamp": "2026-08-23T09:54:19-04:00",
          "tree_id": "183ca53789ed1e70ac8b1d550eaed0b64f0b1997",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/239a9f0324d67f70b30ef4ea5e8aaefa8932dec1"
        },
        "date": 1787493481784,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 7216345.5546875,
            "unit": "ns",
            "range": "± 9027.614399992051"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6897818.3984375,
            "unit": "ns",
            "range": "± 30339.010162099337"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1251238.3209635417,
            "unit": "ns",
            "range": "± 6250.973362689195"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1810055.0911458333,
            "unit": "ns",
            "range": "± 43060.93829850299"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1700768.2981770833,
            "unit": "ns",
            "range": "± 47963.471695976696"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5275006.125,
            "unit": "ns",
            "range": "± 18214.356985107115"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 8412469.140625,
            "unit": "ns",
            "range": "± 394581.11631642777"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5880290.908854167,
            "unit": "ns",
            "range": "± 60546.95964535229"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 45206867.36363637,
            "unit": "ns",
            "range": "± 138722.9419776465"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 104190532.33333333,
            "unit": "ns",
            "range": "± 9439149.533975204"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 48686229.90909091,
            "unit": "ns",
            "range": "± 987031.0394779703"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2202131.03125,
            "unit": "ns",
            "range": "± 15344.835969455797"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 2928507.4635416665,
            "unit": "ns",
            "range": "± 149044.4809064487"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 3173736.9947916665,
            "unit": "ns",
            "range": "± 592067.4092506293"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6950334.0859375,
            "unit": "ns",
            "range": "± 170531.86194545648"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 9487175.822916666,
            "unit": "ns",
            "range": "± 336801.87140228326"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 7674322.260416667,
            "unit": "ns",
            "range": "± 411443.8700809739"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 55140382.166666664,
            "unit": "ns",
            "range": "± 4654917.229477725"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 84189882.5,
            "unit": "ns",
            "range": "± 3399963.3341354947"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 54811378.166666664,
            "unit": "ns",
            "range": "± 5815748.549350491"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 578052.9664713541,
            "unit": "ns",
            "range": "± 30822.55604859463"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1500645.7571614583,
            "unit": "ns",
            "range": "± 12952.447794615186"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1907381.8190104167,
            "unit": "ns",
            "range": "± 45870.90303991719"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 6031602.864583333,
            "unit": "ns",
            "range": "± 17191.140545842998"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 47454945.303030305,
            "unit": "ns",
            "range": "± 561089.3193443015"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 465676863,
            "unit": "ns",
            "range": "± 10893680.831477119"
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
          "id": "53332faeaf7abd6a368271e17e7c9d11aa69ee85",
          "message": "Merge pull request #248 from Chris-Wolfgang/chore/baseline-0.7.1\n\nchore(pack): advance PackageValidation baseline to 0.7.1",
          "timestamp": "2026-08-23T15:45:11-04:00",
          "tree_id": "818c534fec0ccf934ec4313e84239827495a8351",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/53332faeaf7abd6a368271e17e7c9d11aa69ee85"
        },
        "date": 1787514538795,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 6665560.6171875,
            "unit": "ns",
            "range": "± 16706.620318108333"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 6666183.2734375,
            "unit": "ns",
            "range": "± 15275.624827764037"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 1213843.263671875,
            "unit": "ns",
            "range": "± 4861.471614820692"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1738679.4713541667,
            "unit": "ns",
            "range": "± 51676.87019931458"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 2097227.5885416665,
            "unit": "ns",
            "range": "± 46779.28708992498"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 5158438.481770833,
            "unit": "ns",
            "range": "± 9590.936760586892"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 7136765.760416667,
            "unit": "ns",
            "range": "± 921084.1251316866"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 5620203.875,
            "unit": "ns",
            "range": "± 17191.999362991697"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 50073169.19444444,
            "unit": "ns",
            "range": "± 4933852.226553261"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 105292747.66666667,
            "unit": "ns",
            "range": "± 19726481.563431866"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 47866860.45454546,
            "unit": "ns",
            "range": "± 623058.9116239543"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2403920.3802083335,
            "unit": "ns",
            "range": "± 114706.42778124992"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 3026681.0286458335,
            "unit": "ns",
            "range": "± 276756.24080291245"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2923751.5078125,
            "unit": "ns",
            "range": "± 214440.46002516043"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 6639918.419270833,
            "unit": "ns",
            "range": "± 82346.48899366816"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8543933.911458334,
            "unit": "ns",
            "range": "± 989592.3092578655"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 6879947.041666667,
            "unit": "ns",
            "range": "± 59376.83843929313"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 45554386.54545454,
            "unit": "ns",
            "range": "± 741077.6163904502"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 72739285.16666667,
            "unit": "ns",
            "range": "± 3700901.093512441"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 51710901.333333336,
            "unit": "ns",
            "range": "± 1301988.1079588758"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 519098.0856119792,
            "unit": "ns",
            "range": "± 33365.168419442925"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1485601.4108072917,
            "unit": "ns",
            "range": "± 7176.408781229326"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 2150414.7473958335,
            "unit": "ns",
            "range": "± 119013.63077819337"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 5737970.966145833,
            "unit": "ns",
            "range": "± 10712.941541077536"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 45750394.30303031,
            "unit": "ns",
            "range": "± 150969.09657084107"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 439655298.3333333,
            "unit": "ns",
            "range": "± 3027670.7271267683"
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
          "id": "b2f2d30ca93c8222575d79cf717e29e8ab04f08c",
          "message": "Merge pull request #265 from Chris-Wolfgang/vNext\n\nRelease v0.8.0 — options-record configuration and setter deprecation",
          "timestamp": "2026-08-29T20:54:48-04:00",
          "tree_id": "307a767160b092cd952e7b9da84c251e5152c344",
          "url": "https://github.com/Chris-Wolfgang/ETL-Csv/commit/b2f2d30ca93c8222575d79cf717e29e8ab04f08c"
        },
        "date": 1788051491991,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Extract_Memory(RecordCount: 10000)",
            "value": 5130812.080729167,
            "unit": "ns",
            "range": "± 6705.70256457456"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.DateTimeBenchmarks.Load_Memory(RecordCount: 10000)",
            "value": 5083090.833333333,
            "unit": "ns",
            "range": "± 21785.660805909523"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 1000)",
            "value": 963185.7141927084,
            "unit": "ns",
            "range": "± 8847.13581301122"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 1000)",
            "value": 1394626.3072916667,
            "unit": "ns",
            "range": "± 208997.17178575805"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 1000)",
            "value": 1408400.2760416667,
            "unit": "ns",
            "range": "± 151088.98961892346"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 10000)",
            "value": 4072472.625,
            "unit": "ns",
            "range": "± 8316.75500361903"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 10000)",
            "value": 4891381.3125,
            "unit": "ns",
            "range": "± 48396.057847874166"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 10000)",
            "value": 4451328.5625,
            "unit": "ns",
            "range": "± 143006.51659052685"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.Memory_TextReader(RecordCount: 100000)",
            "value": 34979309.64444444,
            "unit": "ns",
            "range": "± 345841.90782907413"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_1KB(RecordCount: 100000)",
            "value": 47170219.696969695,
            "unit": "ns",
            "range": "± 1596888.8981091126"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.ExtractorBenchmarks.File_TextReader_64KB(RecordCount: 100000)",
            "value": 36019898.589743584,
            "unit": "ns",
            "range": "± 696483.0313790484"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 1000)",
            "value": 2237284.3333333335,
            "unit": "ns",
            "range": "± 330100.67334654"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 1000)",
            "value": 2637219.484375,
            "unit": "ns",
            "range": "± 461585.3163367923"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 1000)",
            "value": 2912841.6276041665,
            "unit": "ns",
            "range": "± 397897.1315704727"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 10000)",
            "value": 5129825.1640625,
            "unit": "ns",
            "range": "± 24108.394109351193"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 10000)",
            "value": 8893144.119791666,
            "unit": "ns",
            "range": "± 3478535.988460876"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 10000)",
            "value": 15153569.291666666,
            "unit": "ns",
            "range": "± 9466256.589076916"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.Memory_TextWriter(RecordCount: 100000)",
            "value": 35562999.57777778,
            "unit": "ns",
            "range": "± 72641.4220887591"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_1KB(RecordCount: 100000)",
            "value": 74500585.22222222,
            "unit": "ns",
            "range": "± 25135970.361350015"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.LoaderBenchmarks.File_TextWriter_64KB(RecordCount: 100000)",
            "value": 54713959.77777778,
            "unit": "ns",
            "range": "± 1111421.169065003"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 0)",
            "value": 405091.0325520833,
            "unit": "ns",
            "range": "± 12148.509304594829"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1)",
            "value": 1199520.6139322917,
            "unit": "ns",
            "range": "± 12016.211687809175"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000)",
            "value": 1470123.2408854167,
            "unit": "ns",
            "range": "± 10227.968283445076"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 10000)",
            "value": 4473750.421875,
            "unit": "ns",
            "range": "± 31204.05542709128"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 100000)",
            "value": 36131225.21428571,
            "unit": "ns",
            "range": "± 196896.44353590236"
          },
          {
            "name": "Wolfgang.Etl.Csv.Benchmarks.MemoryDeltaBenchmarks.Extract_MemoryDelta(RecordCount: 1000000)",
            "value": 342886216.6666667,
            "unit": "ns",
            "range": "± 849315.9525832146"
          }
        ]
      }
    ]
  }
}