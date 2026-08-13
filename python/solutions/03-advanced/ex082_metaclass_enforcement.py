"""Exercise 082 — metaclasses and class-creation hooks (reference solution)."""

import re
from typing import Any


def camel_to_kebab(name: str) -> str:
    step1 = re.sub(r"(?<!^)(?=[A-Z][a-z])", "-", name)
    step2 = re.sub(r"(?<=[a-z0-9])(?=[A-Z])", "-", step1)
    return step2.lower()


class ComponentMeta(type):
    registry: dict[str, type] = {}

    def __new__(
        mcls,
        name: str,
        bases: tuple[type, ...],
        namespace: dict[str, Any],
        *,
        abstract: bool = False,
        slug: str | None = None,
        **kwargs: Any,
    ) -> type:
        # Build the class first — the checks below need the fully-formed `cls` (MRO
        # attribute lookup for `kind`, `cls.__name__`, etc).
        cls = super().__new__(mcls, name, bases, namespace, **kwargs)

        if not name[:1].isupper():
            raise TypeError(f"component class name {name!r} must be PascalCase")

        cls._abstract = abstract
        if abstract:
            return cls

        if not hasattr(cls, "kind"):
            raise TypeError(f"{name} must define 'kind' to be a concrete component")

        derived_slug = slug if slug is not None else camel_to_kebab(name)
        if derived_slug in mcls.registry:
            raise ValueError(
                f"slug {derived_slug!r} is already registered by "
                f"{mcls.registry[derived_slug].__name__}"
            )
        cls.slug = derived_slug
        mcls.registry[derived_slug] = cls
        return cls

    def __call__(cls, *args: Any, **kwargs: Any) -> Any:
        # Intercepting instantiation is the one thing `__init_subclass__` cannot do —
        # this runs before `cls.__init__` ever would.
        if getattr(cls, "_abstract", False):
            raise TypeError(f"cannot instantiate abstract component {cls.__name__}")
        return super().__call__(*args, **kwargs)


def reset_registry() -> None:
    ComponentMeta.registry.clear()
