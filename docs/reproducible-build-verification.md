# Reproducible Build Verification

This page moved to [REPRODUCIBLE-BUILD.md](REPRODUCIBLE-BUILD.md), the fleet-standard verifier guide: what a
release publishes (`SHA256SUMS.txt`, `BUILD-MANIFEST.txt`), how to rebuild a tag on Linux and compare it entry by
entry, and how to check the provenance attestation. Every pull request is also checked by the
[Reproducible Build workflow](https://github.com/Chris-Wolfgang/ETL-Csv/actions/workflows/reproducible-build.yaml),
which packs the same commit twice on Ubuntu and fails unless both packs are identical.
