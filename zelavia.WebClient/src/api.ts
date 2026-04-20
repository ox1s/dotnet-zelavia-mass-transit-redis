import axios from "axios";
const api = axios.create({
  baseURL: "/api",
});

export interface Flight {
  flightId: string;
  flightUtc: string;
  price: number;
}

export interface User {
  userId: string;
  email: string;
  wallet: number;
}

export const usersApi = {
  getUsers: () => api.get<User[]>("/users"),
  getUser: (userId: string) => api.get<User>(`/users/${userId}`),
};

export const flightsApi = {
  getFlights: () => api.get<Flight[]>("/flights"),
  getFlight: (flightId: string) => api.get<Flight>(`/flights/${flightId}`),
  bookFlight: (userId: string, userEmail: string, flightId: string) =>
    api.post(`/flights/${flightId}/book`, { userId, userEmail }),
};

export default api;
