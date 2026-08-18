// Exercise 079 - Hero shared element transitions (advanced).
//
// Goal:   Build a two-screen app where tapping a Hero on the first screen
//         navigates to a second screen sharing the same Hero tag.
// Drills: Hero, Navigator.push, shared element transitions.
// Passes: when both the home screen and the pushed detail screen contain a
//         Hero widget with the same tag ("logo").

import 'package:flutter/material.dart';

const heroTag = 'logo';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}

class DetailPage extends StatelessWidget {
  const DetailPage({super.key});

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}

Widget buildHeroDemoApp() {
  throw UnimplementedError('TODO');
}
