# CI Pipeline Documentation (`ci.yml`)

This document describes the **Continuous Integration (CI) pipeline** configured in `.github/workflows/ci.yml`.

## 📋 Overview

**Workflow Name:** `CI/CD Pipeline`  
**File:** `.github/workflows/ci.yml`  
**Purpose:** Automated build, test, and validation on every push and pull request

### Triggers

The workflow runs on:
- ✅ **Push** to `main` or `develop` branches
- ✅ **Pull requests** to `main` or `develop` branches  
- ✅ **Manual trigger** (`workflow_dispatch`)

## 🔄 Workflow Structure

The CI pipeline consists of **5 jobs**:

```
1. build-and-test      (Runs first - required)
   ├── 2. code-coverage     (Runs after build-and-test succeeds)
   ├── 3. docker-build      (Runs after build-and-test succeeds)
   └── 4. code-quality      (Runs in parallel)
5. build-summary       (Runs last - always, even if jobs fail)
```

---

## 📦 Jobs Breakdown

### 1. build-and-test

**Job Name:** `Build & Test`  
**Runs On:** `ubuntu-latest`  
**Purpose:** Compile the solution and run all unit tests

**Steps:**
```yaml
1. Checkout code (actions/checkout@v4)
2. Setup .NET (actions/setup-dotnet@v4)
   - dotnet-version: 9.0.x
3. Restore dependencies
   - Command: dotnet restore
4. Build solution
   - Command: dotnet build --configuration Release --no-restore
5. Run tests
   - Command: dotnet test --configuration Release --no-build --verbosity normal --logger trx --results-directory ./TestResults
```

**Outputs:**
- Test results saved to `./TestResults` directory in TRX format
- Workflow fails if any test fails

**Run Locally:**
```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build --verbosity normal --logger trx --results-directory ./TestResults
```

---

### 2. code-coverage

**Job Name:** `Code Coverage`  
**Runs On:** `ubuntu-latest`  
**Depends On:** `build-and-test` (must succeed)  
**Purpose:** Generate code coverage reports and track coverage metrics

**Steps:**
```yaml
1. Checkout code (actions/checkout@v4)
2. Setup .NET (actions/setup-dotnet@v4)
   - dotnet-version: 9.0.x
3. Restore dependencies
   - Command: dotnet restore
4. Run tests with coverage
   - Command: dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory ./coverage
5. Code Coverage Report (irongut/CodeCoverageSummary@v1.3.0)
   - Input: coverage/**/coverage.cobertura.xml
   - Thresholds: 60% (warning), 80% (good)
   - Output: markdown file (code-coverage-results.md)
6. Add Coverage PR Comment (marocchino/sticky-pull-request-comment@v2)
   - Only runs on pull_request events
   - Posts coverage report as PR comment
7. Upload coverage to Codecov (codecov/codecov-action@v4)
   - Files: ./coverage/**/coverage.cobertura.xml
   - Flags: unittests
   - fail_ci_if_error: false
```

**Coverage Thresholds:**
- 🟢 **Good:** > 80%
- 🟡 **Warning:** 60-80%
- 🔴 **Needs improvement:** < 60%

**Run Locally:**
```bash
dotnet restore
dotnet test --configuration Release --collect:"XPlat Code Coverage" --results-directory ./coverage
```

---

### 3. docker-build

**Job Name:** `Docker Build`  
**Runs On:** `ubuntu-latest`  
**Depends On:** `build-and-test` (must succeed)  
**Purpose:** Validate that the Docker image builds successfully

**Steps:**
```yaml
1. Checkout code (actions/checkout@v4)
2. Set up Docker Buildx (docker/setup-buildx-action@v3)
3. Build Docker image
   - Command: docker build --build-arg RUN_TESTS=false -t congestion-tax-calculator:latest .
   - Environment variable used: DOCKER_IMAGE_NAME
4. Verify Docker image
   - List images with grep
   - Inspect image configuration
```

**Important:** Tests are **NOT** run during Docker build because:
- Tests already passed in `build-and-test` job
- Avoids SQL Server connection issues during Docker build
- Faster build times

**Build Arguments:**
- `RUN_TESTS=false` ← Tests are skipped in Docker build!

**Run Locally:**
```bash
docker build --build-arg RUN_TESTS=false -t congestion-tax-calculator:latest .
```

---

### 4. code-quality

**Job Name:** `Code Quality`  
**Runs On:** `ubuntu-latest`  
**Depends On:** None (runs in parallel)  
**Purpose:** Check code formatting and detect outdated packages

**Steps:**
```yaml
1. Checkout code (actions/checkout@v4)
2. Setup .NET (actions/setup-dotnet@v4)
   - dotnet-version: 9.0.x
3. Restore dependencies
   - Command: dotnet restore
4. Format check
   - Command: dotnet format --verify-no-changes --verbosity diagnostic
   - continue-on-error: true (non-blocking)
5. Install dotnet-outdated
   - Command: dotnet tool install --global dotnet-outdated-tool
6. Check for outdated packages
   - Command: dotnet outdated
   - continue-on-error: true (non-blocking)
```

