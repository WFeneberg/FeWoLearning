import pytest

from ex092_orm_query_builder import QueryBuilder


def test_default_selects_everything_with_no_conditions():
    sql, params = QueryBuilder("users").build()

    assert sql == "SELECT * FROM users"
    assert params == []


def test_select_specific_columns():
    sql, _params = QueryBuilder("users").select("id", "name").build()

    assert sql == "SELECT id, name FROM users"


def test_select_with_no_arguments_still_means_star():
    sql, _params = QueryBuilder("users").select().build()

    assert sql == "SELECT * FROM users"


def test_a_single_where_condition():
    sql, params = QueryBuilder("users").where("age > ?", 18).build()

    assert sql == "SELECT * FROM users WHERE age > ?"
    assert params == [18]


def test_multiple_where_calls_are_anded_together_in_order():
    sql, params = (
        QueryBuilder("users").where("age > ?", 18).where("country = ?", "DE").build()
    )

    assert sql == "SELECT * FROM users WHERE age > ? AND country = ?"
    assert params == [18, "DE"]


def test_where_in_expands_one_placeholder_per_value():
    sql, params = QueryBuilder("users").where_in("id", [1, 2, 3]).build()

    assert sql == "SELECT * FROM users WHERE id IN (?, ?, ?)"
    assert params == [1, 2, 3]


def test_where_in_rejects_an_empty_list():
    with pytest.raises(ValueError):
        QueryBuilder("users").where_in("id", [])


def test_where_and_where_in_combine_and_keep_param_order():
    sql, params = (
        QueryBuilder("users")
        .where("age > ?", 18)
        .where_in("country", ["DE", "FR"])
        .build()
    )

    assert sql == "SELECT * FROM users WHERE age > ? AND country IN (?, ?)"
    assert params == [18, "DE", "FR"]


def test_order_by_accumulates_across_calls():
    sql, _params = QueryBuilder("users").order_by("name").order_by("id").build()

    assert sql == "SELECT * FROM users ORDER BY name, id"


def test_limit_is_appended():
    sql, _params = QueryBuilder("users").limit(10).build()

    assert sql == "SELECT * FROM users LIMIT 10"


def test_negative_limit_is_rejected():
    with pytest.raises(ValueError):
        QueryBuilder("users").limit(-1)


def test_every_clause_together_in_the_right_order():
    sql, params = (
        QueryBuilder("users")
        .select("id", "name")
        .where("age > ?", 18)
        .order_by("name")
        .limit(5)
        .build()
    )

    assert sql == "SELECT id, name FROM users WHERE age > ? ORDER BY name LIMIT 5"
    assert params == [18]


def test_each_method_returns_the_same_builder_for_chaining():
    builder = QueryBuilder("users")

    assert builder.select("id") is builder
    assert builder.where("id = ?", 1) is builder
    assert builder.where_in("id", [1]) is builder
    assert builder.order_by("id") is builder
    assert builder.limit(1) is builder


def test_values_never_appear_inside_the_sql_string():
    sql, params = QueryBuilder("users").where("name = ?", "Robert'); DROP TABLE users;--").build()

    assert "DROP TABLE" not in sql
    assert params == ["Robert'); DROP TABLE users;--"]
