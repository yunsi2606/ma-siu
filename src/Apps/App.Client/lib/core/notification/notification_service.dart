import 'dart:convert';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../api/api_client.dart';

/// FCM token provider
final fcmTokenProvider = StateProvider<String?>((ref) => null);

/// Notification service provider
final notificationServiceProvider = Provider<NotificationService>((ref) {
  return NotificationService(ref);
});

/// Firebase Cloud Messaging service
class NotificationService {
  final Ref _ref;
  final FirebaseMessaging _messaging = FirebaseMessaging.instance;

  NotificationService(this._ref);

  /// Initialize FCM and request permissions
  Future<void> initialize() async {
    // Request permission (iOS)
    final settings = await _messaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
    );

    if (settings.authorizationStatus == AuthorizationStatus.authorized) {
      debugPrint('FCM: User granted permission');
      await _setupToken();
      _setupForegroundHandler();
    } else {
      debugPrint('FCM: User denied permission');
    }
  }

  /// Get and register FCM token
  Future<void> _setupToken() async {
    try {
      final token = await _messaging.getToken();
      if (token != null) {
        _ref.read(fcmTokenProvider.notifier).state = token;
        await _registerTokenWithServer(token);
        debugPrint('FCM Token: $token');
      }

      // Listen for token refresh
      _messaging.onTokenRefresh.listen((newToken) async {
        _ref.read(fcmTokenProvider.notifier).state = newToken;
        await _registerTokenWithServer(newToken);
      });
    } catch (e) {
      debugPrint('FCM Token error: $e');
    }
  }

  /// Register token with backend
  Future<void> _registerTokenWithServer(String token) async {
    try {
      await _ref
          .read(apiClientProvider)
          .post('/api/users/fcm-token', data: {'token': token});
    } catch (e) {
      debugPrint('Failed to register FCM token: $e');
    }
  }

  /// Handle foreground messages
  void _setupForegroundHandler() {
    FirebaseMessaging.onMessage.listen((RemoteMessage message) {
      debugPrint('Foreground message: ${message.notification?.title}');
      // TODO: Show local notification or in-app banner
      _handleMessage(message);
    });

    // Handle notification tap when app is in background
    FirebaseMessaging.onMessageOpenedApp.listen((RemoteMessage message) {
      debugPrint('Notification opened: ${message.data}');
      _handleNotificationTap(message);
    });
  }

  /// Handle incoming message data
  void _handleMessage(RemoteMessage message) {
    final data = message.data;
    debugPrint('Message data: ${jsonEncode(data)}');
    // TODO: Update UI state based on notification type
  }

  /// Handle notification tap
  void _handleNotificationTap(RemoteMessage message) {
    final data = message.data;
    // Navigate based on notification type
    switch (data['type']) {
      case 'new_voucher':
        // TODO: Navigate to voucher
        break;
      case 'new_post':
        // TODO: Navigate to post
        break;
      case 'reward_approved':
        // TODO: Navigate to rewards
        break;
    }
  }

  /// Subscribe to topic
  Future<void> subscribeToTopic(String topic) async {
    await _messaging.subscribeToTopic(topic);
    debugPrint('Subscribed to topic: $topic');
  }

  /// Unsubscribe from topic
  Future<void> unsubscribeFromTopic(String topic) async {
    await _messaging.unsubscribeFromTopic(topic);
    debugPrint('Unsubscribed from topic: $topic');
  }
}

/// Background message handler (must be top-level function)
@pragma('vm:entry-point')
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  debugPrint('Background message: ${message.messageId}');
}
