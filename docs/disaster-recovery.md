# Disaster Recovery

Runbook for compromise scenarios affecting Wolfgang.Etl.Csv's NuGet package, GitHub repository, or maintainer credentials. Designed to be readable end-to-end during an incident — most of the value comes from the first 30 seconds of clarity when the worst has happened.

## When to use this

Trigger the relevant section the moment you suspect — not when you can prove — that one of these is true:

- Your NuGet.org API key has leaked or been used by anyone else
- Your GitHub maintainer account is compromised (suspicious login, unfamiliar PAT, MFA bypassed)
- An unauthorized NuGet package has been published under `Wolfgang.Etl.*`
- A malicious commit, tag, or release has appeared in `Chris-Wolfgang/ETL-Csv` (or any sibling repo) that you did not author
- The `gh-pages` branch contains content you didn't deploy

False positives are cheap. Investigation while the attacker still has access is not.

---

## Scenario 1: NuGet API key compromised

**Detection signals:**
- NuGet.org email about a new push you didn't make
- New version visible at https://www.nuget.org/packages/Wolfgang.Etl.Csv/ that doesn't match any GitHub Release
- `release.yaml` succeeded but you didn't tag a release
- A consumer reports installing a package version you didn't ship

**Immediate actions (first 10 minutes):**

1. **Revoke the leaked key first, before anything else.**
   - Go to https://www.nuget.org/account/apikeys → find the key labeled `ETL-Csv` (or whatever you used) → click **Delete**.
   - If you can't identify which key, delete **all** keys on the account. Re-create only the one you need next.
2. **Rotate the GitHub secret.**
   - Settings → Secrets and variables → Actions → `NUGET_API_KEY` → Update with a fresh key.
