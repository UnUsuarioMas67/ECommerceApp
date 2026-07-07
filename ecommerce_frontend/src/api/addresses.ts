import type { AxiosInstance } from 'axios';
import type { Address, Country } from './types';
import type { AddressUpdate, AddressCreate } from '../schemas/addresses';

export async function fetchAddresses(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Address[]>('/addresses/clients/me');
  return response.data;
}

export async function fetchAddress(axiosInstance: AxiosInstance, id: number) {
  const response = await axiosInstance.get<Address>(`/addresses/${id}`);
  return response.data;
}

export async function addAddress(axiosInstance: AxiosInstance, data: AddressCreate) {
  const response = await axiosInstance.post('/addresses', data);
  return response.data;
}

export async function deleteAddress(axiosInstance: AxiosInstance, id: number) {
  await axiosInstance.delete(`/addresses/${id}`);
}

export async function updateAddress(axiosInstance: AxiosInstance, id: number, data: AddressUpdate) {
  const response = await axiosInstance.put(`/addresses/${id}`, data)
  return response.data
}

export async function fetchCountries(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Country[]>('countries')
  return response.data
}