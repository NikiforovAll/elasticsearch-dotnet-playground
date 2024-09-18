# Elasticsearch .NET Playground

This playground is a collection of notebooks that demonstrate how to use `Elastic.Clients.Elasticsearch` and `NEST` clients.

You may want to use this playground to:
* Learn more about `Elastic.Clients.Elasticsearch` and `NEST` clients
* You want to migrate your existing code from `NEST` to `Elastic.Clients.Elasticsearch`.

> See [playground.ipynb](./playground.ipynb) to get started.

![setup-elastic-infra](./assets/setup-elastic-infra.png)

## Configure

To configure the playground, set the `PLAYGROUND_CONNECTION_STRING=https://elastic:elastic@127.0.0.1:9200/` in .env file in the root of the project. If you don't do it, you will be prompted to enter the connection string every time you set up the client

## Analyzer [![Build](https://github.com/NikiforovAll/elasticsearch-dotnet-playground/actions/workflows/build.yaml/badge.svg)](https://github.com/NikiforovAll/elasticsearch-dotnet-playground/actions/workflows/build.yaml)

This repository contains [Nall.NEST.MigtarionAnalyzer](https://www.nuget.org/packages/Nall.NEST.MigtarionAnalyzer) analyzer that helps with migration from `Nest` to `Elastic.Clients.Elasticsearch`.

```bash
dotnet add package Nall.NEST.MigtarionAnalyzer --version 1.1.0
```

## Devcontainer

You can use the devcontainer to get started with the playground. It will install the required tools and libraries.

> See [.devcontainer/devcontainer.json](./.devcontainer/devcontainer.json) to get started.

This command will confirm that Jupyter now supports C# notebooks:

```bash
jupyter kernelspec list
```

Enter the notebooks folder, and run this to launch the browser interface:

```bash
jupyter-lab
```

## References

* Elasticsearch.NET (latest) - <https://www.elastic.co/guide/en/elasticsearch/client/net-api/8.9/introduction.html>
* Migration Guide - <https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/migration-guide.html>
* Elasticsearch.NET - <https://github.com/elastic/elasticsearch-net>
* NEST + Elasticsearch.NET in one doc - <https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/introduction.html>
* .NET Interactive | Samples - <https://github.com/dotnet/interactive/tree/main/samples/notebooks/csharp>
* .NET Interactive - <https://github.com/dotnet/interactive/blob/main/docs/README.md>