### ARCore Geospatial **VPS** & **GNSS** Evaluation
Unity-based AR research prototype developed as the practical component of a Master's thesis.
The application was created to evaluate and compare positioning information from mobile **GNSS** and the ARCore Geospatial **API**, with a focus on **VPS**-assisted spatial tracking and real-world AR alignment.
### Project Overview
The prototype combines Unity, AR Foundation and ARCore Geospatial functionality to collect positioning telemetry and visualize geospatial content in augmented reality.
The project includes:
- **GNSS** and **VPS** telemetry monitoring
- ARCore Geospatial pose tracking
- geospatial anchor creation
- manual spatial offset correction
- measurement logging to **CSV**
- **QGIS**-derived 3D model integration
- AR point-of-interest visualization
- compass and bearing calculations
- configurable test scenarios
### Measurement Workflow
The application was used to collect measurements under several test conditions, including stationary and moving scenarios.
Recorded measurements include:
- raw **GNSS** latitude and longitude
- reported **GNSS** horizontal accuracy
- **VPS** latitude and longitude
- **VPS** horizontal accuracy
- **VPS** orientation accuracy
- manual model alignment offset
Measurements are stored as **CSV** files for later analysis.
### Spatial Alignment
Real-world spatial models prepared using **GIS** tooling were imported into Unity and aligned with ARCore Geospatial anchors.
The application also provides manual X/Y/Z offset controls for evaluating and correcting visible alignment differences between the virtual model and the physical environment.
Compass and Points of Interest
The prototype contains a lightweight compass **HUD** and point-of-interest system.
Bearings between geographic coordinates are calculated using spherical latitude/longitude calculations and mapped to screen-space compass positions.
### Tech Stack
- Unity 6
- C#
- AR Foundation 6
- ARCore
- ARCore Extensions
- ARCore Geospatial **API**
- **QGIS**
- Cesium for Unity
- glTF
- Android
### Main Components
ARTestLogger
Collects **GNSS** and **VPS** telemetry and stores measurement samples in **CSV** files.
ARDebugController
Manages geospatial anchor recreation and manual model alignment offsets.
GeospatialTelemetry
Displays real-time ARCore Geospatial tracking and accuracy information.
CompassHUD
Calculates bearings to geographic points of interest and displays them on a compass-style **HUD**.
ARPointOfInterest
Controls proximity-based point-of-interest labels in AR.
OnScreenLogger
Provides an in-application view of Unity runtime logs during Android testing.
Dependencies
The original thesis project used:
- ARCore Extensions for AR Foundation 1.48.0 with AR Foundation 6
- Cesium for Unity 1.15.4
These packages were originally installed locally from package archives and are not included in this repository.
ARCore Extensions can be installed through the official AR Foundation 6 Git package branch.
Cesium for Unity can be installed through the official Cesium Unity Package Manager registry.
### Project Status
This repository contains the practical research prototype used during thesis experimentation.
It is preserved as a portfolio and research project rather than a production-ready AR application.