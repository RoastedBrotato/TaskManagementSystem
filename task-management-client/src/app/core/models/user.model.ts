export interface User {
    id: number;
    username: string;
    email: string;
    role: string;
  }
  
  export interface LoginRequest {
    username: string;
    password: string;
  }
  
  export interface LoginResponse {
    token: string;
    userId: number;
    username: string;
    role: string;
  }
  
  export interface CreateUserRequest {
    username: string;
    password: string;
    email: string;
    role: string;
  }
  
  export interface UpdateUserRequest {
    email: string;
    role: string;
  }