// Exercise 099 - deterministic percentage-based feature flag rollout
// (reference solution).

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
    final bucket = _stableHash('$name:$userId') % 100;
    return bucket < rolloutPercent;
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
    return flag.isEnabledFor(userId) ? onChild : offChild;
  }
}
