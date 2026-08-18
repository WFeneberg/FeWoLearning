import 'dart:convert';

import 'package:test/test.dart';

import 'json_encode_decode.dart';

void main() {
  test('encodeUser produces valid JSON', () {
    final json = encodeUser({'name': 'Ada', 'age': 30});
    expect(jsonDecode(json), {'name': 'Ada', 'age': 30});
  });

  test('decodeUser parses JSON back into a map', () {
    expect(decodeUser('{"name":"Ada","age":30}'), {'name': 'Ada', 'age': 30});
  });

  test('round-trip preserves the data', () {
    final original = {'city': 'Berlin', 'zip': 10115};
    expect(decodeUser(encodeUser(original)), original);
  });
}
