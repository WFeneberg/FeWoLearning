import pytest

from ex095_dsl_interpreter import DSLSyntaxError, Token, evaluate, parse, tokenize


def test_tokenize_basic_arithmetic():
    tokens = tokenize("1 + 2")

    assert tokens == [
        Token("NUMBER", "1"),
        Token("PLUS", "+"),
        Token("NUMBER", "2"),
        Token("EOF", ""),
    ]


def test_tokenize_skips_whitespace_and_reads_identifiers():
    tokens = tokenize("  x1 * 2 ")

    assert [t.kind for t in tokens] == ["IDENT", "STAR", "NUMBER", "EOF"]
    assert tokens[0].text == "x1"


def test_tokenize_rejects_an_unknown_character():
    with pytest.raises(DSLSyntaxError):
        tokenize("1 + @")


def test_evaluate_simple_addition():
    assert evaluate(parse("1 + 2"), {}) == 3


def test_multiplication_binds_tighter_than_addition():
    assert evaluate(parse("1 + 2 * 3"), {}) == 7


def test_parentheses_override_precedence():
    assert evaluate(parse("(1 + 2) * 3"), {}) == 9


def test_subtraction_is_left_associative():
    assert evaluate(parse("10 - 3 - 2"), {}) == 5


def test_division_is_left_associative():
    assert evaluate(parse("100 / 5 / 2"), {}) == 10


def test_unary_minus():
    assert evaluate(parse("-5 + 3"), {}) == -2


def test_unary_minus_before_a_parenthesized_expression():
    assert evaluate(parse("-(2 + 3)"), {}) == -5


def test_decimal_numbers():
    assert evaluate(parse("1.5 + 2.5"), {}) == 4.0


def test_variables_are_looked_up_in_the_environment():
    assert evaluate(parse("x + y"), {"x": 3, "y": 4}) == 7


def test_undefined_variable_raises_name_error():
    with pytest.raises(NameError):
        evaluate(parse("x + 1"), {})


def test_division_by_zero_propagates():
    with pytest.raises(ZeroDivisionError):
        evaluate(parse("1 / 0"), {})


def test_a_nested_expression():
    assert evaluate(parse("2 * (3 + 4) - 1"), {}) == 13


def test_trailing_tokens_are_a_syntax_error():
    with pytest.raises(DSLSyntaxError):
        parse("1 2")


def test_an_unclosed_parenthesis_is_a_syntax_error():
    with pytest.raises(DSLSyntaxError):
        parse("(1 + 2")


def test_a_dangling_operator_is_a_syntax_error():
    with pytest.raises(DSLSyntaxError):
        parse("1 +")


def test_an_empty_expression_is_a_syntax_error():
    with pytest.raises(DSLSyntaxError):
        parse("")
