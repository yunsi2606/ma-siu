import 'package:app_client/core/api/api_client.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Task model
class Task {
  final String id;
  final String title;
  final String description;
  final int points;
  final String type; // daily, weekly, one-time
  final bool isCompleted;
  final int progress;
  final int target;
  final String? actionUrl;

  Task({
    required this.id,
    required this.title,
    required this.description,
    required this.points,
    required this.type,
    required this.isCompleted,
    required this.progress,
    required this.target,
    this.actionUrl,
  });

  factory Task.fromJson(Map<String, dynamic> json) {
    return Task(
      id: json['id'],
      title: json['title'],
      description: json['description'],
      points: json['points'],
      type: json['type'],
      isCompleted: json['isCompleted'] ?? false,
      progress: json['progress'] ?? 0,
      target: json['target'] ?? 1,
      actionUrl: json['actionUrl'],
    );
  }

  double get progressPercent =>
      target > 0 ? (progress / target).clamp(0.0, 1.0) : 0.0;
}

/// Tasks provider
final tasksProvider = FutureProvider<List<Task>>((ref) async {
  final apiClient = ref.watch(apiClientProvider);

  try {
    final response = await apiClient.get('/api/tasks');
    if (response.statusCode == 200) {
      final List<dynamic> data = response.data['tasks'] ?? [];
      return data.map((json) => Task.fromJson(json)).toList();
    }
  } catch (e) {
    // Return mock data for demo
  }

  // Mock data
  return [
    Task(
      id: '1',
      title: 'Xem 3 bài viết',
      description: 'Đọc 3 bài viết deal hôm nay',
      points: 10,
      type: 'daily',
      isCompleted: false,
      progress: 1,
      target: 3,
    ),
    Task(
      id: '2',
      title: 'Chia sẻ deal',
      description: 'Chia sẻ 1 deal với bạn bè',
      points: 20,
      type: 'daily',
      isCompleted: false,
      progress: 0,
      target: 1,
    ),
    Task(
      id: '3',
      title: 'Đánh giá voucher',
      description: 'Đánh giá 1 voucher đã sử dụng',
      points: 15,
      type: 'daily',
      isCompleted: true,
      progress: 1,
      target: 1,
    ),
    Task(
      id: '4',
      title: 'Hoàn thành hồ sơ',
      description: 'Thêm ảnh đại diện và số điện thoại',
      points: 50,
      type: 'one-time',
      isCompleted: false,
      progress: 1,
      target: 2,
    ),
    Task(
      id: '5',
      title: 'Mời bạn bè',
      description: 'Mời 5 bạn bè tham gia Mã Siu',
      points: 100,
      type: 'weekly',
      isCompleted: false,
      progress: 2,
      target: 5,
    ),
  ];
});

/// Task Service
class TaskService {
  final ApiClient _apiClient;

  TaskService(this._apiClient);

  Future<bool> completeTask(String taskId) async {
    try {
      final response = await _apiClient.post('/api/tasks/$taskId/complete');
      return response.statusCode == 200;
    } catch (e) {
      return false;
    }
  }
}

final taskServiceProvider = Provider<TaskService>((ref) {
  return TaskService(ref.watch(apiClientProvider));
});
