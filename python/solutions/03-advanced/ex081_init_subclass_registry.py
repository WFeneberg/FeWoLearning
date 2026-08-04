"""Exercise 081 — __init_subclass__ and plugin registries (reference solution)."""

from typing import Any


class Plugin:
    registry: dict[str, type["Plugin"]] = {}
    plugin_name: str

    def __init_subclass__(cls, /, name: str | None = None, register: bool = True, **kwargs: Any) -> None:
        # Implicitly a classmethod — no decorator needed, and `cls` is the *new* subclass.
        # Forward first: a cooperative base further up the MRO may be waiting on kwargs it
        # understands, and swallowing them would break it silently.
        super().__init_subclass__(**kwargs)

        if not register:
            if name is not None:
                raise ValueError(
                    f"{cls.__name__} passed name={name!r} together with register=False, "
                    "so the name would be ignored"
                )
            return

        # The interface check `abc` would charge a metaclass for. `cls.run is Plugin.run`
        # means nothing between cls and Plugin overrode it.
        if cls.run is Plugin.run:
            raise TypeError(f"{cls.__name__} must override run() to be registered")

        key = name if name is not None else cls.__name__.lower()
        if key in Plugin.registry:
            raise ValueError(
                f"plugin name {key!r} is already registered by "
                f"{Plugin.registry[key].__name__}"
            )
        cls.plugin_name = key
        Plugin.registry[key] = cls

    def run(self, payload: str) -> str:
        raise NotImplementedError("Plugin subclasses must implement run()")

    @classmethod
    def reset_registry(cls) -> None:
        Plugin.registry.clear()

    @classmethod
    def available(cls) -> list[str]:
        return sorted(Plugin.registry)

    @classmethod
    def lookup(cls, name: str) -> type["Plugin"]:
        try:
            return Plugin.registry[name]
        except KeyError:
            raise KeyError(f"no plugin registered as {name!r}") from None

    @classmethod
    def create(cls, name: str, *args: Any, **kwargs: Any) -> "Plugin":
        return cls.lookup(name)(*args, **kwargs)


def run_all(payload: str) -> dict[str, str]:
    # Names no plugin and imports nothing: everything that registered gets a turn.
    return {name: plugin_class().run(payload) for name, plugin_class in Plugin.registry.items()}
