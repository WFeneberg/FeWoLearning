from pathlib import Path

import pytest

from ex093_plugin_entry_points import PluginRegistry, load_module_from_path


def write_plugin(directory: Path, filename: str, source: str) -> Path:
    path = directory / filename
    path.write_text(source, encoding="utf-8")
    return path


def test_load_module_from_path_executes_the_file(tmp_path: Path):
    path = write_plugin(
        tmp_path, "greeter.py", "PLUGIN_NAME = 'greeter'\ndef run(payload):\n    return f'hi {payload}'\n"
    )

    module = load_module_from_path(path)

    assert module.PLUGIN_NAME == "greeter"
    assert module.run("bob") == "hi bob"


def test_discover_registers_by_plugin_name(tmp_path: Path):
    write_plugin(
        tmp_path,
        "upper.py",
        "PLUGIN_NAME = 'upper'\ndef run(payload):\n    return payload.upper()\n",
    )

    registry = PluginRegistry()
    registry.discover(tmp_path)

    assert registry.names == ["upper"]
    assert registry.run("upper", "abc") == "ABC"


def test_discover_defaults_to_the_file_stem_without_plugin_name(tmp_path: Path):
    write_plugin(tmp_path, "reverse.py", "def run(payload):\n    return payload[::-1]\n")

    registry = PluginRegistry()
    registry.discover(tmp_path)

    assert registry.names == ["reverse"]
    assert registry.run("reverse", "abc") == "cba"


def test_discover_finds_every_plugin_in_the_directory(tmp_path: Path):
    write_plugin(tmp_path, "upper.py", "def run(payload):\n    return payload.upper()\n")
    write_plugin(tmp_path, "lower.py", "def run(payload):\n    return payload.lower()\n")

    registry = PluginRegistry()
    registry.discover(tmp_path)

    assert registry.names == ["lower", "upper"]


def test_discover_skips_underscore_prefixed_files(tmp_path: Path):
    write_plugin(tmp_path, "_helpers.py", "def shared():\n    return 1\n")
    write_plugin(tmp_path, "real.py", "def run(payload):\n    return payload\n")

    registry = PluginRegistry()
    registry.discover(tmp_path)

    assert registry.names == ["real"]


def test_discover_does_not_recurse_into_subdirectories(tmp_path: Path):
    write_plugin(tmp_path, "top.py", "def run(payload):\n    return payload\n")
    nested = tmp_path / "nested"
    nested.mkdir()
    write_plugin(nested, "buried.py", "def run(payload):\n    return payload\n")

    registry = PluginRegistry()
    registry.discover(tmp_path)

    assert registry.names == ["top"]


def test_a_plugin_without_run_is_rejected(tmp_path: Path):
    write_plugin(tmp_path, "broken.py", "PLUGIN_NAME = 'broken'\n")

    registry = PluginRegistry()
    with pytest.raises(TypeError, match="broken"):
        registry.discover(tmp_path)


def test_a_duplicate_plugin_name_is_rejected(tmp_path: Path):
    write_plugin(tmp_path, "a.py", "PLUGIN_NAME = 'shared'\ndef run(payload):\n    return payload\n")
    write_plugin(tmp_path, "b.py", "PLUGIN_NAME = 'shared'\ndef run(payload):\n    return payload\n")

    registry = PluginRegistry()
    with pytest.raises(ValueError, match="shared"):
        registry.discover(tmp_path)


def test_running_an_unregistered_plugin_raises_key_error(tmp_path: Path):
    registry = PluginRegistry()
    registry.discover(tmp_path)

    with pytest.raises(KeyError):
        registry.run("nope", "x")


def test_two_plugin_modules_do_not_clobber_each_others_state(tmp_path: Path):
    write_plugin(
        tmp_path, "a.py", "PLUGIN_NAME = 'a'\nvalue = 1\ndef run(payload):\n    return f'a:{value}'\n"
    )
    write_plugin(
        tmp_path, "b.py", "PLUGIN_NAME = 'b'\nvalue = 2\ndef run(payload):\n    return f'b:{value}'\n"
    )

    registry = PluginRegistry()
    registry.discover(tmp_path)

    assert registry.run("a", "") == "a:1"
    assert registry.run("b", "") == "b:2"
