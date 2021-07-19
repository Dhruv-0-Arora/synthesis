""" SerialCommand module

Defines what is generated by the Fusion 360 Command.
Use to be protobuf but can only de-serialize from a filestream strangely.
This is used to store in the userdata.
"""

import json
from ...Types.OString import OString


def generateFilePath() -> str:
    """Generates a temporary file path that can be used to save the file for exporting

    Example:
     - "%appdata%/Roaming/Temp/HellionFiles"

    Returns:
        str: file path
    """
    tempPath = OString.TempPath("").getPath()
    return str(tempPath)


class Struct:
    """For decoding the dict values into named values"""

    def __init__(self, **entries):
        self.__dict__.update(entries)


class SerialCommand:
    """ All of the command inputs combined """

    def __init__(self):
        self.general = General()
        self.advanced = Advanced()
        self.filePath = generateFilePath()

    def toJSON(self) -> str:
        """Converts this class into a json object that can be written to the object data

        Returns:
            str: json version of this object
        """
        return json.dumps(self, default=lambda o: o.__dict__, sort_keys=True, indent=1)


class General:
    """ General Options """

    def __init__(self):
        # This is the overall export decision point
        self.exportMode = ExportMode.standard
        self.RenderType = RenderType.basic3D
        self.material = BooleanInput("material", True)
        self.joints = BooleanInput("joints", False)
        self.rigidGroups = BooleanInput("rigidgroup", False)
        #self.wheelType = 
        self.simpleWheelExport = BooleanInput("simplewheelexport", False)


class Advanced:
    """Advanced settings in the command input"""

    def __init__(self):
        self.friction = BooleanInput("friction", True)
        self.density = BooleanInput("density", True)
        self.mass = BooleanInput("mass", True)
        self.volume = BooleanInput("volume", True)
        self.surfaceArea = BooleanInput("surfaceArea", True)
        self.com = BooleanInput("com", True)


class BooleanInput:
    """Class to store the value of a boolean input"""

    def __init__(self, name: str, default: bool):
        self.name = name
        self.checked = default


class ExportMode:
    """Export Mode defines the type of export"""

    standard = 0
    VR = 1
    Simulation = 2


class RenderType:
    """ This will modify the type of material shaders used """

    basic3D = 0
    URP = 1
    HDRP = 2