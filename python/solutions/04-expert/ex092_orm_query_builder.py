"""Exercise 092 — a fluent SQL query builder (reference solution)."""

from typing import Any


class QueryBuilder:
    def __init__(self, table: str) -> None:
        self._table = table
        self._columns: list[str] = ["*"]
        self._conditions: list[str] = []
        self._params: list[Any] = []
        self._order_by: list[str] = []
        self._limit: int | None = None

    def select(self, *columns: str) -> "QueryBuilder":
        self._columns = list(columns) if columns else ["*"]
        return self

    def where(self, condition: str, *params: Any) -> "QueryBuilder":
        self._conditions.append(condition)
        self._params.extend(params)
        return self

    def where_in(self, column: str, values: list[Any]) -> "QueryBuilder":
        if not values:
            raise ValueError(f"where_in({column!r}, ...) needs at least one value")
        placeholders = ", ".join("?" for _ in values)
        self._conditions.append(f"{column} IN ({placeholders})")
        self._params.extend(values)
        return self

    def order_by(self, *columns: str) -> "QueryBuilder":
        self._order_by.extend(columns)
        return self

    def limit(self, count: int) -> "QueryBuilder":
        if count < 0:
            raise ValueError(f"limit must not be negative, got {count}")
        self._limit = count
        return self

    def build(self) -> tuple[str, list[Any]]:
        sql = f"SELECT {', '.join(self._columns)} FROM {self._table}"
        if self._conditions:
            sql += " WHERE " + " AND ".join(self._conditions)
        if self._order_by:
            sql += " ORDER BY " + ", ".join(self._order_by)
        if self._limit is not None:
            sql += f" LIMIT {self._limit}"
        return sql, list(self._params)
