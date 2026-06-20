# Architecture Decision Records

This directory captures the **why** behind significant architectural choices in Wolfgang.Etl.Csv.

Code comments explain *what* a line of code does and *how* — ADRs explain *why* the larger decision was made, what alternatives were considered, and what the tradeoffs were. They survive refactors, package upgrades, and team turnover.

## When to write an ADR

Write one when:

- A choice locks in a tradeoff that's not obvious from the code
- A choice closes off a path that a future maintainer might reasonably want to take
- The decision was contentious or had non-trivial alternatives
- The rationale lives only in a PR thread, issue comment, or Slack message — places where it'll be hard to find later

Don't write one for tactical choices captured fine by code comments or PR descriptions.

## Format

Each ADR is a single markdown file named `<NNNN>-<short-slug>.md`, where `NNNN` is a 4-digit zero-padded sequence number starting at `0001`. Use `0000-template.md` as the starting point.

## Index

| # | Title | Status |
|---|---|---|
| [0001](0001-csvhelper-as-internal-parser.md) | CsvHelper as the internal parser, parser-agnostic public surface | Accepted |
