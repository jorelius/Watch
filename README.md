# Watch

[![Build status](https://ci.appveyor.com/api/projects/status/2178awlmn3471tkc?svg=true)](https://ci.appveyor.com/project/jorelius/watch)
[![NuGet](https://img.shields.io/nuget/v/watch.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/watch/)

Trigger command when a watched resource (e.g. Clipboard, Filesystem, Http, Ftp, etc) changes.  

## Console

Watch is packaged as a dotnet console tool. It exposes many of the features the core framework provides.

Prerequisites
dotnet core sdk

### Install

```console
$ dotnet tool install watch -g
```

### Usage

    Usage - Watch <action> -options

    GlobalOption   Description
    Help (-?)      Shows this help

    Actions

    Clipboard <TargetProgram> [<Arguments>] -options - Watch system clipboard for changes

        Option                Description
        Interval (-i)         Interval in which to poll clipboard in ms [Default='500']
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.

    Filesystem <TargetProgram> [<Arguments>] <Path>  - Watch file or directory for changes

        Option                Description
        Path* (-p)            Path to file or directory to be monitored
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.

    Http <TargetProgram> [<Arguments>] <Uri> -options - Watch for changes in http response

        Option                Description
        Interval (-i)         Interval in which to poll http resource in ms [Default='5000']
        Uri* (-u)             Uri to resource
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.

    FTP <TargetProgram> [<Arguments>] <UserName> <Password> <Uri> -options - Watch for changes in Ftp response

        Option                Description
        UserName* (-un)       account username
        Password* (-pw)       account password
        Interval (-i)         Interval in which to poll ftp resource in ms [Default='5000']
        Uri* (-u)             Uri to resource
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.
