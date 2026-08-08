# Wolfgang.Etl.Csv — shadow-testing sample (#63)

A runnable consumer that exercises the public `CsvExtractor` / `CsvLoader` surface under
realistic traffic shapes, not the tidy inputs unit tests use. It doubles as executable
"real usage" documentation, and [`shadow.yaml`](../../.github/workflows/shadow.yaml) replays
it nightly to catch performance / allocation regressions before release.

## Scenarios

| scenario | shape it models |
| --- | --- |
| `round_trip_mixed` | extract → materialize → load → re-extract, over rows of varied width |
| `concurrent_streaming` | N extractors streaming concurrently (parallel request load) |
| `windowed_paging` | paging a large file in fixed windows via `SkipRecordCount` + `MaxRecordCount` |
| `bursty_small` | many tiny back-to-back extract+load cycles (per-call fixed overhead) |

## Running it

```bash
dotnet run -c Release --project samples/Wolfgang.Etl.Csv.Shadow -- shadow-report.json
```

Each scenario is measured for **allocation** (`GC.GetTotalAllocatedBytes(precise)` — process-wide,
so it covers the concurrent scenario's worker threads), GC collections, and wall-clock, then
written to the report JSON.

## The gate

`shadow.yaml` compares the report against [`shadow-baseline.json`](shadow-baseline.json):

- **Allocation is the hard gate.** It is deterministic (measured < 0.1 % run-to-run), so a
  per-scenario increase past the threshold (default **+50 %**) fails the run and opens a tracking
  issue.
- **Latency is advisory.** CI wall-clock is noisy and the baseline is machine-dependent, so
  latency deltas are reported for trend but never fail the run.

Thresholds are configurable per metric via the `SHADOW_ALLOC_PCT` / `SHADOW_LATENCY_PCT`
workflow inputs (blank = the baseline's own thresholds).

## Re-baselining

After an intentional change to allocation behaviour, run the sample and copy each scenario's
report `AllocatedBytes` value into the matching scenario's `allocatedBytes` field in
`shadow-baseline.json` (the report is PascalCase — C# record serialization — while the baseline
is camelCase; `shadow_delta.py` reads each in its own casing). The `elapsedMs` values are a local
reference only.
