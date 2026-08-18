// Exercise 062 - Form/TextFormField validation (reference solution).

import 'package:flutter/material.dart';

String? emailValidator(String? value) {
  if (value == null || !value.contains('@')) {
    return 'Enter a valid email';
  }
  return null;
}

class EmailForm extends StatefulWidget {
  const EmailForm({super.key});

  @override
  State<EmailForm> createState() => _EmailFormState();
}

class _EmailFormState extends State<EmailForm> {
  final _formKey = GlobalKey<FormState>();
  String _message = '';

  @override
  Widget build(BuildContext context) {
    return Form(
      key: _formKey,
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          TextFormField(validator: emailValidator),
          ElevatedButton(
            onPressed: () {
              setState(() {
                _message = _formKey.currentState!.validate() ? 'Valid' : 'Invalid email';
              });
            },
            child: const Text('Submit'),
          ),
          Text(_message),
        ],
      ),
    );
  }
}
