# Winget Package Manifests

This directory contains Windows Package Manager (winget) manifests for Workvivo CLI.

## Submitting to winget-pkgs

To submit this package to the official winget repository:

1. **Fork the winget-pkgs repository**:
   ```bash
   gh repo fork microsoft/winget-pkgs --clone
   ```

2. **Copy manifests to the correct location**:
   ```bash
   # Format: manifests/{first-letter}/{Publisher}/{PackageName}/{Version}/
   mkdir -p winget-pkgs/manifests/j/JWAB/WorkvivoCli/0.1.0
   cp winget/* winget-pkgs/manifests/j/JWAB/WorkvivoCli/0.1.0/
   ```

3. **Validate the manifests**:
   ```bash
   winget validate winget-pkgs/manifests/j/JWAB/WorkvivoCli/0.1.0/
   ```

4. **Create a pull request**:
   ```bash
   cd winget-pkgs
   git checkout -b JWAB.WorkvivoCli-0.1.0
   git add manifests/j/JWAB/WorkvivoCli/
   git commit -m "New package: JWAB.WorkvivoCli version 0.1.0"
   git push origin JWAB.WorkvivoCli-0.1.0
   gh pr create --title "New package: JWAB.WorkvivoCli version 0.1.0"
   ```

## Testing Locally

Before submitting, test the installation locally:

```powershell
# Install from local manifest
winget install --manifest winget\JWAB.WorkvivoCli.yaml

# Test the installation
wv --version
wv --help
```

## Updating the Package

For new versions, create a new directory with the version number and update the manifests accordingly. The winget-pkgs repository has automated tools to help with updates.

## Resources

- [winget-pkgs Contributing Guide](https://github.com/microsoft/winget-pkgs/blob/master/CONTRIBUTING.md)
- [Manifest Schema Reference](https://github.com/microsoft/winget-cli/blob/master/schemas/JSON/manifests/)
- [winget CLI Documentation](https://learn.microsoft.com/en-us/windows/package-manager/winget/)
