# Watch

[![Build status](https://github.com/jorelius/Watch/workflows/.NET/badge.svg)](https://github.com/jorelius/Watch)
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

    Usage - watch <action> -options

    GlobalOption   Description
    Help (-?)      Shows this help

    Actions

    Clipboard <TargetProgram> [<Arguments>] -options - Watch system clipboard for changes

        Option                Description
        Interval (-i)         Interval in which to poll clipboard in ms [Default='500'] 
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.
        Repeat (-r)           Number of times to repeat watch and trigger. 0 is indefinitely [Default='0'] 

    Filesystem <TargetProgram> [<Arguments>] <Path> -options - Watch file or directory for changes

        Option                Description
        Path* (-p)            Path to file or directory to be monitored
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.
        Repeat (-r)           Number of times to repeat watch and trigger. 0 is indefinitely [Default='0'] 

    Http <TargetProgram> [<Arguments>] <Uri> -options - Watch for changes in http response

        Option                Description
        Interval (-i)         Interval in which to poll http resource in ms [Default='5000'] 
        Uri* (-u)             Uri to resource
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.
        Repeat (-r)           Number of times to repeat watch and trigger. 0 is indefinitely [Default='0'] 

    FTP <TargetProgram> [<Arguments>] <UserName> <Password> <Uri> -options - Watch for changes in Ftp response

        Option                Description
        UserName* (-un)       account username
        Password* (-pw)       account password
        Interval (-i)         Interval in which to poll ftp resource in ms [Default='5000'] 
        Uri* (-u)             Uri to resource
        TargetProgram* (-t)   The target program that is triggered
        Arguments (-a)        Arguments passed to program when Clipboard is updated.
        Repeat (-r)           Number of times to repeat watch and trigger. 0 is indefinitely [Default='0']
