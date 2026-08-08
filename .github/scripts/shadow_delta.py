#!/usr/bin/env python3
"""Compare a shadow-test report against the committed golden baseline (#63).

Allocation is the HARD gate: a per-scenario allocation increase beyond the
threshold exits non-zero and fails the workflow. Latency is ADVISORY only
(CI wall-clock is noisy and the baseline is machine-dependent), so latency
regressions are reported but never fail the run.

Usage:
    shadow_delta.py <baseline.json> <report.json> <summary.md>

Per-metric thresholds come from the SHADOW_ALLOC_PCT / SHADOW_LATENCY_PCT
environment variables, falling back to the baseline's `thresholds` block.
Exit codes: 0 = within budget, 1 = allocation regression, 2 = bad input.
"""
import json
import os
import sys


def pct_change(current, base):
    if base == 0:
        return float("inf") if current > 0 else 0.0
    return (current - base) / base * 100.0


def main():
    # The summary contains non-ASCII (Δ, status glyphs); Windows consoles default to
    # cp1252 and would crash on the stdout echo. The summary file is always UTF-8.
    try:
        sys.stdout.reconfigure(encoding="utf-8")
    except (AttributeError, ValueError):
        pass

    if len(sys.argv) != 4:
        sys.stderr.write("usage: shadow_delta.py <baseline.json> <report.json> <summary.md>\n")
        return 2

    baseline_path, report_path, summary_path = sys.argv[1:4]
    with open(baseline_path, encoding="utf-8") as handle:
        baseline = json.load(handle)
    with open(report_path, encoding="utf-8") as handle:
        report = json.load(handle)

    thresholds = baseline.get("thresholds", {})
    # `or` (not a default arg) so an empty env var — a workflow input left blank — also
    # falls back to the baseline threshold instead of crashing on float("").
    alloc_pct = float(os.environ.get("SHADOW_ALLOC_PCT") or thresholds.get("allocationPct", 50))
    latency_pct = float(os.environ.get("SHADOW_LATENCY_PCT") or thresholds.get("latencyPct", 20))

    base_scenarios = baseline.get("scenarios", {})
    got_scenarios = report.get("Scenarios", {})

    rows = []
    alloc_regressions = []
    latency_advisories = []
    missing = []

    for name, base in sorted(base_scenarios.items()):
        got = got_scenarios.get(name)
        if got is None:
            missing.append(name)
            rows.append(f"| `{name}` | — | — | **MISSING** |")
            continue

        base_alloc = base["allocatedBytes"]
        got_alloc = got["AllocatedBytes"]
        d_alloc = pct_change(got_alloc, base_alloc)

        base_ms = base.get("elapsedMs", 0)
        got_ms = got.get("ElapsedMs", 0)
        d_ms = pct_change(got_ms, base_ms)

        alloc_flag = ""
        if d_alloc > alloc_pct:
            alloc_flag = " 🔴"
            alloc_regressions.append((name, base_alloc, got_alloc, d_alloc))
        if d_ms > latency_pct:
            latency_advisories.append((name, d_ms))

        rows.append(
            f"| `{name}` | {got_alloc:,} B | {d_alloc:+.1f}%{alloc_flag} | {d_ms:+.1f}% (adv) |"
        )

    lines = [
        "### Shadow-test scenario report (#63)",
        "",
        f"Allocation gate: **> {alloc_pct:.0f}%** fails · Latency: advisory (> {latency_pct:.0f}% noted, never fails)",
        "",
        "| scenario | allocated | Δ alloc vs baseline | Δ latency |",
        "| --- | --- | --- | --- |",
    ]
    lines.extend(rows)
    lines.append("")

    if alloc_regressions:
        lines.append("**Allocation regressions (hard fail):**")
        for name, base_alloc, got_alloc, delta in alloc_regressions:
            lines.append(f"- `{name}`: {base_alloc:,} → {got_alloc:,} B ({delta:+.1f}%)")
        lines.append("")
    if missing:
        lines.append(f"**Missing scenarios (hard fail):** {', '.join(missing)}")
        lines.append("")
    if latency_advisories:
        lines.append("**Latency advisories (informational):**")
        for name, delta in latency_advisories:
            lines.append(f"- `{name}`: {delta:+.1f}% vs local reference")
        lines.append("")
    if not alloc_regressions and not missing:
        lines.append("✅ All scenarios within the allocation budget.")

    summary = "\n".join(lines) + "\n"
    with open(summary_path, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(summary)
    sys.stdout.write(summary)

    return 1 if (alloc_regressions or missing) else 0


if __name__ == "__main__":
    sys.exit(main())
