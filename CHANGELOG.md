# Changelog

All notable changes are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/). Releases up to v11.0.0 are
described in the [GitHub releases](https://github.com/imazen/libwebp-net/releases).

## [Unreleased]

### QUEUED BREAKING CHANGES

- None.

### Changed

- Test project: Microsoft.NET.Test.Sdk 18.10.1, System.Drawing.Common 10.0.12 (6cd2528).
- CI: checkout v7, setup-dotnet v6, upload-artifact v7, download-artifact v8, upload-pages-artifact v5, deploy-pages v5 (4d660bc).

### Known Bugs

- `WebPEncoderConfig`'s constructors and `Validate()` call `WebPConfigInit` / `WebPValidateConfig` without `NativeLibraryLoader.FixDllNotFoundException`, so on .NET Framework the first such call throws `DllNotFoundException` when libwebp is only under `runtimes/<rid>/native` and nothing else has loaded it yet. In CI this shows up as order-dependent failures in the net472/net48 runs.
