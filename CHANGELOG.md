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

### Fixed

- `WebPEncoderConfig` (constructors, `SetLosslessPreset`, `Validate`) and `AbiVersionCheck.ValidateOrThrow` load libwebp through `NativeLibraryLoader` like the rest of the library; on .NET Framework they could throw `DllNotFoundException` when they were the first libwebp call and the DLL sat only under `runtimes/<rid>/native` (a792585).
