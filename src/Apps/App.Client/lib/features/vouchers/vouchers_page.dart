import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'voucher_service.dart';
import '../../shared/theme/app_theme.dart';

/// Vouchers page - shows active vouchers
class VouchersPage extends ConsumerWidget {
  const VouchersPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final vouchersAsync = ref.watch(activeVouchersProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Voucher')),
      body: RefreshIndicator(
        onRefresh: () async {
          ref.invalidate(activeVouchersProvider);
        },
        child: vouchersAsync.when(
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (_, __) => const Center(child: Text('Không thể tải voucher')),
          data: (vouchers) => vouchers.isEmpty
              ? const Center(child: Text('Chưa có voucher nào'))
              : ListView.builder(
                  padding: const EdgeInsets.all(16),
                  itemCount: vouchers.length,
                  itemBuilder: (context, index) {
                    final voucher = vouchers[index];
                    return _VoucherCard(voucher: voucher);
                  },
                ),
        ),
      ),
    );
  }
}

class _VoucherCard extends StatelessWidget {
  final Voucher voucher;

  const _VoucherCard({required this.voucher});

  Color _getStatusColor() {
    switch (voucher.status) {
      case VoucherStatus.green:
        return Colors.green;
      case VoucherStatus.yellow:
        return Colors.orange;
      case VoucherStatus.red:
        return Colors.red;
    }
  }

  Color _getPlatformColor() {
    switch (voucher.platform.toLowerCase()) {
      case 'shopee':
        return AppTheme.shopeeColor;
      case 'lazada':
        return AppTheme.lazadaColor;
      case 'tiktok':
        return AppTheme.tiktokColor;
      default:
        return AppTheme.primaryColor;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: InkWell(
        onTap: () {
          Clipboard.setData(ClipboardData(text: voucher.code));
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Đã copy mã: ${voucher.code}'),
              duration: const Duration(seconds: 2),
            ),
          );
        },
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              // Platform badge
              Container(
                width: 60,
                height: 60,
                decoration: BoxDecoration(
                  color: _getPlatformColor(),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Center(
                  child: Text(
                    voucher.platform.substring(0, 1).toUpperCase(),
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ),
              const SizedBox(width: 16),
              // Voucher info
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Container(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 8,
                            vertical: 2,
                          ),
                          decoration: BoxDecoration(
                            color: Colors.grey.shade200,
                            borderRadius: BorderRadius.circular(4),
                          ),
                          child: Text(
                            voucher.code,
                            style: const TextStyle(
                              fontFamily: 'monospace',
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ),
                        const Spacer(),
                        Icon(Icons.copy, size: 16, color: Colors.grey.shade400),
                      ],
                    ),
                    if (voucher.description != null) ...[
                      const SizedBox(height: 4),
                      Text(
                        voucher.description!,
                        style: Theme.of(context).textTheme.bodyMedium,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                    const SizedBox(height: 8),
                    // Progress bar
                    Row(
                      children: [
                        Expanded(
                          child: ClipRRect(
                            borderRadius: BorderRadius.circular(4),
                            child: LinearProgressIndicator(
                              value: voucher.remainingPercent / 100,
                              backgroundColor: Colors.grey.shade200,
                              color: _getStatusColor(),
                              minHeight: 6,
                            ),
                          ),
                        ),
                        const SizedBox(width: 8),
                        Text(
                          '${voucher.remainingPercent}%',
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                            color: _getStatusColor(),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
