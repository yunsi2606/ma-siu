import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:google_sign_in/google_sign_in.dart';
import '../api/api_client.dart';

/// Auth state
class AuthState {
  final bool isAuthenticated;
  final bool isLoading;
  final String? userId;
  final String? email;
  final String? displayName;
  final String? avatarUrl;
  final String? error;

  const AuthState({
    this.isAuthenticated = false,
    this.isLoading = false,
    this.userId,
    this.email,
    this.displayName,
    this.avatarUrl,
    this.error,
  });

  AuthState copyWith({
    bool? isAuthenticated,
    bool? isLoading,
    String? userId,
    String? email,
    String? displayName,
    String? avatarUrl,
    String? error,
  }) {
    return AuthState(
      isAuthenticated: isAuthenticated ?? this.isAuthenticated,
      isLoading: isLoading ?? this.isLoading,
      userId: userId ?? this.userId,
      email: email ?? this.email,
      displayName: displayName ?? this.displayName,
      avatarUrl: avatarUrl ?? this.avatarUrl,
      error: error,
    );
  }
}

/// Auth notifier provider
final authProvider = StateNotifierProvider<AuthNotifier, AuthState>((ref) {
  return AuthNotifier(ref.watch(apiClientProvider));
});

/// Auth state notifier
class AuthNotifier extends StateNotifier<AuthState> {
  final ApiClient _apiClient;
  // Web Client ID from google-services.json (client_type: 3)
  final GoogleSignIn _googleSignIn = GoogleSignIn(
    serverClientId:
        '288811663566-ojkmg31vs1kf1bu975qrjp97n9he5tr6.apps.googleusercontent.com',
  );
  final FlutterSecureStorage _storage = const FlutterSecureStorage();

  static const _accessTokenKey = 'access_token';
  static const _refreshTokenKey = 'refresh_token';

  AuthNotifier(this._apiClient) : super(const AuthState()) {
    _checkAuth();
  }

  /// Check if user is already authenticated
  Future<void> _checkAuth() async {
    state = state.copyWith(isLoading: true);

    try {
      final accessToken = await _storage.read(key: _accessTokenKey);
      if (accessToken != null) {
        _apiClient.setAccessToken(accessToken);
        // TODO: Validate token with backend and get user info
        state = state.copyWith(isAuthenticated: true, isLoading: false);
      } else {
        state = state.copyWith(isLoading: false);
      }
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  /// Sign in with Google
  Future<bool> signInWithGoogle() async {
    state = state.copyWith(isLoading: true, error: null);

    try {
      final googleUser = await _googleSignIn.signIn();
      if (googleUser == null) {
        state = state.copyWith(isLoading: false);
        return false;
      }

      final googleAuth = await googleUser.authentication;
      final idToken = googleAuth.idToken;

      if (idToken == null) {
        state = state.copyWith(
          isLoading: false,
          error: 'Failed to get ID token',
        );
        return false;
      }

      // Send ID token to backend
      final response = await _apiClient.post(
        '/api/auth/google',
        data: {'idToken': idToken},
      );

      if (response.statusCode == 200) {
        final data = response.data as Map<String, dynamic>;
        final accessToken = data['accessToken'] as String;
        final refreshToken = data['refreshToken'] as String;
        final user = data['user'] as Map<String, dynamic>;

        // Store tokens
        await _storage.write(key: _accessTokenKey, value: accessToken);
        await _storage.write(key: _refreshTokenKey, value: refreshToken);
        _apiClient.setAccessToken(accessToken);

        state = state.copyWith(
          isAuthenticated: true,
          isLoading: false,
          userId: user['id'],
          email: user['email'],
          displayName: user['displayName'],
          avatarUrl: user['avatarUrl'],
        );
        return true;
      } else {
        state = state.copyWith(
          isLoading: false,
          error: 'Authentication failed',
        );
        return false;
      }
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
      return false;
    }
  }

  /// Sign out
  Future<void> signOut() async {
    await _googleSignIn.signOut();
    await _storage.delete(key: _accessTokenKey);
    await _storage.delete(key: _refreshTokenKey);
    _apiClient.setAccessToken(null);
    state = const AuthState();
  }
}
