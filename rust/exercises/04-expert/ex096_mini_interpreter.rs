//! Exercise 096 — A mini interpreter: tokenizer, AST, evaluation,
//! environments (expert).
//! Goal:   a tiny interpreter for arithmetic-with-variables programs like
//!         `"let x = 10; let y = x * 2; x + y;"`. Four stages, each a small
//!         piece: `tokenize` (text → `Token`s), a recursive-descent `Parser`
//!         (tokens → `Stmt`/`Expr` AST, handling `*`/`/` binding tighter than
//!         `+`/`-`, and parentheses), `eval_expr` (AST → `i64`, resolving
//!         variables against an `Environment`), and `run` (drives a whole
//!         program, updating the environment for `let` and collecting the
//!         value of every bare-expression statement).
//! Drills: a hand-written lexer, recursive-descent parsing with operator
//!         precedence via two mutually-deferring functions, tree-walking
//!         evaluation, a `HashMap`-backed environment.

use std::collections::HashMap;

#[derive(Debug, Clone, PartialEq)]
pub enum Token {
    Number(i64),
    Ident(String),
    Plus,
    Minus,
    Star,
    Slash,
    LParen,
    RParen,
    Let,
    Equals,
    Semicolon,
}

/// Splits `input` into tokens. Whitespace is skipped. The bare word `let` is
/// the `Let` keyword; any other run of ASCII letters/digits/`_` starting
/// with a letter or `_` is an `Ident`; a run of ASCII digits is a `Number`.
pub fn tokenize(input: &str) -> Vec<Token> {
    todo!(
        "scan `input` char by char: skip whitespace; single-char tokens for \
         + - * / ( ) = ;; runs of ascii_digit -> Number (parse the run as i64); \
         runs starting with an ascii letter/'_' (then letters/digits/'_') -> \
         Ident, except the exact word \"let\" which becomes Token::Let"
    )
}

#[derive(Debug, Clone, PartialEq)]
pub enum Expr {
    Number(i64),
    Var(String),
    Add(Box<Expr>, Box<Expr>),
    Sub(Box<Expr>, Box<Expr>),
    Mul(Box<Expr>, Box<Expr>),
    Div(Box<Expr>, Box<Expr>),
}

#[derive(Debug, Clone, PartialEq)]
pub enum Stmt {
    /// `let NAME = EXPR ;`
    Let(String, Expr),
    /// `EXPR ;` — a bare expression statement, evaluated for its value.
    Expr(Expr),
}

/// A recursive-descent parser over a token slice, tracking its own read
/// position.
struct Parser<'t> {
    tokens: &'t [Token],
    pos: usize,
}

impl<'t> Parser<'t> {
    fn new(tokens: &'t [Token]) -> Self {
        Self { tokens, pos: 0 }
    }

    fn peek(&self) -> Option<&Token> {
        self.tokens.get(self.pos)
    }

    fn advance(&mut self) -> Option<&Token> {
        let tok = self.tokens.get(self.pos);
        self.pos += 1;
        tok
    }

    /// `additive := multiplicative (('+' | '-') multiplicative)*` — lower
    /// precedence than `*`/`/`, so `2 + 3 * 4` parses as `2 + (3 * 4)`.
    fn parse_additive(&mut self) -> Expr {
        todo!(
            "parse one multiplicative, then loop: while the next token is Plus or Minus, \
             consume it, parse another multiplicative, and fold the running result into \
             Expr::Add/Expr::Sub (left-associatively) with it"
        )
    }

    /// `multiplicative := primary (('*' | '/') primary)*`
    fn parse_multiplicative(&mut self) -> Expr {
        todo!(
            "parse one primary, then loop: while the next token is Star or Slash, \
             consume it, parse another primary, and fold the running result into \
             Expr::Mul/Expr::Div (left-associatively) with it"
        )
    }

    /// `primary := NUMBER | IDENT | '(' additive ')'`
    fn parse_primary(&mut self) -> Expr {
        todo!(
            "advance one token: Number(n) -> Expr::Number(n); Ident(name) -> Expr::Var(name); \
             LParen -> parse_additive, then require the next token to be RParen; \
             anything else -> panic! with a message naming the unexpected token"
        )
    }

    /// `statement := 'let' IDENT '=' additive ';' | additive ';'`
    fn parse_statement(&mut self) -> Stmt {
        todo!(
            "if the next token is Let: consume it, then an Ident (the name), then Equals, \
             then parse_additive, then Semicolon, producing Stmt::Let(name, expr); \
             otherwise: parse_additive then Semicolon, producing Stmt::Expr(expr); \
             panic! with a message on any unexpected/missing token"
        )
    }
}

/// Parses a full program: zero or more `;`-terminated statements.
pub fn parse(tokens: &[Token]) -> Vec<Stmt> {
    let mut parser = Parser::new(tokens);
    let mut statements = Vec::new();
    while parser.peek().is_some() {
        statements.push(parser.parse_statement());
    }
    statements
}

pub type Environment = HashMap<String, i64>;

/// Evaluates `expr` against `env`, resolving variables by lookup (panics on
/// an undefined variable).
pub fn eval_expr(expr: &Expr, env: &Environment) -> i64 {
    todo!(
        "Number(n) -> n; Var(name) -> look name up in env (panic with a clear message \
         if it's undefined); Add/Sub/Mul/Div(l, r) -> evaluate both sides recursively and \
         apply the corresponding arithmetic operator"
    )
}

/// Runs a full program against `env` (updating it in place for every `let`),
/// returning the value of every bare-expression statement, in order.
pub fn run(program: &[Stmt], env: &mut Environment) -> Vec<i64> {
    todo!(
        "for each statement in order: Stmt::Let(name, expr) evaluates expr against the \
         CURRENT env and stores the result under name (so later statements see it); \
         Stmt::Expr(expr) evaluates expr against the current env and pushes the result \
         onto the Vec this function returns"
    )
}

#[cfg(test)]
mod tests {
    use super::*;

    fn run_program(source: &str) -> Vec<i64> {
        let tokens = tokenize(source);
        let program = parse(&tokens);
        let mut env = Environment::new();
        run(&program, &mut env)
    }

    #[test]
    fn tokenize_produces_expected_tokens() {
        let tokens = tokenize("let x = 1 + 2;");
        assert_eq!(
            tokens,
            vec![
                Token::Let,
                Token::Ident("x".to_string()),
                Token::Equals,
                Token::Number(1),
                Token::Plus,
                Token::Number(2),
                Token::Semicolon,
            ]
        );
    }

    #[test]
    fn evaluates_arithmetic_with_precedence() {
        assert_eq!(run_program("2 + 3 * 4;"), vec![14]);
    }

    #[test]
    fn evaluates_parenthesized_expressions() {
        assert_eq!(run_program("(2 + 3) * 4;"), vec![20]);
    }

    #[test]
    fn subtraction_and_division_are_left_associative() {
        assert_eq!(run_program("20 - 5 - 5;"), vec![10]);
        assert_eq!(run_program("100 / 5 / 2;"), vec![10]);
    }

    #[test]
    fn let_bindings_are_visible_to_later_statements() {
        assert_eq!(run_program("let x = 10; let y = x * 2; x + y;"), vec![30]);
    }

    #[test]
    fn let_statements_produce_no_result_value() {
        assert!(run_program("let x = 5; let y = 6;").is_empty());
    }

    #[test]
    fn multiple_expression_statements_each_produce_a_result() {
        assert_eq!(run_program("1 + 1; 2 + 2; 3 + 3;"), vec![2, 4, 6]);
    }
}
