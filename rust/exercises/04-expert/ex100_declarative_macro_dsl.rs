//! Exercise 100 — A `macro_rules!` DSL with nested repetition (expert).
//! Goal:   `routes! { ... }`, a tiny DSL for declaring a routing table:
//!
//!         ```ignore
//!         let table: Vec<Route> = routes! {
//!             "users" => {
//!                 "GET" => "list_users",
//!                 "POST" => "create_user",
//!             },
//!             "posts" => {
//!                 "GET" => "list_posts",
//!             },
//!         };
//!         ```
//!
//!         The macro's MATCHER (which syntax it accepts) is already written
//!         below — an outer `$(...)* ` repetition over paths, each
//!         containing an INNER `$(...)* ` repetition over that path's
//!         method/handler pairs. Your job is the TRANSCRIBER: the nested
//!         `vec![...]` construction that turns those captured repetitions
//!         into a `Vec<Route>`.
//! Drills: `macro_rules!` repetition (`$(...)* `) nested two levels deep,
//!         trailing-comma handling (`$(,)?`), `$crate::` paths so the macro
//!         works regardless of where it's invoked from.

/// One route: a path, and the (method, handler) pairs registered under it.
#[derive(Debug, PartialEq)]
pub struct Route {
    pub path: &'static str,
    pub handlers: Vec<(&'static str, &'static str)>,
}

/// See the module doc above. The matcher (left of `=>`) is the given
/// scaffold; only the transcriber (right of `=>`) is the TODO.
macro_rules! routes {
    ( $( $path:literal => { $( $method:literal => $handler:literal ),* $(,)? } ),* $(,)? ) => {
        todo!(
            "build a Vec<Route> via NESTED repetition: for each outer $path (there are as \
             many Routes as $(...)* repeats at the outer level), build one Route whose \
             `handlers` field is itself built from the INNER $(...)* over that path's \
             $method/$handler pairs — i.e. vec![ $crate::ex100_declarative_macro_dsl::Route \
             {{ path: $path, handlers: vec![ (that path's $method, $handler pairs) ] }}, ... ]"
        )
    };
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn builds_a_single_route_with_a_single_handler() {
        let table: Vec<Route> = routes! {
            "users" => {
                "GET" => "list_users",
            },
        };
        assert_eq!(
            table,
            vec![Route {
                path: "users",
                handlers: vec![("GET", "list_users")],
            }]
        );
    }

    #[test]
    fn builds_a_route_with_multiple_handlers() {
        let table: Vec<Route> = routes! {
            "users" => {
                "GET" => "list_users",
                "POST" => "create_user",
            },
        };
        assert_eq!(table.len(), 1);
        assert_eq!(
            table[0].handlers,
            vec![("GET", "list_users"), ("POST", "create_user")]
        );
    }

    #[test]
    fn builds_multiple_routes_each_with_their_own_handlers() {
        let table: Vec<Route> = routes! {
            "users" => {
                "GET" => "list_users",
                "POST" => "create_user",
            },
            "posts" => {
                "GET" => "list_posts",
            },
        };
        assert_eq!(table.len(), 2);
        assert_eq!(table[0].path, "users");
        assert_eq!(table[0].handlers.len(), 2);
        assert_eq!(table[1].path, "posts");
        assert_eq!(table[1].handlers, vec![("GET", "list_posts")]);
    }

    #[test]
    fn trailing_commas_are_accepted_at_both_nesting_levels() {
        let table: Vec<Route> = routes! {
            "users" => {
                "GET" => "list_users",
            },
        };
        assert_eq!(table[0].handlers, vec![("GET", "list_users")]);
    }

    #[test]
    fn works_without_any_trailing_commas() {
        let table: Vec<Route> = routes! {
            "users" => {
                "GET" => "list_users"
            }
        };
        assert_eq!(table[0].handlers, vec![("GET", "list_users")]);
    }
}
