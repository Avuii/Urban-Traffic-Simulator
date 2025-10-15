# 🚗 Urban-Traffic-Simulator  
**A C# city traffic simulation based on real road data from OpenStreetMap.**  
*(Project in progress 🚧)*   
✅ = implemented  
⏳ = in progress 

---

## 🏙️ Overview
**Urban-Traffic-Simulator** is a project that models urban road networks using data from OpenStreetMap (`.geojson`).  
It simulates how cars move through a city, how accidents occur, and how traffic congestion forms and resolves over time.  
The simulation is graph-based — every intersection or roundabout becomes a node, and every street segment becomes an edge.

---

## 🧩 Project Structure
/Urban-Traffic-Simulator  
| 
├── /bin/Debug/net9.0/
| ├── city.geojson            # Input file exported from OpenStreetMap                  ✅  
| ├── roads.csv               # Cleaned and merged road data                            ✅       
| ├── nodes.csv               # List of intersections and roundabouts                   ⏳  
| └── cars_simulation.csv     # Simulation output for each car                          ⏳  
|  
├── roads.cs                 # Reads .geojson, calculates distances, exports CSV files  ⏳  
├── main.cs                  # Builds the traffic graph and runs the simulation         ⏳  
└── results.cs               # Summarizes outcomes (accidents, congestion, speeds)      ⏳  


---

## ⚙️ Features
- Parse and process **OpenStreetMap GeoJSON** data  
- Filter **drivable roads** only (no footpaths or cycleways)  
- Compute **segment lengths** using the **Haversine formula**  
- Build **graph structure** of the road network  
- Simulate **car movements**, **traffic lights**, and **accidents**  
- User can configure:
  - 🚦 Traffic light cycle time  
  - 🚑 Accident blocking duration  
  - 🚗 Frequency of new vehicle spawns  

---

## 🧠 Planned Functionality
- Accident simulation — road segments blocked for a defined time ⏳  
- Vehicle pathfinding between nodes (Dijkstra / A*) ⏳  
- Real-time statistics logging (speed, congestion, delays) ⏳  
- Visualization of city graph and live simulation (optional future goal) ⏳  

---

## 🧰 Technologies
- **C# / .NET 8.0**
- **GeoJSON.Net**, **Newtonsoft.Json**
- **CsvHelper**
- **NetTopologySuite**

---

## 🚀 Getting Started
1. Export your city data from [OpenStreetMap](https://overpass-turbo.eu/) as `.geojson`
2. Place the file in the project directory and rename it to `city.geojson`
3. Run the project — it will generate `roads.csv` and `roadssegments.csv`
4. Start the simulation in `main.cs`

---

## 🧑‍💻 Author
Created for educational purposes by Avui.


 