3. **Audit recent pushes.**
   - `nuget.exe list Wolfgang.Etl.Csv -AllVersions -PreRelease` (or check the package's Versions page on NuGet.org)
   - Compare to `git tag` and `gh release list --repo Chris-Wolfgang/ETL-Csv`. Any NuGet version without a matching tag and release is unauthorized.

**Containment (next hour):**

1. **Unlist (do not delete) any unauthorized package versions.**
   - Find the package on NuGet.org → Manage → Listing → uncheck **List in search results**.
   - NuGet does **not** allow deletion after 72 hours of upload, and even within 72 hours, deletion does not protect consumers who already restored the package. Unlisting prevents new installs without breaking restore for existing lockfiles that pinned a known-good version.
2. **Publish a security advisory.**
   - https://github.com/Chris-Wolfgang/ETL-Csv/security/advisories → New draft
   - Include: which versions are affected, what to look for, what to upgrade to, what the malicious payload did (if known)
3. **Yank the consumer-side recommendation.**
   - Update `README.md` to recommend the latest known-good version explicitly
   - Add a `[!WARNING]` callout at the top of README for as long as the bad version remains discoverable

**Recovery (next day):**

1. Ship a clean patch release with the version number immediately after the compromised one (e.g. if `0.1.5` was malicious, ship `0.1.6` — don't reuse `0.1.5`).
2. Email any known downstream consumers if you have a way to.
3. Post-mortem the leak: how did the key escape? CI logs? Local clipboard? Browser extension? Address the root cause before generating a new key.

---

## Scenario 2: GitHub maintainer account compromised

**Detection signals:**
- GitHub email about a new SSH key, PAT, or OAuth app authorization you didn't create
- Commits in `Chris-Wolfgang/ETL-Csv` (or sibling repos) authored as you but pushed from an unfamiliar location
- Workflow runs triggered by `workflow_dispatch` that you didn't trigger
- New collaborators on repos you own

**Immediate actions (first 10 minutes):**

1. **Force a session reset.**
   - https://github.com/settings/sessions → Revoke all sessions
   - Sign out everywhere — including any GitHub Mobile devices
2. **Rotate the account password.**
   - Use a fresh password not derived from any previous one
3. **Re-enroll MFA from a known-clean device.**
   - https://github.com/settings/two_factor_authentication → Reconfigure
   - If the existing TOTP seed may have leaked, delete and regenerate
4. **Audit credentials.**
   - https://github.com/settings/keys → review all SSH and GPG keys; delete any you don't recognize
   - https://github.com/settings/tokens → revoke **all** PATs; you'll re-create the ones you actually need
   - https://github.com/settings/applications → review OAuth apps; revoke any you don't recognize

**Containment (next hour):**

1. **Pause CI on `main` for sibling repos until audit is complete.**
   - Settings → Actions → General → Disable Actions for any repo where unauthorized commits or workflow runs occurred
   - This stops a half-completed attack from continuing to deploy/publish while you investigate
2. **Force-push-protect the default branches.**
   - Verify the "Protect main branch" ruleset (or equivalent on sibling repos) is intact and `block_force_pushes: true`
3. **Audit recent commits across all owned repos.**
   ```bash
   for r in $(gh repo list Chris-Wolfgang --json name --jq '.[].name'); do
     gh api "repos/Chris-Wolfgang/$r/commits?since=$(date -d '7 days ago' --iso-8601)" \
       --jq '.[] | "\(.commit.author.date) \(.commit.author.name) \(.sha[0:7]) \(.commit.message | split("\n")[0])"'
   done
   ```
4. **Audit recently published NuGet releases** — see Scenario 1 (the attacker may have used the API key during the window they had access).

**Recovery (next day):**

1. Re-create only the PATs and SSH keys you actively need. Scope each to the minimum permissions.
2. Re-enable Actions on the paused repos.
3. Publish a security advisory disclosing the timeline if any consumer-facing code or package was affected.
4. Post-mortem: how was the account compromised? Reused password? Phished MFA? Stolen session token? Address the root cause.

---

## Scenario 3: Malicious commit / tag / release in the repository

**Detection signals:**
- Unfamiliar commits visible in `git log` or the GitHub Activity tab
- A tag you didn't create (verify with `git tag --list 'v*'` against `gh release list`)
- A release published by an unfamiliar actor
- `gh-pages` showing content that doesn't match the last successful `docfx.yaml` deploy

**Immediate actions:**

1. **Don't delete anything yet.** Forensic record matters more than fast cleanup.
2. **Take a screenshot or full log dump.**
   - `gh run list --repo Chris-Wolfgang/ETL-Csv --created '>=YYYY-MM-DD' > runs.txt`
   - `gh release list --repo Chris-Wolfgang/ETL-Csv > releases.txt`
   - `git log --all --pretty=fuller > log.txt` (preserve author/committer metadata)
3. **Verify the attacker's current access is severed** — follow Scenario 2 actions before doing repo-level cleanup.
4. **Roll back `main`.**
   - Identify the last known-good commit you trust: `git log --pretty='%H %an %ae %s' | head -50`
   - If branch protection blocks the rollback, temporarily relax the ruleset (admin override), reset, force-push, then re-enable protection.
5. **Delete unauthorized tags and releases.**
   - `gh release delete vX.Y.Z --repo Chris-Wolfgang/ETL-Csv --cleanup-tag`
6. **Unlist unauthorized NuGet versions** — see Scenario 1.

**Recovery:**

- Same as Scenarios 1 and 2 depending on what was affected.
- Publish a security advisory.

---

## Preventive measures (not reactive, but mentioned here so they're remembered)

- `NUGET_API_KEY` should be scoped to **a single package glob** (`Wolfgang.Etl.Csv`), with **a short expiration** (1 year max), and rotated proactively at every renewal — not just at compromise.
- The maintainer GitHub account should require **hardware-backed MFA** (a security key, not just TOTP).
- `pr.yaml` already runs all workflows from `main` via `pull_request_target` to prevent malicious PR-authored workflow modifications from being trusted; preserve that.
- All third-party GitHub Actions in `.github/workflows/` are **SHA-pinned**, not tag-pinned. If a popular action's GitHub repo is compromised, our workflows continue running the known commit, not whatever the attacker has retagged as the "latest".
- Dependabot is enabled (`nuget` + `github-actions` ecosystems) so a known-vulnerable transitive dependency surfaces as an automated PR instead of waiting for manual discovery.

---

## Who to contact

- **NuGet.org security team:** account@nuget.org (response: ~1 business day)
- **GitHub security team:** https://github.com/contact (escalate to `security@github.com` for active compromise)
- **OSSF supply-chain security team:** https://openssf.org/community/ — best for cross-ecosystem coordination if the incident has fleet-wide implications

If multiple repos in the `Chris-Wolfgang/*` org are affected, treat it as a coordinated supply-chain incident and open a single security advisory at the org level rather than per-repo.
