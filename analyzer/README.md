# Analyzer [![Build](https://github.com/NikiforovAll/elasticsearch-dotnet-playground/actions/workflows/build.yaml/badge.svg)](https://github.com/NikiforovAll/elasticsearch-dotnet-playground/actions/workflows/build.yaml)

This repository contains [Nall.NEST.MigtarionAnalyzer](https://www.nuget.org/packages/Nall.NEST.MigtarionAnalyzer) analyzer that helps with migration from `Nest` to `Elastic.Clients.Elasticsearch`.

```bash
dotnet add package Nall.NEST.MigtarionAnalyzer --version 1.1.0
```

Rules: `ELS001`


```bash
dotnet format analyzers ./path/to/folder \
    --verify-no-changes \
    --diagnostics ELS001 \
    --severity info
```
