class UserModel {
  final String id;
  final String tenantId;
  final String? branchId;
  final String firstName;
  final String lastName;
  final String fullName;
  final String email;
  final String? phone;
  final int role;
  final String roleName;
  final String? tenantName;
  final String? branchName;
  final bool isActive;

  UserModel({
    required this.id,
    required this.tenantId,
    this.branchId,
    required this.firstName,
    required this.lastName,
    required this.fullName,
    required this.email,
    this.phone,
    required this.role,
    required this.roleName,
    this.tenantName,
    this.branchName,
    required this.isActive,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
    return UserModel(
      id: json['id'],
      tenantId: json['tenantId'],
      branchId: json['branchId'],
      firstName: json['firstName'],
      lastName: json['lastName'],
      fullName: json['fullName'],
      email: json['email'],
      phone: json['phone'],
      role: json['role'],
      roleName: json['roleName'],
      tenantName: json['tenantName'],
      branchName: json['branchName'],
      isActive: json['isActive'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'tenantId': tenantId,
      'branchId': branchId,
      'firstName': firstName,
      'lastName': lastName,
      'fullName': fullName,
      'email': email,
      'phone': phone,
      'role': role,
      'roleName': roleName,
      'tenantName': tenantName,
      'branchName': branchName,
      'isActive': isActive,
    };
  }

  bool get isTenantAdmin => role == 1;
  bool get isBranchAdmin => role == 2 || role == 3;
  bool get isSalesStaff => role == 4;
  bool get isTechnicalStaff => role == 5;
}
