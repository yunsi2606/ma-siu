import 'package:flutter/material.dart';

/// Vouchers page - shows active vouchers
class VouchersPage extends StatelessWidget {
  const VouchersPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Voucher')),
      body: const Center(child: Text('Vouchers - Coming soon')),
    );
  }
}
