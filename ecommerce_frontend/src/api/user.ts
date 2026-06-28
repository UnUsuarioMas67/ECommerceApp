import type { AxiosInstance } from "axios";
import type { LoginRequest, RegisterRequest } from "../schemas";
import type { UserAuth, User } from "./types";

export async function postLogin(axiosInstance: AxiosInstance, data: LoginRequest) {
  const response = await axiosInstance.post<UserAuth>('/clients/login', data);
  return response.data;
}

export async function postLogout(axiosInstance: AxiosInstance) {
  await axiosInstance.post('clients/logout');
}

export async function postRegister(axiosInstance: AxiosInstance, data: RegisterRequest) {
  const response = await axiosInstance.post<User>('/clients/login', data);
  return response.data;
}