""" ParserOptions

    - This module targets the creation of the parser used for actually parsing the data
    - Since the parsing can be recursive you can pass a low overhead options construction into each function to detail the parsing
    - Should have factory methods to convert from a given configuration possibly
        - or maybe a configuration should replace this im not certain
        - this is essentially a flat configuration file with non serializable objects
"""

from enum import Enum
from typing import Union
from os import path

import subprocess, os, platform

import adsk.core, adsk.fusion, traceback

# from .unity import Parse
from ..general_imports import A_EP, PROTOBUF

from .SynthesisParser.Parser import Parser


class PhysicalDepth:
    """Depth at which the Physical Properties are generated and saved
    - This is mostly dictatated by export type as flattening or any hierarchical modification takes presedence
    """

    NoPhysical = 0
    """ No Physical Properties are generated """
    Body = 1
    """ Only Body Physical Objects are generated """
    SurfaceOccurrence = 2
    """ Only Occurrence that contain Bodies and Bodies have Physical Properties """
    AllOccurrence = 3
    """ Every Single Occurrence has Physical Properties even if empty """


class ModelHierarchy:
    """
    Enum Class to describe how the model format should look on export to suit different needs
    """

    FusionAssembly = 0
    """ Model exactly as it is shown in Fusion 360 in the model view tree """

    FlatAssembly = 1
    """ Flattened Assembly with all bodies as children of the root object """

    PhysicalAssembly = 2
    """ A Model represented with parented objects that are part of a jointed tree """

    SingleMesh = 3
    """ Generates the root assembly as a single mesh and stores the associated data """


class Mode:
    Synthesis = 0


class ParseOptions:
    """Options to supply to the parser object that will generate the output file"""

    def __init__(
        self,
        fileLocation: str,
        name: str,
        version: str,
        hierarchy=ModelHierarchy.FusionAssembly,
        visual=adsk.fusion.TriangleMeshQualityOptions.LowQualityTriangleMesh,
        physical=adsk.fusion.CalculationAccuracy.LowCalculationAccuracy,
        physicalDepth=PhysicalDepth.AllOccurrence,
        materials=1,
        joints=True,
        mode=Mode.Synthesis,
    ):
        """Generates the Parser Options for the given export

        Args:
            - fileLocation (str): Location of file with file name (given during file explore action)
            - name (str): name of the assembly
            - version (str): root assembly version
            - hierarchy (ModelHierarchy.FusionAssembly, optional): The exported model hierarchy. Defaults to ModelHierarchy.FusionAssembly
            - visual (adsk.fusion.TriangleMeshQualityOptions, optional): Triangle Mesh Export Quality. Defaults to adsk.fusion.TriangleMeshQualityOptions.HighQualityTriangleMesh.
            - physical (adsk.fusion.CalculationAccuracy, optional): Calculation Level of the physical properties. Defaults to adsk.fusion.CalculationAccuracy.MediumCalculationAccuracy.
            - physicalDepth (PhysicalDepth, optional): Enum to define the level of physical attributes exported. Defaults to PhysicalDepth.AllOccurrence.
            - materials (int, optional): Export Materials type: defaults to STANDARD 1
            - joints (bool, optional): Export Joints. Defaults to True.
        """
        self.fileLocation = fileLocation
        self.name = name
        self.version = version
        self.hierarchy = hierarchy
        self.visual = visual
        self.physical = physical
        self.physicalDepth = physicalDepth
        self.materials = materials
        self.joints = joints
        self.mode = mode

    def parse(self, sendReq: bool) -> Union[str, bool]:
        """Parses the file given the options

        Args:
            sendReq (bool): Do you want to send the request generated by the parser with sockets

        Returns:
            str | bool: Either a str indicating success or False indicating failure
        """
        if A_EP:
            A_EP.send_event("Parse", "started_parsing")

        test = Parser(self).export()
        return True


def runPackage(filepath):
    if platform.system() == "Darwin":  # macOS
        subprocess.call(("open", filepath))
    elif platform.system() == "Windows":  # Windows
        os.startfile(filepath)
