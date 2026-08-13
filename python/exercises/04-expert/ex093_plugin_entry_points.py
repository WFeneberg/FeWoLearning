"""Exercise 093 — plugin discovery via importlib (expert).

Goal:   Load plugins from `.py` files that were never `import`ed by name anywhere —
        a directory is scanned, and each file becomes a module on the fly. This is
        the mechanism a real plugin system (`importlib.metadata` entry points, or a
        "drop a file in this folder" convention) is built on top of.
Drills: `importlib.util.spec_from_file_location`, `module_from_spec`,
        `spec.loader.exec_module`, and building a name -> callable registry out of
        whatever the discovered modules expose.
Passes: when `pytest exercises/04-expert/test_ex093_plugin_entry_points.py` is green.

Note:   every discovered module needs its own, non-colliding module name passed to
        `spec_from_file_location` — reusing one name across files would make the
        second `exec_module` overwrite state that the first module's callables
        might still be closing over.
"""

from pathlib import Path
from types import ModuleType
from typing import Callable


def load_module_from_path(path: Path) -> ModuleType:
    """Import the Python file at `path` as a fresh module object and return it.

    Give it a module name derived from `path.stem` (e.g. ``f"_plugin_{path.stem}"``)
    so it cannot collide with a real importable module of the same name. A `path`
    that cannot be loaded (`spec_from_file_location` or its loader come back None)
    raises ImportError.
    """
    raise NotImplementedError


class PluginRegistry:
    """Discovers plugin modules and dispatches to them by name."""

    def __init__(self) -> None:
        raise NotImplementedError

    def discover(self, directory: Path) -> None:
        """Load every `*.py` file directly inside `directory` (not subdirectories),
        skipping any whose filename starts with ``"_"``.

        Each loaded module must define a `run(payload: str) -> str` function —
        missing one raises TypeError naming the file. Register it under its
        `PLUGIN_NAME` module attribute, or the file's stem if the module does not
        define one. A name already registered (from an earlier file, this call or
        a previous one) raises ValueError.
        """
        raise NotImplementedError

    @property
    def names(self) -> list[str]:
        """Registered plugin names, sorted."""
        raise NotImplementedError

    def run(self, name: str, payload: str) -> str:
        """Run the plugin registered under `name` on `payload`.

        An unregistered name raises KeyError.
        """
        raise NotImplementedError
