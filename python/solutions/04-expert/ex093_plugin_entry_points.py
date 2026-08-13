"""Exercise 093 — plugin discovery via importlib (reference solution)."""

import importlib.util
from pathlib import Path
from types import ModuleType
from typing import Callable


def load_module_from_path(path: Path) -> ModuleType:
    module_name = f"_plugin_{path.stem}"
    spec = importlib.util.spec_from_file_location(module_name, path)
    if spec is None or spec.loader is None:
        raise ImportError(f"cannot load plugin from {path}")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class PluginRegistry:
    def __init__(self) -> None:
        self._plugins: dict[str, Callable[[str], str]] = {}

    def discover(self, directory: Path) -> None:
        for path in sorted(directory.glob("*.py")):
            if path.stem.startswith("_"):
                continue
            module = load_module_from_path(path)
            if not hasattr(module, "run"):
                raise TypeError(f"plugin {path.name} does not define run()")
            name = getattr(module, "PLUGIN_NAME", path.stem)
            if name in self._plugins:
                raise ValueError(f"duplicate plugin name {name!r} from {path.name}")
            self._plugins[name] = module.run

    @property
    def names(self) -> list[str]:
        return sorted(self._plugins)

    def run(self, name: str, payload: str) -> str:
        return self._plugins[name](payload)