**Note:** Both steps have `continue-on-error: true`, meaning failures are **non-blocking** and won't fail the entire workflow.

**Run Locally:**
```bash
dotnet restore
dotnet format --verify-no-changes --verbosity diagnostic
dotnet tool install --global dotnet-outdated-tool
dotnet outdated
```

---

### 5. build-summary

**Job Name:** `Build Summary`  
**Runs On:** `ubuntu-latest`  
**Depends On:** `[build-and-test, code-coverage, docker-build, code-quality]`  
**Condition:** `if: always()` (runs even if previous jobs fail)  
**Purpose:** Create a summary of all job results

**Steps:**
```yaml
1. Check build status
   - Creates a markdown summary using $GITHUB_STEP_SUMMARY
   - Displays:
     - Job status table (Build & Test, Code Coverage, Docker Build, Code Quality)
     - Build date and time
     - Commit SHA
     - Branch name
```

**Output Example:**
```markdown
## Build Summary 🚀

| Job | Status |
|-----|--------|
| Build & Test | success |
| Code Coverage | success |
| Docker Build | success |
| Code Quality | success |

**Build Date:** 2025-11-12 15:30:00
**Commit:** abc123def456
**Branch:** main
```

**Note:** This job always runs regardless of previous job failures, providing a complete summary of the pipeline execution.

---

## 🔧 Environment Variables

The workflow defines these environment variables:

```yaml
env:
  DOTNET_VERSION: '9.0.x'                      # .NET SDK version
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: 1         # Skip first-time setup
  DOTNET_NOLOGO: true                          # Suppress .NET logo
  DOCKER_IMAGE_NAME: congestion-tax-calculator # Docker image name
```

These variables are accessible in all jobs using `${{ env.VARIABLE_NAME }}`.

---

## ⚙️ Configuration

### Optional Secrets

- `CODECOV_TOKEN`: For private repository coverage uploads (public repos use GITHUB_TOKEN automatically)

### Output Files

The workflow generates these files:

| File/Directory | Job | Description |
|----------------|-----|-------------|
| `./TestResults/*.trx` | build-and-test | Test results in TRX format |
| `./coverage/**/coverage.cobertura.xml` | code-coverage | Coverage data in Cobertura XML format |
| `code-coverage-results.md` | code-coverage | Coverage summary in Markdown format |

---

## 🐛 Troubleshooting

### Build Fails

**Problem:** `dotnet build` fails  
**Solution:**
```bash
# Test locally
dotnet restore --verbosity detailed
dotnet build --configuration Release
```

### Tests Fail in CI but Pass Locally

**Possible causes:**
- .NET version mismatch (check workflow uses 9.0.x)
- Environment-specific issues (time zones, culture)
- Missing dependencies

### Docker Build Fails with SQL Server Error

**Problem:** `A network-related or instance-specific error occurred...`  
**Solution:** This is fixed! The workflow uses `RUN_TESTS=false` to skip tests during Docker build.

**Verify locally:**
```bash
docker build --build-arg RUN_TESTS=false -t test-image .
```

### Code Quality Failures

**Note:** These are non-blocking. To fix:
```bash
# Format code
dotnet format

# Update packages
dotnet outdated
```

---

## 🔄 Running Locally

To replicate the CI pipeline locally:

```bash
# 1. Build & Test
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build

# 2. Code Coverage
dotnet test --collect:"XPlat Code Coverage"

# 3. Docker Build
docker build --build-arg RUN_TESTS=false -t congestion-tax-calculator .

# 4. Code Quality
dotnet format --verify-no-changes
```

---

## 📝 Modifying the Workflow

To customize the CI pipeline, edit `.github/workflows/ci.yml`:

```yaml
# Example: Change .NET version
env:
  DOTNET_VERSION: '10.0.x'

# Example: Add a new step to build-and-test job
- name: Custom Validation
  run: echo "Your custom command"
  
# Example: Add a new job
jobs:
  custom-check:
    name: Custom Check
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - name: Run check
        run: ./custom-script.sh
```

---

## ✅ Success Criteria

The CI pipeline is considered successful when:
- ✅ **build-and-test** passes (all tests pass)
- ✅ **code-coverage** completes successfully
- ✅ **docker-build** builds the image without errors
- ✅ **code-quality** completes (non-blocking, can have warnings)
- ✅ **build-summary** displays final status

**Note:** Only `build-and-test`, `code-coverage`, and `docker-build` are blocking. The `code-quality` job has `continue-on-error: true` so it won't fail the pipeline.

---

**Last Updated:** November 12, 2025  
**Workflow File:** `.github/workflows/ci.yml`

