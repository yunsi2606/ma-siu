import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../core/api/api_client.dart';

/// Reward model
class Reward {
  final String id;
  final String name;
  final String description;
  final int pointsCost;
  final String type;
  final int quantityAvailable;
  final String? imageUrl;

  Reward({
    required this.id,
    required this.name,
    required this.description,
    required this.pointsCost,
    required this.type,
    required this.quantityAvailable,
    this.imageUrl,
  });

  factory Reward.fromJson(Map<String, dynamic> json) {
    return Reward(
      id: json['id'],
      name: json['name'],
      description: json['description'],
      pointsCost: json['pointsCost'],
      type: json['type'],
      quantityAvailable: json['quantityAvailable'],
      imageUrl: json['imageUrl'],
    );
  }

  bool get isAvailable => quantityAvailable != 0;
}

/// Points balance model
class PointsBalance {
  final int totalPoints;
  final int availablePoints;
  final int pendingPoints;

  PointsBalance({
    required this.totalPoints,
    required this.availablePoints,
    required this.pendingPoints,
  });

  factory PointsBalance.fromJson(Map<String, dynamic> json) {
    return PointsBalance(
      totalPoints: json['totalPoints'],
      availablePoints: json['availablePoints'],
      pendingPoints: json['pendingPoints'],
    );
  }
}

/// Reward service provider
final rewardServiceProvider = Provider<RewardService>((ref) {
  return RewardService(ref.watch(apiClientProvider));
});

/// Reward catalog provider
final rewardCatalogProvider = FutureProvider<List<Reward>>((ref) async {
  return ref.watch(rewardServiceProvider).getCatalog();
});

/// Points balance provider
final pointsBalanceProvider = FutureProvider<PointsBalance?>((ref) async {
  return ref.watch(rewardServiceProvider).getBalance();
});

/// Reward service
class RewardService {
  final ApiClient _apiClient;

  RewardService(this._apiClient);

  Future<List<Reward>> getCatalog() async {
    try {
      final response = await _apiClient.get('/api/rewards');
      if (response.statusCode == 200) {
        final data = response.data as Map<String, dynamic>;
        return (data['rewards'] as List)
            .map((json) => Reward.fromJson(json))
            .toList();
      }
      return [];
    } catch (e) {
      return [];
    }
  }

  Future<PointsBalance?> getBalance() async {
    try {
      final response = await _apiClient.get('/api/points/balance');
      if (response.statusCode == 200) {
        return PointsBalance.fromJson(response.data);
      }
      return null;
    } catch (e) {
      return null;
    }
  }

  Future<bool> redeem(String rewardId) async {
    try {
      final response = await _apiClient.post('/api/rewards/$rewardId/redeem');
      return response.statusCode == 200;
    } catch (e) {
      return false;
    }
  }
}
