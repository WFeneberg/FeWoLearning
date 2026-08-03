from collections import ChainMap
from typing import Any

import pytest

from ex066_chainmap_layers import (
    drop_front,
    flatten,
    layered_config,
    resolve,
    stays_live,
    which_layer,
    with_overrides,
    write_goes_to_the_front,
)


def make() -> ChainMap:
    return layered_config(
        {"debug": True},
        {"host": "env-host", "port": 9000},
        {"host": "default-host", "port": 80, "timeout": 5},
    )


def test_layered_config_search_order() -> None:
    config = make()

    assert config["debug"] is True
    assert config["host"] == "env-host"
    assert config["port"] == 9000
    assert config["timeout"] == 5


def test_layered_config_is_a_chainmap_with_three_layers() -> None:
    config = make()

    assert isinstance(config, ChainMap)
    assert len(config.maps) == 3


def test_resolve() -> None:
    config = make()

    assert resolve(config, "host") == "env-host"
    assert resolve(config, "timeout") == 5


def test_resolve_falls_back() -> None:
    assert resolve(make(), "absent") is None
    assert resolve(make(), "absent", "fb") == "fb"


@pytest.mark.parametrize(
    "key, expected",
    [("debug", 0), ("host", 1), ("port", 1), ("timeout", 2), ("absent", -1)],
)
def test_which_layer(key: str, expected: int) -> None:
    assert which_layer(make(), key) == expected


def test_stays_live_sees_later_mutations() -> None:
    base = {"a": "original"}

    config, before = stays_live(base)

    assert before == "original"
    # A ChainMap holds a reference; {**base} would have frozen a copy.
    assert config["a"] == "changed"


def test_with_overrides_adds_a_front_layer() -> None:
    config = make()

    overridden = with_overrides(config, host="override-host")

    assert overridden["host"] == "override-host"
    assert len(overridden.maps) == 4


def test_with_overrides_leaves_the_original_alone() -> None:
    config = make()

    with_overrides(config, host="override-host")

    assert config["host"] == "env-host"
    assert len(config.maps) == 3


def test_with_overrides_without_any() -> None:
    config = make()

    overridden = with_overrides(config)

    assert overridden["host"] == "env-host"


def test_write_goes_to_the_front_layer_only() -> None:
    config = make()

    effective, front = write_goes_to_the_front(config, "host", "written")

    assert effective == "written"
    assert front["host"] == "written"
    # The env layer, which originally provided "host", is untouched.
    assert config.maps[1]["host"] == "env-host"


def test_write_creates_the_key_in_the_front_layer() -> None:
    config = make()

    _, front = write_goes_to_the_front(config, "brand-new", 1)

    assert front["brand-new"] == 1


def test_flatten() -> None:
    assert flatten(make()) == {
        "debug": True,
        "host": "env-host",
        "port": 9000,
        "timeout": 5,
    }


def test_flatten_returns_a_plain_dict() -> None:
    assert type(flatten(make())) is dict


def test_drop_front() -> None:
    config = make()

    without = drop_front(config)

    assert len(without.maps) == 2
    # "debug" only existed in the layer that was dropped.
    assert "debug" not in without
    assert without["host"] == "env-host"


def test_drop_front_of_a_single_layer_chain() -> None:
    single = ChainMap({"a": 1})

    result = drop_front(single)

    assert isinstance(result, ChainMap)
    assert dict(result) == {}
