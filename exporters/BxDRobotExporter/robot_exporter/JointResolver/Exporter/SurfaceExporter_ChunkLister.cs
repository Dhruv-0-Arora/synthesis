﻿using Inventor;
using System.IO;
using System;
using System.Collections.Generic;

public partial class SurfaceExporter
{
    /// <summary>
    /// Should the exporter attempt to automatically ignore small parts.
    /// </summary>
    private bool adaptiveIgnoring = true;
    /// <summary>
    /// The minimum ratio between a sub component's bounding box volume and the average bounding box volume for an object
    /// to be considered small.  The higher the number the less that is dropped, while if the value is one about half the objects
    /// would be dropped.
    /// </summary>
    private double adaptiveDegredation = 7;

    /// <summary>
    /// Adds the mesh for the given component, and all its subcomponents to the mesh storage structure.
    /// </summary>
    /// <param name="occ">The component to export</param>
    /// <param name="bestResolution">Use the best possible resolution</param>
    /// <param name="separateFaces">Export each face as a separate mesh</param>
    /// <param name="ignorePhysics">Don't add the physical properties of this component to the exporter</param>
    /// <returns>All the sufaces to export</returns>
    private List<SurfaceBody> GenerateExportList(ComponentOccurrence occ, bool ignorePhysics = false)
    {
        List<SurfaceBody> plannedExports = new List<SurfaceBody>();
        
        // Invisible objects don't need to be exported
        if (!occ.Visible)
            return plannedExports;

        if (!ignorePhysics)
        {
            // Compute physics
            try
            {
                outputMesh.physics.Add((float) occ.MassProperties.Mass, Utilities.ToBXDVector(occ.MassProperties.CenterOfMass));
            }
            catch
            {
                Console.Write("Failed to get physics data for " + occ.Name);
            }
        }

        // Prepare exporting surfaces
        foreach (SurfaceBody surf in occ.SurfaceBodies)
        {
            plannedExports.Add(surf);
        }

        // Add sub-occurences
        double totalVolume = 0;
        foreach (ComponentOccurrence occ2 in occ.SubOccurrences)
        {
            totalVolume += Utilities.BoxVolume(occ2.RangeBox);
        }
        totalVolume /= occ.SubOccurrences.Count * adaptiveDegredation;

        foreach (ComponentOccurrence item in occ.SubOccurrences)
        {
            if (!adaptiveIgnoring || Utilities.BoxVolume(item.RangeBox) >= totalVolume)
            {
                plannedExports.AddRange(GenerateExportList(item, true));
            }
        }

        return plannedExports;
    }

    /// <summary>
    /// Adds the mesh for all the components and their subcomponenets in the custom rigid group.  <see cref="ExportAll(ComponentOccurrence,bool,bool,bool)"/>
    /// </summary>
    /// <remarks>
    /// This uses the best resolution and separate faces options stored inside the provided custom rigid group.
    /// </remarks>
    /// <param name="group">The group to export from</param>
    /// <returns>All the sufaces to export</returns>
    private List<SurfaceBody> GenerateExportList(CustomRigidGroup group)
    {
        List<SurfaceBody> plannedExports = new List<SurfaceBody>();

        double totalVolume = 0;
        foreach (ComponentOccurrence occ in group.occurrences)
        {
            totalVolume += Utilities.BoxVolume(occ.RangeBox);
        }
        totalVolume /= group.occurrences.Count * adaptiveDegredation;

        foreach (ComponentOccurrence occ in group.occurrences)
        {
            if (!adaptiveIgnoring || Utilities.BoxVolume(occ.RangeBox) >= totalVolume)
            {
                plannedExports.AddRange(GenerateExportList(occ));
            }
        }

        return plannedExports;
    }
}