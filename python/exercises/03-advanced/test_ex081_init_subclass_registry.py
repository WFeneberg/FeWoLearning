import pytest

from ex081_init_subclass_registry import Plugin, run_all


def test_a_subclass_registers_itself() -> None:
    Plugin.reset_registry()

    class Csv(Plugin, name="csv"):
        def run(self, payload: str) -> str:
            return f"csv:{payload}"

    assert Plugin.registry == {"csv": Csv}


def test_the_base_class_is_not_registered() -> None:
    Plugin.reset_registry()

    assert Plugin.available() == []


def test_the_name_defaults_to_the_lowercased_class_name() -> None:
    Plugin.reset_registry()

    class Json(Plugin):
        def run(self, payload: str) -> str:
            return payload

    assert Plugin.available() == ["json"]
    assert Json.plugin_name == "json"


def test_the_name_is_stored_on_the_class() -> None:
    Plugin.reset_registry()

    class Csv(Plugin, name="csv"):
        def run(self, payload: str) -> str:
            return payload

    assert Csv.plugin_name == "csv"


def test_available_is_sorted() -> None:
    Plugin.reset_registry()

    class Zip(Plugin, name="zip"):
        def run(self, payload: str) -> str:
            return payload

    class Csv(Plugin, name="csv"):
        def run(self, payload: str) -> str:
            return payload

    class Json(Plugin, name="json"):
        def run(self, payload: str) -> str:
            return payload

    assert Plugin.available() == ["csv", "json", "zip"]


def test_a_duplicate_name_is_rejected() -> None:
    Plugin.reset_registry()

    class Csv(Plugin, name="csv"):
        def run(self, payload: str) -> str:
            return payload

    with pytest.raises(ValueError, match="csv"):

        class OtherCsv(Plugin, name="csv"):
            def run(self, payload: str) -> str:
                return payload


def test_register_false_stays_out_of_the_registry() -> None:
    Plugin.reset_registry()

    class Abstract(Plugin, register=False):
        pass

    assert Plugin.available() == []


def test_register_false_still_allows_registering_children() -> None:
    Plugin.reset_registry()

    class TextBased(Plugin, register=False):
        def run(self, payload: str) -> str:
            return f"text:{payload}"

    class Csv(TextBased, name="csv"):
        pass

    assert Plugin.available() == ["csv"]
    assert Plugin.create("csv").run("x") == "text:x"


def test_naming_a_plugin_that_does_not_register_is_an_error() -> None:
    Plugin.reset_registry()

    with pytest.raises(ValueError):

        class Confused(Plugin, name="confused", register=False):
            def run(self, payload: str) -> str:
                return payload


def test_a_plugin_without_run_is_rejected() -> None:
    Plugin.reset_registry()

    with pytest.raises(TypeError):

        class Incomplete(Plugin, name="incomplete"):
            pass


def test_lookup_returns_the_class() -> None:
    Plugin.reset_registry()

    class Csv(Plugin, name="csv"):
        def run(self, payload: str) -> str:
            return payload

    assert Plugin.lookup("csv") is Csv


def test_lookup_of_an_unknown_name() -> None:
    Plugin.reset_registry()

    with pytest.raises(KeyError):
        Plugin.lookup("nope")


def test_create_instantiates_the_plugin() -> None:
    Plugin.reset_registry()

    class Csv(Plugin, name="csv"):
        def run(self, payload: str) -> str:
            return f"csv:{payload}"

    instance = Plugin.create("csv")

    assert isinstance(instance, Csv)
    assert instance.run("row") == "csv:row"


def test_create_forwards_constructor_arguments() -> None:
    Plugin.reset_registry()

    class Prefixer(Plugin, name="prefixer"):
        def __init__(self, prefix: str, *, suffix: str = "!") -> None:
            self.prefix = prefix
            self.suffix = suffix

        def run(self, payload: str) -> str:
            return f"{self.prefix}{payload}{self.suffix}"

    assert Plugin.create("prefixer", ">> ", suffix="?").run("x") == ">> x?"


def test_run_all_dispatches_to_every_plugin() -> None:
    Plugin.reset_registry()

    class Upper(Plugin, name="upper"):
        def run(self, payload: str) -> str:
            return payload.upper()

    class Reverse(Plugin, name="reverse"):
        def run(self, payload: str) -> str:
            return payload[::-1]

    assert run_all("abc") == {"upper": "ABC", "reverse": "cba"}


def test_run_all_on_an_empty_registry() -> None:
    Plugin.reset_registry()

    assert run_all("abc") == {}


def test_init_subclass_forwards_keywords_up_the_mro() -> None:
    Plugin.reset_registry()
    seen: list[str] = []

    class Mixin:
        def __init_subclass__(cls, /, tag: str = "", **kwargs: object) -> None:
            super().__init_subclass__(**kwargs)
            if tag:
                seen.append(tag)

    class Csv(Plugin, Mixin, name="csv", tag="from-mixin"):
        def run(self, payload: str) -> str:
            return payload

    # Without `super().__init_subclass__(**kwargs)` the mixin never sees `tag`.
    assert seen == ["from-mixin"]
    assert Plugin.available() == ["csv"]
