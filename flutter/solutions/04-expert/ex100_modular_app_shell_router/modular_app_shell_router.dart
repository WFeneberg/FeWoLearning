// Exercise 100 - a hand-built declarative router (reference solution).

import 'package:flutter/material.dart';

typedef RouteWidgetBuilder = Widget Function(BuildContext context);

class AppRouter {
  AppRouter(this.routes, {required this.notFoundBuilder});

  final Map<String, RouteWidgetBuilder> routes;
  final RouteWidgetBuilder notFoundBuilder;

  Route<dynamic> onGenerateRoute(RouteSettings settings) {
    final builder = routes[settings.name] ?? notFoundBuilder;
    return MaterialPageRoute<dynamic>(
      settings: settings,
      builder: builder,
    );
  }
}
