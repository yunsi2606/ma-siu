import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../core/api/api_client.dart';

/// Post model
class Post {
  final String id;
  final String title;
  final String? description;
  final String platform;
  final String? imageUrl;
  final String? affiliateUrl;
  final DateTime createdAt;

  Post({
    required this.id,
    required this.title,
    this.description,
    required this.platform,
    this.imageUrl,
    this.affiliateUrl,
    required this.createdAt,
  });

  factory Post.fromJson(Map<String, dynamic> json) {
    return Post(
      id: json['id'],
      title: json['title'],
      description: json['description'],
      platform: json['platform'],
      imageUrl: json['imageUrl'],
      affiliateUrl: json['affiliateUrl'],
      createdAt: DateTime.parse(json['createdAt']),
    );
  }
}

/// Feed service provider
final feedServiceProvider = Provider<FeedService>((ref) {
  return FeedService(ref.watch(apiClientProvider));
});

/// Feed posts provider
final feedPostsProvider = FutureProvider<List<Post>>((ref) async {
  return ref.watch(feedServiceProvider).getPosts();
});

/// Feed service
class FeedService {
  final ApiClient _apiClient;

  FeedService(this._apiClient);

  Future<List<Post>> getPosts({int page = 1, int pageSize = 20}) async {
    try {
      final response = await _apiClient.get(
        '/api/posts',
        queryParameters: {'page': page, 'pageSize': pageSize},
      );

      if (response.statusCode == 200) {
        final data = response.data as Map<String, dynamic>;
        final posts = (data['posts'] as List)
            .map((json) => Post.fromJson(json))
            .toList();
        return posts;
      }
      return [];
    } catch (e) {
      return [];
    }
  }
}
