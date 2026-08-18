// Exercise 100 - a hand-built declarative router (expert).
//
// Goal:   Implement AppRouter.onGenerateRoute so a small path -> WidgetBuilder
//         table drives Navigator, falling back to a "not found" page for
//         unknown routes.
// Drills: Navigator.onGenerateRoute, MaterialPageRoute, declarative route
//         tables instead of imperative push() call sites.
// Passes: when onGenerateRoute() looks up settings.name in the route table
//         and returns a MaterialPageRoute building the matching widget, or
//         a MaterialPageRoute building the notFoundBuilder widget when the
//         name isn't registered.

import 'package:flutter/material.dart';

typedef RouteWidgetBuilder = Widget Function(BuildContext context);

class AppRouter {
  AppRouter(this.routes, {required this.notFoundBuilder});

  final Map<String, RouteWidgetBuilder> routes;
  final RouteWidgetBuilder notFoundBuilder;

  Route<dynamic> onGenerateRoute(RouteSettings settings) {
    throw UnimplementedError('TODO');
  }
}
