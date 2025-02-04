![Synthesis: An Autodesk Technology](/engine/Assets/Resources/Branding/Synthesis/Synthesis-An-Autodesk-Technology-2023-lockup-Blk-OL-No-Year-stacked.png#gh-light-mode-only)
![Synthesis: An Autodesk Technology](/engine/Assets/Resources/Branding/Synthesis/Synthesis-An-Autodesk-Technology-2023-lockup-Wht-OL-No-Year-stacked.png#gh-dark-mode-only)

<br/>

[![Engine](https://github.com/Autodesk/synthesis/actions/workflows/Engine.yml/badge.svg?branch=master)](https://github.com/Autodesk/synthesis/actions/workflows/Engine.yml)
[![API](https://github.com/Autodesk/synthesis/actions/workflows/API.yml/badge.svg?branch=master)](https://github.com/Autodesk/synthesis/actions/workflows/API.yml)
[![Clang Format](https://github.com/Autodesk/synthesis/actions/workflows/ClangFormat.yml/badge.svg?branch=master)](https://github.com/Autodesk/synthesis/actions/workflows/ClangFormat.yml)
[![Black Format](https://github.com/Autodesk/synthesis/actions/workflows/BlackFormat.yml/badge.svg?branch=master)](https://github.com/Autodesk/synthesis/actions/workflows/BlackFormat.yml)

Synthesis is a robotics simulator designed by and for FIRST robotics students to help teams design, strategize, test and practice. Teams have the ability to import their own robots and fields using our [Fusion 360 Exporter](/exporter/) or use the pre-packaged ones that come included with every release of Synthesis.

For more information on the product itself or the team, visit [http://synthesis.autodesk.com](http://synthesis.autodesk.com/).

## Goals

Synthesis is built with a direct focus on the FIRST community. Every single one of our developers is a FIRST student. We've also made the project completely open source in order to better involve the community. This way contributors can help make Synthesis better or modify Synthesis to better suit their team’s needs.

Here are some of our primary goals for Synthesis:

- **Ease of Use**: It's important for us that Synthesis is out of the box ready for teams to use. We want to make sure that teams can get up and running with Synthesis as quickly as possible. To that end, every release of Synthesis comes pre-packaged with a variety of robots and fields; in addition to the ability to export and import your own.
- **Testing Robot Designs**: Synthesis is designed to be a tool for teams to quickly test their robot designs in a semi-realistic environment. Are you a builder who wants to use some crazy virtual four-bar linkage and your team says it's a waste of time? Well now you can prove them wrong by testing it in Synthesis!
- **Exploring the Field Environment**: Every year on kickoff, for both FTC and FRC FIRST competitions, Synthesis has a new release with the brand new field for that year included. This allows teams to explore the field through a 3D model, drive a robot around, and begin to strategize for the upcoming season's game.
- **Driver Practice & Strategy**: Not getting enough driver practice or don't have a full field available to you? Synthesis has you covered with the ability to drive your robot around with a gamepad from a first-person view at the driver station; allowing you to get a feel for potential control scheme layouts and any line-of-sight challenges that may arise. This also allows the drive team and the programmers to communicate about what control layouts work best for each driver.

## Getting Started

If you are a FIRST robotics student who just wants to use Synthesis, you *don't* need this repo. Simply **install the latest release of Synthesis from [synthesis.autodesk.com/download](https://synthesis.autodesk.com/download.html)**.

If you're a developer who wants to contribute to Synthesis, you're in the right place. Synthesis is comprised of 3 main components that can be developed separately. These components include:

- [Synthesis API](/api/)
- [Simulation Engine](/engine/)
- [Fusion Robot Exporter (Fusion 360 Plugin)](/exporter/)

Each of this components can be manually compiled separately, but this is not recommended. Instead, we recommend using the *init* scripts provided (`init.bat` & `init.sh`) to build and link each component together (excluding the Fusion Robot Exporter).

### Compatibility Notes

As Fusion 360 is not supported on linux the linux installer does not come with the Fusion 360 Addin for exporting robots and fields.

Also note that the linux installer is tailored for Debian based distributions. Synthesis can be installed and ran on other distros but the installer is not guaranteed for your operating system.

### Dependencies

Synthesis Version `6.0.0` has the following dependencies:

- [.NET Standard 2.0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0)
- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- [Protobuf 23.3](https://github.com/protocolbuffers/protobuf/releases/tag/v23.3)
- Unity Version 2022.3.2f1 (d74737c6db50)

These dependencies need to be satisfied before attempting to build Synthesis with unity. You can either install these dependencies manually or use the *resolve dependencies* scripts (`scripts/win/resolve_deps.bat` & `scripts/osx/resolve_deps.sh`) to install some of them for you.

To automatically install .NET and Protobuf run the following script depending on your operating system:
- Windows: `scripts/win/resolve_deps.bat`
- MacOS & Linux: `scripts/osx/resolve_deps.sh`

Dependency Notes:

- The windows script requires admin privileges to install .NET and Protobuf to the system PATH. For this reason it is not included by default in `init.bat` unlike how `init.sh` includes `resolve_deps.sh`.
- Unity must be installed manually through the Unity Hub app which can be downloaded from [https://unity.com/download](https://unity.com/download).

### How To Build Synthesis Using An `init` Script And Unity

Before attempting to build Synthesis, ensure you have all dependencies installed.

1. Open a command prompt.
2. Change directories to a location where you'd like to clone the Synthesis repository.
3. Clone the Synthesis repository using `git clone https://github.com/Autodesk/synthesis --recurse-submodules`.
4. Change directories into the root of the Synthesis repository and run the `init` script for your operating system.
    - Windows: `init.bat`
    - MacOS & Linux: `init.sh`
5. Open `synthesis/engine` in Unity.
6. From there, you can run the simulation engine inside the Unity editor by opening `MainScene` from the `Assets/Scenes` directory or build it as a standalone application.
    - To build Synthesis as a standalone application, go to `File -> Build Settings` and select your target platform. Then, click `Build` and select a location to save the built application.

## Contributing

This project welcomes community suggestions and contributions. Synthesis is 100% open source and relies on the FIRST community to help make it better. The [Synthesis Contribution Guide](/CONTRIBUTING.md) suggests ways in which you can get involved through development and non-development avenues.

Before you contribute to this repository, please first discuss the change you wish to make via a GitHub issue, email us ([frc@autodesk.com](mailto:frc@autodesk.com)), or reach out through our [community discord](https://www.discord.gg/hHcF9AVgZA). This way we can ensure that there is no overlap between outside contributors and internal development work.

When ready to contribute, just submit a pull request and be sure to fill out the template accordingly so that we can verify them and eventually merge. Feel free to check out our [contributing guidelines](/CONTRIBUTING.md) to learn more.

## Code Formatting And Style

Synthesis uses [clang-format](https://clang.llvm.org/docs/ClangFormat.html) and [black](https://black.readthedocs.io/en/stable/) to format all of our `C++`, `C#` and `Python` code. Additionally we have GitHub workflows that verify these formatting standards for each and every pull request. For more information about how to run these formatters yourself, check out the [formatting tools](/scripts/format/) we use.

## Components

Here is a brief overview of each of the components found in this repository. For more information about each component, their dependencies and their individual build processes, check out their respective *READMEs*.

- [Synthesis API](/api/)
  - Components used to extend Synthesis' functionality while remaining Unity agnostic.
- [Code Emulation](/emulation/)
  - Synthesis tool designed to help users test their FRC robot code in a simulated environment. 
  - Code emulation is an advanced feature that is still under development, functionality and support is limited.
- [Core Engine](/engine/)
  - The core of Synthesis, the Unity project that contains the simulation engine.
- [Fusion Exporter](/exporter/SynthesisFusionAddin/)
  - The Fusion 360 plugin that allows users to export their robots and fields into Synthesis.
- [Installer](/installer/)
  - Tools used to build the Synthesis installer for Windows, MacOS and Linux featured on our [Download Page](https://synthesis.autodesk.com/download.html).
- [Mirabuf](https://github.com/HiceS/mirabuf/)
  - A custom 3D file format developed by [@HiceS](https://github.com/HiceS) that our Fusion 360 exporter uses.
- [Tutorials](/tutorials/)
  - Our source code for the tutorials featured on our [Tutorials Page](https://synthesis.autodesk.com/tutorials.html).

## Tutorials

We have a variety of tutorials available to help you get started with Synthesis. These tutorials can be found on our [Tutorials Page](https://synthesis.autodesk.com/tutorials.html) on our website. Additionally, you can view these same tutorials as Markdown files in the [tutorials](/tutorials/) directory of this repository.

Updating our tutorials is a ongoing process. If you are at all interested in helping, checkout the [Synthesis Contribution Guide](/CONTRIBUTING.md) for more information on how to get started.

## Immersion Program

Annually, since 2014, Autodesk has sponsored the Synthesis Immersion Program for FIRST robotics students to develop Synthesis. The immersion program is a 10 week paid work experience at the Portland, Oregon Autodesk office from June 20th to August 25th. The immersion program focuses on not only developing Synthesis, but also allowing for opportunities to meet and collaborate with other Autodesk employees. For more information about the immersion program, visit our website at [synthesis.autodesk.com/internship](https://synthesis.autodesk.com/internship.html).

### Want To Be A Part Of The Team?

If you're a FIRST robotics student who wants to be a part of the Synthesis development team here is some basic information about applying.

Applicants must be:

- At least 16 years of age
- Been a member of a FIRST Robotics team for at least one full season

Applications open each year during the spring. For more information about applying, exceptions to these requirements or for more info about specific positions offered, please visit the [Synthesis Immersion Program](https://synthesis.autodesk.com/internship.html) website.

## Contact

If you have any questions about Synthesis or the Immersion Program, you can contact us through email ([frc@autodesk.com](mailto:frc@autodesk.com)). Additionally please reach out through our [community discord](https://www.discord.gg/hHcF9AVgZA). It's the best way to get in touch with not only the community, but Synthesis' current development team.

## License

Copyright (c) Autodesk

SPDX-License-Identifier: Apache-2.0
