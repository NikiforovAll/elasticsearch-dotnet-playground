# Search notebooks

This folder contains a number of notebooks that demonstrate the fundamentals of Elasticsearch, like indexing embeddings, running lexical, semantic and _hybrid_ searches, and more.

The following notebooks are available:

0. [Quick start](#0-quick-start)
1. [Keyword, querying, filtering](#1-keyword-querying-filtering)


## Notebooks

### 0. Quick start

In  the [`00-quick-start.ipynb`](./00-quick-start.ipynb) notebook you'll learn how to:

- Use the Elasticsearch Python client for various operations.
- Create and define an index for a sample dataset with `dense_vector` fields.
- Transform book titles into embeddings using [Sentence Transformers](https://www.sbert.net) and index them into Elasticsearch.
- Perform k-nearest neighbors (knn) semantic searches.

### 1. Keyword, querying, filtering

In the [`01-keyword-querying-filtering.ipynb`](./01-keyword-querying-filtering.ipynb) notebook, you'll learn how to:

- Use [query and filter contexts](https://www.elastic.co/guide/en/elasticsearch/reference/current/query-filter-context.html) to search and filter documents in Elasticsearch.
- Execute full-text searches with `match` and `multi-match` queries.
- Query and filter documents based on `text`, `number`, `date`, or `boolean` values.
- Run multi-field searches using the `multi-match` query.
- Prioritize specific fields in the `multi-match` query for tailored results.