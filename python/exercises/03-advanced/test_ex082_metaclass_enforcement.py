import pytest

from ex082_metaclass_enforcement import ComponentMeta, camel_to_kebab, reset_registry


def test_camel_to_kebab_simple():
    assert camel_to_kebab("UserProfile") == "user-profile"


def test_camel_to_kebab_with_an_acronym():
    assert camel_to_kebab("HTTPServer") == "http-server"


def test_camel_to_kebab_single_word():
    assert camel_to_kebab("Widget") == "widget"


def test_the_abstract_base_is_not_registered():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    assert ComponentMeta.registry == {}


def test_instantiating_the_abstract_base_raises():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    with pytest.raises(TypeError, match="Component"):
        Component()


def test_abstract_init_never_runs():
    reset_registry()
    calls = []

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class Base(Component, abstract=True):
        def __init__(self) -> None:
            calls.append("init")

    with pytest.raises(TypeError):
        Base()

    assert calls == []


def test_a_concrete_subclass_registers_by_derived_slug():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class UserProfile(Component):
        kind = "profile"

    assert UserProfile.slug == "user-profile"
    assert ComponentMeta.registry == {"user-profile": UserProfile}


def test_an_explicit_slug_overrides_the_derived_one():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class UserProfile(Component, slug="profile-widget"):
        kind = "profile"

    assert UserProfile.slug == "profile-widget"
    assert ComponentMeta.registry == {"profile-widget": UserProfile}


def test_concrete_class_without_kind_is_rejected():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    with pytest.raises(TypeError, match="kind"):

        class Broken(Component):
            pass


def test_kind_may_be_inherited_from_a_concrete_ancestor():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class Base(Component):
        kind = "widget"

    class Sub(Base):
        pass

    assert Sub.slug == "sub"
    assert ComponentMeta.registry == {"base": Base, "sub": Sub}


def test_duplicate_slug_is_rejected():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class Alpha(Component, slug="shared"):
        kind = "a"

    with pytest.raises(ValueError, match="shared"):

        class Beta(Component, slug="shared"):
            kind = "b"


def test_lowercase_class_names_are_rejected():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    with pytest.raises(TypeError, match="PascalCase"):

        class lowercase(Component):
            kind = "x"


def test_lowercase_is_rejected_even_for_abstract_classes():
    reset_registry()

    with pytest.raises(TypeError, match="PascalCase"):

        class lowercase(metaclass=ComponentMeta, abstract=True):
            pass


def test_intermediate_abstract_layers_are_never_registered():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class Base(Component, abstract=True):
        kind = "ignored"

    assert ComponentMeta.registry == {}

    class Concrete(Base):
        pass

    assert ComponentMeta.registry == {"concrete": Concrete}


def test_concrete_instances_construct_normally():
    reset_registry()

    class Component(metaclass=ComponentMeta, abstract=True):
        pass

    class Widget(Component):
        kind = "widget"

        def __init__(self, label: str) -> None:
            self.label = label

    widget = Widget("ok")
    assert widget.label == "ok"
    assert isinstance(widget, Widget)
