# Author-Shawn Hice
# Description-SketchUp Website uploader

from .src.general_imports import root_logger, gm, INTERNAL_ID, APP_NAME

from .src.UI import HUI, Handlers, Camera, Helper, ConfigCommand
from .src.UI.Toolbar import Toolbar
from .src.Types.OString import OString
from .src.configure import setAnalytics, unload_config

from shutil import rmtree
import logging.handlers, traceback, importlib.util, os


def run(context):
    """## Entry point to application from Fusion 360.

    Arguments:
        **context** *context* -- Fusion 360 context to derive app and UI.
    """

    try:
        # Remove all items prior to start just to make sure
        unregister_all()

        # creates the UI elements
        register_ui()

    except:
        logging.getLogger(f"{INTERNAL_ID}").error(
            "Failed:\n{}".format(traceback.format_exc())
        )


def stop(context):
    """## Fusion 360 exit point - deconstructs buttons and handlers

    Arguments:
        **context** *context* -- Fusion 360 Data.
    """
    try:
        unregister_all()

        # nm.deleteMe()

        # should make a logger class
        handlers = root_logger.handlers[:]
        for handler in handlers:
            handler.close()
            # I think this will clear the file completely
            # root_logger.removeHandler(handler)

        unload_config()

        for file in gm.files:
            try:
                os.remove(file)
            except OSError:
                pass

        # removes path so that proto files don't get confused

        import sys

        path = os.path.abspath(os.path.dirname(__file__))

        if path in sys.path:
            sys.path.remove(path)

    except:
        logging.getLogger(f"{INTERNAL_ID}").error(
            "Failed:\n{}".format(traceback.format_exc())
        )


def unregister_all() -> None:
    """Unregisters all UI elements in case any still exist.

    - Good place to unregister custom app events that may repeat.
    """
    try:
        Camera.clearIconCache()

        for element in gm.elements:
            element.deleteMe()

        for tab in gm.tabs:
            tab.deleteMe()

    except:
        logging.getLogger(f"{INTERNAL_ID}").error(
            "Failed:\n{}".format(traceback.format_exc())
        )


def register_ui() -> None:
    """ #### Generic Function to add all UI objects in a simple non destructive way. """

    # if A_EP:
    #     A_EP.send_view("open")

    # toolbar = Toolbar('SketchFab')
    work_panel = Toolbar.getNewPanel(f"{APP_NAME}", f"{INTERNAL_ID}", "ToolsTab")

    commandButton = HUI.HButton(
        "Synthesis Exporter",
        work_panel,
        Helper.check_solid_open,
        ConfigCommand.ConfigureCommandCreatedHandler,
        description="Opens Dialog to configure and export model for Mira",
        command=True,
    )

    gm.elements.append(commandButton)
