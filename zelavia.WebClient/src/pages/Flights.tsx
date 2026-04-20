import { flightsApi, usersApi } from "@/api";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useEffect, useState } from "react";

export default function Flights() {
  const [flights, setFlights] = useState([]);
  const [userId, setUserId] = useState("");
  const [userEmail, setUserEmail] = useState("");
  const [users, setUsers] = useState([]);

  const loadData = async () => {
    try {
      const flights = await flightsApi.getFlights();
      const flightsData = await flights.json();
      const users = await usersApi.getUsers();
      const usersData = await users.json();
      setFlights(flightsData);
      setUsers(usersData);
    } catch (error) {
      console.error(error);
    }
  };
  useEffect(() => {
    loadData();
  }, []);

  const handleUserChange = async (value: string) => {
    setUserId(value);
    try {
      const user = await usersApi.getUser(value);
      const data = await user.json();
      setUserEmail(data.email);
    } catch (error) {
      console.error(error);
    }
  };

  const handleBook = async (
    userId: string,
    userEmail: string,
    flightId: string,
  ) => {
    try {
      const response = await flightsApi.bookFlight(userId, userEmail, flightId);
      if (response.ok) {
      }
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <>
      <header className="flex h-(--header-height) shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height)">
        <div className="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6">
          <h1 className="text-base font-medium">Documents</h1>
          <div className="ml-auto flex items-center gap-2">
            <Select>
              <SelectTrigger className="w-45">
                <SelectValue placeholder="User" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {users.map((user) => (
                    <SelectItem key={user.id} value={user.id}>
                      {user.userId} {user.email}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        </div>
      </header>
      <Table>
        <TableCaption>Доступные рейсы</TableCaption>
        <TableHeader>
          <TableRow>
            <TableHead className="w-25">Invoice</TableHead>
            <TableHead>Id</TableHead>
            <TableHead>Время</TableHead>
            <TableHead>Прайс</TableHead>
            <TableHead>Действия</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {flights.map((flight) => (
            <TableRow key={flight.id}>
              <TableCell className="font-medium">{flight.invoice}</TableCell>
              <TableCell>{flight.id}</TableCell>
              <TableCell>{flight.time}</TableCell>
              <TableCell>{flight.price}</TableCell>
              <TableCell>
                <Button
                  variant="ghost"
                  onClick={() => handleBook(userId, userEmail, flight.id)}
                >
                  Book
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </>
  );
}
