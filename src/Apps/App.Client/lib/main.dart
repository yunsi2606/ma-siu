import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'core/config/router.dart';
import 'core/notification/notification_service.dart';
import 'shared/theme/app_theme.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Initialize Firebase (optional - will work without it)
  bool firebaseInitialized = false;
  try {
    await Firebase.initializeApp();
    FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);
    firebaseInitialized = true;
    debugPrint('Firebase initialized successfully');
  } catch (e) {
    debugPrint('Firebase not configured: $e');
    debugPrint('App will continue without push notifications');
  }

  runApp(
    ProviderScope(child: MaSiuApp(firebaseInitialized: firebaseInitialized)),
  );
}

class MaSiuApp extends ConsumerStatefulWidget {
  final bool firebaseInitialized;

  const MaSiuApp({super.key, required this.firebaseInitialized});

  @override
  ConsumerState<MaSiuApp> createState() => _MaSiuAppState();
}

class _MaSiuAppState extends ConsumerState<MaSiuApp> {
  @override
  void initState() {
    super.initState();
    if (widget.firebaseInitialized) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        ref.read(notificationServiceProvider).initialize();
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final router = ref.watch(routerProvider);

    return MaterialApp.router(
      title: 'Mã Siu',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      routerConfig: router,
    );
  }
}
