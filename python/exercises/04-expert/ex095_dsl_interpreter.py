"""Exercise 095 — a tiny expression language: tokenizer, parser, evaluator (expert).

Goal:   Build the three classic stages of an interpreter for a small arithmetic
        language with variables: turn text into tokens, tokens into an AST, and an
        AST (plus an environment) into a value.
Drills: hand-written lexing (skip whitespace, recognize numbers/identifiers/
        operators, reject anything else), recursive-descent parsing with operator
        precedence baked into the grammar (`expr` calls `term` calls `factor`, so
        `*`/`/` bind tighter than `+`/`-` without a precedence table), left
        associativity from looping rather than recursing on the same level, and
        evaluating an AST by dispatching on node type.
Passes: when `pytest exercises/04-expert/test_ex095_dsl_interpreter.py` is green.

Note:   the grammar, as one operator-precedence-climbing chain:

            expr   := term (("+" | "-") term)*
            term   := factor (("*" | "/") factor)*
            factor := NUMBER | IDENT | "-" factor | "(" expr ")"

        Recursing back to `parse_expr` inside the `"(" expr ")"` case of `factor` is
        what makes parentheses reset precedence.
"""

from dataclasses import dataclass
from typing import Union


class DSLSyntaxError(Exception):
    """Raised for anything the tokenizer or parser cannot make sense of."""


@dataclass(frozen=True)
class Token:
    kind: str  # "NUMBER", "IDENT", "PLUS", "MINUS", "STAR", "SLASH", "LPAREN", "RPAREN", "EOF"
    text: str


@dataclass(frozen=True)
class Num:
    value: float


@dataclass(frozen=True)
class Var:
    name: str


@dataclass(frozen=True)
class BinOp:
    op: str  # "+", "-", "*", "/"
    left: "Expr"
    right: "Expr"


@dataclass(frozen=True)
class UnaryOp:
    op: str  # "-"
    operand: "Expr"


Expr = Union[Num, Var, BinOp, UnaryOp]

_SINGLE_CHAR_TOKENS = {
    "+": "PLUS",
    "-": "MINUS",
    "*": "STAR",
    "/": "SLASH",
    "(": "LPAREN",
    ")": "RPAREN",
}


def tokenize(source: str) -> list[Token]:
    """Turn `source` into a list of `Token`, ending with one `Token("EOF", "")`.

    Skip spaces. A run of digits (with at most one ``.``) is a `NUMBER`. A run of
    letters/digits/underscores starting with a letter or underscore is an `IDENT`.
    `+ - * / ( )` are each their own single-character token. Anything else raises
    `DSLSyntaxError` naming the offending character.
    """
    raise NotImplementedError


class Parser:
    """Recursive-descent parser over a token list, one `Expr` out of `parse()`."""

    def __init__(self, tokens: list[Token]) -> None:
        raise NotImplementedError

    def parse(self) -> Expr:
        """Parse a full expression and require an `EOF` token right after it —
        trailing input (``"1 2"``) is a `DSLSyntaxError`."""
        raise NotImplementedError

    def parse_expr(self) -> Expr:
        """`term (("+" | "-") term)*`, left-associative."""
        raise NotImplementedError

    def parse_term(self) -> Expr:
        """`factor (("*" | "/") factor)*`, left-associative."""
        raise NotImplementedError

    def parse_factor(self) -> Expr:
        """`NUMBER | IDENT | "-" factor | "(" expr ")"`.

        An unclosed `(` (running out of tokens, or the next token is not `)`) is a
        `DSLSyntaxError`, as is any token that cannot start a factor at all.
        """
        raise NotImplementedError


def parse(source: str) -> Expr:
    """Tokenize and parse `source` in one call."""
    raise NotImplementedError


def evaluate(expr: Expr, env: dict[str, float]) -> float:
    """Evaluate `expr` under variable bindings `env`.

    An undefined `Var` name raises NameError. Division follows Python's ``/``
    (including its `ZeroDivisionError`) — nothing here needs to catch it.
    """
    raise NotImplementedError
