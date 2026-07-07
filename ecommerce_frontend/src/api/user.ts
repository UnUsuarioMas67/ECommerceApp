import type { AxiosInstance } from 'axios';
import type { LoginRequest, RegisterRequest } from '../schemas/auth';
import type { UserAuth, User } from './types';

export async function postLogin(axiosInstance: AxiosInstance, data: LoginRequest) {
  const response = await axiosInstance.post<UserAuth>('/clients/login', data);
  return response.data;
}

export async function postLogout(axiosInstance: AxiosInstance) {
  await axiosInstance.post('clients/logout');
}

export async function postRegister(axiosInstance: AxiosInstance, data: RegisterRequest) {
  const response = await axiosInstance.post<User>('/clients/register', data);
  return response.data;
}

export async function fetchCurrentUser(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<User>('/clients/me');
  return response.data;
}
