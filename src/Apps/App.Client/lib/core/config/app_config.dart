/// App configuration for different environments
class AppConfig {
  static const String apiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: 'http://localhost:5000', // API Gateway URL
  );

  static const String googleClientId = String.fromEnvironment(
    'GOOGLE_CLIENT_ID',
    defaultValue: '', // TODO: Add from Firebase config
  );

  // Feature flags
  static const bool enablePushNotifications = true;
  static const bool enableAnalytics = false;
}
