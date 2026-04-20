import { BrowserRouter, Route, Routes } from "react-router";
import "./App.css";
import Flights from "./pages/Flights";

function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/flights" element={<Flights />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
