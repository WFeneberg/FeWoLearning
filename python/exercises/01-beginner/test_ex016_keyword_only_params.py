import pytest

from ex016_keyword_only_params import clamp, connect, divide, make_url, rename


def test_connect_defaults() -> None:
    assert connect("db") == "db:5432 (timeout=5.0s)"


def test_connect_with_keywords() -> None:
    assert connect("db", port=1234, timeout=2.5) == "db:1234 (timeout=2.5s)"


def test_connect_refuses_a_positional_port() -> None:
    with pytest.raises(TypeError):
        connect("db", 1234)  # type: ignore[misc]


def test_divide() -> None:
    assert divide(6, 3) == 2


def test_divide_refuses_keywords() -> None:
    with pytest.raises(TypeError):
        divide(a=6, b=3)  # type: ignore[call-arg]


def test_divide_by_zero_raises() -> None:
    with pytest.raises(ZeroDivisionError):
        divide(1, 0)


@pytest.mark.parametrize(
    "value, expected",
    [(50, 50), (-5, 0), (500, 100), (0, 0), (100, 100)],
)
def test_clamp_defaults(value: int, expected: int) -> None:
    assert clamp(value) == expected


def test_clamp_low_may_be_positional_or_keyword() -> None:
    assert clamp(5, 10) == 10
    assert clamp(5, low=10) == 10


def test_clamp_high_is_keyword_only() -> None:
    assert clamp(50, 0, high=20) == 20
    with pytest.raises(TypeError):
        clamp(50, 0, 20)  # type: ignore[misc]


def test_clamp_value_is_positional_only() -> None:
    with pytest.raises(TypeError):
        clamp(value=5)  # type: ignore[call-arg]


def test_clamp_rejects_an_inverted_range() -> None:
    with pytest.raises(ValueError):
        clamp(5, 10, high=1)


def test_make_url_defaults() -> None:
    assert make_url("https", "example.com") == "https://example.com/"


def test_make_url_with_path() -> None:
    assert make_url("http", "localhost", path="/api") == "http://localhost/api"


def test_make_url_with_query() -> None:
    url = make_url("https", "example.com", path="/search", query={"q": "vue", "page": "2"})
    assert url == "https://example.com/search?q=vue&page=2"


def test_make_url_ignores_an_empty_query() -> None:
    assert make_url("https", "example.com", query={}) == "https://example.com/"


def test_make_url_refuses_a_positional_path() -> None:
    with pytest.raises(TypeError):
        make_url("https", "example.com", "/api")  # type: ignore[misc]


def test_rename() -> None:
    assert rename({"a": 1, "b": 2}, a="x") == {"x": 1, "b": 2}


def test_rename_does_not_modify_the_input() -> None:
    original = {"a": 1}

    rename(original, a="x")

    assert original == {"a": 1}


def test_rename_without_renames_returns_a_copy() -> None:
    original = {"a": 1}
    result = rename(original)

    assert result == original
    assert result is not original


def test_rename_can_rename_a_key_called_mapping() -> None:
    # Only possible because `mapping` is positional-only.
    assert rename({"mapping": 1}, mapping="renamed") == {"renamed": 1}
