from ex087_singledispatch_dispatch import Formatter, describe


def test_bool_dispatch_is_not_swallowed_by_the_int_registration():
    # If describe_bool were never registered, True/False would fall through to the
    # int implementation (bool is an int subclass) and this would read "the integer 1".
    assert describe(True) == "yes"
    assert describe(False) == "no"
    assert describe(True) != "the integer 1"


def test_int():
    assert describe(3) == "the integer 3"


def test_float():
    assert describe(3.5) == "the float 3.5"


def test_str():
    assert describe("hi") == 'the string "hi"'


def test_list():
    assert describe([1, 2, 3]) == "a list of 3 items"


def test_registry_is_complete_and_the_fallback_still_covers_everything_else():
    assert {object, bool, int, float, str, list} <= set(describe.registry)
    assert describe(None) == "a NoneType"
    assert describe({"a": 1}) == "a dict"


def test_formatter_dispatches_int():
    assert Formatter().format(5) == "#5"


def test_formatter_dispatches_str():
    assert Formatter().format("x") == "'x'"


def test_formatter_dispatch_is_shared_across_instances_and_still_falls_back():
    a = Formatter()
    b = Formatter()
    assert a.format(1) == b.format(1) == "#1"
    assert Formatter().format(3.5) == "<3.5>"
