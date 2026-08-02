#!/usr/bin/env python3
"""Compare two sets of BenchmarkDotNet JSON reports and emit a markdown delta table.

Usage: bdn_delta.py <base_dir> <head_dir>

Reads every *-report-full-compressed.json under each directory, aggregates the
Benchmarks arrays by FullName, and prints a markdown table (to stdout) of the
per-benchmark Mean (time) and allocation deltas of head vs base. Prefixed with a
stable HTML marker so the workflow can find and replace its own comment.
"""
import glob
import json
import os
import sys

MARKER = "<!-- pr-benchmarks-delta -->"
TIME_WARN = 10.0   # % slower on Mean → flag
ALLOC_WARN = 20.0  # % more allocated → flag


def load(directory):
    """FullName -> (mean_ns, alloc_bytes) aggregated across all report files."""
    out = {}
    for path in glob.glob(os.path.join(directory, "*.json")):
        with open(path, encoding="utf-8-sig") as handle:
            data = json.load(handle)
        for bench in data.get("Benchmarks", []):
            name = bench.get("FullName")
            if not name:
                continue
            mean = (bench.get("Statistics") or {}).get("Mean")
            alloc = (bench.get("Memory") or {}).get("BytesAllocatedPerOperation")
            out[name] = (mean, alloc)
    return out


def short_name(full):
    # Drop the namespace/assembly prefix; keep Class.Method for readability.
    tail = full.split(".")
    return ".".join(tail[-2:]) if len(tail) >= 2 else full


def pct(base, head):
    if base is None or head is None:
        return None
    if base == 0:
        return None
    return (head - base) / base * 100.0


def fmt_delta(p):
    if p is None:
        return "—"
    sign = "+" if p >= 0 else ""
    return f"{sign}{p:.1f}%"


def main():
    # Markdown output carries emoji; force UTF-8 so it encodes on any host
    # (CI runners are UTF-8; a Windows console defaults to cp1252).
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")

    base = load(sys.argv[1])
    head = load(sys.argv[2])

    names = sorted(set(base) | set(head), key=short_name)

    rows = []
    flagged = False
    for name in names:
        b_mean, b_alloc = base.get(name, (None, None))
        h_mean, h_alloc = head.get(name, (None, None))

        if name not in base:
            rows.append(f"| `{short_name(name)}` | _new_ | _new_ |")
            continue
        if name not in head:
            rows.append(f"| `{short_name(name)}` | _removed_ | _removed_ |")
            continue

        t = pct(b_mean, h_mean)
        a = pct(b_alloc, h_alloc)
        warn = (t is not None and t > TIME_WARN) or (a is not None and a > ALLOC_WARN)
        flag = " ⚠️" if warn else ""
        flagged = flagged or warn
        rows.append(f"| `{short_name(name)}` | {fmt_delta(t)}{flag} | {fmt_delta(a)} |")

    print(MARKER)
    print("## 📊 Benchmark delta (PR head vs base)")
    print()
    if flagged:
        print(f"⚠️ One or more benchmarks regressed beyond the thresholds "
              f"(time > {TIME_WARN:.0f}%, allocations > {ALLOC_WARN:.0f}%).")
    else:
        print("✅ No benchmark regressed beyond the thresholds "
              f"(time > {TIME_WARN:.0f}%, allocations > {ALLOC_WARN:.0f}%).")
    print()
    print("| Benchmark | Time Δ | Alloc Δ |")
    print("|-----------|--------|---------|")
    if rows:
        print("\n".join(rows))
    else:
        print("| _no benchmarks found_ | — | — |")
    print()
    print("<sub>ShortRun on a noisy GitHub runner — trust double-digit time "
          "changes and allocation changes; smaller deltas are noise.</sub>")


if __name__ == "__main__":
    main()
