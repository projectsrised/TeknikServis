import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:teknik_servis_mobile/core/network/api_client.dart';
import 'package:teknik_servis_mobile/shared/models/user_model.dart';

final apiClientProvider = Provider((ref) => ApiClient());

final authProvider = StateNotifierProvider<AuthNotifier, AsyncValue<UserModel?>>((ref) {
  return AuthNotifier(ref.read(apiClientProvider));
});

class AuthNotifier extends StateNotifier<AsyncValue<UserModel?>> {
  final ApiClient _apiClient;
  
  AuthNotifier(this._apiClient) : super(const AsyncValue.loading()) {
    _checkAuth();
  }
  
  Future<void> _checkAuth() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final token = prefs.getString('accessToken');
      
      if (token == null) {
        state = const AsyncValue.data(null);
        return;
      }
      
      final response = await _apiClient.getCurrentUser();
      if (response.statusCode == 200) {
        final user = UserModel.fromJson(response.data['data']);
        state = AsyncValue.data(user);
      } else {
        state = const AsyncValue.data(null);
      }
    } catch (e) {
      state = const AsyncValue.data(null);
    }
  }
  
  Future<void> login(String email, String password) async {
    state = const AsyncValue.loading();
    
    try {
      final response = await _apiClient.login(email, password);
      
      if (response.statusCode == 200 && response.data['success'] == true) {
        final data = response.data['data'];
        
        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('accessToken', data['accessToken']);
        await prefs.setString('refreshToken', data['refreshToken']);
        
        final user = UserModel.fromJson(data['user']);
        state = AsyncValue.data(user);
      } else {
        throw Exception(response.data['message'] ?? 'Giriş başarısız');
      }
    } catch (e) {
      state = AsyncValue.error(e, StackTrace.current);
      rethrow;
    }
  }
  
  Future<void> logout() async {
    try {
      await _apiClient.logout();
    } finally {
      final prefs = await SharedPreferences.getInstance();
      await prefs.remove('accessToken');
      await prefs.remove('refreshToken');
      state = const AsyncValue.data(null);
    }
  }
}
