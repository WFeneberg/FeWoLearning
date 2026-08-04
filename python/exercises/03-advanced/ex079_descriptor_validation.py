"""Exercise 079 — descriptors for validation (advanced).

Goal:   Build the reusable validation primitive that `property` cannot be: one
        descriptor class, reused across many attributes and many owner classes.
Drills: `__set_name__`, `__get__`, `__set__`, per-instance storage in `__dict__`,
        data descriptors winning over the instance dict, and class-level access
        returning the descriptor itself.
Passes: when `pytest exercises/03-advanced/test_ex079_descriptor_validation.py` is green.

Note:   the classic bug is storing the value on the *descriptor* (``self.value = …``).
        A descriptor is a class attribute, so one instance would then overwrite every
        other instance's value. Store it under a private key in the *instance's*
        ``__dict__`` — and let `__set_name__` tell you what that key should be called.

Note:   the owner classes live in the test file, not here. `__set_name__` runs when the
        owner class is *created*, so a module-scope owner class would fire these stubs
        at import time and break collection rather than failing a test.

        A finished owner class looks like this — and contains no validation at all,
        which is the payoff::

            class Product:
                name = NonEmptyString()
                price = Positive()
                category = OneOf("tools", "toys", "books")
"""

from typing import Any


class Validated:
    """Base class for validating data descriptors.

    Subclasses implement `validate`, which either returns the value to store
    (possibly transformed) or raises. Everything else lives here.
    """

    def __set_name__(self, owner: type, name: str) -> None:
        """Record the attribute name Python bound this descriptor to.

        Keep the public name (for error messages) as `public_name` and derive a private
        storage key ``"_" + name`` as `private_name` for the instance dict.
        """
        raise NotImplementedError

    def __get__(self, instance: Any, owner: type | None = None) -> Any:
        """Return the stored value.

        Accessed on the *class* (``instance is None``) return the descriptor itself —
        that is what makes ``Product.price`` introspectable. Accessed on an instance
        that was never assigned, raise AttributeError naming the attribute.
        """
        raise NotImplementedError

    def __set__(self, instance: Any, value: Any) -> None:
        """Validate, then store the result under the private key."""
        raise NotImplementedError

    def validate(self, value: Any) -> Any:
        """Return the value to store, or raise. Subclasses must override."""
        raise NotImplementedError


class Positive(Validated):
    """Accepts ints and floats greater than zero.

    A non-number raises TypeError; zero or negative raises ValueError. Both messages
    must contain the attribute name, which is why `__set_name__` exists. Watch out for
    `bool`, which is a subclass of `int` and must be rejected as a type error.
    """

    def validate(self, value: Any) -> Any:
        raise NotImplementedError


class NonEmptyString(Validated):
    """Accepts non-blank strings, stored stripped.

    A non-string raises TypeError; blank or whitespace-only raises ValueError. Both
    messages must contain the attribute name.
    """

    def validate(self, value: Any) -> Any:
        raise NotImplementedError


class OneOf(Validated):
    """Accepts only one of a fixed set of options.

    ``OneOf("a", "b")``; anything else raises ValueError whose message contains the
    attribute name and the options in the order they were given.
    """

    def __init__(self, *options: Any) -> None:
        """Store the options. Constructing with no options at all raises ValueError."""
        raise NotImplementedError

    def validate(self, value: Any) -> Any:
        raise NotImplementedError


def storage_keys(instance: Any) -> list[str]:
    """Return the sorted keys a descriptor-backed instance actually stores.

    Proof that the values live in the instance's own ``__dict__`` under private names,
    and not on the shared descriptors.
    """
    raise NotImplementedError
