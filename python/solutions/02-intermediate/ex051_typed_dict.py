"""Exercise 051 — TypedDict (reference solution)."""

from typing import Any, NotRequired, TypedDict


class Address(TypedDict):
    street: str
    city: str


class UserRecord(TypedDict):
    id: int
    name: str
    email: NotRequired[str]
    address: NotRequired[Address]


class Partial(TypedDict, total=False):
    a: int
    b: str


def make_user(user_id: int, name: str, email: str | None = None) -> UserRecord:
    user: UserRecord = {"id": user_id, "name": name}
    if email is not None:
        # Omitting the key beats storing None: the declared type is str, not str|None.
        user["email"] = email
    return user


def _is_address(value: Any) -> bool:
    if not isinstance(value, dict):
        return False
    if set(value) != {"street", "city"}:
        return False
    return isinstance(value["street"], str) and isinstance(value["city"], str)


def is_valid_user(value: Any) -> bool:
    # isinstance(value, UserRecord) is a TypeError: a TypedDict is only a dict at
    # runtime, so the schema has to be checked by hand.
    if not isinstance(value, dict):
        return False

    known = {"id", "name", "email", "address"}
    if not known.issuperset(value):
        return False
    if not {"id", "name"}.issubset(value):
        return False

    # bool is a subclass of int, so exclude it explicitly for an `id`.
    if not isinstance(value["id"], int) or isinstance(value["id"], bool):
        return False
    if not isinstance(value["name"], str):
        return False
    if "email" in value and not isinstance(value["email"], str):
        return False
    if "address" in value and not _is_address(value["address"]):
        return False
    return True


def display_name(user: UserRecord) -> str:
    email = user.get("email")
    return f"{user['name']} <{email}>" if email else user["name"]


def city_of(user: UserRecord, default: str = "unknown") -> str:
    address = user.get("address")
    return default if address is None else address["city"]


def merge_partial(base: Partial, override: Partial) -> Partial:
    return {**base, **override}


def required_keys() -> set[str]:
    return set(UserRecord.__required_keys__)
