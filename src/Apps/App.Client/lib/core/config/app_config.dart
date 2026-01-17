/// App configuration for different environments
class AppConfig {
  // Environment detection
  static const bool isProduction = bool.fromEnvironment('dart.vm.product');

  // API Base URL - can be overridden via --dart-define=API_BASE_URL=xxx
  static const String apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: isProduction
        ? 'https://api.masiu.nhatcuong.io.vn' // Production: Use your domain
        : 'http://10.0.2.2:5300', // Development: Android emulator localhost
  );

  // For physical Android device, use your machine's local IP:
  // flutter run --dart-define=API_BASE_URL=http://192.168.1.xxx:5300

  static const String googleClientId = String.fromEnvironment(
    'GOOGLE_CLIENT_ID',
    defaultValue:
        '288811663566-ojkmg31vs1kf1bu975qrjp97n9he5tr6.apps.googleusercontent.com',
  );

  // Feature flags
  static const bool enablePushNotifications = true;
  static const bool enableAnalytics = isProduction;
}
