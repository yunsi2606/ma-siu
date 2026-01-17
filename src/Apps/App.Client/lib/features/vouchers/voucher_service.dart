import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../core/api/api_client.dart';

/// Voucher model
class Voucher {
  final String id;
  final String code;
  final String platform;
  final String? description;
  final int remainingPercent;
  final DateTime expiresAt;

  Voucher({
    required this.id,
    required this.code,
    required this.platform,
    this.description,
    required this.remainingPercent,
    required this.expiresAt,
  });

  factory Voucher.fromJson(Map<String, dynamic> json) {
    return Voucher(
      id: json['id'],
      code: json['code'],
      platform: json['platform'],
      description: json['description'],
      remainingPercent: json['remainingPercent'] ?? 100,
      expiresAt: DateTime.parse(json['expiresAt']),
    );
  }

  VoucherStatus get status {
    if (remainingPercent >= 50) return VoucherStatus.green;
    if (remainingPercent >= 20) return VoucherStatus.yellow;
    return VoucherStatus.red;
  }
}

enum VoucherStatus { green, yellow, red }

/// Voucher service provider
final voucherServiceProvider = Provider<VoucherService>((ref) {
  return VoucherService(ref.watch(apiClientProvider));
});

/// Active vouchers provider
final activeVouchersProvider = FutureProvider<List<Voucher>>((ref) async {
  return ref.watch(voucherServiceProvider).getActiveVouchers();
});

/// Voucher service
class VoucherService {
  final ApiClient _apiClient;

  VoucherService(this._apiClient);

  Future<List<Voucher>> getActiveVouchers() async {
    try {
      final response = await _apiClient.get('/api/vouchers/active');
      if (response.statusCode == 200) {
        final data = response.data as Map<String, dynamic>;
        return (data['vouchers'] as List)
            .map((json) => Voucher.fromJson(json))
            .toList();
      }
      return [];
    } catch (e) {
      return [];
    }
  }
}
