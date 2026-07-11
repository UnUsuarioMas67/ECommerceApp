import type { AxiosInstance } from 'axios';
import type { LoginRequest, RegisterRequest, UserDataUpdate, UserPasswordUpdate } from '../schemas/user';
import type { UserAuth, User } from './types';

export async function postLogin(axiosInstance: AxiosInstance, data: LoginRequest) {
  const response = await axiosInstance.post<UserAuth>('/clients/login', data);
  return response.data;
}

export async function postLogout(axiosInstance: AxiosInstance) {
  await axiosInstance.post('/clients/logout');
}

export async function postRegister(axiosInstance: AxiosInstance, data: RegisterRequest) {
  const response = await axiosInstance.post<User>('/clients/register', data);
  return response.data;
}

export async function fetchCurrentUser(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<User>('/clients/me', {
    validateStatus: (status) => status === 200 || status === 401,
  });

  if (response.status === 401) return null;
  
  return response.data;
}

export async function updateUser(axiosInstance: AxiosInstance, data: UserDataUpdate | UserPasswordUpdate) {
  const response = await axiosInstance.put<User>('/clients', data);
  return response.data;
}
