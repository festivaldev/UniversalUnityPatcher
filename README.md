# UniversalUnityPatcher

UniversalUnityPatcher can be used to patch Unity assemblies (if your game of choice doesn't use IL2CPP) and probably other .NET assemblies at IL instruction level. Originally designed for use with [Paradise](https://github.com/festivaldev/Paradise), you can use patches to change a game's behavior, inject other assemblies, change strings, etc.

## Command Line Options

Option | Description | Alias | GUI Mode | CLI Mode
--- | --- | --- | --- | ---
`--assembly-dir` | `-i` | Sets the assembly directory in which to patch assemblies | ✅ | ✅
`--backup-dir` |  | Specifies the path where files a backed up to before patching | ✅ | ✅
`--backup` | `-b` | Enables backup of files to be patched | ❌ | ✅
`--disabled-patches` |  | Force specific patch indices to be disabled, starting at 1 | ✅ | ✅
`--enabled-patches` |  | Force specific patch indices to be enabled, starting at 1 | ✅ | ✅
`--ignore-duplicate-patch` |  | Ignore warnings for duplicate patches and patch anyways | ✅ | ✅
`--no-gui` | | Disables the graphical user interface for use in automated environments (eg. installers) | ❌ | ✅
`--output-dir` | `-o` | Sets the output directory where patched assemblies will be copied to (default: same as assembly directory) | ✅ | ✅
`--patch` | `-p` | Specifies the path for the patch XML | ✅ | ✅
`--silent` | `-s` | Disables any output from the application | ❌ | ✅

## Credits
_Developers_
* [@SniperGER](https://github.com/SniperGER)

_Used libraries_
* _[CommandLineParser](https://github.com/commandlineparser/commandline)_ by gsscoder,nemec,ericnewton76,moh-hassan
* _[Windows-API-Code-Pack-1.1.5](https://github.com/contre/Windows-API-Code-Pack-1.1/)_ by rpastric,contre,dahall
* _[Mono.Cecil](https://github.com/jbevain/cecil/)_ by Jb Evain