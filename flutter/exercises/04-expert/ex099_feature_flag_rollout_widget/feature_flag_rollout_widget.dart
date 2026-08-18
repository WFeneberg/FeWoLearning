// Exercise 099 - deterministic percentage-based feature flag rollout
// (expert).
//
// Goal:   Decide whether a feature is enabled for a given user by hashing
//         the user id into a stable 0-99 bucket and comparing it against a
//         rollout percentage, then render different widgets based on that
//         decision.
// Drills: deterministic hashing for stable bucketing, percentage rollout
//         logic, conditional widget composition.
// Passes: when isEnabledFor() is deterministic (same user id -> same
//         result every call) and roughly respects the rollout percentage
//         across many distinct ids, and FeatureFlagView renders the "on"
//         child only when the flag is enabled for the given user.

import 'package:flutter/widgets.dart';

int _stableHash(String input) {
  var hash = 0;
  for (final codeUnit in input.codeUnits) {
    hash = (hash * 31 + codeUnit) & 0x7fffffff;
  }
  return hash;
}

class FeatureFlag {
  const FeatureFlag(this.name, this.rolloutPercent)
      : assert(rolloutPercent >= 0 && rolloutPercent <= 100);

  final String name;
  final int rolloutPercent;

  bool isEnabledFor(String userId) {
    throw UnimplementedError('TODO');
  }
}

class FeatureFlagView extends StatelessWidget {
  const FeatureFlagView({
    super.key,
    required this.flag,
    required this.userId,
    required this.onChild,
    required this.offChild,
  });

  final FeatureFlag flag;
  final String userId;
  final Widget onChild;
  final Widget offChild;

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
