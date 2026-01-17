import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'reward_service.dart';

/// Rewards page - shows reward catalog and redemption
class RewardsPage extends ConsumerWidget {
  const RewardsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final rewardsAsync = ref.watch(rewardCatalogProvider);
    final balanceAsync = ref.watch(pointsBalanceProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Đổi quà')),
      body: Column(
        children: [
          // Points balance header
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(20),
            decoration: const BoxDecoration(
              gradient: LinearGradient(
                colors: [Color(0xFF667EEA), Color(0xFF764BA2)],
              ),
            ),
            child: balanceAsync.when(
              loading: () => const Center(
                child: CircularProgressIndicator(color: Colors.white),
              ),
              error: (_, __) =>
                  const Text('--', style: TextStyle(color: Colors.white)),
              data: (balance) => Column(
                children: [
                  const Text(
                    'Điểm của bạn',
                    style: TextStyle(color: Colors.white70, fontSize: 14),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    '${balance?.availablePoints ?? 0}',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 36,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  if (balance != null && balance.pendingPoints > 0)
                    Text(
                      '(+${balance.pendingPoints} đang chờ)',
                      style: const TextStyle(
                        color: Colors.white60,
                        fontSize: 12,
                      ),
                    ),
                ],
              ),
            ),
          ),

          // Reward catalog
          Expanded(
            child: RefreshIndicator(
              onRefresh: () async {
                ref.invalidate(rewardCatalogProvider);
                ref.invalidate(pointsBalanceProvider);
              },
              child: rewardsAsync.when(
                loading: () => const Center(child: CircularProgressIndicator()),
                error: (_, __) =>
                    const Center(child: Text('Không thể tải danh sách quà')),
                data: (rewards) => rewards.isEmpty
                    ? const Center(child: Text('Chưa có quà nào'))
                    : GridView.builder(
                        padding: const EdgeInsets.all(16),
                        gridDelegate:
                            const SliverGridDelegateWithFixedCrossAxisCount(
                              crossAxisCount: 2,
                              childAspectRatio: 0.75,
                              crossAxisSpacing: 12,
                              mainAxisSpacing: 12,
                            ),
                        itemCount: rewards.length,
                        itemBuilder: (context, index) {
                          final reward = rewards[index];
                          final balance = balanceAsync.value;
                          final canRedeem =
                              balance != null &&
                              balance.availablePoints >= reward.pointsCost &&
                              reward.isAvailable;

                          return _RewardCard(
                            reward: reward,
                            canRedeem: canRedeem,
                            onRedeem: () => _handleRedeem(context, ref, reward),
                          );
                        },
                      ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _handleRedeem(
    BuildContext context,
    WidgetRef ref,
    Reward reward,
  ) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Xác nhận đổi quà'),
        content: Text(
          'Bạn muốn đổi ${reward.pointsCost} điểm lấy "${reward.name}"?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Hủy'),
          ),
          ElevatedButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Đổi ngay'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      final success = await ref.read(rewardServiceProvider).redeem(reward.id);
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(success ? 'Đổi quà thành công!' : 'Đổi quà thất bại'),
            backgroundColor: success ? Colors.green : Colors.red,
          ),
        );
        if (success) {
          ref.invalidate(rewardCatalogProvider);
          ref.invalidate(pointsBalanceProvider);
        }
      }
    }
  }
}

class _RewardCard extends StatelessWidget {
  final Reward reward;
  final bool canRedeem;
  final VoidCallback onRedeem;

  const _RewardCard({
    required this.reward,
    required this.canRedeem,
    required this.onRedeem,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      clipBehavior: Clip.antiAlias,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Image
          AspectRatio(
            aspectRatio: 1,
            child: reward.imageUrl != null
                ? Image.network(
                    reward.imageUrl!,
                    fit: BoxFit.cover,
                    errorBuilder: (_, __, ___) => _placeholder(),
                  )
                : _placeholder(),
          ),
          // Info
          Padding(
            padding: const EdgeInsets.all(8),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  reward.name,
                  style: const TextStyle(fontWeight: FontWeight.bold),
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                ),
                const SizedBox(height: 4),
                Row(
                  children: [
                    const Icon(Icons.stars, size: 14, color: Colors.amber),
                    const SizedBox(width: 4),
                    Text(
                      '${reward.pointsCost}',
                      style: const TextStyle(fontWeight: FontWeight.bold),
                    ),
                    const Spacer(),
                    if (reward.quantityAvailable > 0)
                      Text(
                        'Còn ${reward.quantityAvailable}',
                        style: TextStyle(
                          fontSize: 10,
                          color: Colors.grey.shade600,
                        ),
                      ),
                  ],
                ),
                const SizedBox(height: 8),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: canRedeem ? onRedeem : null,
                    style: ElevatedButton.styleFrom(
                      padding: const EdgeInsets.symmetric(vertical: 4),
                    ),
                    child: const Text('Đổi', style: TextStyle(fontSize: 12)),
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _placeholder() {
    return Container(
      color: Colors.grey.shade200,
      child: const Icon(Icons.card_giftcard, size: 48, color: Colors.grey),
    );
  }
}
