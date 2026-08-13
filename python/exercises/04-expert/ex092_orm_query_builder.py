"""Exercise 092 — a fluent SQL query builder (expert).

Goal:   Build SQL strings by chaining method calls, the way a minimal ORM's query
        layer does, while keeping every value out of the string itself.
Drills: a fluent builder (each method returns `self`), assembling SQL clauses only
        when they were actually used, and parameter binding — placeholders
        (``?``) go in the SQL, the actual values travel alongside in a separate
        list, never interpolated into the string.
Passes: when `pytest exercises/04-expert/test_ex092_orm_query_builder.py` is green.

Note:   this only builds `(sql, params)` tuples — nothing here executes against a
        real database. `params` staying separate from `sql` is the property that
        matters: it is what makes the result safe to hand to `cursor.execute(sql,
        params)` no matter what a value contains.
"""

from typing import Any


class QueryBuilder:
    """Builds a single `SELECT` statement, one clause at a time."""

    def __init__(self, table: str) -> None:
        """Start a query against `table`, selecting every column until `select` says
        otherwise, with no conditions, ordering, or limit yet."""
        raise NotImplementedError

    def select(self, *columns: str) -> "QueryBuilder":
        """Set the selected columns, replacing any previous selection.

        No arguments means "all columns" (``*``). Returns `self` for chaining.
        """
        raise NotImplementedError

    def where(self, condition: str, *params: Any) -> "QueryBuilder":
        """Add a `condition` (containing zero or more ``?`` placeholders) and its
        `params`, in order.

        Every call to `where` accumulates — two calls AND together in the final
        SQL rather than the second replacing the first. Returns `self`.
        """
        raise NotImplementedError

    def where_in(self, column: str, values: list[Any]) -> "QueryBuilder":
        """Add a ``column IN (?, ?, ...)`` condition with one placeholder per value.

        An empty `values` list raises ValueError — ``IN ()`` is not valid SQL, and
        silently building it would hide the bug in the caller's data instead of
        surfacing it here. Returns `self`.
        """
        raise NotImplementedError

    def order_by(self, *columns: str) -> "QueryBuilder":
        """Append columns to the ORDER BY clause (accumulates across calls, like
        `where`, rather than replacing). Returns `self`."""
        raise NotImplementedError

    def limit(self, count: int) -> "QueryBuilder":
        """Set a row limit. A negative `count` raises ValueError. Returns `self`."""
        raise NotImplementedError

    def build(self) -> tuple[str, list[Any]]:
        """Assemble the final `(sql, params)`.

        Include ``WHERE``/``ORDER BY``/``LIMIT`` only if something was actually
        added for them. `params` is every value passed to `where`/`where_in`, in
        the order those calls were made.
        """
        raise NotImplementedError
