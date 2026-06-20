# Reproducible Build Verification

This document describes how to verify that a Wolfgang.Etl.Csv NuGet package built from a tagged commit on one machine is **byte-for-byte identical** to one built from the same tag on a different machine or at a different time.

Reproducible builds are a supply-chain security property: if a published package's bytes match what reproducing the build yields, that's strong evidence that the published bits weren't substituted between build-time and publish-time.

## What's already in place

The relevant MSBuild properties are set in `Directory.Build.props`:

| Property | Value | Purpose |
|---|---|---|
| `ContinuousIntegrationBuild` | `true` when `$(CI) == 'true'` | Forces deterministic compilation flags (`-deterministic`, fixed source paths) |
| `EmbedUntrackedSources` | `true` | Embeds non-checked-in sources into the PDB so the same SHA produces the same PDB |
| `PublishRepositoryUrl` | `true` | Embeds the repository URL into NuGet metadata |
| `Deterministic` | (implicit `true` on modern SDK) | Removes machine-specific paths and timestamps |

Plus `Microsoft.SourceLink.GitHub` which embeds the commit SHA into PDBs (separate property, also in `Directory.Build.props`).

`release.yaml`'s `pack-and-validate` job runs on a clean GitHub-hosted Ubuntu runner, so the build environment is well-defined.

## Verification recipe (one-shot, run by hand)

This is intentionally a recipe rather than a CI step — verifying reproducibility *automatically* requires running the build twice on the same input and comparing artifacts, which is wasteful to do on every PR. Run it manually after a release if you want supply-chain confidence:

```bash
# 1. Tag and check out the released commit
git checkout v0.1.0

# 2. Pack in CI-equivalent mode locally
dotnet pack src/Wolfgang.Etl.Csv/Wolfgang.Etl.Csv.csproj \
  -c Release \
  --output ./local-pack \
  -p:ContinuousIntegrationBuild=true \
  -p:PublishRepositoryUrl=true

# 3. Download the published package from NuGet.org
curl -sLO https://www.nuget.org/api/v2/package/Wolfgang.Etl.Csv/0.1.0
mv 0.1.0 ./published.nupkg

# 4. Compare byte-by-byte (the .nupkg is a zip; compare the extracted contents,
#    since zip headers can differ in timestamp/order without affecting payload)
mkdir -p ./extracted-local ./extracted-published
unzip -o ./local-pack/Wolfgang.Etl.Csv.0.1.0.nupkg -d ./extracted-local
unzip -o ./published.nupkg -d ./extracted-published
diff -r ./extracted-local ./extracted-published
```

**Expected result:** `diff` reports no differences in the `lib/`, `content/`, and metadata XML.

Differences that are **acceptable and don't break reproducibility:**
- `.nuspec` ordering of `<files>` entries (zip directory order is not normative).
- `.signature.p7s` is only present on the published package (NuGet.org signs at upload time).
- Timestamp metadata inside the zip's central directory (not payload).

Differences that are **not** acceptable:
- Differing `.dll` bytes — indicates non-deterministic compilation or a different toolchain.
- Differing `.pdb` bytes — indicates SourceLink or `EmbedUntrackedSources` regression.
- Differing `README.md` or `icon_256.png` — indicates a different commit was packed.

## When verification fails

A reproducibility regression usually means one of:

1. **Toolchain drift.** The CI runner's .NET SDK patch version differs from the local one. Pin via `global.json` if this becomes a pattern.
2. **`ContinuousIntegrationBuild` not propagating.** Verify `-p:ContinuousIntegrationBuild=true` was actually applied (check the `*.dll`'s embedded build metadata via `dotnet ildasm` or `dnSpy`).
3. **An analyzer or source generator emits non-deterministic output.** Each source generator should be reviewed for deterministic-output compliance. None of our current analyzers (Roslynator, Meziantou, Sonar, etc.) emit source — they're pure analyzers — so this shouldn't be a current concern.

## Why not automate this?

The reproducibility check is most valuable when run by a **different party than the publisher** (e.g. a downstream consumer building the same tag on their own machine). If we automate it in our own CI, we're just asserting "my CI produces the same bytes as my CI," which doesn't add supply-chain value.

If we ever adopt a third-party reproducibility verifier (e.g. `r-b.org` for .NET libraries — equivalent of reproducible-builds.org for Debian), wire it up at that point.

## SLSA + SBOM tracking

Build provenance attestation (SLSA Level 3) and SBOM generation are tracked separately in #71 (supply-chain hardening). This document is the lightweight starting point; #71's scope is the full supply-chain story.
