using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

// Class representing a road segment
class RoadRecord
{
    public string RoadName { get; set; }        
    public string From { get; set; }            // Start coordinates (lat;lon)
    public string To { get; set; }              // End coordinates (lat;lon)
    public double LengthKm { get; set; }        // Length of the segment in kilometers
    public string SpeedLimitKmH { get; set; }   // Speed limit for the segment in kilometers per hour
}

class Program
{
    static void Main(string[] args)
    {
        // Input and output files
        string inputFile = "city.geojson";   // GeoJSON file exported from https://overpass-turbo.eu/
        string outputFile = "roads.csv";     // CSV file to save road segments

        // Check if input file exists
        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"ERROR: File {inputFile} not found.");
            return;
        }

        // Read the GeoJSON file
        string json = File.ReadAllText(inputFile);

        // Deserialize GeoJSON using Newtonsoft.Json into a FeatureCollection
        var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(json);

        var records = new List<RoadRecord>();

        // Only include roads suitable for car driving
        var allowedHighways = new HashSet<string>
        {
            "motorway", "trunk", "primary", "secondary", "tertiary",
            "unclassified", "residential", "living_street"
        };

        // Iterate through each feature in the GeoJSON
        foreach (var feature in featureCollection.Features)
        {
            if (feature.Properties == null) continue;

            var props = feature.Properties;

            // Get the highway type
            string highway = props.ContainsKey("highway") ? props["highway"]?.ToString() ?? "" : "";
            if (!allowedHighways.Contains(highway)) continue; // Skip roads not allowed for cars

            // Get the road name
            string name = props.ContainsKey("name") ? props["name"]?.ToString() ?? "" : "";
            if (string.IsNullOrEmpty(name)) continue; // Skip roads without a name

            // Get speed limit if available
            string maxspeed = props.ContainsKey("maxspeed") ? props["maxspeed"]?.ToString() ?? "" : "";

            // Check if geometry is a LineString (road segment)
            if (feature.Geometry is GeoJSON.Net.Geometry.LineString line)
            {
                // Convert coordinates to NetTopologySuite Coordinate objects
                var coords = line.Coordinates.Select(c => new Coordinate(c.Longitude, c.Latitude)).ToArray();

                // Calculate the length of the road segment using the Haversine formula
                // Haversine formula computes the great-circle distance between two points
                // on the Earth specified by latitude and longitude in degrees.
                // Formula:
                //    a = sin²(Δlat/2) + cos(lat1) * cos(lat2) * sin²(Δlon/2)
                //    c = 2 * arcsin(√a)
                //    distance = R * c
                // where R is the Earth's radius (6371 km)
                
                double lengthKm = 0;
                for (int i = 0; i < coords.Length - 1; i++)
                    lengthKm += Haversine(coords[i].Y, coords[i].X, coords[i + 1].Y, coords[i + 1].X);

                // Create a road record
                var record = new RoadRecord
                {
                    RoadName = name,
                    From = $"{coords.First().Y};{coords.First().X}",
                    To = $"{coords.Last().Y};{coords.Last().X}",
                    LengthKm = Math.Round(lengthKm, 3),
                    SpeedLimitKmH = string.IsNullOrEmpty(maxspeed) ? DefaultSpeed(highway) : maxspeed
                };

                records.Add(record);
            }
        }

        // Write the road records to a CSV file with semicolon as delimiter
        using (var writer = new StreamWriter(outputFile))
        using (var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(new CultureInfo("en-US"))
               {
                   Delimiter = ";"  // Use semicolon for Excel compatibility or change for ,
               }))
        {
            csv.WriteRecords(records);
        }

        Console.WriteLine($"Saved {records.Count} road segments to {outputFile}");
    }

    // Calculate Haversine distance between two points in kilometers
    static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371; // Radius of Earth in km
        double dLat = (lat2 - lat1) * Math.PI / 180.0;
        double dLon = (lon2 - lon1) * Math.PI / 180.0;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Asin(Math.Sqrt(a));
        return R * c;
    }

    // Provide default speed limits based on highway type
    static string DefaultSpeed(string highwayType)
    {
        return highwayType switch
        {
            "motorway" => "90",
            "trunk" => "70",
            "primary" => "60",
            "secondary" => "50",
            "tertiary" => "50",
            "residential" => "30",
            "service" => "20",
            _ => "50" // Default speed if unknown
        };
    }
}
