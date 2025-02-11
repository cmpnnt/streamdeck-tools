# StreamDeck Tools Changes

There are a number of changes to Barraider's StreamDeck Tools. Some of the changes are stylistic, while others are focused on ease of use, performance and compatibility. There are also several breaking changes, meaning this library is not backwards compatible with the original.

The two biggest updates are **Native AOT** compatibility and **cross-platform** enhancements.

Other changes, in no particular order:

- TODO: `Newtonsoft.Json` replaced with source-generated `System.Text.Json`
- Reflection-based plugin action autoloading replaced with source generation
- `System.Drawing` replaced with `Skia Sharp` for cross-platform purposes
- Sample plugin updated to use Skia Sharp, and include dial press and encoder examples
- Sample project now uses plugin `UUID` from `manifest.json` for build output directory
- In Progress: New MSBuild tasks to automate plugin installation during development (in debug mode only)
- Minor refactoring to use new language features
- Remove code marked `deprecated` and `obsolete` by Barraider
- Drop legacy .NET Framework in favor of .NET 8 minimum
- TODO: Update to latest Stream Deck SDK 
- TODO: Reversioning the changelog

## Backlogged TODO:

In addition to the TODO comments in the code:

- Update to Barraider's Easy PI v2
- Update documentation and wiki to reflect changes
- CI Pipeline
- Test suite