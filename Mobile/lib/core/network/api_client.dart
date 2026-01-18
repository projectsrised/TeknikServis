import 'package:dio/dio.dart';
import 'package:shared_preferences/shared_preferences.dart';

class ApiClient {
  static const String baseUrl = 'http://10.0.2.2:5000/api'; // Android emulator
  // static const String baseUrl = 'http://localhost:5000/api'; // iOS simulator
  
  late final Dio _dio;
  
  ApiClient() {
    _dio = Dio(BaseOptions(
      baseUrl: baseUrl,
      connectTimeout: const Duration(seconds: 30),
      receiveTimeout: const Duration(seconds: 30),
      headers: {
        'Content-Type': 'application/json',
      },
    ));
    
    _dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) async {
        final prefs = await SharedPreferences.getInstance();
        final token = prefs.getString('accessToken');
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
      onError: (error, handler) async {
        if (error.response?.statusCode == 401) {
          // Token refresh logic
          final refreshed = await _refreshToken();
          if (refreshed) {
            // Retry the request
            final retryResponse = await _dio.fetch(error.requestOptions);
            return handler.resolve(retryResponse);
          }
        }
        return handler.next(error);
      },
    ));
  }
  
  Future<bool> _refreshToken() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final refreshToken = prefs.getString('refreshToken');
      
      if (refreshToken == null) return false;
      
      final response = await Dio().post(
        '$baseUrl/auth/refresh',
        data: {'refreshToken': refreshToken},
      );
      
      if (response.statusCode == 200) {
        final data = response.data['data'];
        await prefs.setString('accessToken', data['accessToken']);
        await prefs.setString('refreshToken', data['refreshToken']);
        return true;
      }
    } catch (e) {
      // Clear tokens on refresh failure
      final prefs = await SharedPreferences.getInstance();
      await prefs.remove('accessToken');
      await prefs.remove('refreshToken');
    }
    return false;
  }
  
  // Auth
  Future<Response> login(String email, String password) async {
    return _dio.post('/auth/login', data: {
      'email': email,
      'password': password,
    });
  }
  
  Future<Response> logout() async {
    return _dio.post('/auth/logout');
  }
  
  Future<Response> getCurrentUser() async {
    return _dio.get('/auth/me');
  }
  
  // Sales
  Future<Response> getSales({int page = 1, int pageSize = 20, String? search}) async {
    return _dio.get('/sales', queryParameters: {
      'page': page,
      'pageSize': pageSize,
      if (search != null) 'search': search,
    });
  }
  
  Future<Response> scanBarcode(String code) async {
    return _dio.get('/sales/scan/$code');
  }
  
  Future<Response> createSale(Map<String, dynamic> data) async {
    return _dio.post('/sales', data: data);
  }
  
  // Technical Services
  Future<Response> getServices({int page = 1, int pageSize = 20, int? status}) async {
    return _dio.get('/technical-services', queryParameters: {
      'page': page,
      'pageSize': pageSize,
      if (status != null) 'status': status,
    });
  }
  
  Future<Response> createService(Map<String, dynamic> data) async {
    return _dio.post('/technical-services', data: data);
  }
  
  Future<Response> updateServiceStatus(String id, int status, String? notes) async {
    return _dio.post('/technical-services/$id/status', data: {
      'newStatus': status,
      'notes': notes,
    });
  }
  
  // Inventory Count
  Future<Response> createInventoryCount(String warehouseId) async {
    return _dio.post('/inventory-counts', data: {
      'warehouseId': warehouseId,
    });
  }
  
  Future<Response> scanCountItem(String countId, String code) async {
    return _dio.post('/inventory-counts/scan', data: {
      'inventoryCountId': countId,
      'serialOrBarcode': code,
    });
  }
  
  Future<Response> completeInventoryCount(String id) async {
    return _dio.post('/inventory-counts/$id/complete', data: {});
  }
  
  // Products
  Future<Response> getProducts({int page = 1, int pageSize = 20, String? categoryId}) async {
    return _dio.get('/products', queryParameters: {
      'page': page,
      'pageSize': pageSize,
      if (categoryId != null) 'categoryId': categoryId,
    });
  }
  
  // Categories
  Future<Response> getCategories() async {
    return _dio.get('/categories');
  }
  
  // Dashboard
  Future<Response> getBranchDashboard() async {
    return _dio.get('/dashboard/branch');
  }
}
